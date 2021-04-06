using MahApps.Metro.Controls;

namespace PackTracker.Controls.Settings
{
    /// <summary>
    /// Interaktionslogik für General.xaml
    /// </summary>
    public partial class General : MetroContentControl, ITitledElement
    {
        public string Title => "General";

        public General(PackTracker.Settings Settings)
        {
            this.InitializeComponent();
            this.DataContext = Settings;
        }
    }
}
