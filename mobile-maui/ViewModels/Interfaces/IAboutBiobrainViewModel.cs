using System.Threading.Tasks;

namespace BioBrain.ViewModels.Interfaces
{
    public interface IAboutBiobrainViewModel: IBasePurchasableViewModel
    {
        bool IsSendPromocod { get; }

        bool CheckIsAuthorized();
        //Task GetEmailAndRetrySend();
        void SendPromocode();
        Task<bool> ActivatePromo();
    }
}