using DonationHub.Application.Features.Users.DTOs;
using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Users.Commands.DeactivateUser;

public record DeactivateUserCommand(Guid UserId) : IRequest<Result<UserResponseDto>>;
