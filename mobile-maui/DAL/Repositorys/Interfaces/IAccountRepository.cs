using System.Collections.Generic;
using DAL.Models.Interfaces;

namespace DAL.Repositorys.Interfaces
{
    public interface IAccountRepository : IBaseRepository<IAccountModel>
    {
        IAccountModel GetAccountModel();
        IRegistrationModel GetRegisterModel();
        void AddUpdateAvailableDate();
        void DeleteUpdateAvailableDate();
        bool RemoveLocalAccount();
    }
}