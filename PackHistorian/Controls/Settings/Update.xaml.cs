using MahApps.Metro.Controls;
using PackTracker.Update;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace PackTracker.Controls.Settings
{
    /// <summary>
    /// Interaktionslogik für Update.xaml
    /// </summary>
    public partial class Update : MetroContentControl, ITitledElement
    {
        private Updater _updater;
        private Timer _timer;

        public string Title => "Update";

        public Update(PackTracker.Settings Settings, Updater Updater)
        {
            this.InitializeComponent();
            this.DataContext = Settings;

            this._updater = Updater;
            this._timer = new Timer((new TimeSpan(0, 0, 10)).TotalMilliseconds) { AutoReset = false };
            this._timer.Elapsed += (sender, e) => this.Dispatcher.Invoke(() => this.btn_Refresh.IsEnabled = true);

            Loaded += this.Update_Loaded;
        }

        private void Update_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= this.Update_Loaded;
            this.Refresh();
        }

        private void Refresh()
        {
            this.btn_Refresh.IsEnabled = false;
            this.btn_Update.IsEnabled = false;
            this.pb_Bar.Visibility = Visibility.Visible;

            this.txt_ChangeLog.Inlines.Clear();
            var bw = new BackgroundWorker();

            bw.DoWork += (sender, e) => e.Result = this._updater.GetAllReleases();

            bw.RunWorkerCompleted += (sender, e) =>
            {
                if (e.Result is IEnumerable<Release>)
                {
                    var Result = (IEnumerable<Release>)e.Result;
                    this.InsertInlines(Result, this.txt_ChangeLog.Inlines);
                    this.btn_Update.IsEnabled = Result.Any(x => Updater.ParseVersion(x.tag_name) > Plugin.CurrentVersion);
                }
                else
                {
                    MessageBox.Show("Request failed", "Update", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                this.pb_Bar.Visibility = Visibility.Hidden;
                this._timer.Start();
            };

            bw.RunWorkerAsync();
        }

        private void InsertInlines(IEnumerable<Release> Releases, InlineCollection Target)
        {
            foreach (var Release in Releases)
            {
                var Headline = new Run(Release.tag_name + (Release.name != Release.tag_name ? (" \"" + Release.name + "\"") : ""))
                {
                    FontStyle = FontStyles.Oblique,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.CornflowerBlue,
                    TextDecorations = TextDecorations.Underline,
                    FontSize = 18,
                };
                Target.Add(Headline);

                if (Plugin.CurrentVersion == Updater.ParseVersion(Release.tag_name))
                {
                    var Installed = new Run(" (installed)\n")
                    {
                        FontStyle = FontStyles.Oblique,
                        Foreground = Brushes.White,
                        FontSize = 10,
                    };
                    Target.Add(Installed);
                }
                else
                {
                    Headline.Text += "\n";
                }

                var Body = new Run(Release.body + "\n\n")
                {
                    Foreground = Brushes.White,
                };
                Target.Add(Body);
            }
        }

        private void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            this.Refresh();
        }

        private void btn_Update_Click(object sender, RoutedEventArgs e)
        {
            var bw = new BackgroundWorker();
            bw.DoWork += (bwsender, bwe) => bwe.Result = this._updater.Update();
            bw.RunWorkerCompleted += (bwsender, bwe) =>
            {
                this.pb_Bar.Visibility = Visibility.Hidden;

                if ((bool)bwe.Result)
                {
                    MessageBox.Show("Update completed\nPlease restart Hearthstone Deck Tracker", "Pack Tracker: Update");
                }
                else
                {
                    this.btn_Refresh.IsEnabled = true;
                    this.btn_Refresh.IsEnabled = true;
                    MessageBox.Show("Update failed\nPlease try again later or download on Github", "Pack Tracker: Update");
                }
            };

            this.btn_Refresh.IsEnabled = false;
            this.btn_Update.IsEnabled = false;
            this.pb_Bar.Visibility = Visibility.Visible;
            this._timer.Stop();
            bw.RunWorkerAsync();
        }
    }
}
