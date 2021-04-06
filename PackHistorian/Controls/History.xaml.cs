//using HearthDb;

namespace PackTracker.Controls
{
    /// <summary>
    /// Interaktionslogik für History.xaml
    /// </summary>
    public partial class History
    {
        public History(PackTracker.History History, HistoryDatePicker DatePicker)
        {
            this.InitializeComponent();
            this.ic_Cards.DataContext = DatePicker;
            this.uc_Date.Content = DatePicker;
        }
    }
}
