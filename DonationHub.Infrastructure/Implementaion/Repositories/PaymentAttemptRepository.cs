using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Infrastructure.Implementaion.Repositories
{
    public class PaymentAttemptRepository: GenericRepository<PaymentAttempt>, IPaymentAttemptRepository
    {
        public PaymentAttemptRepository(AppDbContext context) : base(context) { }
    }
}
