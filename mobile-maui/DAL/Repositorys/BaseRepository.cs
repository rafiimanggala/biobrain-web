using Common;
using Common.Enums;
using Common.Interfaces;
using SQLite;
using Microsoft.Maui.Controls;

namespace DAL.Repositorys
{
    public class BaseRepository <T> : IBaseRepository<T>
    {
        protected AppContentType ContentType;
        private readonly ISQLite sqLite = DependencyService.Get<ISQLite>();
        private readonly IProjectDataWorker dataWorker = DependencyService.Get<IProjectDataWorker>();

        public BaseRepository()
        {
            ContentType = Settings.ContentType;
        }

        public BaseRepository(AppContentType contentType)
        {
            ContentType = contentType;
        }

        public virtual SQLiteConnection GetConnection()
        {
	        return dataWorker.FullDataBaseExist()
		        ? ContentType == AppContentType.Full
			        ? sqLite.GetConnection()
			        : sqLite.GetDemoConnection()
		        : sqLite.GetDemoConnection();
        }

        public virtual bool Insert(T model)
        {
            using (var database = GetConnection())
            {
                return database.Insert(model) > 0;
            }
        }

        public virtual bool Update(T model)
        {
            using (var database = GetConnection())
            {
                return database.Update(model) > 0;
            }
        }

        public virtual bool Remove(T model)
        {
            using (var database = GetConnection())
            {
                return database.Delete(model) > 0;
            }
        }
    }
}
