using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;

namespace PackTracker.Controls
{
    /// <summary>
    /// Interaktionslogik für HistoryDatePicker.xaml
    /// </summary>
    public partial class HistoryDatePicker : UserControl, INotifyPropertyChanged
    {
        private PackTracker.History _history;
        private Dictionary<DateTime?, ObservableCollection<Entity.Pack>> _associatedPacks = new Dictionary<DateTime?, ObservableCollection<Entity.Pack>>();

        public ObservableCollection<Entity.Pack> AssociatedPack
        {
            get
            {
                var Selection = this.dp_DatePicker.SelectedDate;
                if (Selection == null)
                {
                    return new ObservableCollection<Entity.Pack>();
                }

                if (!this._associatedPacks.ContainsKey(Selection))
                {
                    this._associatedPacks.Add(Selection, new ObservableCollection<Entity.Pack>(this._history.Where(x => x.Time.Date == Selection)));
                }

                return this._associatedPacks[Selection];
            }
        }

        public HistoryDatePicker(PackTracker.History History)
        {
            this.InitializeComponent();
            this._history = History;

            if (History.Count > 0)
            {
                this.InitializeCalender(History);
            }
            else
            {
                this.dp_DatePicker.DisplayDateStart = DateTime.Today;
                History.CollectionChanged += this.InitializeCalender;
            }

            this.dp_DatePicker.SelectedDateChanged += (sender, e) => this.OnPropertyChanged("AssociatedPack");
            History.CollectionChanged += (sender, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (var Pack in e.NewItems)
                    {
                        if (Pack is Entity.Pack NewPack)
                        {
                            if (this._associatedPacks.ContainsKey(NewPack.Time.Date))
                            {
                                this._associatedPacks[NewPack.Time.Date].Add(NewPack);
                            }

                            if (this.dp_DatePicker.SelectedDate != NewPack.Time.Date)
                            {
                                this.dp_DatePicker.SelectedDate = NewPack.Time.Date;
                            }
                        }
                    }
                }
            };

            this.dp_DatePicker.MouseWheel += (sender, e) =>
            {
                if (this.dp_DatePicker.SelectedDate == null)
                {
                    return;
                }

                var day = e.Delta < 0 ? -1 : 1;
                var Date = (DateTime)this.dp_DatePicker.SelectedDate;
                var First = History.First().Time.Date;
                var Last = History.Last().Time.Date;

                do
                {
                    Date = Date.AddDays(day);

                    if (day == 1)
                    {
                        if (Date > Last)
                        {
                            return;
                        }

                        if (Date < First)
                        {
                            this.dp_DatePicker.SelectedDate = First;
                            return;
                        }
                    }
                    else
                    {
                        if (Date < First)
                        {
                            return;
                        }

                        if (Date > Last)
                        {
                            this.dp_DatePicker.SelectedDate = Last;
                            return;
                        }
                    }

                } while (this.dp_DatePicker.BlackoutDates.Contains(Date));

                this.dp_DatePicker.SelectedDate = Date;
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void InitializeCalender(PackTracker.History History)
        {
            var FirstPack = History.First();
            this.dp_DatePicker.DisplayDateStart = FirstPack.Time;
            this.dp_DatePicker.SelectedDate = History.Last().Time.Date;

            var HistoryDates = History.Select(x => x.Time.Date).Distinct();
            for (var i = FirstPack.Time.Date; i.Date < DateTime.Today; i = i.AddDays(1))
            {
                if (!HistoryDates.Contains(i))
                {
                    this.dp_DatePicker.BlackoutDates.Add(new CalendarDateRange(i));
                }
            }
        }

        private void InitializeCalender(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems.Count > 0 && sender is PackTracker.History history)
            {
                this.InitializeCalender(history);
                history.CollectionChanged -= this.InitializeCalender;
            }
        }
    }
}
