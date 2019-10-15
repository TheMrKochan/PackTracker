// Author: Ellekappae <https://github.com/Ellekappae>
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using HDTCard = Hearthstone_Deck_Tracker.Hearthstone.Card;

namespace PackTracker.Controls
{
    /// <summary>
    /// Logica di interazione per CardSelector.xaml
    /// </summary>
    public partial class CardSelector : UserControl
    {
        public Entity.Card SelectedCard { get; set; }

        public ObservableCollection<HDTCard> SetHdtCards
        {
            get => (ObservableCollection<HDTCard>)this.GetValue(SetHdtCardsProperty);
            set => this.SetValue(SetHdtCardsProperty, value);
        }

        public static readonly DependencyProperty SetHdtCardsProperty = DependencyProperty.Register("SetHdtCards", typeof(ObservableCollection<HDTCard>), typeof(CardSelector), null);

        public CardSelector()
        {
            this.InitializeComponent();
        }
    }
}