using System.Collections.Generic;
using System.Linq;
using DAL.Models.Implementations;
using DAL.Models.Interfaces;
using DAL.Repositorys.Interfaces;

namespace DAL.Repositorys.Implementations
{
    public class GlossaryRepository : BaseRepository<IGlossaryModel>, IGlossaryRepository
    {
        public List<IGlossaryModel> GetAll()
        {
            using (var database = GetConnection())
            {
                return (from i in database.Table<GlossaryModel>() select i).ToList<IGlossaryModel>();
            }
        }

        public IGlossaryModel GetByID(int glossaryID)
        {
            using (var database = GetConnection())
            {
                return database.Get<GlossaryModel>(glossaryID);
            }
        }

        public IGlossaryModel GetByRef(string refID)
        {
            using (var database = GetConnection())
            {
                return database.Query<GlossaryModel>($"Select * From Glossary Where Ref Like {refID}").ToList<IGlossaryModel>().FirstOrDefault();
            }
        }

        public IGlossaryModel GetByTerm(string term)
        {
            using (var database = GetConnection())
            {
                return database.Query<GlossaryModel>($"Select * From Glossary Where Term Like '{term}'").ToList<IGlossaryModel>().FirstOrDefault();
            }
        }

        public List<IGlossaryModel> GetByLetter(string letter)
        {
            using (var database = GetConnection())
            {
                return database.Query<GlossaryModel>($"Select * From Glossary Where lower(Term) Like '{letter.ToLower()}%'").ToList<IGlossaryModel>();
            }
        }

        public List<IGlossaryModel> GetByText(string text)
        {
            using (var database = GetConnection())
            {
                return database.Query<GlossaryModel>($"Select * From Glossary Where lower(Term) Like '%{text.ToLower()}%'").ToList<IGlossaryModel>();
            }
        }
    }
}