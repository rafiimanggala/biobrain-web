using System.Collections.Generic;
using System.Linq;
using DAL.Models.Implementations;
using DAL.Models.Interfaces;
using DAL.Repositorys.Interfaces;

namespace DAL.Repositorys.Implementations
{
    public class LevelTypesRepository : BaseRepository<ILevelTypeModel>, ILevelTypesRepository
    {
        public int FirstLevelID => GetFirstLevelId();

        public List<ILevelTypeModel> GetAll()
        {
            using (var database = GetConnection())
            {
                return (from i in database.Table<LevelTypeModel>() select i).ToList<ILevelTypeModel>();
            }
        }

        public ILevelTypeModel GetByID(int levelTypeID)
        {
            using (var database = GetConnection())
            {
                return database.Get<LevelTypeModel>(levelTypeID);
            }
        }

        public int GetFirstLevelId()
        {
            using (var database = GetConnection())
            {
                return (from i in database.Table<LevelTypeModel>() select i).ToList<ILevelTypeModel>().Min(l=>l.LevelTypeID);
            }
        }
    }
}