using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Auth.Commands.Register;

public record RegisterCommand(string Email, string FullName, string Password, string PreferredLanguage = "en"): IRequest<Result>;
