using Common.Enums;

namespace DAL.Repositorys
{
    public interface IBaseRepository <in T>
    {
        bool Insert(T model);
        bool Update(T model);
        bool Remove(T model);
    }
}