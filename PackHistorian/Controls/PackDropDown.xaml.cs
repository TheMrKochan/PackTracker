using MahApps.Metro.Controls;
using PackTracker.Entity;
using PackTracker.View;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PackTracker.Controls
{
    public partial class PackDropDown : SplitButton
    {
        private ObservableCollection<int> _dropDown;
        internal bool ShowUntracked;

        public PackDropDown()
        {
            this.InitializeComponent();

            this._dropDown = new ObservableCollection<int>();
        }

        private void dd_Packs_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.ShowUntracked)
            {
                this._dropDown = new ObservableCollection<int>(PackNameConverter.PackNames.Keys);
                this.dd_Packs.ItemsSource = this._dropDown;
                return;
            }

            this._dropDown.Clear();

            if (e.NewValue is PackTracker.History)
            {
                var History = (PackTracker.History)e.NewValue;
                this._dropDown = new ObservableCollection<int>(History.Select(x => x.Id).Distinct().OrderBy(x => x));
                this.dd_Packs.ItemsSource = this._dropDown;
                History.CollectionChanged += this.DropDown_NewEntry;
            }

            if (e.OldValue is PackTracker.History)
            {
                ((PackTracker.History)e.OldValue).CollectionChanged -= this.DropDown_NewEntry;
            }

            if (this._dropDown.Count > 0)
            {
                this.dd_Packs.SelectedIndex = -1;
                if (e.NewValue is PackTracker.History)
                {
                    this.dd_Packs.SelectedItem = ((PackTracker.History)e.NewValue).Last().Id;
                }
                else
                {
                    this.dd_Packs.SelectedIndex = 0;
                }
            }
        }

        private void DropDown_NewEntry(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Pack newPack in e.NewItems)
                {
                    if (!this._dropDown.Contains(newPack.Id))
                    {
                        var isInserted = false;

                        foreach (var id in this._dropDown)
                        {
                            if (newPack.Id < id)
                            {
                                this._dropDown.Insert(this._dropDown.IndexOf(id), newPack.Id);
                                isInserted = true;
                                break;
                            }
                        }

                        if (!isInserted)
                        {
                            this._dropDown.Add(newPack.Id);
                        }
                    }

                    this.dd_Packs.SelectedItem = newPack.Id;
                }
            }
        }

        private void dd_Packs_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (this.dd_Packs.SelectedIndex > 0)
                {
                    this.dd_Packs.SelectedIndex--;
                }
            }
            else
            {
                if (this.dd_Packs.SelectedIndex < this.dd_Packs.Items.Count - 1)
                {
                    this.dd_Packs.SelectedIndex++;
                }
            }
        }

        private void dd_Packs_Click(object sender, RoutedEventArgs e)
        {
            this.dd_Packs.IsExpanded = !this.dd_Packs.IsExpanded;
        }
    }
}
