using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using DAL.Models.Implementations;
using DAL.Models.Interfaces;
using DAL.Repositorys.Interfaces;

namespace DAL.Repositorys.Implementations
{
    public class DoneAreasRepository : BaseRepository<IDoneAreaModel>, IDoneAreasRepository
    {
        public DoneAreasRepository():base()
        {

        }

        public DoneAreasRepository(AppContentType contentType): base(contentType)
        {

        }

        public List<IDoneAreaModel> GetAll()
        {
            using (var database = GetConnection())
            {
                return (from i in database.Table<DoneAreaModel>() select i).ToList<IDoneAreaModel>();
            }
        }

        public bool IsAreaDone(int areaID)
        {
            using (var database = GetConnection())
            {
                return database.ExecuteScalar<int>($"Select Count(*) From DoneAreas where AreaID = {areaID}") > 0;
            }
        }
    }
}
