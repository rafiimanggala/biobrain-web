using System.Collections.Generic;
using DAL.Models.Implementations;

namespace DAL.Models.Interfaces
{
    public interface IUserFirebaseModel
    {
        string FirstName { get; set; }
        string Surname { get; set; }
        string Email { get; set; }
        string Password { get; set; }
        string Country { get; set; }
        string State { get; set; }
        Dictionary<string, FirebasePurchaseModel> Purchases { get; set; }

        IRegistrationModel ToRegistrationModel();
    }
}