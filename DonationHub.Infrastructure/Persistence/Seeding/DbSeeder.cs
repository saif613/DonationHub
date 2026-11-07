using DonationHub.Application.Interfaces.UnitOfWork;
using DonationHub.Domain.Constants;
using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Identity;
using DonationHub.Infrastructure.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DonationHub.Infrastructure.Persistence.Seeding;

public class DbSeeder
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;
    private readonly SeedAdminOptions _adminOptions;
    private readonly SeedDevUsersOptions _devOptions;
    private readonly ILogger<DbSeeder> _logger;

    public DbSeeder(AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, IUnitOfWork unitOfWork,
        TimeProvider timeProvider,
        IOptions<SeedAdminOptions> adminOptions,
        IOptions<SeedDevUsersOptions> devOptions,
        ILogger<DbSeeder> logger)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
        _adminOptions = adminOptions.Value;
        _devOptions = devOptions.Value;
        _logger = logger;
    }

    private DateTime GetUtcNow()=> _timeProvider.GetUtcNow().UtcDateTime;

    public async Task SeedAsync(bool isDevelopment, CancellationToken ct = default)
    {
        _logger.LogInformation("Database seeding started.");

        await SeedRolesAsync(ct);

        await SeedAdminAsync(ct);
        
        await SeedCategoriesAsync(ct);
       
        if (isDevelopment)
        {
            await SeedDevUsersAsync(ct);
        }

        _logger.LogInformation("Database seeding completed.");
    }


    private async Task SeedRolesAsync(CancellationToken ct)
    {
        var roles = new[]
        {
            Roles.Admin,
            Roles.CampaignManager,
            Roles.Donor
        };

        foreach (var role in roles)
        {
            ct.ThrowIfCancellationRequested();

            if (await _roleManager.RoleExistsAsync(role))
                continue;

            var result = await _roleManager.CreateAsync(
                new IdentityRole<Guid>(role));

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"Failed to seed role '{role}': {errors}");
            }

            _logger.LogInformation("Seeded role: {Role}", role);
        }
    }

    private async Task SeedAdminAsync(CancellationToken ct)
    {
        await EnsureUserAsync(
            email: _adminOptions.Email,
            password: _adminOptions.Password,
            fullName: _adminOptions.FullName,
            language: _adminOptions.PreferredLanguage,
            role: Roles.Admin,
            ct: ct);
    }

    private async Task SeedDevUsersAsync(CancellationToken ct)
    {
        var manager = _devOptions.CampaignManager;

        await EnsureUserAsync(
            email: manager.Email,
            password: manager.Password,
            fullName: manager.FullName,
            language: "en",
            role: Roles.CampaignManager,
            ct: ct);

        var donor = _devOptions.Donor;

        await EnsureUserAsync(
            email: donor.Email,
            password: donor.Password,
            fullName: donor.FullName,
            language: "en",
            role: Roles.Donor,
            ct: ct);
    }

    private async Task EnsureUserAsync(string email, string password, string fullName, string language, string role, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("Seed credentials for role {Role} are missing. Skipped.", role);
            return;
        }

        var applicationUser = await _userManager.FindByEmailAsync(email);

        if (applicationUser is null)
        {
            applicationUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(
                applicationUser,
                password);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"Failed to create user '{email}': {errors}");
            }

            _logger.LogInformation("Created Identity user: {Email}", email);
        }

        if (!await _userManager.IsInRoleAsync(applicationUser, role))
        {
            var roleResult = await _userManager.AddToRoleAsync(applicationUser, role);

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"Failed to assign role '{role}' to '{email}': {errors}");
            }
        }

     
        var domainUserExists = await _context.Set<User>()
            .AnyAsync(u => u.Id == applicationUser.Id, ct);

        if (!domainUserExists)
        {
            var user = User.Create(
                applicationUser.Id,
                fullName,
                language,
                GetUtcNow(),
                null);

            _context.Set<User>().Add(user);

            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Created Domain user: {Email}", email);
        }

        _logger.LogInformation("Ensured user {Email} has role {Role}.", email, role);
    }

 
    private async Task SeedCategoriesAsync(CancellationToken ct)
    {
        var now = GetUtcNow();

        var categories = new[]
        {
            Category.Create(
                "الصحة",
                "Health",
                "دعم الخدمات والمبادرات الصحية",
                "Supporting healthcare services and initiatives",
                now,
                null),

            Category.Create(
                "التعليم",
                "Education",
                "دعم التعليم وتوفير فرص التعلم",
                "Supporting education and learning opportunities",
                now,
                null),

            Category.Create(
                "الإغاثة",
                "Relief",
                "تقديم المساعدات الإنسانية والإغاثية",
                "Providing humanitarian and emergency relief",
                now,
                null),

            Category.Create(
                "الأيتام",
                "Orphans",
                "دعم ورعاية الأطفال الأيتام",
                "Supporting and caring for orphaned children",
                now,
                null),

            Category.Create(
                "البيئة",
                "Environment",
                "دعم حماية البيئة والاستدامة",
                "Supporting environmental protection and sustainability",
                now,
                null)
        };

        foreach (var category in categories)
        {
            ct.ThrowIfCancellationRequested();

            var exists = await _context.Set<Category>()
                .AnyAsync(c => c.NameEn == category.NameEn, ct);

            if (exists)
                continue;

            _context.Set<Category>().Add(category);

            _logger.LogInformation(
                "Added category: {Category}",
                category.NameEn);
        }

        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Categories seeding completed.");
    }
}