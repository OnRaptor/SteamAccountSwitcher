using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamAccountSwitcher.ViewModels.Components;
using SteamAccountSwitcher.ViewModels.Dialogs;

namespace SteamAccountSwitcher.ViewModels.Framework
{
    public interface IViewModelFactory
    {
        AccountItemViewModel CreateAccountItemViewModel();
        AddDialogViewModel CreateAddDialogViewModel();
    }
}
