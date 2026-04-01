using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using DAL.Models.Implementations;
using DAL.Models.Interfaces;
using DAL.Repositorys.Interfaces;

namespace DAL.Repositorys.Implementations
{
    public class DoneQuestionRepository : BaseRepository<IDoneQuestionModel>, IDoneQuestionsRepository
    {
        public DoneQuestionRepository() : base()
        {

        }

        public DoneQuestionRepository(AppContentType contentType) : base(contentType)
        {

        }

        public List<IDoneQuestionModel> GetAll()
        {
            using (var database = GetConnection())
            {
                return (from i in database.Table<DoneQuestionModel>() select i).ToList<IDoneQuestionModel>();
            }
        }

        public IDoneQuestionModel GetByID(int doneQuestionID)
        {
            using (var database = GetConnection())
            {
                return database.Get<DoneQuestionModel>(doneQuestionID);
            }
        }

        public IDoneQuestionModel GetByQuestionID(int doneQuestionID)
        {
            using (var database = GetConnection())
            {
                return database.Query<DoneQuestionModel>($"Select * From DoneQuestions Where QuestionID = {doneQuestionID}").FirstOrDefault();
            }
        }

        public bool IsQuestionDone(int questionID)
        {
            using (var database = GetConnection())
            {
                return database.ExecuteScalar<int>($"Select Count(*) From DoneQuestions where QuestionID = {questionID}") > 0;
            }
        }

        public void Add(IDoneQuestionModel model)
        {
            using (var database = GetConnection())
            {
                database.ExecuteScalar<int>($"Insert Into DoneQuestions (QuestionID, Score, IsCorrect) VALUES ({model.QuestionID},{model.Score},{Convert.ToInt32(model.IsCorrect)})");
            }
        }

        public void DeleteByQuestionId(int questionID)
        {
            using (var database = GetConnection())
            {
                database.ExecuteScalar<int>($"Delete From DoneQuestions Where QuestionID={questionID}");
            }
        }
    }
}