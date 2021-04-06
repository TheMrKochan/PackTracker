using PackTracker.Entity;
using System.Windows.Controls;

namespace PackTracker.Controls
{
    /// <summary>
    /// Interaktionslogik für Toast.xaml
    /// </summary>
    public partial class Toast : UserControl
    {
        public Toast(Pack Pack, View.Average Average)
        {
            this.InitializeComponent();
            this.ctr_Cards.DataContext = Pack.Cards;
            this.ctr_Average.DataContext = Average;
        }
    }
}
