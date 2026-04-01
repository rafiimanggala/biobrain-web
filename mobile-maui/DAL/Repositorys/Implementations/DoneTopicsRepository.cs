using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using DAL.Models.Implementations;
using DAL.Models.Interfaces;
using DAL.Repositorys.Interfaces;

namespace DAL.Repositorys.Implementations
{
    public class DoneTopicsRepository : BaseRepository<IDoneTopicModel>, IDoneTopicsRepository
    {
        public DoneTopicsRepository() : base()
        {

        }

        public DoneTopicsRepository(AppContentType contentType) : base(contentType)
        {

        }

        public List<IDoneTopicModel> GetAll()
        {
            using (var database = GetConnection())
            {
                return (from i in database.Table<DoneTopicModel>() select i).ToList<IDoneTopicModel>();
            }
        }

        public bool IsTopicDone(int topicID)
        {
            using (var database = GetConnection())
            {
                return database.ExecuteScalar<int>($"Select Count(*) From DoneTopics where TopicID = {topicID}") > 0;
            }
        }
    }
}
