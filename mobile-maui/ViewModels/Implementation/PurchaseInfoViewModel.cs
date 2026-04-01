using System.Collections.Generic;
using BioBrain.ViewModels.Interfaces;
using DAL.Models.Interfaces;

namespace BioBrain.ViewModels.Implementation
{
    public class PurchaseInfoViewModel : IPurchaseInfoViewModel
    {
        private readonly KeyValuePair<string, IPurchaseInfo> model;
        public PurchaseInfoViewModel(KeyValuePair<string, IPurchaseInfo> model)
        {
            this.model = model;
        }

        public string Name => model.Value.Name;
        public string PurchaseId => model.Value.ProductId;
        public string PurchaseLocalId => model.Key;
        public double Cost => model.Value.Cost;
        public string PurchaseDisplayName => $"{Name} - ${Cost}";
    }
}