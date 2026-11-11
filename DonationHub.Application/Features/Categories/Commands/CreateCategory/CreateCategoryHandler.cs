using DonationHub.Application.Exceptions;
using DonationHub.Application.Features.Category.DTOs.Responses;
using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Application.Interfaces.Services;
using DonationHub.Application.Interfaces.UnitOfWork;
using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using CategoryEntity = DonationHub.Domain.Entities.Category;

namespace DonationHub.Application.Features.Categories.Commands.CreateCategory
{
    public sealed class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Result<CategoryResponseDto>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        private readonly TimeProvider _timeProvider;

        public CreateCategoryHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUser, TimeProvider timeProvider)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _timeProvider = timeProvider;
        }
        private DateTime GetUtcNow() => _timeProvider.GetUtcNow().UtcDateTime;
        public async Task<Result<CategoryResponseDto>> Handle(CreateCategoryCommand request, CancellationToken ct = default)
        {
            var nameAr = request.NameAr.Trim();
            var nameEn = request.NameEn.Trim();

            if (await _categoryRepository.ExistsByNameArAsync(nameAr, ct:ct))
                return Errors.Category.ArNameAlreadyExists;


            if (await _categoryRepository.ExistsByNameEnAsync(nameEn, ct:ct))
                return Errors.Category.EnNameAlreadyExists;

            var category = CategoryEntity.Create(nameAr, nameEn, request.DescriptionAr!, request.DescriptionEn!, GetUtcNow(), _currentUser.UserId);

            await _categoryRepository.AddAsync(category, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            var response = new CategoryResponseDto(category.Id, category.NameAr, category.NameEn, category.DescriptionAr, category.DescriptionEn);

            return Result.Success(response);
        }
    }
}
