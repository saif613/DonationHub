using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.Commands.DeleteCategory
{
    public record DeleteCategoryCommand(Guid id) : IRequest<Result>;
    
}
