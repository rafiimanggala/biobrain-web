using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Enums;
using DAL.Models.Implementations;
using DAL.Models.Interfaces;

namespace DAL.Repositorys.Interfaces
{
    public interface IFirebaseRepository
    {
        Task<UserFirebaseModel> GetUser();

        Task<bool> AddPurchase(IFirebasePurchaseModel purchase, PurchaseTypes type);

        Task<bool> Authorize(string email, string password, string appVersion);

        Task<bool> Register(IRegistrationModel model);
        Task UpdateUser(IRegistrationModel model);

        Task<int> GetDataVersion(string key);

        Task<string> GetMinimumAppVersion();
        Task<string> GetMinimumAppVersionMessage();

        Task<int> GetStructureVersion(string key);

        Task<bool> GetIsGetMetrics();

        Task<Dictionary<string, IFirebaseFileModel>> GetDemoFilesList(string key);
        Task<IFirebaseFileModel> GetDemoDatabaseInfo(string key);

        Task<Dictionary<string, IFirebaseFileModel>> GetFullFilesList(string key);
        Task<IFirebaseFileModel> GetFullDatabaseInfo(string key);

        Task<bool> DeletePurchase();


        bool IsAuthorized();
        Task SendLog(string log, string version, string code = LogTypes.Default);
        Task<IEnumerable<IVersionModel>> GetVersions();
        Task SendReview(string review);
        Task<string> UploadAvatar(string path);
        Task ResetPassword(string email);
        Task AddPurchaseHistory(IEnumerable<IFirebasePurchaseModel> purchases);

        Task AddReason(string reason);
        Task DeleteUser();
        Task DeleteAccount();
    }
}