using Gestion.core.model;
using Gestion.core.session;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Gestion.presentation.views.windows;

public partial class ClienteCartolaWindow : Window
{
    private DataGrid _dataGrid;
    private readonly FacturaViewModel _viewModel;
    public ClienteCartolaWindow(FacturaViewModel viewModel, Cliente? cliente, DateTime fechaDesde,DateTime fechaHasta)
    {
        var total = 0;
        InitializeComponent();
         _viewModel = viewModel;
        DataContext = _viewModel;
        Title = $"Facturas";

        Loaded += async (_, _) => await _viewModel.LoadAllByRutClienteBetweenFecha(cliente.Rut, fechaDesde, fechaHasta);

        
        _dataGrid = dgFacturas;
        //_dataGrid.ItemContainerGenerator.StatusChanged += DgFacturas_StatusChanged;

        TxtCartola.Text = "Cartola de "+cliente.Razon_Social+" ("+cliente.Rut+")";
        TxtSaldo.Text = "Saldo inicial al "+fechaDesde.ToString("dd/mm/yyyy")+": "+total;
        TxtSaldoFinal.Text = "Saldo final al "+fechaHasta.ToString("dd/mm/yyyy")+": "+total;


    }

    private async void dgFacturas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (_dataGrid.SelectedItem is Factura facturaSeleccionado)
            await editar(facturaSeleccionado, "Editar Facturas");
    }

    private async Task editar(Factura factura, string titulo)
    {
        if (factura == null)
            return;

        var ventana = new EntidadEditorWindow(factura, titulo);

        if (ventana.ShowDialog() != true)
        {
            var facturaCancelada = (Factura)ventana.EntidadEditada;
            return;
        }

        var facturaEditada = (Factura)ventana.EntidadEditada;

        await _viewModel.Update(facturaEditada);
    }

    private void BtnFactura_Click(object sender, RoutedEventArgs e)
    {

    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
    
    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Enter:
                BtnFactura_Click(sender, e);
                e.Handled = true;
                break;

            case Key.Escape:
                BtnCancelar_Click(sender, e);
                e.Handled = true;
                break;
        }
    }

}
