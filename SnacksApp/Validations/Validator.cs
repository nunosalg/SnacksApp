using System.Text.RegularExpressions;

namespace SnacksApp.Validations
{
    public class Validator : IValidator
    {
        public string NameError { get; set; } = "";

        public string EmailError { get; set; } = "";

        public string PhoneError { get; set; } = "";

        public string PasswordError { get; set; } = "";

        private const string EmptyNameErrorMsg = "Please, type your name.";
        private const string InvalidNameErrorMsg = "Please, type a valid name.";
        private const string EmptyEmailErrorMsg = "Please, type your email.";
        private const string InvalidEmailErrorMsg = "Please, type a valid email.";
        private const string EmptyPhoneErrorMsg = "Please, type your phone.";
        private const string InvalidPhoneErrorMsg = "Please, type a valid phone.";
        private const string EmptyPasswordErrorMsg = "Please, type your password.";
        private const string InvalidPasswordErrorMsg = "Password must contain 8 characters, including letters and numbers.";

        public Task<bool> Validate(string name, string email, string phone, string password)
        {
            var isNameValid = ValidateName(name);
            var isEmailValid = ValidateEmail(email);
            var isPhoneValid = ValidatePhone(phone);
            var isPasswordValid = ValidatePassword(password);

            return Task.FromResult(isNameValid && isEmailValid && isPhoneValid && isPasswordValid);
        }

        private bool ValidateName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                NameError = EmptyNameErrorMsg;
                return false;
            }

            if (name.Length < 3)
            {
                NameError = InvalidNameErrorMsg;
                return false;
            }

            NameError = "";
            return true;
        }

        private bool ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                EmailError = EmptyEmailErrorMsg;
                return false;
            }

            if (!Regex.IsMatch(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
            {
                EmailError = InvalidEmailErrorMsg;
                return false;
            }

            EmailError = "";
            return true;
        }

        private bool ValidatePhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                PhoneError = EmptyPhoneErrorMsg;
                return false;
            }

            if (phone.Length < 12)
            {
                PhoneError = InvalidPhoneErrorMsg;
                return false;
            }

            PhoneError = "";
            return true;
        }

        private bool ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                PasswordError = EmptyPasswordErrorMsg;
                return false;
            }

            if (password.Length < 8 || !Regex.IsMatch(password, @"[a-zA-Z]") || !Regex.IsMatch(password, @"\d"))
            {
                PasswordError = InvalidPasswordErrorMsg;
                return false;
            }

            PasswordError = "";
            return true;
        }
    }
}
