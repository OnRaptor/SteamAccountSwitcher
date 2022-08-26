using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SteamAccountSwitcher.Views
{
    /// <summary>
    /// Логика взаимодействия для RootView.xaml
    /// </summary>
    public partial class RootView : Window
    {
        public RootView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            settings_popup.IsPopupOpen = !settings_popup.IsPopupOpen;
        }
    }
}
