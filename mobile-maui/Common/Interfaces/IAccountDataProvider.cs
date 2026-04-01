namespace Common.Interfaces
{
    public interface IAccountDataProvider
    {
        string GetAccountData();
        void SetAccountData(string json);
    }
}