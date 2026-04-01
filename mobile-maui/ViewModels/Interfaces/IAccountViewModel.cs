using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BioBrain.ViewModels.Interfaces
{
    public interface IAccountViewModel
    {
        bool IsEmailEditable { get; }

        List<string> GenderList { get; }

        List<string> EducationLevelList { get; }

        string FirstName { get; set; }

        string Surname { get; set; }

        int Gender { get; set; }

        DateTime DateOfBirdth { get; set; }

        int EducationLevel { get; set; }

        string AvatarPath { get; set; }

        string Email { get; set; }

        bool IsDoBVisible { get; }

        void Save();

        void Update();

        //Task GetEmailAndRetry();
    }
}