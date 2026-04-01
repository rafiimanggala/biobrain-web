using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using DAL.Models.Implementations;
using DAL.Models.Interfaces;
using DAL.Repositorys.Interfaces;

namespace DAL.Repositorys.Implementations
{
    public class DoneMaterialsRepository : BaseRepository<IDoneMaterialModel>, IDoneMaterialsRepository
    {
        public DoneMaterialsRepository() : base()
        {

        }

        public DoneMaterialsRepository(AppContentType contentType) : base(contentType)
        {

        }

        public List<IDoneMaterialModel> GetAll()
        {
            using (var database = GetConnection())
            {
                return (from i in database.Table<DoneMaterialModel>() select i).ToList<IDoneMaterialModel>();
            }
        }

        public IDoneMaterialModel GetByID(int doneMaterialID)
        {
            using (var database = GetConnection())
            {
                return database.Get<DoneMaterialModel>(doneMaterialID);
            }
        }

        public bool IsMaterialDone(int materilaID)
        {
            using (var database = GetConnection())
            {
                return database.ExecuteScalar<int>($"Select Count(*) From DoneMaterials where MaterialID = {materilaID}") > 0;
            }
        }

        public List<IDoneMaterialModel> GetEntriesForMaterial(int materialID)
        {
            using (var database = GetConnection())
            {
                return database.Query<DoneMaterialModel>($"Select * From DoneMaterials where MaterialID = {materialID}").ToList<IDoneMaterialModel>();
            }
        }

        public void Clean()
        {
            using (var database = GetConnection())
            {
                database.Execute($"TRUNCATE TABLE DoneMaterials");
            }
        }
    }
}