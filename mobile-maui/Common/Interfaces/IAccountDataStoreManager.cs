using Common.Enums;

namespace Common.Interfaces
{
    public interface IAccountDataStoreManager
    {
        void SaveUserRateResult(RateResult result);
        void SaveEnterNumber(int enterNumber);
        RateResult GetUserRateResult();
        int GetEnterNumber();
    }
}