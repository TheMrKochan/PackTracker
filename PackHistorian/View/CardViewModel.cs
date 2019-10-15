// Author: Ellekappae <https://github.com/Ellekappae>
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HDTCard = Hearthstone_Deck_Tracker.Hearthstone.Card;

namespace PackTracker.View
{
    public class CardViewModel : INotifyPropertyChanged
    {
        private HDTCard _card;
        private bool _premium;

        public HDTCard HDTCard
        {
            get => this._card;
            set
            {
                this._card = value;
                this.OnPropertyChanged();
            }
        }

        public bool Premium
        {
            get => this._premium;
            set
            {
                this._premium = value;
                this.OnPropertyChanged();
            }
        }

        public CardViewModel(HDTCard Card, bool premium)
        {
            this._card = Card;
            this._premium = premium;
        }

        public CardViewModel()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;

            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}