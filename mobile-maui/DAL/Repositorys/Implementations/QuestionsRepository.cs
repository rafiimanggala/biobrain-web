using System.Collections.Generic;
using System.Linq;
using DAL.Models.Implementations;
using DAL.Models.Interfaces;
using DAL.Repositorys.Interfaces;

namespace DAL.Repositorys.Implementations
{
    public class QuestionsRepository : BaseRepository<IQuestionModel>, IQuestionsRepository
    {
        public List<IQuestionModel> GetAll()
        {
            using (var database = GetConnection())
            {
                var a = (from i in database.Table<QuestionModel>() select i).ToList<QuestionModel>();
                return (from i in database.Table<QuestionModel>() select i).ToList<IQuestionModel>();
            }
        }

        public IQuestionModel GetByID(int questionID)
        {
            using (var database = GetConnection())
            {
                return database.Get<QuestionModel>(questionID);
            }
        }

        public List<IQuestionModel> GetByMaterial(int materialID)
        {
            using (var database = GetConnection())
            {
                return database.Query<QuestionModel>($"Select * From Questions Where MaterialID = {materialID}").ToList<IQuestionModel>();
            }
        }

        public List<IQuestionModel> GetByType(int type)
        {
            using (var database = GetConnection())
            {
                return database.Query<QuestionModel>($"Select * From Questions Where QuestionType = {type}").ToList<IQuestionModel>();
            }
        }

        public ITopicModel GetTopic(int materialID)
        {
            using (var database = GetConnection())
            {
                return database.Query<TopicModel>($"Select * From Topics Where TopicID = (Select TopicID From Materials Where MaterialID = {materialID})").ToList<ITopicModel>().FirstOrDefault() as ITopicModel;
            }
        }
    }
}