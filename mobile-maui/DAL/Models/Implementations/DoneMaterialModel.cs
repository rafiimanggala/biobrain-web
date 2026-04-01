using System;
using DAL.Models.Interfaces;
using SQLite;

namespace DAL.Models.Implementations
{
    [Table("DoneMaterials")]
    public class DoneMaterialModel : IDoneMaterialModel
    {
        [PrimaryKey, AutoIncrement]
        public int DoneMaterialID { get; set; }

        public int MaterialID { get; set; }

        public int Score { get; set; }

        public int Adge { get; set; }

        public DateTime Date { get; set; }
    }
}