using DAL.Models.Interfaces;
using SQLite;

namespace DAL.Models.Implementations
{
    [Table("Account")]
    public class RegistrationModel : IRegistrationModel
    {
        [PrimaryKey, AutoIncrement]
        public int AccountID { get; set; }

        public string AvatarPath { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Country { get; set; }

        public string State { get; set; }
    }
}