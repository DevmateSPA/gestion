using System.Reflection;
using System.Windows;
using System.Windows.Input;
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
    private readonly Func<EntidadEditorTableWindow, Task>? _btn1Action;
    private readonly string _titleBtnEx1;
    private readonly DialogService _dialogService = new();

    public EntidadEditorTableWindow(
        object entidad,
        Func<object, Task<bool>>? guardar,
        Action<OrdenTrabajo>? imprimir,
        Func<EntidadEditorTableWindow, Task>? btn1Action,
        ModoFormulario modo,
        bool shouldImprimir,
        string titulo,
        string titutloBtnEx1)
    {
        InitializeComponent();
        Title = titulo;

        _guardar = guardar;
        _imprimir = imprimir;
        _titleBtnEx1 = titutloBtnEx1;
        _btn1Action = btn1Action;

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

            if (_btn1Action != null) // Si esta definida la accion muestra si no no
                builder.SetBotonExtra1(btnExtra1, _titleBtnEx1);

        // Se genera la UI
        builder.Build();

        // Recuperamos los controles
        _controles = builder.GetControles();

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
        };
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

        _dialogService.ShowToast(this, "Se ha impreso correctamente.");

        // Para que no se cierre
        /*var ventana = Window.GetWindow(this);
        
        if (ventana != null)
            ventana.DialogResult = true;*/
    }

    private async void BtnExtra1_Click(object sender, RoutedEventArgs e)
    {
        if (_btn1Action != null)
            await _btn1Action(this);
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}