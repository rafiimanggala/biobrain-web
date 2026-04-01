using System.Collections.Generic;
using System.Linq;
using DAL.Models.Implementations;
using DAL.Models.Interfaces;
using DAL.Repositorys.Interfaces;
using SQLite;
using Microsoft.Maui.Controls;

namespace DAL.Repositorys.Implementations
{
    public class TopicsRepository : BaseRepository<ITopicModel>, ITopicsRepository
    {
        public List<ITopicModel> GetAll()
        {
            using (var database = GetConnection())
            {
                return (from i in database.Table<TopicModel>() select i).ToList<ITopicModel>();
            }
        }

        public List<ITopicModel> GetForArea(int areaID)
        {
            using (var database = GetConnection())
            {
                return database.Query<TopicModel>($"Select * From Topics Where AreaID = {areaID}").ToList<ITopicModel>();
            }
        }

        public ITopicModel GetByAreaAndOrder(int areaID, int order)
        {
            using (var database = GetConnection())
            {
                return database.Query<TopicModel>($"Select * From Topics Where AreaID = {areaID} and TopicOrder = {order}").ToList<ITopicModel>().FirstOrDefault();
            }
        }

        public ITopicModel GetByID(int topicID)
        {
            using (var database = GetConnection())
            {
                return database.Get<TopicModel>(topicID);
            }
        }

        public string GetNameByID(int topicID)
        {
            using (var database = GetConnection())
            {
                return database.ExecuteScalar<string>($"Select TopicName From Topics Where TopicID = {topicID}");
            }
        }

        public int CountForArea(int areaID)
        {
            using (var database = GetConnection())
            {
                return database.ExecuteScalar<int>($"Select Count(*) From Topics where AreaID = {areaID}");
            }
        }

        public int CountDoneForArea(int areaID)
        {
            using (var database = GetConnection())
            {
                    var doneTopicIds = database.Query<int>($@"Select TopicID from DoneTopics");
                    return database.ExecuteScalar<int>($@"Select Count(*) From Topics 
                                                         where AreaID = {areaID} and TopicID IN 
                                                         ({string.Join(", ", doneTopicIds)})");
            }
        }
    }
}
