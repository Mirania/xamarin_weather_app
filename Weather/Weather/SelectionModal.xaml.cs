using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Weather
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectionModal : ContentPage
    {
        private readonly string[] choices = { "Porto", "Lisboa", "Coimbra", "Aveiro", "Faro", "Braga" };
        private MainPage mainPage;

        public SelectionModal(MainPage m)
        {
            InitializeComponent();
            mainPage = m;

            foreach (string c in choices)
                if (!mainPage.IsEntry(c))
                    picker.Items.Add(c);
        }

        public void AddAndPopModal(object sender, EventArgs e)
        {
            if (picker.SelectedIndex >= 0)
                mainPage.AddEntry((string)picker.SelectedItem);
            PopModal(sender, e);
        }

        public void PopModal(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}