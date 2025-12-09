using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.session;
namespace Gestion.presentation.views.pages;

public partial class OrdenTrabajoDetallePage : Window
{
    private readonly object _entidadOriginal;

    public object EntidadEditada { get; private set; }

    public OrdenTrabajoDetallePage(Page padre, object entidad)
    {
        InitializeComponent();
        this.Owner = Window.GetWindow(padre);

        _entidadOriginal = entidad;

        // Crear copia profunda simple
        EntidadEditada = Activator.CreateInstance(entidad.GetType())!;
        foreach (var prop in entidad.GetType().GetProperties())
        {
            if (prop.CanWrite)
                prop.SetValue(EntidadEditada, prop.GetValue(entidad));
        }

        // Esc para cerrar
        this.PreviewKeyDown += (s, e) =>
        {
            if (e.Key == Key.Escape)
                this.DialogResult = false;
        };

        DataContext = EntidadEditada;
    }

    private void BtnGuardar_Click(object sender, RoutedEventArgs e)
    {
        EntidadEditada.GetType()
            .GetProperty("Empresa")?
            .SetValue(EntidadEditada, SesionApp.IdEmpresa);

        this.DialogResult = true;
    }

    

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        this.DialogResult = false;
    }
}