using DonationHub.Application.Features.Auth.DTOs.Response;
using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Auth.Queries.GetCurrentUser
{
    public record GetCurrentUserQuery(): IRequest<Result<CurrentUserResponseDto>>;
}
