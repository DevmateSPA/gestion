using System;
using System.Drawing.Printing;
using System.Linq;
using System.Windows;

namespace Gestion.presentation.views.windows
{
    public partial class ImpresoraModal : Window
    {
        public string ImpresoraSeleccionada { get; private set; }

        public ImpresoraModal()
        {
            InitializeComponent();
            CargarImpresoras();
        }

        private void CargarImpresoras()
        {
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                lvImpresoras.Items.Add(printer);
            }
        }

        private void BtnSeleccionar_Click(object sender, RoutedEventArgs e)
        {
            if (lvImpresoras.SelectedItem == null)
            {
                MessageBox.Show("Debes seleccionar una impresora.");
                return;
            }

            ImpresoraSeleccionada = lvImpresoras.SelectedItem.ToString();
            DialogResult = true;
            Close();
        }
    }
}
