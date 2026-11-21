using DonationHub.Application.Features.Auth.Commands.Login;
using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Application.Interfaces.Services;
using DonationHub.Application.Interfaces.Services.Model;
using DonationHub.Application.Interfaces.UnitOfWork;
using DonationHub.Application.Options;
using DonationHub.Domain.Common.Results;
using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using Moq;

namespace DonationHub.Application.UnitTests.Features.Identity.Commands.Login
{
    public class LoginHandlerTest
    {
        public LoginCommand CreateLoginCommand()
        {
            return new LoginCommand(
                Email: "test@example.com",
                Password: "Abcdefgh1");
        }

        public IdentityUserInfo CreateIdentityUserInfo()
        {
            return new IdentityUserInfo(
                Id: Guid.NewGuid(),
                Email: "test@example.com",
                UserName: "test");
        }
        [Fact]
        public async Task Handle_WhenUserDoesNotExist_ShouldReturnInvalidCredentials()
        {
            var command = CreateLoginCommand();

            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var timeProvider = new FakeTimeProvider();
            var jwtTokenService = new Mock<IJwtTokenService>();
            var refreshTokenRepository = new Mock<IRefreshTokenRepository>();
            var identityService = new Mock<IIdentityService>();

            identityService.Setup(x => x.FindByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((IdentityUserInfo?) null);

            var jwtOptions = Microsoft.Extensions.Options.Options.Create(new JwtOptions());

            var handler = new LoginHandler(userRepository.Object, unitOfWork.Object, timeProvider, jwtTokenService.Object, refreshTokenRepository.Object, jwtOptions, identityService.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Auth.InvalidCredentials);

            identityService.Verify(x => x.FindByEmailAsync(command.Email, It.IsAny<CancellationToken>()), Times.Once);

            userRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);

            identityService.Verify(x => x.CheckPasswordAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);

            unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenPasswordIsIncorrect_ShouldReturnInvalidCredentials()
        {
            var command = CreateLoginCommand();
            var identityUserInfo = CreateIdentityUserInfo();

            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var timeProvider = new FakeTimeProvider();
            var jwtTokenService = new Mock<IJwtTokenService>();
            var refreshTokenRepository = new Mock<IRefreshTokenRepository>();
            var identityService = new Mock<IIdentityService>();

            identityService.Setup(x => x.FindByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
             .ReturnsAsync(identityUserInfo);

            identityService.Setup(x => x.CheckPasswordAsync(identityUserInfo.Id,command.Password, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var jwtOptions = Microsoft.Extensions.Options.Options.Create(new JwtOptions());

            var handler = new LoginHandler(userRepository.Object, unitOfWork.Object, timeProvider, jwtTokenService.Object, refreshTokenRepository.Object, jwtOptions, identityService.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Auth.InvalidCredentials);

            identityService.Verify(x => x.FindByEmailAsync(command.Email, It.IsAny<CancellationToken>()), Times.Once);

            identityService.Verify(x => x.CheckPasswordAsync(identityUserInfo.Id, command.Password, It.IsAny<CancellationToken>()), Times.Once);

            userRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);

            unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
