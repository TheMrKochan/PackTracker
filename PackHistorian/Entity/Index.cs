using System;

namespace PackTracker.Entity
{
    internal class Index
    {
        public Card Card { get; }
        public DateTime DateTime { get; }

        public Index(Card Card, DateTime DateTime)
        {
            this.Card = Card;
            this.DateTime = DateTime;
        }
    }
}
