using DonationHub.Application.Features.Users.DTOs;
using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Users.Commands.UpdateProfile;

public record UpdateProfileCommand(string FullName, string Email) : IRequest<Result<UserResponseDto>>;
