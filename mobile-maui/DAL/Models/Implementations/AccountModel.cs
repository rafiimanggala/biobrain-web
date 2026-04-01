using System;
using DAL.Models.Interfaces;
using SQLite;

namespace DAL.Models.Implementations
{
    [Table("Account")]
    public class AccountModel : IAccountModel, IRegistrationModel
    {
        [PrimaryKey, AutoIncrement]
        public int AccountID { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public int Gender { get; set; }

        public DateTime DateOfBirdth { get; set; }

        public int EducationLevel { get; set; }

        public string AvatarPath { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Country { get; set; }

        public string State { get; set; }

        public DateTime? SubscriptionDate { get; set; }

        public string SubscriptionId { get; set; }

        public string UpdateAvailableDate { get; set; }
    }
}