using DonationHub.Application.Features.Users.DTOs;
using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Users.Commands.ChangeLanguage;

public record ChangeLanguageCommand(string Language): IRequest<Result<UserResponseDto>>;
