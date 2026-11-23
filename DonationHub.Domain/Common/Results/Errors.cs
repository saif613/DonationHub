using DonationHub.Domain.Entities;
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
            public static readonly Error Unauthorized = new("Auth.Unauthorized", "User Unauthorized", ErrorType.Unauthorized);
            public static readonly Error UserNotFound = new("Auth.UserNotFound", "User does not exist", ErrorType.NotFound);
            public static readonly Error RoleNotFound = new("Auth.RoleNotFound", "Role does not exist", ErrorType.NotFound);
            public static readonly Error EmailAlreadyExists = new("Auth.EmailAlreadyExists", "Email already exists", ErrorType.Conflict);
            public static readonly Error RefreshTokenInvalid = new("Auth.RefreshTokenInvalid", "Refresh token is invalid ", ErrorType.Unauthorized);
            public static readonly Error AccountDeactivated = new("Auth.AccountDeactivated", "Account is deactivated ", ErrorType.Forbidden);
            public static readonly Error FailedToAssignRole = new("Auth.FailedToAssignRole", "Failed to assign the requested role to the user", ErrorType.Failure);
            public static readonly Error PasswordChangeFailed = new("Auth.PasswordChangeFailed", "Failed to change password", ErrorType.Validation);
            public static readonly Error EmailUpdateFailed = new("Auth.EmailUpdateFailed ", "Failed to update email", ErrorType.Failure);
            public static readonly Error UserNameUpdateFailed = new("Auth.UserNameUpdateFailed ", "Failed to update username", ErrorType.Failure);
        }

        public static class Category
        {
            public static readonly Error ArNameAlreadyExists = new("Category.ArNameAlreadyExists", "Arabic category name already exists", ErrorType.Conflict);
            public static readonly Error EnNameAlreadyExists = new("Category.EnNameAlreadyExists", "English category name already exists", ErrorType.Conflict);
            public static readonly Error NotFound = new("Category.NotFound", "Category does not exist", ErrorType.NotFound);
            public static readonly Error HasCampaigns = new("Category.HasCampaigns", "Cannot delete category with linked campaigns", ErrorType.Conflict);
            public static readonly Error NameArRequired = new("Category.NameArRequired", "Arabic name is required", ErrorType.Validation);
            public static readonly Error NameEnRequired = new("Category.NameEnRequired", "English name is required", ErrorType.Validation);
            public static readonly Error NameArTooLong = new("Category.NameArTooLong", "Arabic name cannot exceed 100 characters", ErrorType.Validation);
            public static readonly Error NameEnTooLong = new("Category.NameEnTooLong", "English name cannot exceed 100 characters", ErrorType.Validation);
        }

        public static class Campaign
        {
            public static readonly Error NotFound = new("Campaign.NotFound", "Campaign does not exist", ErrorType.NotFound); 
            public static readonly Error NotActive = new("Campaign.NotActive", "Campaign is complete or expired", ErrorType.Conflict);           
            public static readonly Error EndDateInPast = new("Campaign.EndDateInPast", "End date must be in the future", ErrorType.UnprocessableEntity);
            public static readonly Error ConcurrencyConflict = new("Campaign.ConcurrencyConflict", "Concurrent update conflict detected", ErrorType.Conflict);
            public static readonly Error ArTitleAlreadyExists = new("Campaign.ArTitleAlreadyExists", "Arabic campaign title already exists", ErrorType.Conflict);
            public static readonly Error EnTitleAlreadyExists = new("Campaign.EnTitleAlreadyExists", "English campaign title already exists", ErrorType.Conflict);
            public static readonly Error TitleArRequired = new("Campaign.TitleArRequired", "Arabic title is required", ErrorType.Validation);
            public static readonly Error TitleEnRequired = new("Campaign.TitleEnRequired", "English title is required", ErrorType.Validation);
            public static readonly Error DescriptionArRequired = new("Campaign.DescriptionArRequired", "Arabic description is required", ErrorType.Validation);
            public static readonly Error DescriptionEnRequired = new("Campaign.DescriptionEnRequired", "English description is required", ErrorType.Validation);
            public static readonly Error GoalAmountRequired = new("Campaign.GoalAmountRequired", "Goal amount is required", ErrorType.Validation);
            public static readonly Error CategoryIdRequired = new("Campaign.CategoryIdRequired", "Category id is required", ErrorType.Validation);
            public static readonly Error CreatorIdRequired = new("Campaign.CreatorIdRequired", "Creator id is required", ErrorType.Validation);
            public static readonly Error CurrencyMismatch = new("Campaign.CurrencyMismatch", "Currency does not match campaign currency", ErrorType.Validation);
            public static readonly Error GoalBelowRaisedAmount = new("Campaign.GoalBelowRaisedAmount", "Goal amount cannot be less than raised amount", ErrorType.Validation);
            public static readonly Error CannotAcceptDonation = new("Campaign.CannotAcceptDonation", "Campaign cannot accept donations", ErrorType.Conflict);
            public static readonly Error EndDateNotReached = new("Campaign.EndDateNotReached", "Campaign end date has not been reached", ErrorType.Conflict);
        }

        public static class Donation
        {
            public static readonly Error NotFound = new("Donation.NotFound", "Donation does not exist or owned", ErrorType.NotFound);
            public static readonly Error AmountOutOfRange = new("Donation.AmountOutOfRange", "Amount outside DonationOptions range", ErrorType.UnprocessableEntity);
            public static readonly Error InvalidState = new("Donation.InvalidState", "Donation already in a final state", ErrorType.Conflict);
            public static readonly Error CampaignIdRequired = new("Donation.CampaignIdRequired", "Campaign ID is required", ErrorType.Validation);
            public static readonly Error DonorUserIdRequired = new("Donation.DonorUserIdRequired", "Donor user ID is required", ErrorType.Validation);
            public static readonly Error AmountRequired = new("Donation.AmountRequired", "Donation amount is required", ErrorType.Validation);
            public static readonly Error FailureReasonRequired = new("Donation.FailureReasonRequired", "Failure reason is required", ErrorType.Validation);
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
            public static readonly Error DonationisIdRequired = new("PaymentAttempt.AmountRequired", "Payment amount is required", ErrorType.Validation);
            public static readonly Error PaymentAttemptFailureCodeRequired = new ("PaymentAttempt.FailureCodeRequired", "Failure code is required", ErrorType.Validation);
            public static readonly Error PaymentIntentIdRequired = new ("PaymentAttempt.PaymentIntentIdRequired", "Payment intent ID is required", ErrorType.Validation);
            public static readonly Error PaymentAttemptCheckoutSessionIdRequired = new ("PaymentAttempt.CheckoutSessionIdRequired", "Stripe checkout session ID is required", ErrorType.Validation);
            public static readonly Error PaymentAttemptInvalidState = new ("PaymentAttempt.InvalidState", "Payment attempt is not in a created state", ErrorType.Conflict);
            public static readonly Error PaymentAttemptCheckoutSessionAlreadySet = new ("PaymentAttempt.CheckoutSessionAlreadySet", "Stripe checkout session ID has already been set", ErrorType.Conflict);
        } 

        public static class Idempotency
        {
            public static readonly Error KeyReusedWithDifferentRequest = new("Idempotency.KeyReusedWithDifferentRequest", "Same Idempotency-Key with a diffrent body", ErrorType.UnprocessableEntity);
            public static readonly Error UserIdRequired = new("Idempotency.UserIdRequired", "User is required", ErrorType.Validation);
            public static readonly Error RequestHashRequired = new("Idempotency.RequestHashRequired", "Request hash is required", ErrorType.Validation);
            public static readonly Error InvalidExpiration = new("Idempotency.InvalidExpiration", "Expiration must be after creation", ErrorType.Validation);
            public static readonly Error ResponseBodyRequired = new("Idempotency.ResponseBodyRequired", "Response body is required", ErrorType.Validation);
        }

        public static class User
        {
            public static readonly Error NotFound = new("User.NotFound", "User does not exist", ErrorType.NotFound);
            public static readonly Error IdRequired = new("User.IdRequired", "User ID is required", ErrorType.Validation);
            public static readonly Error FullNameRequired = new("User.FullNameRequired", "Full name is required", ErrorType.Validation);
            public static readonly Error InvalidLanguage = new("User.InvalidLanguage", "Preferred language must be en or ar", ErrorType.Validation);
            public static readonly Error AlreadyDeactivated = new("User.AlreadyDeactivated", "User is already deactivated", ErrorType.Conflict);
            public static readonly Error AlreadyActivated = new("User.AlreadyActivated", "User is already activated", ErrorType.Conflict);
        }
        
        public static class ProcessedStripeEvent
        {
            public static readonly Error EventIdIsRequired = new("ProcessedStripeEvent.EventIdIsRequired", "Event id is required", ErrorType.Validation);
            public static readonly Error EventTypeIsRequired = new("ProcessedStripeEvent.EventTypeIsRequired", "Event Type is required", ErrorType.Validation);
        }

        public static class RefreshToken
        {
            public static readonly Error UserIdRequired = new("RefreshToken.UserIdRequired", "User is required", ErrorType.Validation);
            public static readonly Error TokenHashRequired = new("RefreshToken.TokenHashRequired", "Token hash is required", ErrorType.Validation);
            public static readonly Error InvalidExpiration = new("RefreshToken.InvalidExpiration", "Expiration must be after creation", ErrorType.Validation);
            public static readonly Error AlreadyRevoked = new("RefreshToken.AlreadyRevoked", "Token is already revoked", ErrorType.Conflict);
            public static readonly Error Expired = new("RefreshToken.Expired", "Token has expired", ErrorType.Conflict);
            public static readonly Error InvalidReplacementTokenId = new("RefreshToken.InvalidReplacementTokenId", "Replacement token ID is invalid", ErrorType.Validation);
            public static readonly Error NotActive = new("RefreshToken.NotActive", "Token is not active", ErrorType.Conflict);
        }

        public static class Money
        {
            public static readonly Error InvalidAmount = new("Money.InvalidAmount", "Amount must be greater than zero", ErrorType.Validation);
            public static readonly Error CurrencyRequired = new("Money.CurrencyRequired", "Currency is required", ErrorType.Validation);
            public static readonly Error InvalidCurrency = new("Money.InvalidCurrency", "Currency must be a three-letter code", ErrorType.Validation);
        }
    }
}
