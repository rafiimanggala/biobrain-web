using System;
using System.Linq;
using System.Threading.Tasks;
using BioBrain.Extensions;
using BioBrain.ViewModels;
using Common;
using Common.Enums;
using Common.Interfaces;
using CustomControls.Dialogs;
using Microsoft.Maui.Controls;

namespace BioBrain.Views
{
    public class BasePurchaseView : BaseView
    {
	    protected SubscriptionDialog SubscriptionDialog;
	    protected Grid SpinnerLayout;
	    protected bool IsQuestionView = false;
	    protected bool IsAreasView = false;
	    public event EventHandler Purchase;
	    //private bool isDialogOpen = false;
	    private readonly IErrorLog logger = DependencyService.Get<IErrorLog>();

        public BasePurchaseView(MenuItemsEnum menuItem) : base(menuItem)
        {
	        InitComponents();
        }

        private void InitComponents()
        {
	        var grid = new Grid();
	        grid.Children.Add(new ActivityIndicator
	        {
		        HorizontalOptions = LayoutOptions.CenterAndExpand,
		        VerticalOptions = LayoutOptions.CenterAndExpand,
		        Style = (Style)Application.Current.Resources["SpinnerStyle"],
		        GestureRecognizers = { new TapGestureRecognizer { Command = new Command(() => {/*Fake gesture for ios to prevent click through*/}) } },
		        IsRunning = true
	        });
	        SpinnerLayout = grid;
        }

        protected void StartProcess()
        {
	        var baseGrid = (Grid)InternalChildren.First();

	        if (baseGrid != null)
	        {
	            Grid.SetColumn(SpinnerLayout, 0);
	            Grid.SetRow(SpinnerLayout, 0);
	            baseGrid.Children.Add(SpinnerLayout);
	        }
        }

        protected void EndProcess()
        {
	        var baseGrid = (Grid)InternalChildren.First();

	        baseGrid?.Children.Remove(SpinnerLayout);
        }

        protected override async void OnAppearing()
        {
	        base.OnAppearing();

	        try
	        {
		        var baseGrid = (Grid)InternalChildren.First();
		        if (baseGrid == null) return;

		        var oldDialog = baseGrid.Children.FirstOrDefault(x => x.GetType() == typeof(SubscriptionDialog));
		        if (oldDialog != null) baseGrid.Children.Remove(oldDialog);

#if !DEBUG
		        // Only fetch IAP prices in Release builds
		        if (string.IsNullOrEmpty(Settings.MonthSubscriptionPrice))
		        {
			        var price = await BaseViewModel.GetMonthPrice();
			        if (price != null)
			        {
				        Settings.MonthSubscriptionPrice = price.LocalizedPrice;
				        Settings.MicrosMonthSubscriptionPrice = price.MicrosPrice;
			        }
		        }

		        if (string.IsNullOrEmpty(Settings.YearSubscriptionPrice))
		        {
			        var price = await BaseViewModel.GetYearPrice();
			        if (price != null)
			        {
				        Settings.YearSubscriptionPrice = price.LocalizedPrice;
				        Settings.MicrosYearSubscriptionPrice = price.MicrosPrice;
			        }
		        }
#endif

		        SubscriptionDialog = new SubscriptionDialog(Settings.MonthSubscriptionPrice, Settings.YearSubscriptionPrice, ((double)Settings.MicrosYearSubscriptionPrice)/1000000);

		        Grid.SetColumn(SubscriptionDialog, 0);
		        Grid.SetRow(SubscriptionDialog, 0);
		        baseGrid.Children.Add(SubscriptionDialog);
		        SubscriptionDialog.Result += SubscriptionDialogOnResult;
	        }
	        catch (Exception ex)
	        {
		        logger.Log($"BasePurchaseView.OnAppearing error: {ex.Message}");
	        }
        }

        protected override void OnDisappearing()
        {
	        base.OnDisappearing();

	        if (SubscriptionDialog != null)
		        SubscriptionDialog.Result -= SubscriptionDialogOnResult;
        }

        private IBasePurchasableViewModel BaseViewModel => (IBasePurchasableViewModel)BindingContext;

        protected async Task Alert()
        {
            var isPurchaseConfirmed = false;
            if (BaseViewModel == null) return;

            if (Device.RuntimePlatform == Device.Android)
            {
                var price = await BaseViewModel.GetPrice();
                if (price == null) return;
                logger.Log($"Price get {price}");

                if (!BaseViewModel.IsAuthorized)
                {
                    logger.Log($"Not authorized");
                    var action = await DependencyService.Get<IActionSheet>().UseActionSheet(this,
                        string.Format(StringResource.GetFullVersionOrAuthorizeString, price.LocalizedPrice),
                        StringResource.CancelString, StringResource.GetFullString, StringResource.ProvideEmailString);
                    if (action == StringResource.CancelString)
                    {
                        logger.Log($"Cancel authorization");
                        return;
                    }

                    var account = BaseViewModel.GetAccount();
                    logger.Log($"Account get {account?.Email}");
                    if (string.IsNullOrEmpty(account?.Email)) return;
                    if (!await BaseViewModel.Authorize()) return;
                    isPurchaseConfirmed = action == StringResource.GetFullString;
                }

                if (await BaseViewModel.IsPurchased())
                {
                    await DisplayAlert(StringResource.PurchaseString, StringResource.AlreadyPurchasedMessage,
                        StringResource.OkString);
                    logger.Log($"Already purchased but not updated");
                    return;
                }

                var result = isPurchaseConfirmed || await DisplayAlert(StringResource.LiteVersionHeader,
                    string.Format(StringResource.GetFullVersionString, price), StringResource.GetFullString,
                    StringResource.CancelString);

                if (result)
                {
                    logger.Log($"Start purchase");
                    if (await BaseViewModel.MakePurchase())
                    {
                        logger.Log($"Purchase complete");
                        await Navigation.MoveToAreasAndInitUpdate();
                    }
                    else
                    {
                        logger.Log($"Purchase failed");
                        if (IsQuestionView)
                            await Navigation.MoveToTopicFromQuestion();
                    }
                }
            }

            if (Device.RuntimePlatform == Device.iOS)
            {
	            try
	            {
		            StartProcess();

		            if (!BaseViewModel.IsAuthorized)
		            {
			            if(!await BaseViewModel.Authorize()) return;
		            }

		            logger.Log($"Check purchase");
		            if (await BaseViewModel.IsPurchased())
		            {
			            if (
				            await DisplayAlert(StringResource.PurchaseString, StringResource.AlreadyPurchasedMessage,
					            StringResource.OkString, StringResource.CancelString))
				            await Navigation.MoveToUpdates(true);
			            logger.Log($"Purchase exist but not updated");
			            return;
		            }

		            //isDialogOpen = true;
		            logger.Log($"Show purchase dialog");
		            SubscriptionDialog.Show();
		            //while (isDialogOpen) await Task.Delay(100);
	            }
	            catch (Exception e)
	            {
					logger.Log(e.ToString());
	            }
                finally
                {
                    EndProcess();
                }
            }
        }

        private async void SubscriptionDialogOnResult(object sender, SubscriptionDialogResult result)
        {
	        try
	        {
		        logger.Log($"Close purchase dialog {result}");
		        if (result == SubscriptionDialogResult.Cancel) return;

		        StartProcess();

		        if (!BaseViewModel.IsAuthorized)
		        {
			        if (!await BaseViewModel.Authorize())
			        {
				        logger.Log($"Purchase - User not authorized");
				        return;
			        }
		        }

		        if (await BaseViewModel.MakePurchase(result))
		        {
			        logger.Log($"Purchase success");
			        // If on areas can't navigate to areas. Need start update.
			        //if (IsAreasView)
				       // OnPurchase();
			        //else
				       // await Navigation.MoveToAreasAndInitUpdate();
				    await Navigation.MoveToUpdates(true);
		        }
		        else
		        {
			        logger.Log($"Purchase failed");
			        if (IsQuestionView)
				        // if on question view need to finish quiz and navigate to topics
				        await Navigation.MoveToTopicFromQuestion();
		        }
	        }
	        catch (Exception e)
	        {
		        logger.Log(e.ToString());
		        await BaseViewModel.SendLogs();
	        }
	        finally
	        {
		        EndProcess();
	        }
        }

        public virtual void OnPurchase()
        {
	        Purchase?.Invoke(this, EventArgs.Empty);
        }
    }
}
