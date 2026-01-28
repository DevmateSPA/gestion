using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Gestion.core.model;
using Gestion.core.model.detalles;
using Gestion.core.services;
using Gestion.helpers;
using Gestion.presentation.views.util;
using Gestion.presentation.views.windows;

namespace Gestion.presentation.views.pages;

public partial class OrdenTrabajoDetallePage : Window
{
    private readonly OrdenTrabajo _entidadOriginal;
    private readonly Func<OrdenTrabajo, Task<bool>> _accion;
    private readonly Func<OrdenTrabajo, Task> _syncDetalles;

    public OrdenTrabajo EntidadEditada { get; private set; }

    private readonly DialogService _dialogService = new(); 
	private string _tituloBotonEntregadas = "Entregada (F5)"; 

    public OrdenTrabajoDetallePage(
        OrdenTrabajo entidad,
        Func<OrdenTrabajo, Task<bool>> accion,
        Func<OrdenTrabajo, Task> syncDetalles)
    {
        InitializeComponent();

        _entidadOriginal = entidad;
        _accion = accion;
        _syncDetalles = syncDetalles;

        EntidadEditada = ClonarEntidad(_entidadOriginal);
        DataContext = EntidadEditada;

        Loaded += OnLoaded;
        PreviewKeyDown += OnPreviewKeyDown;
    }

    private static OrdenTrabajo ClonarEntidad(OrdenTrabajo entidad)
    {
        var tipo = entidad.GetType();
        var clon = (OrdenTrabajo)Activator.CreateInstance(tipo)!;

        foreach (var prop in tipo.GetProperties())
        {
            if (!prop.CanWrite)
                continue;

            if (prop.Name == nameof(OrdenTrabajo.Detalles))
                continue;

            prop.SetValue(clon, prop.GetValue(entidad));
        }

        clon.Detalles = new ObservableCollection<DetalleOrdenTrabajo>(entidad.Detalles.Select(d => d.Clone()));

        return clon;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        BindearCamposDinamico();
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
            DialogResult = false;
    }

    // =========================
    // GUARDAR
    // =========================
    private bool Validar()
    {
        var errores = ValidationHelper.GetValidationErrors(spCampos);

        if (errores.Count == 0)
            return true;

        DialogUtils.MostrarErroresValidacion(errores);
        return false;
    }

    private async Task EjecutarAcción()
    {
        if (_accion != null)
        {
            bool ok = await _accion(EntidadEditada);

            if (!ok)
                return;

            await _syncDetalles(EntidadEditada);
        }

        DialogUtils.MostrarInfo(Mensajes.OperacionExitosa, "Éxito");
        DialogResult = true;
        Close();
    }

    private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
    {
        if (!Validar())
            return;

        await EjecutarAcción();
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

        // 1️⃣ Leer impresora guardada
        string impresora = LocalConfig.ObtenerImpresora();

        // 2️⃣ Si no hay impresora válida, abrir modal
        if (string.IsNullOrWhiteSpace(impresora) || impresora == "")
        {
            var modal = new ImpresoraModal
            {
                Owner = this
            };

            if (modal.ShowDialog() != true)
                return; // Usuario canceló

            impresora = modal.ImpresoraSeleccionada;

            if (string.IsNullOrWhiteSpace(impresora))
                return;
        }

        // 3️⃣ Continuar impresión
        string pdfPath = PrintUtils.GenerarOrdenTrabajoPrint(_entidadOriginal as OrdenTrabajo);

        string sumatra = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "SumatraPDF.exe"
        );

        PrintUtils.PrintFile(pdfPath, impresora, sumatra);
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
                var binding = BindingFactory.CreateValidateBinding(propInfo, DataContext, BindingMode.TwoWay);
                tb.SetBinding(TextBox.TextProperty, binding);
            }
            // DatePicker
            else if (ctrl is DatePicker dp)
            {
                var binding = BindingFactory.CreateValidateBinding(
                    propInfo,
                    DataContext,
                    BindingMode.TwoWay,
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