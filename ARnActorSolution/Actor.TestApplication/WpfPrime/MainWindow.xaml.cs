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
            ActorServer.Start("localhost", 80, false);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var prime = new actPrime(100);
            IActor act = new actActor();
            prime.SendMessage(new Tuple<int, IActor>(100, act));
        }
    }
}
