using PackTracker.Entity;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace PackTracker
{
    public class History : IEnumerable<Pack>, INotifyCollectionChanged
    {
        private ObservableCollection<Pack> _packs;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public int Count => this._packs.Count;

        public History()
        {
            this._packs = new ObservableCollection<Pack>();
            this.Initialize();
        }

        public History(IEnumerable<Pack> Packs)
        {
            this._packs = new ObservableCollection<Pack>(Packs);
            this.Initialize();
        }

        private void Initialize()
        {
            this._packs.CollectionChanged += (sender, e) => this.OnCollectionChanged(e);
        }

        public History Ascending => new History(this._packs.OrderBy(x => x.Time));

        public void Add(Pack Pack)
        {
            this._packs.Add(Pack);
        }

        public Pack First()
        {
            return this._packs.OrderBy(p => p.Time.Ticks).FirstOrDefault();
        }

        public Pack Last()
        {
            return this._packs.OrderByDescending(p => p.Time.Ticks).FirstOrDefault();
        }

        public IEnumerator<Pack> GetEnumerator()
        {
            return this._packs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._packs.GetEnumerator();
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs Args)
        {
            CollectionChanged?.Invoke(this, Args);
        }
    }
}
