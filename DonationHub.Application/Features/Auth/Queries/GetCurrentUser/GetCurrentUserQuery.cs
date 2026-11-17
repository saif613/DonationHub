using DonationHub.Application.Features.DTOs.Responses;
using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Auth.Queries.GetCurrentUser;

public record GetCurrentUserQuery() : IRequest<Result<CurrentUserResponseDto>>;
