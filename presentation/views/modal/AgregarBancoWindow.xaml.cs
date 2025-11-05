using System.Windows;

namespace Gestion.presentation.views.windows
{
    public partial class AgregarBancoWindow : Window
    {
        public string Codigo => txtCodigo.Text;
        public string Nombre => txtNombre.Text;
        public string Direccion => txtDireccion.Text;

        public AgregarBancoWindow()
        {
            InitializeComponent();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
