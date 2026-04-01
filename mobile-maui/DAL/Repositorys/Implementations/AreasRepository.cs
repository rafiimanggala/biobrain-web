using System.Collections.Generic;
using System.Linq;
using DAL.Models.Implementations;
using DAL.Models.Interfaces;
using DAL.Repositorys.Interfaces;

namespace DAL.Repositorys.Implementations
{
    public class AreasRepository : BaseRepository<IAreaModel>, IAreasRepository
    {
        public List<IAreaModel> GetAll()
        {
            using (var database = GetConnection())
            {
                return (from i in database.Table<AreaModel>() select i).ToList<IAreaModel>();
            }
        }

        public string GetNameByID(int areaID)
        {
            using (var database = GetConnection())
            {
                return database.ExecuteScalar<string>($"Select AreaName From Areas Where AreaID = {areaID}");
            }
        }

        public IAreaModel GetByID(int areaID)
        {
            using (var database = GetConnection())
            {
                return database.Get<AreaModel>(areaID);
            }
        }
    }
}
