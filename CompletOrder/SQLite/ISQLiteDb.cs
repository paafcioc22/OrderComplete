

using SQLite;

namespace CompletOrder.SQLite
{
    public interface ISQLiteDb
    {
        SQLiteAsyncConnection GetConnection();
    }
}
