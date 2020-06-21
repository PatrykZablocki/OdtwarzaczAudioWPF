using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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

namespace OdtwarzaczAudio8441
{
    /// <summary>
    /// Logika interakcji dla klasy WindowPupUp.xaml
    /// </summary>
    public partial class WindowPupUp : Window
    {
        MainWindow mainWindow;

        public WindowPupUp(MainWindow window ,string title, string text, Brush color)
        {
            InitializeComponent();

            mainWindow = window;
            mainWindow.IsEnabled = false;

            LabelTitle.Content = title;
            MainText.Text = text;

            Top.Background = color;
            ButtonOK.Background = color;
            popup.BorderBrush = color;

            SystemSounds.Exclamation.Play();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            mainWindow.IsEnabled = true;
        }

        private void Top_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}
