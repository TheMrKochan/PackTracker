using PackTracker.Update;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Animation;

namespace PackTracker.Controls.Settings
{
    /// <summary>
    /// Interaktionslogik für Settings.xaml
    /// </summary>
    public partial class Settings
    {
        private PackTracker.Settings _settings;
        private bool allowClosing = false;

        public Settings(PackTracker.Settings Settings)
        {
            this.InitializeComponent();
            this.lb_tabs.ItemsSource = new List<ITitledElement>() {
        new General(Settings),
        new Update(Settings, new Updater()),
        new Credits(),
      };
            this.lb_tabs.SelectedIndex = 0;

            this._settings = Settings;
            this.AnimateSizeToContentStart();
        }

        private void AnimateSizeToContentStart()
        {
            SizeChanged += this.ChangeSize;
            this.SizeToContent = SizeToContent.Width;
        }

        private void AnimateSizeToContentStop()
        {
            SizeChanged -= this.ChangeSize;
            this.SizeToContent = SizeToContent.Manual;
        }

        private void ChangeSize(object sender, SizeChangedEventArgs e)
        {
            var at = new DoubleAnimation(e.PreviousSize.Width, e.NewSize.Width, new Duration(new TimeSpan(6000000)));
            this.AnimateSizeToContentStop();
            at.EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseInOut };
            at.Completed += (sender2, e2) => this.AnimateSizeToContentStart();

            this.BeginAnimation(WidthProperty, at);
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.allowClosing)
            {
                return;
            }
            else
            {
                e.Cancel = true;
            }

            this.AnimateSizeToContentStop();
            var Duration = new Duration(TimeSpan.FromSeconds(.4));
            IEasingFunction Easing = new ExponentialEase() { EasingMode = EasingMode.EaseInOut };

            var Width = new DoubleAnimation(this.ActualWidth, 2, Duration) { EasingFunction = Easing };
            Width.Completed += (sender2, e2) =>
            {
                var Height = new DoubleAnimation(this.ActualHeight, 0, Duration) { EasingFunction = Easing };
                Height.Completed += (sender3, e3) =>
                {
                    this.allowClosing = true;
                    this.Close();
                };

                this.BeginAnimation(HeightProperty, Height);
            };

            this.BeginAnimation(WidthProperty, Width);
        }
    }
}
