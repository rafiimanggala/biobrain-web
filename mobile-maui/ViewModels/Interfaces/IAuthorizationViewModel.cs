using System;
using System.Threading.Tasks;

namespace BioBrain.ViewModels.Interfaces
{
    public interface IAuthorizationViewModel 
    {
        event EventHandler Finish;
        event EventHandler<string> Error;
        event EventHandler ResetPasswordEmailSent;

        string FirstName { get; set; }

        string LastName { get; set; }

        string AvatarPath { get; set; }

        string Email { get; set; }

        string Password { get; set; }

        string ConfirmPassword { get; set; }

        string Country { get; set; }

        string State { get; set; }

        Task Submit();
        void SetLoginMode();
        void SetRegisterMode();
        void SetResetMode();
    }
}