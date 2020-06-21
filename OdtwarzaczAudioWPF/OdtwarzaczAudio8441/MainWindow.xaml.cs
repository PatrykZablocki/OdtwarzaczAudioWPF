using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace OdtwarzaczAudio8441
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WindowPupUp window;

        MediaElement media;
        DispatcherTimer timer;

        List<Source> music = new List<Source>();

        bool kontynuacja = false;


        public MainWindow()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += Timer_Tick;
            timer.Start();

            media = new MediaElement();
            media.MediaOpened += new RoutedEventHandler(media_MediaOpened);
            media.MediaEnded += new RoutedEventHandler(media_MediaEnded);
            media.LoadedBehavior = MediaState.Manual;
            media.UnloadedBehavior = MediaState.Manual;

            SliderGlosnosc.Maximum = 100;
            SliderGlosnosc.Minimum = 0;
            SliderGlosnosc.TickFrequency = 1;
            SliderGlosnosc.IsSnapToTickEnabled = true;
            SliderGlosnosc.Value = 100;
            LabelGlosnosc.Content = 100;

            ButtonOdtwarzajZatrzymaj.Content = FindResource("Play");

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            SliderTime.Value = media.Position.TotalSeconds;
            TimeSpan time = TimeSpan.FromSeconds(media.Position.TotalSeconds);
            if(time.Seconds < 10) LabelTime.Content = time.Minutes + ":0" + time.Seconds;
            else LabelTime.Content = time.Minutes + ":" + time.Seconds;
        }

        private void ButtonDodaj_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "Pliki MP3 (*.mp3)|*.mp3|Pliki WAV (*.wav)|*.wav";

            if (ofd.ShowDialog() == true)
            {

                for (int i = 0; i < ofd.FileNames.Length; i++)
                {
                    music.Add(new Source { filename = ofd.SafeFileNames[i], path = ofd.FileNames[i] });
                    ListBoxSongs.Items.Add(ofd.SafeFileNames[i]);
                }

            }

        }

        private void ButtonOdtwarzajZatrzymaj_Click(object sender, RoutedEventArgs e)
        {
            if(ButtonOdtwarzajZatrzymaj.Content == FindResource("Play"))
            {
                odtwarzajMuzyke();
            }
            else if(ButtonOdtwarzajZatrzymaj.Content == FindResource("Pause"))
            {
                wstrzymajMuzyke();
            }
        }

        private void ButtonNastepny_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ustawIndexListBox();
                media.Stop();
                if (ListBoxSongs.SelectedIndex == ListBoxSongs.Items.Count - 1) ListBoxSongs.SelectedIndex = 0;
                else ListBoxSongs.SelectedIndex += 1;
                odtwarzajMuzyke();
            }
            catch { }
        }

        private void ButtonPoprzedni_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ustawIndexListBox();
                media.Stop();
                ListBoxSongs.SelectedIndex -= 1;
                odtwarzajMuzyke();
            }
            catch { }
        }

        private void SliderTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            media.Position = TimeSpan.FromSeconds(SliderTime.Value);
        }

        private void SliderGlosnosc_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                LabelGlosnosc.Content = SliderGlosnosc.Value.ToString();
                media.Volume = SliderGlosnosc.Value / 100;
            }
            catch { }
        }

        private void ListBoxSongs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            kontynuacja = false;
        }

        void odtwarzajMuzyke()
        {
            if(ListBoxSongs.SelectedIndex == -1) ListBoxSongs.SelectedIndex = 0;
            if(ListBoxSongs.Items.Count != 0)
            {
                if(kontynuacja == false)
                {
                    media.Stop();
                    media.Source = new Uri(music[ListBoxSongs.SelectedIndex].path);
                    kontynuacja = true;
                    media.Position = TimeSpan.FromMilliseconds(0);
                }
                LabelTytulValue.Content = music[ListBoxSongs.Items.IndexOf(ListBoxSongs.SelectedItem)].filename;
                LabelTytulValue.ToolTip = LabelTytulValue.Content;
                media.Play();
                media.Volume = SliderGlosnosc.Value / 100;
                ButtonOdtwarzajZatrzymaj.Content = FindResource("Pause");
            }
        }

        void wstrzymajMuzyke()
        {
            media.Pause();
            ButtonOdtwarzajZatrzymaj.Content = FindResource("Play");
        }

        void media_MediaOpened(object sender, RoutedEventArgs e)
        {
            SliderTime.Maximum = (int)media.NaturalDuration.TimeSpan.TotalSeconds;
        }

        void media_MediaEnded(object sender, RoutedEventArgs e)
        {
            ustawIndexListBox();
            if (CheckBoxPetla.IsChecked == true && CheckBoxPlaylista.IsChecked == true)
            {
                if (ListBoxSongs.SelectedIndex == ListBoxSongs.Items.Count - 1) {
                    ListBoxSongs.SelectedIndex = 0;
                    odtwarzajMuzyke();
                } 
                else
                {
                    ButtonNastepny_Click(sender, e);
                }
            }
            else if (CheckBoxPetla.IsChecked == true && CheckBoxPlaylista.IsChecked == false)
            {
                kontynuacja = false;
                odtwarzajMuzyke();
            }
            else if (CheckBoxPetla.IsChecked == false && CheckBoxPlaylista.IsChecked == true)
            {
                ButtonNastepny_Click(sender, e);
            }
        }

        private void ButtonWyczysc_Click(object sender, RoutedEventArgs e)
        {
            ListBoxSongs.Items.Clear();
            music.Clear();
        }

        private void ListBoxSongs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            odtwarzajMuzyke();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            media.Stop(); media.Close();
            this.Close();
        }

        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ButtonPrzesunDoGory_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxSongs.SelectedIndex != -1 && ListBoxSongs.SelectedIndex != 0)
            {
                int newIndex = ListBoxSongs.SelectedIndex - 1;

                podmiana(newIndex);
            }
        }

        private void ButtonPrzesunDoDolu_Click(object sender, RoutedEventArgs e)
        {
            if(ListBoxSongs.SelectedIndex != -1 && ListBoxSongs.SelectedIndex != ListBoxSongs.Items.Count -1)
            {
                int newIndex = ListBoxSongs.SelectedIndex + 1;

                podmiana(newIndex);
            }
        }

        void podmiana(int _newIndex)
        {
            Source musicSource = music[ListBoxSongs.SelectedIndex];
            music.RemoveAt(ListBoxSongs.SelectedIndex);
            music.Insert(_newIndex, musicSource);

            object selectedItem = ListBoxSongs.SelectedItem;
            ListBoxSongs.Items.Remove(selectedItem);
            ListBoxSongs.Items.Insert(_newIndex, selectedItem);

            ListBoxSongs.SelectedIndex = _newIndex;
        }

        private void ButtonUsun_Click(object sender, RoutedEventArgs e)
        {
            if(ListBoxSongs.SelectedItem != null)
            {
                music.RemoveAt(ListBoxSongs.SelectedIndex);
                ListBoxSongs.Items.RemoveAt(ListBoxSongs.SelectedIndex);
            }

            ustawIndexListBox();
        }

        void ustawIndexListBox()
        {
            try
            {
                foreach (string item in ListBoxSongs.Items)
                {
                    if(item == LabelTytulValue.Content.ToString())
                    {
                        ListBoxSongs.SelectedItem = item;
                    }
                }
            }
            catch { }
        }

        private void ZapiszPlayliste_Click(object sender, RoutedEventArgs e)
        {
            if(ListBoxSongs.Items.Count > 0)
            {
                try
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "Playlista plik xml |*.xml";

                    if (sfd.ShowDialog() == true)
                    {
                        XmlSerializer serial = new XmlSerializer(music.GetType());
                        StreamWriter writer = new StreamWriter(sfd.FileName);
                        serial.Serialize(writer, music);

                        window = new WindowPupUp(this, string.Empty, "Zapisano playliste", Brushes.Green);
                        window.Show();
                    }


                }

                catch (Exception ex)
                {
                    window = new WindowPupUp(this, "Error", ex.Message, Brushes.Red);
                    window.Show();
                }
            }
            else
            {
                window = new WindowPupUp(this, "Błąd", "Playlista jest pusta", Brushes.Red );
                window.Show();
            }

        }

        private void WczytajPlayliste_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "Playlista plik xml |*.xml";

            if(ofd.ShowDialog() == true)
            {
                try
                {
                    XmlSerializer serial = new XmlSerializer(music.GetType());
                    StreamReader reader = new StreamReader(ofd.FileName);
                    music = (List<Source>)serial.Deserialize(reader);

                    ListBoxSongs.Items.Clear();
                    for (int i = 0; i < music.Count; i++)
                    {
                        ListBoxSongs.Items.Add(music[i].filename);
                    }

                    WindowPupUp window = new WindowPupUp(this, string.Empty, "Wczytano playliste", Brushes.Green);
                    window.Show();
                }

                catch (Exception ex)
                {
                    window = new WindowPupUp(this, "Error", ex.Message, Brushes.Red);
                    window.Show();
                }

            }

        }

    }
}
