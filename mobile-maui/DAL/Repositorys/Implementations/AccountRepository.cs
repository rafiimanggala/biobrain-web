using System;
using DAL.Models.Implementations;
using DAL.Models.Interfaces;
using DAL.Repositorys.Interfaces;
using Common.Interfaces;
using Newtonsoft.Json;
using Microsoft.Maui.Controls;

namespace DAL.Repositorys.Implementations
{
    public class AccountRepository : BaseRepository<IAccountModel>, IAccountRepository
    {
        private readonly IAccountDataProvider accountProvider = DependencyService.Get<IAccountDataProvider>();
        public IAccountModel GetAccountModel()
        {
            var json = accountProvider.GetAccountData();
            if (string.IsNullOrEmpty(json)) return null;

            var model = JsonConvert.DeserializeObject<AccountModel>(json);
            return model;
        }

        public IRegistrationModel GetRegisterModel()
        {
            return (IRegistrationModel)GetAccountModel();
        }

        public override bool Insert(IAccountModel model)
        {
            accountProvider.SetAccountData(JsonConvert.SerializeObject(model));
            return true;
        }

        public override bool Update(IAccountModel model)
        {
            return Insert(model);
        }

        public bool RemoveLocalAccount()
        {
            accountProvider.SetAccountData(string.Empty);
            return true;
        }

        public void AddUpdateAvailableDate()
        {
	        var model = GetAccountModel();
	        model.UpdateAvailableDate = DateTime.UtcNow.ToString("d");
	        Update(model);
        }

        public void DeleteUpdateAvailableDate()
        {
	        var model = GetAccountModel();
	        model.UpdateAvailableDate = "";
	        Update(model);
        }
    }
}