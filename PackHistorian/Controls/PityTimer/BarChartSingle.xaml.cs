using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PackTracker.Controls.PityTimer
{
    /// <summary>
    /// Interaktionslogik für BarChartSingle.xaml
    /// </summary>
    public partial class BarChartSingle : UserControl, INotifyPropertyChanged
    {
        private ObservableValue _single = new ObservableValue(0);
        private ColumnSeries _cs = new ColumnSeries();

        public SeriesCollection Single { get; }
        public string Title { get; set; }
        public int Threshold { get; set; }
        public int SoftThreshold { get; set; }
        public double? MaxValue { get; set; }
        public AxisPosition Position { get; set; } = AxisPosition.LeftBottom;
        public Brush Fill
        {
            set
            {
                this._cs.Fill = value.Clone();
                this._cs.Fill.Opacity = .9;
            }
        }

        public int? Average => this.DataContext is View.PityTimer ? ((View.PityTimer)this.DataContext).Average : null;

        public BarChartSingle()
        {
            this.InitializeComponent();
            this.Chart.DataContext = this;

            var cv = new ChartValues<ObservableValue>
            {
                this._single
            };

            this._cs.Values = cv;

            this.Single = new SeriesCollection() {
        this._cs,
      };

            DataContextChanged += this.BarChartSingle_DataContextChanged;
        }

        private void BarChartSingle_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is View.PityTimer)
            {
                var pt = (View.PityTimer)e.OldValue;
                pt.PropertyChanged -= this.Pt_PropertyChanged;
            }

            if (e.NewValue is View.PityTimer)
            {
                var pt = (View.PityTimer)e.NewValue;
                this._single.Value = pt.Current;
                pt.PropertyChanged += this.Pt_PropertyChanged;
            }
            else
            {
                this._single.Value = 0;
            }

            this.OnPropertyChanged("Average");
        }

        private void Pt_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Current":
                    this._single.Value = ((View.PityTimer)sender).Current;
                    break;
                case "Average":
                    this.OnPropertyChanged(e.PropertyName);
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
