using HearthDb.Enums;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace PackTracker.View
{
    public class Average : INotifyPropertyChanged
    {
        private List<int> _countsEpic = new List<int>();
        private List<int> _countsLeg = new List<int>();
        private bool _skippingEpic = true;
        private bool _skippingLeg = true;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Id { get; }

        public int? AverageEpic => this._countsEpic.Count > 1 ? (int?)Math.Round(this._countsEpic.Average(), 0) : null;
        public int? AverageLegendary => this._countsLeg.Count > 1 ? (int?)Math.Round(this._countsLeg.Average(), 0) : null;

        public int CurrentEpic { get; private set; } = 0;
        public int CurrentLegendary { get; private set; } = 0;

        public Average(int PackId, History History)
        {
            this.Id = PackId;
            this.AddCounts(History);

            History.CollectionChanged += this.History_CollectionChanged;
        }

        private void History_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                this.AddCounts(e.NewItems.Cast<Entity.Pack>());
            }
        }

        private void AddCounts(IEnumerable<Entity.Pack> Packs)
        {
            var notifyAverageEpic = false;
            var notifyAverageLegendary = false;
            var notifyCurrent = false;

            foreach (var Pack in Packs)
            {
                if (Pack.Id == this.Id)
                {
                    this.CurrentEpic++;
                    this.CurrentLegendary++;

                    notifyCurrent = true;

                    if (Pack.Cards.Any(x => x.Rarity == Rarity.EPIC))
                    {
                        if (this._skippingEpic)
                        {
                            this._skippingEpic = false;
                        }
                        else
                        {
                            this._countsEpic.Add(this.CurrentEpic);
                            notifyAverageEpic = true;
                        }

                        this.CurrentEpic = 0;
                    }

                    if (Pack.Cards.Any(x => x.Rarity == Rarity.LEGENDARY))
                    {
                        if (this._skippingLeg)
                        {
                            this._skippingLeg = false;
                        }
                        else
                        {
                            this._countsLeg.Add(this.CurrentLegendary);
                            notifyAverageLegendary = true;
                        }

                        this.CurrentLegendary = 0;
                    }
                }
            }

            if (notifyAverageEpic)
            {
                this.OnPropertyChanged("AverageEpic");
            }

            if (notifyAverageLegendary)
            {
                this.OnPropertyChanged("AverageLegendary");
            }

            if (notifyCurrent)
            {
                this.OnPropertyChanged("CurrentEpic");
                this.OnPropertyChanged("CurrentLegendary");
            }
        }

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
