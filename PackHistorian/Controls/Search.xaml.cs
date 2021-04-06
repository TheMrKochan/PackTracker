using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PackTracker.Controls
{
    /// <summary>
    /// Interaktionslogik für Search.xaml
    /// </summary>
    public partial class Search
    {
        private IndexRepository _index;

        public Search(PackTracker.History History)
        {
            this.InitializeComponent();

            this._index = new IndexRepository(History);
            this.txt_Search.Focus();
        }

        private void txt_Search_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.dg_Result.ItemsSource = this._index.Find(((TextBox)sender).Text);
                this.txt_Search.SelectAll();
            }
        }

        private void btn_Search_Click(object sender, RoutedEventArgs e)
        {
            this.dg_Result.ItemsSource = this._index.Find(this.txt_Search.Text);
            this.txt_Search.Focus();
            this.txt_Search.SelectAll();
        }

        private void MetroWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}
