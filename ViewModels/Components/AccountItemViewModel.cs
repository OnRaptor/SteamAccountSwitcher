using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stylet;
using SteamAccountSwitcher.ViewModels.Framework;
using SteamAccountSwitcher.Model;

namespace SteamAccountSwitcher.ViewModels.Components
{
    public class AccountItemViewModel : PropertyChangedBase
    {
        public Account account { get; set; }
    }
    public static class AccountItemViewModelExtensions
    {
        public static AccountItemViewModel CreateAccountItemViewModel(
            this IViewModelFactory factory,
            Account account
            )
        {
            var vm = factory.CreateAccountItemViewModel();
            vm.account = account;
            return vm;
        }
    }
}
