using SQLite;

namespace DAL
{
    public interface ISQLite
    {
        SQLiteConnection GetConnection();
        SQLiteConnection GetDemoConnection();
    }
}