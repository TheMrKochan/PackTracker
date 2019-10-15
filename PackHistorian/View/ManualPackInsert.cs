// Author: Ellekappae <https://github.com/Ellekappae>
using CardSet = HearthDb.Enums.CardSet;
using CardClass = HearthDb.Enums.CardClass;
using Hearthstone_Deck_Tracker.Utility;
using PackTracker.Entity;
using PackTracker.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using HDTCard = Hearthstone_Deck_Tracker.Hearthstone.Card;

namespace PackTracker.View
{
    internal class ManualPackInsert : INotifyPropertyChanged
    {
        private readonly History _history;
        private readonly IHistoryStorage _historyStorage;
        private DateTime? _selectedDateTime;
        private List<int> _sets;
        private int _selectedSet;
        private ObservableCollection<HDTCard> _cardsInCurrentSet = new ObservableCollection<HDTCard>();

        private ObservableCollection<CardViewModel> _packCards = new ObservableCollection<CardViewModel>();

        public DateTime? SelectedDateTime
        {
            get => this._selectedDateTime;
            set
            {
                this._selectedDateTime = value;
                this.OnPropertyChanged(nameof(this.SelectedDateTime));
            }
        }

        public int SelectedSet
        {
            get => this._selectedSet;
            set
            {
                this._selectedSet = value;
                this.RefreshCardsInCurrentSet();
                this.OnPropertyChanged(nameof(this.SelectedSet));
            }
        }

        public List<int> Sets
        {
            get => this._sets;
            set
            {
                this._sets = value;
                this.OnPropertyChanged(nameof(this.Sets));
            }
        }

        public ObservableCollection<HDTCard> CardsInCurrentSet
        {
            get => this._cardsInCurrentSet;
            set
            {
                this._cardsInCurrentSet = value;
                this.OnPropertyChanged(nameof(this.CardsInCurrentSet));
            }
        }

        public ObservableCollection<CardViewModel> PackCards
        {
            get => this._packCards;
            set
            {
                this._packCards = value;
                this.OnPropertyChanged(nameof(this.PackCards));
            }
        }

        public bool AddNewPackEnabled => this.SelectedDateTime != null && this.PackCards.All(c => c.HDTCard != null);

        private readonly Dictionary<int, List<HDTCard>> _setsCache = new Dictionary<int, List<HDTCard>>();

        private static readonly List<int> _golden = new List<int> { 23, 603, 643, 686 };
        private static readonly Dictionary<int, Func<HearthDb.Card, bool>> _filter = new Dictionary<int, Func<HearthDb.Card, bool>>
        {
            [1] = card => card.Set == CardSet.EXPERT1,
            [9] = card => card.Set == CardSet.GVG,
            [10] = card => card.Set == CardSet.TGT,
            [11] = card => card.Set == CardSet.OG,
            [17] = card => card.Set == CardSet.EXPERT1,
            [18] = card => card.Set == CardSet.EXPERT1,
            [19] = card => card.Set == CardSet.GANGS,
            [20] = card => card.Set == CardSet.UNGORO,
            [21] = card => card.Set == CardSet.ICECROWN,
            [23] = card => card.Set == CardSet.EXPERT1,
            [30] = card => card.Set == CardSet.LOOTAPALOOZA,
            [31] = card => card.Set == CardSet.GILNEAS,
            [38] = card => card.Set == CardSet.BOOMSDAY,
            [40] = card => card.Set == CardSet.TROLL,
            [41] = card => card.Set == CardSet.UNGORO || card.Set == CardSet.ICECROWN || card.Set == CardSet.LOOTAPALOOZA,
            [49] = card => card.Set == CardSet.DALARAN,
            [128] = card => card.Set == CardSet.ULDUM,
            [181] = card => card.Set == CardSet.EXPERT1,
            [347] = card => card.Set == CardSet.DRAGONS,
            [423] = card => card.Set == CardSet.BLACK_TEMPLE,
            [465] = card => card.Set == CardSet.EXPERT1,
            [468] = card => card.Set == CardSet.SCHOLOMANCE,
            [553] = card => card.Set == CardSet.THE_BARRENS,
            [603] = card => card.Set == CardSet.SCHOLOMANCE,
            [616] = card => card.Set == CardSet.DARKMOON_FAIRE,
            [643] = card => card.Set == CardSet.DARKMOON_FAIRE,
            [686] = card => card.Set == CardSet.THE_BARRENS,
            [498] = card => card.Set == CardSet.DALARAN || card.Set == CardSet.ULDUM || card.Set == CardSet.DRAGONS,
            [688] = card => card.Set == CardSet.BLACK_TEMPLE || card.Set == CardSet.SCHOLOMANCE || card.Set == CardSet.DARKMOON_FAIRE,
            [470] = card => card.Class == CardClass.HUNTER && (int)card.Set > (int)CardSet.TROLL,
            [545] = card => card.Class == CardClass.MAGE && (int)card.Set > (int)CardSet.TROLL,
            [631] = card => card.Class == CardClass.DRUID && (int)card.Set > (int)CardSet.TROLL,
            [632] = card => card.Class == CardClass.PALADIN && (int)card.Set > (int)CardSet.TROLL,
            [633] = card => card.Class == CardClass.WARRIOR && (int)card.Set > (int)CardSet.TROLL,
            [634] = card => card.Class == CardClass.PRIEST && (int)card.Set > (int)CardSet.TROLL,
            [635] = card => card.Class == CardClass.ROGUE && (int)card.Set > (int)CardSet.TROLL,
            [636] = card => card.Class == CardClass.SHAMAN && (int)card.Set > (int)CardSet.TROLL,
            [637] = card => card.Class == CardClass.WARLOCK && (int)card.Set > (int)CardSet.TROLL,
            [638] = card => card.Class == CardClass.DEMONHUNTER && (int)card.Set > (int)CardSet.TROLL,
        };

        public ManualPackInsert()
        {
            this._selectedDateTime = DateTime.Now;

            this._sets = PackNameConverter.PackNames.Select(set => set.Key).ToList();

            this.SelectedSet = this._sets.FirstOrDefault();

            this.ResetPackCards();
        }

        public ManualPackInsert(History History) : this()
        {
            this._history = History;
            this._historyStorage = new XmlHistory();
        }

        private void RefreshCardsInCurrentSet()
        {
            this.CardsInCurrentSet.Clear();

            if (!this._setsCache.ContainsKey(this.SelectedSet))
            {
                this._setsCache[this.SelectedSet] = HearthDb.Cards.Collectible.Values
                    .Where(_filter.ContainsKey(this.SelectedSet) ? _filter[this.SelectedSet] : _ => false)
                    .OrderBy(card => card.Rarity)
                    .ThenBy(card => card.Cost)
                    .ThenBy(card => card.Name)
                    .Select(card => new HDTCard(card))
                    .ToList();
            }

            this._setsCache[this.SelectedSet].ForEach(c => this.CardsInCurrentSet.Add(c));
        }

        private ICommand _addNewPackCommand;

        public ICommand AddNewPackCommand
        {
            get
            {
                if (this._addNewPackCommand == null)
                {
                    this._addNewPackCommand = new Command(this.AddNewPack);
                }
                return this._addNewPackCommand;
            }
        }

        private void AddNewPack()
        {
            var Cards = new List<Entity.Card>();

            if (this.SelectedDateTime != null && this.PackCards.All(c => c.HDTCard != null))
            {
                this.PackCards.ToList().ForEach(c => Cards.Add(new Entity.Card(c.HDTCard, c.Premium)));

                var newPack = new Pack(this.SelectedSet, (DateTime)this.SelectedDateTime, Cards);

                this._history.Add(newPack);
                this._historyStorage.Store(this._history.Ascending);

                this.ClearData();
            }
        }

        private void ResetPackCards()
        {
            this.PackCards.Clear();

            Enumerable.Range(1, 5).ToList().ForEach(i => this.PackCards.Add(new CardViewModel()));

            foreach (var cardViewModel in this.PackCards)
            {
                cardViewModel.PropertyChanged += this.CardViewModel_PropertyChanged;
            }
        }

        private void CardViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CardViewModel.HDTCard))
            {
                this.OnPropertyChanged(nameof(this.AddNewPackEnabled));
            }
            if (_golden.Contains(this.SelectedSet))
            {
                foreach (var item in this.PackCards)
                {
                    if (!item.Premium)
                    {
                        item.Premium = true;
                    }
                }
            }
        }

        private void ClearData()
        {
            this.SelectedDateTime = DateTime.Now;

            this.ResetPackCards();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}