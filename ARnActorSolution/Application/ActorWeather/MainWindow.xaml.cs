using Actor.Server;
using System;
using System.Windows;

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

        private readonly StateFullStringCatcher _catcher = new StateFullStringCatcher();

        private readonly RestReaderActor fReader = new RestReaderActor();

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            fReader.SendRest(new Uri("http://api.sunrise-sunset.org/json?lat=36.7201600&lng=-4.4203400"), _catcher); // 5 day weather forecast for Paris
            var future = _catcher.GetState();
            tbText.DataContext = await future.GetResultAsync().ConfigureAwait(false);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ActorServer.Start(new ActorConfigManager());
        }
    }
}
