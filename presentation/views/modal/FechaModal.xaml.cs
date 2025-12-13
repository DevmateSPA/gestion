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

        public DateTime? FechaDesde =>
            DateTime.TryParseExact(txtDesde.Text, "dd/MM/yyyy", null, DateTimeStyles.None, out var d)
                ? d
                : null;

        public DateTime? FechaHasta =>
            DateTime.TryParseExact(txtHasta.Text, "dd/MM/yyyy", null, DateTimeStyles.None, out var d)
                ? d
                : null;

        public FechaModal(string titulo)
        {
            InitializeComponent();
            Titulo = titulo;
            DataContext = this;

            string hoy = DateTime.Now.ToString("dd/MM/yyyy");
            txtDesde.Text = hoy;
            txtHasta.Text = hoy;
        }

        public FechaModal()
        {
        }

        private void FechaMask(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text[0]))
            {
                e.Handled = true;
                return;
            }

            var tb = (TextBox)sender;

            // Posición actual del cursor
            int pos = tb.SelectionStart;

            string text = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
            text = text.Insert(tb.SelectionStart, e.Text);

            text = text.Replace("/", "");

            if (text.Length > 8)
            {
                e.Handled = true;
                return;
            }

            // reconstruimos dd/MM/yyyy
            if (text.Length >= 2)
                text = text.Insert(2, "/");

            if (text.Length >= 5)
                text = text.Insert(5, "/");

            tb.Text = text;
            tb.CaretIndex = tb.Text.Length;

            e.Handled = true;
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


    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;

        if (txtDesde.IsKeyboardFocusWithin)
        {
            txtHasta.Focus();
            txtHasta.CaretIndex = txtHasta.Text.Length;
        }
        else if (txtHasta.IsKeyboardFocusWithin)
        {
            BtnSiguiente_Click(sender, e);
        }
        else
        {
            BtnSiguiente_Click(sender, e);
        }

        e.Handled = true;
    }

    }
}
