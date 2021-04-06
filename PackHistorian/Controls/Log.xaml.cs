using HearthDb.Enums;
using PackTracker.Entity;
using PackTracker.View;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Media;

namespace PackTracker.Controls
{
    /// <summary>
    /// Interaktionslogik für Log.xaml
    /// </summary>
    public partial class Log
    {
        private SolidColorBrush Legendary, Epic, Rare;

        public Log(PackTracker.History History)
        {
            this.InitializeComponent();
            this.Legendary = (SolidColorBrush)this.FindResource("Legendary");
            this.Epic = (SolidColorBrush)this.FindResource("Epic");
            this.Rare = (SolidColorBrush)this.FindResource("Rare");

            Loaded += (sender, e) => this.AddLogs(History);

            History.CollectionChanged += (sender, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    this.AddLogs(e.NewItems.Cast<Pack>());
                }
            };
        }

        private void AddLogs(IEnumerable<Pack> Packs)
        {
            var sb = new StringBuilder();
            var DateTimeConverter = new DateTimeConverter();
            var PackNameConverter = new PackNameConverter();
            var sep = ",";

            foreach (var Pack in Packs)
            {
                sb.Clear();

                var date = DateTimeConverter.Convert(Pack.Time, null, null, null).ToString();
                var packname = PackNameConverter.Convert(Pack.Id, null, null, null).ToString();
                var commons = Pack.Cards.Count(x => x.Rarity == Rarity.COMMON);
                var commonGolds = commons > 0 ? Pack.Cards.Count(x => x.Premium && x.Rarity == Rarity.COMMON) : 0;
                var rares = Pack.Cards.Count(x => x.Rarity == Rarity.RARE);
                var rareGolds = rares > 0 ? Pack.Cards.Count(x => x.Premium && x.Rarity == Rarity.RARE) : 0;
                var epics = Pack.Cards.Count(x => x.Rarity == Rarity.EPIC);
                var epicGolds = epics > 0 ? Pack.Cards.Count(x => x.Premium && x.Rarity == Rarity.EPIC) : 0;
                var legendarys = Pack.Cards.Count(x => x.Rarity == Rarity.LEGENDARY);
                var legendaryGolds = legendarys > 0 ? Pack.Cards.Count(x => x.Premium && x.Rarity == Rarity.LEGENDARY) : 0;

                var Color = null as SolidColorBrush;
                if (legendarys > 0)
                {
                    Color = this.Legendary;
                }
                else if (epics > 0)
                {
                    Color = this.Epic;
                }
                else
                {
                    Color = this.Rare;
                }

                sb
                  .Append(date).Append(": ")
                  .Append(packname).Append("(")
                  .Append(commons);
                this.AddGoldStars(commonGolds, Color, sb);

                sb
                  .Append(sep)
                  .Append(rares);
                this.AddGoldStars(rareGolds, Color, sb);

                sb
                  .Append(sep)
                  .Append(epics);
                this.AddGoldStars(epicGolds, Color, sb);

                sb
                  .Append(sep)
                  .Append(legendarys);
                this.AddGoldStars(legendaryGolds, Color, sb);

                sb.AppendLine(")");
                this.txt_Log.Inlines.Add(new Run(sb.ToString()) { Foreground = Color });
            }

            this.sv_Scrollbar.ScrollToEnd();
        }

        private void AddGoldStars(int amount, SolidColorBrush Color, StringBuilder sb)
        {
            if (amount > 0)
            {
                this.txt_Log.Inlines.Add(new Run(sb.ToString()) { Foreground = Color });
                sb.Clear().Append('*', amount);

                this.txt_Log.Inlines.Add(new Run(sb.ToString()) { Foreground = Brushes.Gold });
                sb.Clear();
            }
        }
    }
}
