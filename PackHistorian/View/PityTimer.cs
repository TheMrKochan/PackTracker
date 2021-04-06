using HearthDb.Enums;
using PackTracker.Entity;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace PackTracker.View
{
    public class PityTimer : INotifyPropertyChanged
    {
        public int PackId { get; }
        public Rarity Rarity { get; }
        public bool Premium { get; }
        public bool SkipFirst { get; }
        public bool WaitForFirst { get; private set; }

        public int Current { get; private set; } = 0;
        public ObservableCollection<int> Prev { get; } = new ObservableCollection<int>();
        public int? Average => this.Prev.Count > 0 ? (int?)Math.Round(this.Prev.Average(), 0) : null;

        public PityTimer(History History, int packId, Rarity rarity, bool premium, bool skipFirst)
        {
            this.PackId = packId;
            this.Rarity = rarity;
            this.Premium = premium;
            this.SkipFirst = this.WaitForFirst = skipFirst;

            foreach (var Pack in History)
            {
                this.AddPack(Pack);
            }

            History.CollectionChanged += (sender, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (Pack Pack in e.NewItems)
                    {
                        this.AddPack(Pack);
                    }
                }
            };
        }

        private void AddPack(Pack Pack)
        {
            if (Pack.Id != this.PackId)
            {
                return;
            }

            if (this.Condition(Pack))
            {
                var newCurr = this.Current;
                this.Current = 0;

                if (this.WaitForFirst)
                {
                    this.WaitForFirst = false;
                }
                else
                {
                    this.Prev.Add(newCurr);
                    this.OnPropertyChanged("Average");
                }
            }
            else
            {
                this.Current++;
            }

            this.OnPropertyChanged("Current");
        }

        private bool Condition(Pack Pack)
        {
            return Pack.Cards.Any(x => x.Rarity == this.Rarity && (this.Premium ? x.Premium : true));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
