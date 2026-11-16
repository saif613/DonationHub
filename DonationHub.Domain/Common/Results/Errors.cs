using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Common.Results
{
    public static class Errors
    {
        public static class Validation
        {
            public static readonly Error Failed = new("Validation.Failed", "Input validation failed", ErrorType.Validation);
        }

        public static class Auth
        {
            public static readonly Error InvalidCredentials = new("Auth.InvalidCredentials", "Invalid Email or Password", ErrorType.Unauthorized);
            public static readonly Error EmailAlreadyExists = new("Auth.EmailAlreadyExists", "Email already exists", ErrorType.Conflict);
            public static readonly Error RefreshTokenInvalid = new("Auth.RefreshTokenInvalid", "Refresh token is invalid ", ErrorType.Unauthorized);
            public static readonly Error AccountDeactivated = new("Auth.AccountDeactivated", "Account is deactivated ", ErrorType.Forbidden);
        }

        public static class Category
        {
            public static readonly Error ArNameAlreadyExists = new("Category.ArNameAlreadyExists", "Arabic category name already exists", ErrorType.Conflict);
            public static readonly Error EnNameAlreadyExists = new("Category.EnNameAlreadyExists", "English category name already exists", ErrorType.Conflict);
            public static readonly Error NotFound = new("Category.NotFound", "Category does not exist", ErrorType.NotFound);
            public static readonly Error HasCampaigns = new("Category.HasCampaigns", "Cannot delete category with linked campaigns", ErrorType.Conflict);
        }

        public static class Campaign
        {
            public static readonly Error NotFound = new("Campaign.NotFound", "Campaign does not exist", ErrorType.NotFound); 
            public static readonly Error NotActive = new("Campaign.NotActive", "Campaign is complete or expired", ErrorType.Conflict);
            public static readonly Error GoalBelowRaised = new("Campaign.GoalBelowRaised", "Goal amount cannot be lower than raised amount", ErrorType.UnprocessableEntity);
            public static readonly Error EndDateInPast = new("Campaign.EndDateInPast", "End date must be in the future", ErrorType.UnprocessableEntity);
            public static readonly Error ConcurrencyConflict = new("Campaign.ConcurrencyConflict", "Concurrent update conflict detected", ErrorType.Conflict);
        }

        public static class Donation
        {
            public static readonly Error NotFound = new("Donation.NotFound", "Donation does not exist or owned", ErrorType.NotFound);
            public static readonly Error AmountOutOfRange = new("Donation.AmountOutOfRange", "Amount outside DonationOptions range", ErrorType.UnprocessableEntity);
            public static readonly Error InvalidState = new("Donation.InvalidState", "Donation already in a final state", ErrorType.Conflict);
        }

        public static class Common
        {
            public static readonly Error RateLimite = new("Common.RateLimite", "Rate limit exceeded", ErrorType.TooManyRequests);
        }

        public static class File
        {
            public static readonly Error InvalidType = new("File.InvalidType", "File signature not allowed", ErrorType.Validation);
            public static readonly Error TooLarge = new("File.TooLarge", "File exceeds max size", ErrorType.PayloadTooLarge);
        }

        public static class Payment
        {
            public static readonly Error InvalidSignature = new("Payment.InvalidSignature", "Stripe webhook signature invalid", ErrorType.Validation);
        } 

        public static class Idempotency
        {
            public static readonly Error KeyReusedWithDifferentRequest = new("Idempotency.KeyReusedWithDifferentRequest", "Same Idempotency-Key with a diffrent body", ErrorType.UnprocessableEntity);
        }
    }
}
