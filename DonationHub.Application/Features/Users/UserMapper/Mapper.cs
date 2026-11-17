using DonationHub.Application.Features.Users.DTOs;
using DonationHub.Application.Interfaces.Services.Model;
using DonationHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Users.UserMapper
{
    internal class Mapper
    {
        public static UserResponseDto ToResponse(User user, IdentityUserInfo identityUser)
        {
            return new UserResponseDto(identityUser.Email, user.FullName, user.PreferredLanguage, user.IsActive);
        }
    }
}
