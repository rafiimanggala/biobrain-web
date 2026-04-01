using System.Collections.Generic;
using System.Linq;
using DAL.Models.Implementations;
using DAL.Models.Interfaces;
using DAL.Repositorys.Interfaces;

namespace DAL.Repositorys.Implementations
{
    public class AnswersRepository : BaseRepository<IAnswerModel>, IAnswersRepository
    {
        public List<IAnswerModel> GetAll()
        {
            using (var database = GetConnection())
            {
                return (from i in database.Table<AnswerModel>() select i).ToList<IAnswerModel>();
            }
        }

        public List<IAnswerModel> GetForQuestion(int questionID)
        {
            using (var database = GetConnection())
            {
                return database.Query<AnswerModel>($"Select * From Answers Where QuestionID = {questionID} Order by AnswerOrder").ToList<IAnswerModel>();
            }
        }

        public IAnswerModel GetByID(int answerID)
        {
            using (var database = GetConnection())
            {
                return database.Get<AnswerModel>(answerID);
            }
        }
    }
}