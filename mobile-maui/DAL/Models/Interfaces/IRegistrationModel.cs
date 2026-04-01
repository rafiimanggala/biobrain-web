using System;

namespace DAL.Models.Interfaces
{
    public interface IRegistrationModel
    {
        int AccountID { get; set; }
        string AvatarPath { get; set; }
        string FirstName { get; set; }
        string Surname { get; set; }
        string Email { get; set; }
        string Password { get; set; }
        string Country { get; set; }
        string State { get; set; }
    }
}