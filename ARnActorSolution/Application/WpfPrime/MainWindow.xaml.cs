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
using WpfPrime.Prime;

namespace WpfPrime
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ActorServer.Start("localhost", 80, null);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var prime = new actPrime(987654321);
            prime.SendMessage("start");
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var echo = new BaseActor();
            var floorrevenge = new Fkn(echo,7,100);
        }
    }
}
