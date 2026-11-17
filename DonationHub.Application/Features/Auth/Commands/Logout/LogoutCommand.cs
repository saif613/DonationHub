using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Auth.Commands.Logout;

public record LogoutCommand(string RefreshToken): IRequest<Result>;
