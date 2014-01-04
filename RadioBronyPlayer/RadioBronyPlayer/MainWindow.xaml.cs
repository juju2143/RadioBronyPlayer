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
using System.Net;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Reflection;

namespace RadioBronyPlayer
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Boolean isPlaying = false;

        private readonly BackgroundWorker infoRefresher = new BackgroundWorker();
        private String currentTitle = "";
        private String currentArtist = "";
        private String currentListeners = "";

        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            infoRefresher.RunWorkerAsync();
        }

        public MainWindow()
        {
            InitializeComponent();
            infoRefresher.RunWorkerAsync();
            infoRefresher.DoWork += infoRefresher_DoWork;
            infoRefresher.RunWorkerCompleted += infoRefresher_RunWorkerCompleted;
            ExitButton.Click += ExitButton_Click;
            mediaControlButton.Click += mediaControlButton_Click;
            VolumeSlider.ValueChanged += VolumeSlider_ValueChanged;
            changeVolume(VolumeSlider.Value);
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 10);
            dispatcherTimer.Start();
            MinimizeButton.MouseUp += MinimizeButton_Click;
            
        }

        void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            MinimizeButton.ReleaseAllTouchCaptures();
            WindowState = WindowState.Minimized;
        }            

        void infoRefresher_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            title.Text = currentTitle;
            title.ToolTip = "Chanson : " + currentTitle;
            artist.Text = currentArtist;
            artist.ToolTip = "Artiste : " + currentArtist;
            listeners.Content = currentListeners;
            listeners.ToolTip = "Auditeurs : " + currentListeners;
        }

        void infoRefresher_DoWork(object sender, DoWorkEventArgs e)
        {
            currentTitle = getInfo("track");
            currentArtist = getInfo("artist");
            currentListeners = getInfo("listeners");
        }

        void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            changeVolume(VolumeSlider.Value);
        }

        void changeVolume(double volume)
        {
            Player.Volume = volume;
        }

        string getInfo(string get)
        {
            WebClient c = new WebClient();
            var json = c.DownloadString("http://www.radiobrony.fr/wp-content/plugins/radiobrony/ajax/API/music_info.json");
            JObject o = JObject.Parse(json);
            if (get.Equals("track"))
            {
                String song = (string)o["now_playing"][get];
                song = HttpUtility.HtmlDecode(song);
                return song;
            }
            else if (get.Equals("artist"))
            {
                String song = (string)o["now_playing"][get];
                song = HttpUtility.HtmlDecode(song);
                return song;
            }
            else if (get.Equals("listeners"))
            {
                String song = (string)o[get];
                song = HttpUtility.HtmlDecode(song);
                return song;
            }
            else
                return "error";
        }

        void mediaControlButton_Click(object sender, RoutedEventArgs e)
        {
            if (isPlaying)
            {
                BitmapImage tempIcon = new BitmapImage();
                tempIcon.BeginInit();
                tempIcon.UriSource = new Uri("pack://application:,,,/Resources/play.png");
                tempIcon.EndInit();
                iconPlaying.Source = tempIcon;
                Player.Source = null;
                Player.Stop();
            }
            else
            {
                BitmapImage tempIcon = new BitmapImage();
                tempIcon.BeginInit();
                tempIcon.UriSource = new Uri("pack://application:,,,/Resources/pause.png");
                tempIcon.EndInit();
                iconPlaying.Source = tempIcon;
                Player.Source = new Uri("http://www.radiobrony.fr:8000/live.m3u");
                Player.Play();
            }
            isPlaying = !isPlaying;
        }

        void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void mouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!MinimizeButton.IsMouseOver)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    // this prevents win7 aerosnap
                    if (this.ResizeMode != System.Windows.ResizeMode.NoResize)
                    {
                        this.ResizeMode = System.Windows.ResizeMode.NoResize;
                        this.UpdateLayout();
                    }

                    DragMove();
                };
            }
        }

    }
}
