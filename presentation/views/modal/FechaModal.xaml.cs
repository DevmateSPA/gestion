using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;

namespace Gestion.presentation.views.windows
{
    public partial class FechaModal : Window
    {
        public string Titulo { get; set; }

        public DateTime? FechaDesde; 

        public DateTime? FechaHasta;

        public FechaModal()
        {
            InitializeComponent();
            Titulo = "Seleccione fechas";
            DataContext = this;

        }


        
        private void BtnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            if (FechaDesde == null || FechaHasta == null)
            {
                MessageBox.Show("Debes completar ambas fechas.");
                return;
            }

            DialogResult = true;
            Close();
        }

        
        private void FechaDesdeChanged(object sender, SelectionChangedEventArgs e)
        {
            FechaDesde = dpFechaDesde.SelectedDate;
        }

        private void FechaHastaChanged(object sender, SelectionChangedEventArgs e)
        {
            FechaHasta = dpFechaHasta.SelectedDate;
        }



    }
}
