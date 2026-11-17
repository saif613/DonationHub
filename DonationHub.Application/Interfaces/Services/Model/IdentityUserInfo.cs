using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Interfaces.Services.Model
{
    public record IdentityUserInfo(Guid Id, string Email, string UserName);
    
}
