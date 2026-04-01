using System.Collections.Generic;
using System.Linq;
using DAL.Models.Implementations;
using DAL.Models.Interfaces;
using DAL.Repositorys.Interfaces;

namespace DAL.Repositorys.Implementations
{
    public class MaterialsRepository : BaseRepository<IMaterialModel>, IMaterialsRepository
    {
        public List<IMaterialModel> GetAll()
        {
            using (var database = GetConnection())
            {
                return (from i in database.Table<MaterialModel>() select i).ToList<IMaterialModel>();
            }
        }

        public List<IMaterialModel> GetForTopic(int topicID)
        {
            using (var database = GetConnection())
            {
                return database.Query<MaterialModel>($"Select * From Materials Where TopicID = {topicID}").ToList<IMaterialModel>();
            }
        }

        public IMaterialModel GetByID(int materialID)
        {
            using (var database = GetConnection())
            {
                return database.Get<MaterialModel>(materialID);
            }
        }

        public IMaterialModel GetForLevelAndTopic(int levelID, int topicID)
        {
            using (var database = GetConnection())
            {
                return database.Query<MaterialModel>($"Select * From Materials Where LevelTypeID = {levelID} AND TopicID = {topicID}").FirstOrDefault();
            }
        }

        public int CountDoneForTopic(int topicID)
        {
            using (var database = GetConnection())
            {
                var doneMaterials = database.Query<DoneMaterialModel>($@"Select * from DoneMaterials");
                var doneMaterialIds = doneMaterials.Select(dm => dm.MaterialID);
                return
                    database.ExecuteScalar<int>(
                        $@"Select Count(*) From Materials Where TopicID = {topicID} AND MaterialID IN ({
                            string.Join(", ", doneMaterialIds)})");
            }
        }

        public int CountAllForTopic(int topicID)
        {
            using (var database = GetConnection())
            {
                return database.ExecuteScalar<int>($"Select Count(*) From Materials Where TopicID = {topicID}");
            }
        }
    }
}