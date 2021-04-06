using MahApps.Metro.Controls;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace PackTracker.Controls.PityTimer
{
    /// <summary>
    /// Interaktionslogik für Label.xaml
    /// </summary>
    public partial class Label : UserControl, INotifyPropertyChanged
    {
        private int _curr = 0;
        private bool _stillSyncing;
        private int? _packId = null;

        public int Current
        {
            get => this._curr;
            set
            {
                if (this._curr == value)
                {
                    return;
                }

                this.pt_Curr.Transition = value > this._curr ? TransitionType.Down : TransitionType.Up;
                this._curr = value;
                this.OnPropertyChanged("Current");
            }
        }

        public bool Popup { get; set; } = false;
        public string PopupText => this.GeneratePopupText();
        public int Limit { get; set; }
        public string RarityPlaceholder { get; set; } = null;

        public Label()
        {
            this.InitializeComponent();
            this.Counters.DataContext = this;
        }

        private void This_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is View.PityTimer)
            {
                var pt = (View.PityTimer)e.OldValue;

                pt.PropertyChanged -= this.Pt_PropertyChanged;
            }

            if (e.NewValue is View.PityTimer)
            {
                var pt = (View.PityTimer)e.NewValue;

                this.Current = pt.Current;
                pt.PropertyChanged += this.Pt_PropertyChanged;

                this._stillSyncing = pt.SkipFirst && pt.WaitForFirst;
                this._packId = pt.PackId;
            }
            else
            {
                this.Current = 0;
                this._packId = null;
            }
        }

        private void Pt_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is View.PityTimer))
            {
                return;
            }
            var pt = (View.PityTimer)sender;

            switch (e.PropertyName)
            {
                case "Current":
                    this.Current = pt.Current;
                    this._stillSyncing = pt.SkipFirst && pt.WaitForFirst;
                    break;
                case "Average":
                    break;
            }
        }

        private void btn_Popup_Click(object sender, RoutedEventArgs e)
        {
            this.Popup = true;
            this.OnPropertyChanged("PopupText");
            this.OnPropertyChanged("Popup");
        }

        private string GeneratePopupText()
        {
            var rarity = this.RarityPlaceholder ?? "a card of the respective rarity";
            var packName = this._packId != null ? View.PackNameConverter.Convert((int)this._packId, HearthDb.Enums.Locale.enUS) : "the selected set";

            var sb = new StringBuilder();
            sb.AppendLine("This is your pity counter.");
            if (this._stillSyncing)
            {
                sb.Append("You haven't had ").Append(rarity).AppendLine(" in ").Append(packName).Append(" while recording.")
                  .Append(Plugin.NAME).AppendLine(" waits for your pity timer to reset in order to sync up with it.")
                  .AppendLine("Just keep opening packs to achieve this.")
                  .Append("\nSince recording, you have opened ").Append(this._curr).Append(this._curr != 1 ? " packs of " : " pack of ").Append(packName).Append(" without gaining ").Append(rarity).Append(".")
                ;
            }
            else
            {
                sb.Append("Your ").Append(Plugin.NAME).Append(" synced up with ").Append(packName).Append(" already. This was achieved by you, opening ").Append(rarity).AppendLine(".")
                  .Append("\nSince that, ").Append(this._curr).Append(this._curr != 1 ? " packs of " : " pack of ").Append(packName).Append(this._curr != 1 ? " have" : " has").Append(" been opened without ").Append(rarity).Append(".")
                ;
            }
            sb.AppendLine(" This is indicated by the number above the line.")
              .Append("The longest possible streak of ").Append(rarity).Append(" is ").Append(this.Limit).Append(". ")
              .AppendLine("This is indicated by the number below the line.")
            ;

            if (this._stillSyncing)
            {
                sb.AppendLine("\nSince your Pity Timer is not synced yet, it is most likely that ").Append(packName).Append(" was recently started by you or you just started tracking it.");
            }

            sb.AppendLine("\nAll of this, is also indicated by the bar charts on the left.");
            if (this._stillSyncing)
            {
                sb.Append("Once you have opned a pack with ").Append(rarity).AppendLine(", the bar that you see, will drop back to 0 and start all over again. Because it wasn't synced yet,")
                  .Append("keeping that streak would falsify the data. Therefor, ").Append(Plugin.NAME).AppendLine(" cleans is up at the reset of your pity timer.")
                ;
            }
            else
            {
                sb.Append("They represent all your streaks without ").Append(rarity).AppendLine(".")
                  .Append("Every time you gain ").Append(rarity).AppendLine(", the right bar becomes static and gets pushed to the left, to make space for a new bar, that count from 0 again.")
                ;
            }

            sb.Append("The half transparent bar is just another representation of this numeric label. The yellow line indicated the average that Blizzard aims on to gain ").Append(rarity).AppendLine(".")
              .Append("The red line is the maximum, that's why it is located excactly on ").Append(this.Limit).AppendLine(". If a bar passes that line, there is probably something wrong with the data.")
              .Append("A dashed line is visable when your pity timer was reseted for the second time and is meant to display your personal average.")
            ;

            return sb.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
