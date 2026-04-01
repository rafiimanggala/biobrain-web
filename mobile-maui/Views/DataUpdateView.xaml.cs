using System;
using System.ComponentModel;
using BioBrain.AppResources;
using BioBrain.Extensions;
using BioBrain.Interfaces;
using BioBrain.ViewModels.Interfaces;
using Common;
using Common.Enums;
using Common.Interfaces;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace BioBrain.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DataUpdateView
	{
		readonly IErrorLog logger = DependencyService.Get<IErrorLog>();
		private readonly IAnalyticTracker analyticTracker = DependencyService.Get<IAnalyticTracker>();

		private IDataUpdateViewModel ViewModel
		{
			get => (IDataUpdateViewModel)BindingContext;
			set => BindingContext = value;
		}

		public DataUpdateView(IDataUpdateViewModel viewModel) : base(MenuItemsEnum.More)
		{
			logger.Log("Data Update View");
			analyticTracker.SetView("Data Update Page", nameof(AccountView));
			InitializeComponent();
			ViewModel = viewModel;
			ViewModel.Error += ViewModelOnError;
			ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
			ViewModel.NoEmailError += ViewModelOnNoEmailError;
			ViewModel.NavigateHome += ViewModelOnNavigateHome;
			ViewModel.ApplicationUpdateNeed += ViewModelOnApplicationUpdateNeed;
		}

		private async void ViewModelOnNavigateHome(object sender, EventArgs e)
		{
			await Navigation.MoveToHome();
		}

		private async void ViewModelOnApplicationUpdateNeed(object sender, string message)
		{
			if (await DisplayAlert(StringResource.UpgradeString, message,
				StringResource.NavigateToString +
				$" {(Device.RuntimePlatform == Device.iOS ? StringResource.AppStore : StringResource.GooglePlay)}",
				StringResource.CancelString))
			{

				var paths = DependencyService.Get<IFilesPath>();
				await Launcher.OpenAsync(new Uri(paths.MarketUrl));
			}
			Application.Current.Quit();
		}

		private async void ViewModelOnNoEmailError(object sender, EventArgs e)
		{
			await Navigation.MoveToAuthorization();
		}

		private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(ViewModel.IsBusy))
			{
				if(ViewModel.IsBusy) StartProcess();
				else EndProcess();
			}
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();
			await ViewModel.CheckForUpdatesInternal();
		}

		private void ViewModelOnError(object sender, string e)
		{
			DisplayAlert(StringResource.ErrorString, e, StringResource.OkString);
		}

		private async void DownloadButton_OnClicked(object sender, object e)
		{
			await ViewModel.DownloadAndUpdate();
		}

		private async void RefreshView_OnRefreshing(object sender, EventArgs e)
		{
			var refreshView = (RefreshView) sender;
			if (refreshView != null)
				refreshView.IsRefreshing = false;

			await ViewModel.CheckForUpdatesInternal();
		}
	}
}