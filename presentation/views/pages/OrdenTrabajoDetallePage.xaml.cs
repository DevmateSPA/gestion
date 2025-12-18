using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Gestion.core.model;
using Gestion.core.session;
using Gestion.presentation.utils;
using Gestion.presentation.views.util;
using Gestion.presentation.views.windows;

namespace Gestion.presentation.views.pages;

public partial class OrdenTrabajoDetallePage : Window
{
    private readonly object _entidadOriginal;

    public object EntidadEditada { get; private set; }

    public OrdenTrabajoDetallePage(object padre, object entidad)
    {
        InitializeComponent();
        if (padre is Window windowPadre)
        {
            this.Owner = windowPadre; 
        }
        else if (padre is Page pagePadre)
        {
            this.Owner = Window.GetWindow(pagePadre);
        }

        _entidadOriginal = entidad;

        // Clonado simple de la entidad
        EntidadEditada = Activator.CreateInstance(entidad.GetType())!;
        foreach (var prop in entidad.GetType().GetProperties())
        {
            if (prop.CanWrite)
                prop.SetValue(EntidadEditada, prop.GetValue(entidad));
        }

        DataContext = EntidadEditada;

        Loaded += (_, _) =>
        {
            BindearCamposDinamico();
        };

        // ESC para cerrar
        PreviewKeyDown += (s, e) =>
        {
            if (e.Key == Key.Escape)
                DialogResult = false;
        };
    }

    // =========================
    // GUARDAR
    // =========================
    private void BtnGuardar_Click(object sender, RoutedEventArgs e)
    {
        var errores = ValidationHelper.GetValidationErrors(this);
        if (errores.Count != 0)
        {
            DialogUtils.MostrarErroresValidacion(errores);
            return;
        }

        EntidadEditada.GetType()
            .GetProperty("Empresa")?
            .SetValue(EntidadEditada, SesionApp.IdEmpresa);

        DialogResult = true;
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    // =========================
    // IMPRIMIR
    // =========================
    private void BtnImprimir_Click(object sender, RoutedEventArgs e)
    {
        var errores = ValidationHelper.GetValidationErrors(this);
        if (errores.Count != 0)
        {
            DialogUtils.MostrarErroresValidacion(errores);
            return;
        }
        var modal = new ImpresoraModal
        {
            Owner = this 
        };

        if (modal.ShowDialog() == true)
        {
            string impresora = modal.ImpresoraSeleccionada;
            MessageBox.Show("Impresora seleccionada: " + impresora);
            string pdfPath = PrintUtils.GenerarOrdenTrabajoPrint(_entidadOriginal as OrdenTrabajo);
            string sumatra = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SumatraPDF.exe");
            PrintUtils.PrintFile(pdfPath, impresora, sumatra);
        }
       
    }

    // =========================
    // VALIDACIÓN
    // =========================
    private void BindearCamposDinamico()
    {
        if (DataContext == null)
            return;

        var tipo = DataContext.GetType();

        foreach (var ctrl in FindVisualChildren<Control>(this))
        {
            string? propiedad = ctrl.Name switch
            {
                var n when n.StartsWith("txt") => n.Substring(3),
                var n when n.StartsWith("dp")  => n.Substring(2),
                _ => null
            };

            if (string.IsNullOrWhiteSpace(propiedad))
                continue;

            var propInfo = tipo.GetProperty(propiedad);
            if (propInfo == null || !propInfo.CanWrite)
                continue;

            // TextBox
            if (ctrl is TextBox tb)
            {
                var binding = BindingFactory.CreateValidateBinding(propInfo, DataContext);
                tb.SetBinding(TextBox.TextProperty, binding);
            }
            // DatePicker
            else if (ctrl is DatePicker dp)
            {
                var binding = BindingFactory.CreateValidateBinding(
                    propInfo,
                    DataContext,
                    "dd/MM/yyyy"
                );

                dp.SetBinding(DatePicker.SelectedDateProperty, binding);
            }
        }

        // 🔥 Fuerza validación inicial
        ValidationHelper.ForceValidation(this);
    }

    private static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent)
        where T : DependencyObject
    {
        if (parent == null)
            yield break;

        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);

            if (child is T t)
                yield return t;

            foreach (var sub in FindVisualChildren<T>(child))
                yield return sub;
        }
    }
}