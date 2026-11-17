using DonationHub.Application.Features.Users.DTOs;
using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Users.Commands.ActivateUser;

public record ActivateUserCommand(Guid UserId): IRequest<Result<UserResponseDto>>;
