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
            CargarImpresoraGuardada();
        }

        private void CargarImpresoras()
        {
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                lvImpresoras.Items.Add(printer);
            }
        }

        private void CargarImpresoraGuardada()
        {
            var aux = LocalConfig.ObtenerImpresora();
            var guardada = aux == "" ? "No seleccionada" : aux;

            txtImpresoraSeleccionada.Text =
                $"Impresora seleccionada: {guardada}";

            if (guardada != "No seleccionada")
            {
                lvImpresoras.SelectedItem =
                    lvImpresoras.Items
                        .Cast<string>()
                        .FirstOrDefault(p => p == guardada);
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

            // Guardar en JSON
            LocalConfig.GuardarImpresora(ImpresoraSeleccionada);

            txtImpresoraSeleccionada.Text =
                $"Impresora seleccionada: {ImpresoraSeleccionada}";

            DialogResult = true;
            Close();
        }
    }
}
