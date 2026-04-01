using System;

namespace DAL.Models.Interfaces
{
    public interface IAccountModel
    {
        int AccountID { get; set; }
        string FirstName { get; set; }
        string Surname { get; set; }
        int Gender { get; set; }
        DateTime DateOfBirdth { get; set; }
        int EducationLevel { get; set; }
        string AvatarPath { get; set; }
        string Email { get; set; }
        string Password { get; set; }
        string Country { get; set; }
        string State { get; set; }
        DateTime? SubscriptionDate { get; set; }
        string SubscriptionId { get; set; }
        string UpdateAvailableDate { get; set; }
    }
}