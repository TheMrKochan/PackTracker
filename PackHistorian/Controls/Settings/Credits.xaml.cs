using MahApps.Metro.Controls;
using System.Windows.Navigation;

namespace PackTracker.Controls.Settings
{
    /// <summary>
    /// Interaktionslogik für Credits.xaml
    /// </summary>
    public partial class Credits : MetroContentControl, ITitledElement
    {
        public string Title => "Credits";

        public Credits()
        {
            this.InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }
    }
}
