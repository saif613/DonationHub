using DonationHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Interfaces.Services
{
    public interface IJwtTokenService
    {
        string GenerateJwtToken(Guid id,string email,string userName,string fullName, IReadOnlyList<string> roles);
    }
}
