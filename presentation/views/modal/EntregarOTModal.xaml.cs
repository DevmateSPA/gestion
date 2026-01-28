using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using Gestion.core.model;

namespace Gestion.presentation.views.windows
{
    public partial class EntregarOTModal : Window 
    {
        public string Titulo { get; set; }
        public DateTime? FechaEntrega; 

        public EntregarOTModal()
        {
            InitializeComponent();
            Titulo = "Marcar OT como entregada";
            DataContext = this;
        }


        private void BtnEntregar_Click(object sender, RoutedEventArgs e)
        {
            if (FechaEntrega == null)
            {
                MessageBox.Show("Debes completar la fecha.");
                return;
            }

            DialogResult = true;
            Close();
        }

        private void FechaChanged(object sender, SelectionChangedEventArgs e)
        {
            FechaEntrega = dpFechaEntrega.SelectedDate;
        }

    }
}
