// Author: Ellekappae <https://github.com/Ellekappae>
namespace PackTracker.Controls
{
    /// <summary>
    /// Interaktionslogik für ManualPackInsert.xaml
    /// </summary>
    public partial class ManualPackInsert
    {
        public ManualPackInsert(PackTracker.History History)
        {
            this.DataContext = new View.ManualPackInsert(History);

            this.InitializeComponent();
        }
    }
}