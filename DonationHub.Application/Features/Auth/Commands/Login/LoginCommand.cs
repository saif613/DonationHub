using DonationHub.Application.Features.Auth.DTOs.Response;
using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<Result<TokenResponseDto>>;
