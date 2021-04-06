using HearthDb.Enums;
using System.Collections.Generic;
using System.Linq;

namespace PackTracker.View.Cache
{
    public class PityTimerRepository
    {
        private History _history;
        private List<PityTimer> _cache = new List<PityTimer>();

        public PityTimerRepository(History History)
        {
            this._history = History;
        }

        public PityTimer GetPityTimer(int packId, Rarity rarity, bool premium, bool skipFirst)
        {
            var pt = this._cache.FirstOrDefault(x => x.PackId == packId && x.Rarity == rarity && x.Premium == premium && x.SkipFirst == skipFirst);
            if (!(pt is PityTimer))
            {
                pt = new PityTimer(this._history, packId, rarity, premium, skipFirst);
                this._cache.Add(pt);
            }

            return pt;
        }
    }
}
