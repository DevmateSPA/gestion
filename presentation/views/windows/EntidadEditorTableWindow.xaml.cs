using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.attributes;
using Gestion.core.interfaces.lookup;
using Gestion.core.model;
using Gestion.core.services;
using Gestion.presentation.enums;
using Gestion.presentation.views.util;
using Gestion.presentation.views.util.buildersUi;

namespace Gestion.presentation.views.windows;

public partial class EntidadEditorTableWindow: Window
{
    private Dictionary<PropertyInfo, FrameworkElement> _controles = [];
    public object? EntidadEditada { get; private set; }
    private readonly Func<object, Task<bool>>? _guardar;
    private readonly Action<OrdenTrabajo>? _imprimir;
    private readonly DialogService _dialogService = new();

    private readonly Func<EntidadEditorTableWindow, Task>? _btn1Action;
    private readonly string _titleBtnEntregar;
    private readonly ILookupResolver _lookupResolver;

    public EntidadEditorTableWindow(
        ILookupResolver lookupResolver,
        object entidad,
        Func<object, Task<bool>>? guardar,
        Action<OrdenTrabajo>? imprimir,
        Func<EntidadEditorTableWindow, Task>? btn1Action,
        ModoFormulario modo,
        bool shouldImprimir,
        string titulo = "Ventana con tabla",
        string titutloBtnEntregar = "")
    {
        InitializeComponent();
        Title = titulo;
        _titleBtnEntregar = titutloBtnEntregar;

        _guardar = guardar;
        _imprimir = imprimir;
        _btn1Action = btn1Action;

        _lookupResolver = lookupResolver;

        ClonarEntidad(entidad);

        if (EntidadEditada == null)
            throw new InvalidOperationException("Entidad clonada no definida.");

        InicializarUI(EntidadEditada, modo, shouldImprimir);
        InicializarEventos();
    }

    private void ClonarEntidad(object entidad)
    {
        EntidadEditada = Activator.CreateInstance(entidad.GetType())!;

        foreach (var prop in entidad.GetType().GetProperties())
        {
            if (prop.CanWrite)
                prop.SetValue(EntidadEditada, prop.GetValue(entidad));
        }
    }

    private void InicializarUI(object entidad, ModoFormulario modo, bool shouldImprimir)
    {
        // Crear el builder para la entidad
        var builder = new VentanaBuilder<object>()
            .SetEntidad(entidad)
            .SetContenedorCampos(spCampos)
            .SetContenedorTablas(spTabla)
            .SetBotonGuardar(btnGuardar)
            .SetBotonImprimir(btnImprimir, shouldImprimir)
            .SetModo(modo);
        if (_btn1Action != null) 
            builder.SetBotonEntregar(btnEntregar, _titleBtnEntregar);
        // Se genera la UI
        builder.Build();

        // Recuperamos los controles
        _controles = builder.GetControles();

        InicializarLookups();

        // Validamos los campos inicialmente
        FormularioValidator.ForzarValidacionInicial(_controles);

        // Se enfooca el control
        _controles.Values.FirstOrDefault()?.Focus();
    }

    private void InicializarEventos()
    {
        PreviewKeyDown += (_, e) =>
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
            } 
            if (e.Key == Key.F5)
            {
                BtnEntregar_Click(null, null);
                e.Handled = true;
            }
        };
    }

    private void HookLookup(
        PropertyInfo prop,
        FrameworkElement control)
    {
        if (control is TextBox tb)
        {
            tb.LostFocus += async (_, __) =>
            {
                if (EntidadEditada == null)
                    return;

                await _lookupResolver.ResolveAsync(
                    EntidadEditada,
                    prop.Name
                );

                // 🔧 PARCHE AQUÍ
                tb.GetBindingExpression(TextBox.TextProperty)
                ?.UpdateTarget();
            };
        }
    }

    private void InicializarLookups()
    {
        foreach (var (prop, control) in _controles)
        {
            var lookup = prop.GetCustomAttribute<LookupAttribute>();
            if (lookup == null)
                continue;

            HookLookup(prop, control);
        }
    }

    private bool Validar()
    {
        var errores = ValidationHelper.GetValidationErrors(spCampos);

        if (errores.Count == 0)
            return true;

        DialogUtils.MostrarErroresValidacion(errores);
        return false;
    }

    private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
    {
        if (!Validar())
            return;

        if (EntidadEditada == null)
            throw new InvalidOperationException("Entidad clonada no definida.");

        if (_guardar == null)
            throw new InvalidOperationException("Función de guardado no proporcionada.");
            
        var ok = await _guardar(EntidadEditada);

        if (!ok)
            return;

        _dialogService.ShowToast(this, "Los datos se han guardado correctamente.");

        var ventana = Window.GetWindow(this);
        
        if (ventana != null)
            ventana.DialogResult = true;
    }

    private async void BtnImprimir_Click(object sender, RoutedEventArgs e)
    {
        if (!Validar())
            return;

        if (EntidadEditada == null)
            throw new InvalidOperationException("Entidad clonada no definida.");

        if (_guardar != null)
            await _guardar(EntidadEditada);

        if (_imprimir == null)
            throw new InvalidOperationException("Función de impresión no proporcionada.");

        _imprimir((OrdenTrabajo)EntidadEditada);

        //_dialogService.ShowToast(this, "Se ha impreso correctamente.");

        //Window.GetWindow(this)?.Close();
        var ventana = Window.GetWindow(this);
        
        //if (ventana != null)
        //    ventana.DialogResult = true;
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private async void BtnEntregar_Click(object sender, RoutedEventArgs e)
    {
        if (_btn1Action != null)
            await _btn1Action(this);
    }

}