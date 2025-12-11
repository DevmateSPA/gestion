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
    public ClienteCartolaWindow(Cliente cliente,DateTime fechaDesde,DateTime fechaHasta)
    {
        var total = 0;
        InitializeComponent();
        Console.Write(fechaDesde);
        TxtCartola.Text = "Cartola de "+cliente.Razon_Social+" ("+cliente.Rut+")";
        TxtSaldo.Text = "Saldo inicial al "+fechaDesde.ToString("dd/mm/yyyy")+": "+total;
        TxtSaldoFinal.Text = "Saldo final al "+fechaHasta.ToString("dd/mm/yyyy")+": "+total;
    }

    private void ClienteCartolaWindow_Loaded(object sender, RoutedEventArgs e)
    {
        
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
