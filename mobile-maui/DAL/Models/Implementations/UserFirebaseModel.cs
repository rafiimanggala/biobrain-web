using System.Collections.Generic;
using DAL.Models.Interfaces;

namespace DAL.Models.Implementations
{
    public class UserFirebaseModel : IUserFirebaseModel
    {
        public string AvatarPath { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public Dictionary<string, FirebasePurchaseModel> Purchases { get; set; }

        public IRegistrationModel ToRegistrationModel()
        {
            var model = new RegistrationModel
            {
                Password = Password,
                Email = Email,
                State = State,
                Country = Country,
                Surname = Surname,
                FirstName = FirstName,
                AvatarPath = AvatarPath,
            };
            return model;
        }
    }
}