using System;

namespace DAL.Models.Interfaces
{
    public interface IDoneMaterialModel
    {
        int DoneMaterialID { get; set; }

        int MaterialID { get; set; }

        int Score { get; set; }

        int Adge { get; set; }
        DateTime Date { get; set; }
    }
}