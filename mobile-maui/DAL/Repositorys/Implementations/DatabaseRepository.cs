using System;
using System.Diagnostics;
using System.Linq;
using Common;
using DAL.Models.Implementations;
using DAL.Models.Interfaces;
using DAL.Repositorys.Interfaces;

namespace DAL.Repositorys.Implementations
{
    public class DatabaseRepository : BaseRepository<IDatabaseDataModel>, IDatabaseRepository
    {
        private readonly IMaterialsRepository materialsRepository;
        private readonly string FileErrorString = "FileError";
        private readonly string LastUpdateCheckKey = "LastUpdateCheck";
        private readonly string DataUpdateKey = "Date";
        private readonly string FileErrorValue = "1";

        public DatabaseRepository(IMaterialsRepository materialsRepository)
        {
            this.materialsRepository = materialsRepository;
        }

        public void AddFileError()
        {
            Insert(new DatabaseDataModel {Property = FileErrorString, Value = FileErrorValue});
        }

        public void AddLastUpdate()
        {
            using (var database = GetConnection())
            {
                try
                {
                    database
                        .Query<DatabaseDataModel>(
                            $"Delete From DatabaseData Where Property = '{LastUpdateCheckKey}'");
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            Insert(new DatabaseDataModel { Property = LastUpdateCheckKey, Value = DateTime.UtcNow.ToString() });
        }

        public string GetDatabaseDate()
        {
            using (var database = GetConnection())
            {
                try
                {
                    var entry = database
                        .Query<DatabaseDataModel>(
                            $"Select * From DatabaseData Where Property = '{DataUpdateKey}'").FirstOrDefault();
                    if (entry == null) return string.Empty;

                    return entry.Value;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message); 
                    return string.Empty;
                }
            }
        }

        public DateTime? GetLastUpdate()
        {
            using(var database = GetConnection())
            {
                try
                {
                    var model = database
                        .Query<DatabaseDataModel>(
                            $"Select * From DatabaseData Where Property = '{LastUpdateCheckKey}'")
                        .FirstOrDefault();
                    if (model == null) return null;

                    return DateTime.Parse(model.Value);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }

            return null;
        }

        public bool CheckIsFilesError()
        {
            using (var database = GetConnection())
            {
                try
                {
                    var model = database
                        .Query<DatabaseDataModel>(
                            $"Select * From DatabaseData Where Property = '{FileErrorString}'")
                        .FirstOrDefault();

                    database
                        .Query<DatabaseDataModel>(
                            $"Delete From DatabaseData Where Property = '{FileErrorString}'");

                    return !string.IsNullOrEmpty(model?.Value);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
                return false;
            }
        }

        //public bool CheckIsDemoDatabase()
        //{
        //    using (var database = GetConnection())
        //    {
        //        try
        //        {
        //            var model = database
        //                .Query<DatabaseDataModel>($"Select * From DatabaseData Where Property = '{Settings.DatabaseTypeProproperty}'")
        //                .FirstOrDefault();
        //            return model?.Value == Settings.DemoValue;
        //        }
        //        catch
        //        {}
        //        //For old versions
        //        try
        //        {
        //            return materialsRepository.CountAllForTopic(2) == 1;
        //        }
        //        catch (Exception)
        //        {
        //            // ignored
        //        }

        //        return true;
        //    }
        //}
    }
}