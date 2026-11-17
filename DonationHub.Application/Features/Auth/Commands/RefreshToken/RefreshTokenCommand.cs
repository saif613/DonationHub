using DonationHub.Application.Features.Auth.DTOs.Response;
using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken): IRequest<Result<TokenResponseDto>>;
