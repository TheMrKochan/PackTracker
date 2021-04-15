using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace PackTracker.Controls.PityTimer
{
    /// <summary>
    /// Interaktionslogik für BarChart.xaml
    /// </summary>
    public partial class BarChartPrev : UserControl, INotifyPropertyChanged
    {
        private ColumnSeries _cs;
        private ObservableValue _currTimer = new ObservableValue(0);
        private ChartValues<ObservableValue> _prevTimer = new ChartValues<ObservableValue>();
        private Brush _fillOrig, _fillPrev, _fillCurr;

        public SeriesCollection Prev { get; }
        public Brush Fill
        {
            set
            {
                this._fillPrev = value.Clone();
                this._fillPrev.Opacity = .9;
                this._fillCurr = value.Clone();
                this._fillCurr.Opacity = .5;
                this._fillOrig = value;
            }
        }

        public int SoftThreshold { get; set; }
        public int Threshold { get; set; }
        public int? Average => this.DataContext is View.PityTimer timer ? timer.Average : null;
        public string XTitle { get; set; }
        public string YTitle { get; set; }
        public double? MaxValue { get; set; }

        public BarChartPrev()
        {
            this.InitializeComponent();
            this.Chart.DataContext = this;

            var mapper = Mappers.Xy<ObservableValue>()
              .Fill((x, y) => x == this._currTimer ? this._fillCurr : this._fillPrev)
              .Y((obs, y) => obs.Value)
              .X((obs, x) => x)
            ;
            this._cs = new ColumnSeries(mapper)
            {
                Values = _prevTimer,
            };

            this.Prev = new SeriesCollection() {
                this._cs,
            };

            DataContextChanged += (sender, e) =>
            {
                this._prevTimer.Clear();

                if (e.NewValue is View.PityTimer pt)
                {
                    this._prevTimer.AddRange(pt.Prev.Select(x => new ObservableValue(x)));
                    this._currTimer = new ObservableValue(pt.Current);
                    this._prevTimer.Add(this._currTimer);

                    pt.Prev.CollectionChanged += this.PrevChanged;
                    pt.PropertyChanged += this.AverageChanged;
                    pt.PropertyChanged += this.CurrentChanged;
                }

                if (e.OldValue is View.PityTimer timer)
                {
                    timer.Prev.CollectionChanged -= this.PrevChanged;
                    timer.PropertyChanged -= this.AverageChanged;
                    timer.PropertyChanged -= this.CurrentChanged;
                }

                this.OnPropertyChanged("Average");
            };
        }

        private void CurrentChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Current" && sender is View.PityTimer timer)
            {
                this._currTimer.Value = timer.Current;
            }
        }

        private void AverageChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Average")
            {
                this.OnPropertyChanged(e.PropertyName);
            }
        }

        private void PrevChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.DataContext is View.PityTimer pt)
            {
                this._currTimer = new ObservableValue(pt.Current);
                this._prevTimer.Add(this._currTimer);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
