using Microsoft.AspNetCore.Identity;

namespace ASSC.Services
{
    public class RussianIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError
            {
                Description = $"Пароль должен содержать минимум {length} символов."
            };
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError
            {
                Description = "Пароль должен содержать хотя бы один спецсимвол."
            };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError
            {
                Description = "Пароль должен содержать хотя бы одну строчную букву."
            };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError
            {
                Description = "Пароль должен содержать хотя бы одну заглавную букву."
            };
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError
            {
                Description = "Пароль должен содержать хотя бы одну цифру."
            };
        }
    }
}