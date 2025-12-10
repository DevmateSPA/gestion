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

public partial class ClienteCartolaWindow : Window
{
    public ClienteCartolaWindow()
    {
        InitializeComponent();
    }

    private async void ClienteCartolaWindow_Loaded(object sender, RoutedEventArgs e)
    {
        
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
    

}
