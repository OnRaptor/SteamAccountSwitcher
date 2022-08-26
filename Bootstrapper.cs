using System;
using Stylet;
using StyletIoC;
using SteamAccountSwitcher.ViewModels;
using SteamAccountSwitcher.Services;
using SteamAccountSwitcher.ViewModels.Framework;
using System.Windows.Threading;
using System.Windows;

namespace SteamAccountSwitcher
{
    public class Bootstrapper : Bootstrapper<RootViewModel>
    {
        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            base.ConfigureIoC(builder);
            builder.Bind<AccountDataStorage>().ToSelf().InSingletonScope();
            builder.Bind<IViewModelFactory>().ToAbstractFactory();
        }

        protected override void OnStart()
        {
            base.OnStart();

            AccountDataStorage.Init();
        }
#if !DEBUG
        protected override void OnUnhandledException(DispatcherUnhandledExceptionEventArgs e)
        {
            base.OnUnhandledException(e);

            MessageBox.Show(e.Exception.ToString(), "Error occured", MessageBoxButton.OK, MessageBoxImage.Error);
        }
#endif
    }
}
