using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Infrastructure.Implementaion.Repositories
{
    public class DonationRepository: GenericRepository<Donation>, IDonationRepository
    {
        public DonationRepository(AppDbContext context) : base(context) { }
    }
}
