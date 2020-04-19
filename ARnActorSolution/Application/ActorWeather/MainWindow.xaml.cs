using Actor.Base;
using Actor.Server;
using Actor.Util;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ActorWeather
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private StateFullStringCatcher fCatcher = new StateFullStringCatcher();

        private RestReaderActor fReader = new RestReaderActor();

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            fReader.SendRest(new Uri("http://api.sunrise-sunset.org/json?lat=36.7201600&lng=-4.4203400"), fCatcher); // 5 day weather forecast for Paris
            var future = fCatcher.GetState();
            tbText.DataContext = await future.GetResultAsync().ConfigureAwait(false);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ActorServer.Start(new ActorConfigManager());
        }
    }
}
