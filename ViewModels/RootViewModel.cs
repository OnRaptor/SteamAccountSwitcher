using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stylet;
using SteamAccountSwitcher.Model;
using SteamAccountSwitcher.Services;
using SteamAccountSwitcher.ViewModels.Framework;
using SteamAccountSwitcher.ViewModels.Components;
using MaterialDesignThemes.Wpf;
using SteamAccountSwitcher.ViewModels.Dialogs;
using System.Windows;
using System.Xml;
using System.Diagnostics;

namespace SteamAccountSwitcher.ViewModels
{
    public class RootViewModel : Screen
    {
        public BindableCollection<AccountItemViewModel> AccountList { get; set; } = new BindableCollection<AccountItemViewModel>();
        private readonly IViewModelFactory _viewModelFactory;
        private readonly DialogManager _dialogManager;
        public bool Loading { get; set; } = false;
        public bool showMenu { get; set; } = false;
        public SnackbarMessageQueue Notifications { get; } = new(TimeSpan.FromSeconds(5));
        public RootViewModel(IViewModelFactory viewModelFactory, DialogManager dialogManager)
        {
            _viewModelFactory = viewModelFactory;
            _dialogManager = dialogManager;
        }

        public async void Login(AccountItemViewModel acc)
        {
            await Task.Run(() =>
            {
                Notifications.Enqueue("Running Steam...");
                SteamProvider.Login(acc.account.login, acc.account.password);
            });
        }

        public async void AddAccount()
        {
            var account = await _dialogManager.ShowDialogAsync(_viewModelFactory.CreateAddDialogViewModel());
            if (account == null)
                return;
            AccountDataStorage.AddAccount(account);
            AccountList.Add(_viewModelFactory.CreateAccountItemViewModel(await SteamProvider.FetchProfileData(account, () => Notifications.Enqueue($"Failed to update profile data from steam for {account.login} account", "Close", () => Notifications.Clear()))));
        }
        public void RemoveAccount(AccountItemViewModel acc)
        {
            AccountDataStorage.DeleteAccount(acc.account);
            AccountList.Remove(acc);
        }
        public async void EditAccount(AccountItemViewModel acc)
        {
            var copy = acc.account.Clone() as Account;
            var account = await _dialogManager.ShowDialogAsync(_viewModelFactory.CreateAddDialogViewModel(acc.account));
            if (account == null)
                return;
            if (!string.IsNullOrEmpty(acc.account.SteamUrl))
                acc.account = await SteamProvider.FetchProfileData(account, null);
            if (!AccountDataStorage.ReplaceAccount(acc.account, copy))
                Notifications.Enqueue($"Failed to replace account, check accounts.json file");
            if (!string.IsNullOrEmpty(acc.account.SteamUrl))
                UpdateAccountList();
        }
        private async void UpdateAccountList()
        {
            Loading = true;
            var accs = AccountDataStorage.ReadAccounts();
            AccountList.Clear();
            foreach (var account in accs)
                AccountList.Add(_viewModelFactory.CreateAccountItemViewModel(await SteamProvider.FetchProfileData(account, () => Notifications.Enqueue("Failed to update profile data from steam", "Close", () => Notifications.Clear()))));
            Loading = false;


            //caching
            accs.Clear();
            foreach (var _acc in AccountList)
                accs.Add(_acc.account);

            AccountDataStorage.SaveAccounts(accs);
        }
        public async void FetchAccountInfo()
        {
            Loading = true;
            var accs = AccountList.ToArray();
            AccountList.Clear();
            foreach (var account in accs)
                AccountList.Add(_viewModelFactory.CreateAccountItemViewModel(await SteamProvider.FetchProfileData(account.account, () => Notifications.Enqueue("Failed to fetch info"), false)));
            Loading = false;
        }


        public async void OnViewFullyLoaded()
        {
            var accs = AccountDataStorage.ReadAccounts();
            if (accs.Count == 0)
            {
                var account = await _dialogManager.ShowDialogAsync(_viewModelFactory.CreateAddDialogViewModel());
                AccountDataStorage.AddAccount(account);
                UpdateAccountList();
            }
            else
                UpdateAccountList();

            var steam_status = SteamProvider.SteamStatus();
            if (steam_status != "OK")
                Notifications.Enqueue("Bad steam status\n" + steam_status, "OK", null);

            App.Current.MainWindow.Title = "Steam Account Switcher " + SteamProvider.GetLocationSteam();
            Notifications.DiscardDuplicates = true;
        }

        public void openUri(string url) => System.Diagnostics.Process.Start("explorer.exe", url);

        public void toogleMenu() => showMenu = !showMenu;

    }
}
