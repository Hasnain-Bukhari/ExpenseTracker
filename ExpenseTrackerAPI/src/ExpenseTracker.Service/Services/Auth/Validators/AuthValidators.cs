using FluentValidation;
using ExpenseTracker.Dtos.Auth;

namespace ExpenseTracker.Service.Services.Auth.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(320);
            RuleFor(x => x.Password).MinimumLength(8).When(x => !string.IsNullOrEmpty(x.Password));
            RuleFor(x => x.AcceptTerms).Equal(true).WithMessage("Terms must be accepted");
        }
    }

    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
        }
    }

    public class SocialLoginRequestValidator : AbstractValidator<SocialLoginRequest>
    {
        public SocialLoginRequestValidator()
        {
            RuleFor(x => x.Provider).NotEmpty();
            RuleFor(x => x.Token).NotEmpty();
        }
    }

    public class RefreshRequestValidator : AbstractValidator<RefreshRequest>
    {
        public RefreshRequestValidator() => RuleFor(x => x.RefreshToken).NotEmpty();
    }

    public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
    {
        public ForgotPasswordRequestValidator() => RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }

    public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidator() => RuleFor(x => x.NewPassword).MinimumLength(8);
    }
}
