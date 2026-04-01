using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BioBrain.Annotations;
using BioBrain.ViewModels.Interfaces;
using Common;
using DAL.Models.Interfaces;
using DAL.Repositorys.Interfaces;
// using Unity; // Replaced by MAUI DI
using Microsoft.Maui.Controls;

namespace BioBrain.ViewModels.Implementation
{

    public delegate void AccountDataUpdate();

    public class AccountViewModel : IAccountViewModel, INotifyPropertyChanged
    {
        private static AccountDataUpdate updateDelegate;

        public List<string> GenderList => new List<string> { StringResource.UnspecifiedString, StringResource.ManString, StringResource.WomanString};

        public List<string> EducationLevelList
            =>
                new List<string>
                {
                    StringResource.UnspecifiedString,
                    StringResource.PrimaryString,
                    StringResource.SecondaryString,
                    StringResource.TertiaryString,
                    StringResource.OtherString,
                    StringResource.NotStudingString
                };

        public bool IsEmailEditable => Device.RuntimePlatform == Device.iOS;

        public string FirstName
        {
            get { return model.FirstName; }
            set { model.FirstName = value; }
        }

        public string Surname
        {
            get { return model.Surname; }
            set { model.Surname = value; }
        }

        public int Gender
        {
            get { return model.Gender; }
            set { model.Gender = value; }
        }

        public DateTime DateOfBirdth
        {
            get { return model.DateOfBirdth == DateTime.MinValue ? DateTime.Now : model.DateOfBirdth; }
            set
            {
                model.DateOfBirdth = value; 
                OnPropertyChanged(nameof(IsDoBVisible));
            }
        }

        public bool IsDoBVisible => model.DateOfBirdth != DateTime.MinValue && model.DateOfBirdth != DateTime.Today;

        public int EducationLevel
        {
            get { return model.EducationLevel; }
            set { model.EducationLevel = value; }
        }

        public string AvatarPath {
            get => string.IsNullOrEmpty(model.AvatarPath) ? string.Empty : model.AvatarPath;
            set
            {
                model.AvatarPath = value;
                OnPropertyChanged(nameof(AvatarPath));
            }
        }

        public string Email {
            get => model.Email;
            set => model.Email = value;
        }

        private readonly IAccountRepository accountRepository;
        private IAccountModel model;

        public AccountViewModel(IAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
            updateDelegate = GetModel;
            GetModel();
        }

        private void GetModel()
        {
            model = accountRepository.GetAccountModel() ?? App.Container.Resolve<IAccountModel>();
            OnPropertyChanged(nameof(AvatarPath));
            OnPropertyChanged(nameof(IsDoBVisible));
        }

        public void Save()
        {
            if (model.AccountID < 1) accountRepository.Insert(model);
            else accountRepository.Update(model);
        }

        public void Update()
        {
            updateDelegate?.Invoke();
        }

        //public async Task GetEmailAndRetry()
        //{
        //    var email = await firebaseService.GetEmail();
        //    if (string.IsNullOrEmpty(email)) return;
        //    Email = email;
        //    OnPropertyChanged(nameof(Email));
        //}

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}