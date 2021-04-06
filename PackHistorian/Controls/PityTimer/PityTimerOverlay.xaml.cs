using HearthDb.Enums;
using PackTracker.Entity;
using PackTracker.View.Cache;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;

namespace PackTracker.Controls.PityTimer
{
    /// <summary>
    /// Interaktionslogik für PityTimerOverlay.xaml
    /// </summary>
    public partial class PityTimerOverlay : Window, INotifyPropertyChanged
    {
        public int? PackId { get; private set; } = null;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public PityTimerOverlay(PackTracker.History History, PityTimerRepository PityTimers)
        {
            this.InitializeComponent();
            this.DataContext = this;

            if (History.Count > 0)
            {
                this.PackId = History.Last().Id;
                this.Chart_Epic.DataContext = PityTimers.GetPityTimer((int)this.PackId, Rarity.EPIC, false, true);
                this.Chart_Leg.DataContext = PityTimers.GetPityTimer((int)this.PackId, Rarity.LEGENDARY, false, true);
            }

            History.CollectionChanged += (sender, e) =>
            {
                foreach (Pack Pack in e.NewItems)
                {
                    this.Chart_Epic.DataContext = PityTimers.GetPityTimer(Pack.Id, Rarity.EPIC, false, true);
                    this.Chart_Leg.DataContext = PityTimers.GetPityTimer(Pack.Id, Rarity.LEGENDARY, false, true);
                    this.PackId = Pack.Id;
                }

                this.OnPropertyChanged("PackId");
            };
        }

        private void UniformGrid_Loaded(object sender, RoutedEventArgs e)
        {
            this.SetPosition();
        }

        private void SetPosition()
        {
            var relativeRight = .01;
            var relativeBottom = .35;

            this.Left = SystemParameters.PrimaryScreenWidth - (SystemParameters.PrimaryScreenWidth * relativeRight) - this.Width;
            this.Top = SystemParameters.PrimaryScreenHeight - (SystemParameters.PrimaryScreenHeight * relativeBottom) - this.Height;
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            var helper = new WindowInteropHelper(this);
            Hearthstone_Deck_Tracker.User32.SetWindowExStyle(helper.Handle, Hearthstone_Deck_Tracker.User32.WsExTransparent | Hearthstone_Deck_Tracker.User32.WsExToolWindow);
        }
    }
}
