using System;
using System.Collections.Generic;

namespace PackTracker.Entity
{
    public class Pack
    {
        public int Id { get; }
        public DateTime Time { get; }
        public IEnumerable<Card> Cards { get; }

        public Pack(int id, DateTime Time, IEnumerable<Card> Cards)
        {
            this.Id = id;
            this.Time = Time;
            this.Cards = Cards;
        }
    }
}
