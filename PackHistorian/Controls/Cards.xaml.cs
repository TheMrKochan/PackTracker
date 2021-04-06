using System.Collections.Generic;
using System.Windows.Controls;

namespace PackTracker.Controls
{
    /// <summary>
    /// Interaktionslogik für PackToast.xaml
    /// </summary>
    public partial class Cards : UserControl
    {
        public Cards()
        {
            this.InitializeComponent();
        }

        public Cards(IEnumerable<Entity.Card> Cards)
        {
            this.InitializeComponent();
            this.DataContext = Cards;
        }
    }
}
