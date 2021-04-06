using System.Collections.Generic;

namespace PackTracker.Controls
{
    /// <summary>
    /// Interaktionslogik für Statistic.xaml
    /// </summary>
    public partial class Statistic
    {
        public Statistic(PackTracker.History History)
        {
            this.InitializeComponent();

            var _statistics = new Dictionary<int, View.Statistic>();

            this.dd_Packs.SelectionChanged += (sender, e) =>
            {
                if (e.AddedItems.Count == 1)
                {
                    var selection = (int)e.AddedItems[0];
                    if (!_statistics.ContainsKey(selection))
                    {
                        _statistics.Add(selection, new View.Statistic(selection, History));
                    }

                    this.dp_Statistic.DataContext = _statistics[selection];
                }
                else
                {
                    this.dp_Statistic.DataContext = null;
                }
            };

            Loaded += (sender, e) => this.dd_Packs.DataContext = History;
            this.dd_Packs.Focus();
        }
    }
}
