using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stylet;

namespace SteamAccountSwitcher.ViewModels.Framework
{
    public abstract class DialogScreen<T> : PropertyChangedBase
    {
        public T? DialogResult { get; private set; }

        public event EventHandler? Closed;

        public void Close(T? dialogResult = default)
        {
            DialogResult = dialogResult;
            Closed?.Invoke(this, EventArgs.Empty);
        }
    }

    public abstract class DialogScreen : DialogScreen<bool?>
    {
    }
}
