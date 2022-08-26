using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamAccountSwitcher.ViewModels.Framework;
using SteamAccountSwitcher.Model;

namespace SteamAccountSwitcher.ViewModels.Dialogs
{
    public class AddDialogViewModel : DialogScreen<Account>
    {
        private readonly IViewModelFactory _viewModelFactory;
        private readonly DialogManager _dialogManager;
        public Account EditAccount { get; set; } = new Account("", "");

        public AddDialogViewModel(
        IViewModelFactory viewModelFactory,
        DialogManager dialogManager)
        {
            _viewModelFactory = viewModelFactory;
            _dialogManager = dialogManager;
        }

        public void Confirm()
        {
            Close(EditAccount);
        }
    }

    public static class AddDialogViewModelExtensions
    {
        public static AddDialogViewModel CreateAddDialogViewModel(this IViewModelFactory factory,
            Account editableAccount)
        {
            var vm = factory.CreateAddDialogViewModel();
            vm.EditAccount = editableAccount;
            return vm;
        }
    }

}
