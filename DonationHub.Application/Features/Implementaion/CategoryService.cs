using DonationHub.Application.Exceptions;
using DonationHub.Application.Features.Category.DTOs.Requests;
using DonationHub.Application.Features.Category.DTOs.Responses;
using DonationHub.Application.Features.DTOs.Responses;
using DonationHub.Application.Features.Interfaces;
using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Application.Interfaces.Services;
using DonationHub.Application.Interfaces.UnitOfWork;
using CategoryEntity = DonationHub.Domain.Entities.Category;


namespace DonationHub.Application.Features.Implementaion
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        private readonly TimeProvider _timeProvider;

        public CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUser, TimeProvider timeProvider)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _timeProvider = timeProvider;
        }

        private DateTime GetUtcNow() => _timeProvider.GetUtcNow().UtcDateTime;
        public async Task<CategoryResponseDto> CreateAsync(CategoryRequestDto request, CancellationToken ct = default)
        {
            var nameAr = request.NameAr.Trim();
            var nameEn = request.NameEn.Trim();

            if (await _categoryRepository.ExistsByNameArAsync(nameAr, ct:ct))
                throw new AppException("Category.ArNameAlreadyExists", 409);

            if (await _categoryRepository.ExistsByNameEnAsync(nameEn, ct:ct))
                throw new AppException("Category.EnNameAlreadyExists", 409);

            var category = CategoryEntity.Create(nameAr, nameEn, request.DescriptionAr, request.DescriptionEn, GetUtcNow(), _currentUser.UserId);

            await _categoryRepository.AddAsync(category, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return new CategoryResponseDto(category.Id, category.NameAr, category.NameEn, category.DescriptionAr, category.DescriptionEn);
        }

        public async Task<IReadOnlyList<CategoryResponseDto>> GetAllAsync(CancellationToken ct = default)
        {
            var categories = await _categoryRepository.GetAllAsync(ct);

            return categories.Select(category =>
            new CategoryResponseDto(
                category.Id,
                category.NameAr,
                category.NameEn,
                category.DescriptionAr,
                category.DescriptionEn
                )).ToList();
        }

        public async Task<CategoryResponseDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var category = await _categoryRepository.GetByIdAsync(id, ct);

            if (category is null)
                throw new AppException("Category.NotFound", 404);

            return new CategoryResponseDto(category.Id, category.NameAr, category.NameEn, category.DescriptionAr, category.DescriptionEn);
        }

        public async Task<PagedResponse<CategoryResponseDto>> PagedAsync(int pageNumber, int pageSize, CancellationToken ct = default)
        {
            if (pageNumber < 1)
                throw new AppException("Pagination,InvalidPageNumber", 400);

            if (pageSize < 1 || pageSize > 100)
                throw new AppException("Pagination,InvalidPageSize", 400);

            var (categories, totalCount) = await _categoryRepository.GetPagedAsync(pageNumber, pageSize, ct);

            var data = categories.Select(category => new CategoryResponseDto(category.Id, category.NameAr, category.NameEn,
            category.DescriptionAr,
            category.DescriptionEn))
           .ToList();

            return new PagedResponse<CategoryResponseDto>(data, pageNumber, pageSize, totalCount);
        }

        public async Task SoftDelete(Guid id, CancellationToken ct = default)
        {
            var category = await _categoryRepository.GetByIdAsync(id, ct);

            if (category is null)
                throw new AppException("Category.NotFound", 404);

            var isCategoryHasCampaign = await _categoryRepository.IsCategoryHasCampaign(id, ct);

            if(isCategoryHasCampaign)
                throw new AppException("Category.HasCampaigns", 409);

            _categoryRepository.Delete(category);

            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task<CategoryResponseDto> Update(CategoryRequestDto request, Guid id, CancellationToken ct = default)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category is null)
                throw new AppException("Category.NotFound", 404);

            var nameAr = request.NameAr.Trim();
            var nameEn = request.NameEn.Trim();

            if (await _categoryRepository.ExistsByNameArAsync(nameAr, ct:ct))
                throw new AppException("Category.ArNameAlreadyExists", 409);

            if (await _categoryRepository.ExistsByNameEnAsync(nameEn, ct:ct))
                throw new AppException("Category.EnNameAlreadyExists", 409);

            category.Update(nameAr, nameEn, request.DescriptionAr, request.DescriptionEn, GetUtcNow(), _currentUser.UserId);

            await _unitOfWork.SaveChangesAsync(ct);

            return new CategoryResponseDto(category.Id, category.NameAr, category.NameEn, category.DescriptionAr, category.DescriptionEn);
        }
    }
}
