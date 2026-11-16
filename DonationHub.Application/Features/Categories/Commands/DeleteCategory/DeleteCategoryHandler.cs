using DonationHub.Application.Exceptions;
using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Application.Interfaces.UnitOfWork;
using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.Commands.DeleteCategory
{
    public sealed class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, Result>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCategoryHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken ct = default)
        {
            var category = await _categoryRepository.GetByIdAsync(request.id, ct);

            if (category is null)
                return Errors.Category.NotFound;

            var isCategoryHasCampaign = await _categoryRepository.IsCategoryHasCampaign(request.id, ct);

            if (isCategoryHasCampaign)
                return Errors.Category.HasCampaigns;

            _categoryRepository.Delete(category);

            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}
