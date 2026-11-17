using DonationHub.Application.Features.Users.DTOs;
using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Users.Commands.CreateStaff;

public record CreateStaffCommand(string Email, string FullName, string Role, string PreferredLanguage = "en"): IRequest<Result<CreateStaffResponseDto>>;
