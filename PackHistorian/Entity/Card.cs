using HearthDb.Enums;
using HDTCard = Hearthstone_Deck_Tracker.Hearthstone.Card;

namespace PackTracker.Entity
{
    public class Card
    {
        public HDTCard HDTCard { get; }
        public bool Premium { get; }
        public Rarity Rarity => this.HDTCard.Rarity;

        public Card(HDTCard Card, bool premium)
        {
            this.HDTCard = Card;
            this.Premium = premium;
        }
    }
}