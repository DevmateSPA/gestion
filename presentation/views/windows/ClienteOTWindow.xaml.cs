using Gestion.core.model;
using Gestion.core.session;
using Gestion.presentation.viewmodel;
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

public partial class ClienteOTWindow : Window
{
    public ClienteOTWindow(Cliente cliente,String fechaDesde,String fechaHasta)
    {
        InitializeComponent();
        TxtOT.Text = "OT de "+cliente.Razon_Social+" ("+fechaDesde+" - "+fechaHasta+")";
    }

    private void ClienteOTWindow_Loaded(object sender, RoutedEventArgs e)
    {
        
    }

    private void BtnImprimir_Click(object sender, RoutedEventArgs e)
    {
        
    }


    private void BtnDocumento_Click(object sender, RoutedEventArgs e)
    {
        
    }


    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
{
    if (e.Key == Key.F4)
    {
        BtnImprimir_Click(sender, e);
        e.Handled = true;
        return;
    }

    if (e.Key == Key.Enter)
    {
        BtnDocumento_Click(sender, e);
        e.Handled = true;
        return;
    }

    if (e.Key == Key.Escape)
    {
        BtnCancelar_Click(sender, e);
        e.Handled = true;
        return;
    }
}

}
