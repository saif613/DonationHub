using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Interfaces.Services
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
    }
}
