using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Gestion.core.model;
using Gestion.core.services;
using Gestion.presentation.viewmodel;

namespace Gestion.presentation.views.pages;

public partial class AgregarPage : Page
{
    private readonly AgregarViewModel _viewModel;
    public AgregarPage()
    {
        InitializeComponent();
        _viewModel = new AgregarViewModel(App.ClienteService);
        this.DataContext = _viewModel;
    }

    private async void BtnVerClientes_Click(object sender, RoutedEventArgs e)
    {
        // Obtienes la lista desde el ViewModel
        List<Cliente> clientes = await _viewModel.GetClientes();

        ClientesListView.ItemsSource = clientes;
    }
}
