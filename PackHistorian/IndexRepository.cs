using HearthDb.Enums;
using Hearthstone_Deck_Tracker;
using PackTracker.Entity;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace PackTracker
{
    internal class IndexRepository
    {
        private readonly StringBuilder _sb = new StringBuilder();
        private Dictionary<string, string> _index = new Dictionary<string, string>();                       //<searchstring, id>
        private Dictionary<string, List<Index>> _indexObjects = new Dictionary<string, List<Index>>();      //<id, index-objects>

        public IndexRepository(History History)
        {
            foreach (var Pack in History)
            {
                foreach (var Card in Pack.Cards)
                {
                    this.Add(new Index(Card, Pack.Time));
                }
            }

            History.CollectionChanged += (sender, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (Pack Pack in e.NewItems)
                    {
                        foreach (var Card in Pack.Cards)
                        {
                            this.Add(new Index(Card, Pack.Time));
                        }
                    }
                }
            };
        }

        private void Add(Index Index)
        {
            if (!this._index.ContainsValue(Index.Card.HDTCard.Id))
            {
                var DbCard = HearthDb.Cards.GetFromDbfId(Index.Card.HDTCard.DbfIf);
                var name = DbCard.Name?.ToLower();
                var text = DbCard.Text?.ToLower();

                var locName = "";
                var locText = "";

                if (Enum.TryParse(Config.Instance.SelectedLanguage, out Locale lang))
                {
                    locName = DbCard.GetLocName(lang)?.ToLower();
                    locText = DbCard.GetLocText(lang)?.ToLower();
                }

                this._sb.Append(locName).Append(name).Append(locText).Append(text);
                this._index.Add(this._sb.ToString(), Index.Card.HDTCard.Id);
                this._sb.Clear();

                this._indexObjects.Add(Index.Card.HDTCard.Id, new List<Index>());
            }

            this._indexObjects[Index.Card.HDTCard.Id].Add(Index);
        }

        public IEnumerable<Index> Find(string searchString)
        {
            var elems = searchString.Split(' ');
            var Filtered = this._index.Where(x => x.Key.Contains(elems[0]));

            foreach (var elem in elems.Skip(1))
            {
                Filtered = Filtered.Where(x => x.Key.Contains(elem));
            }

            var Result = new List<Index>();
            foreach (var Index in this._indexObjects.Where(x => Filtered.Select(y => y.Value).Contains(x.Key)).Select(x => x.Value))
            {
                Result.AddRange(Index);
            }

            return Result.Distinct();
        }
    }
}
