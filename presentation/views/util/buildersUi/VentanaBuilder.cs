using System.Collections;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Gestion.presentation.enums;

namespace Gestion.presentation.views.util.buildersUi;

public class VentanaBuilder<TEntidad>
{
    private readonly FormularioBuilder _fomularioBuilder = new();
    private readonly DetallesBuilder _detallesBuilder = new();
    private Dictionary<PropertyInfo, FrameworkElement> _controles = [];
    private Panel? _contenedorCampos;
    private Panel? _contenedorTablas;
    private Button? _btnGuardar;
    private Button? _btnImprimir;
    private TEntidad? _entidad;
    private ModoFormulario _modo = ModoFormulario.Edicion;

    public VentanaBuilder<TEntidad> SetEntidad(TEntidad entidad)
    {
        _entidad = entidad;
        return this;
    }

    public VentanaBuilder<TEntidad> SetContenedorCampos(Panel contenedor)
    {
        _contenedorCampos = contenedor;
        return this;
    }

    public VentanaBuilder<TEntidad> SetContenedorTablas(Panel contenedor)
    {
        _contenedorTablas = contenedor;
        return this;
    }

    public VentanaBuilder<TEntidad> SetBotonGuardar(Button btn)
    {
        _btnGuardar = btn;
        return this;
    }

    public VentanaBuilder<TEntidad> SetBotonImprimir(Button btn)
    {
        _btnImprimir = btn;
        return this;
    }

    public VentanaBuilder<TEntidad> SetModo(ModoFormulario modo)
    {
        _modo = modo;
        return this;
    }

    public void Build()
    {
        AplicarVisibilidadBotones();

        _fomularioBuilder
            .SetEntidad(_entidad!)
            .SetContenedor(_contenedorCampos!)
            .SetControles(_controles)
            .SetModo(_modo)
            .SetMaxFila(3)
            .Build();

        _detallesBuilder
            .SetEntidad(_entidad!)
            .SetContenedor(_contenedorTablas!)
            .SetModo(_modo)
            .Build();
    }

    private void AplicarVisibilidadBotones()
    {
        if (_btnGuardar != null)
        {
            _btnGuardar.Visibility =
                _modo == ModoFormulario.Edicion
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        if (_btnImprimir != null)
        {
            _btnImprimir.Visibility =
                _modo == ModoFormulario.SoloLectura
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }
    }

    public Dictionary<PropertyInfo, FrameworkElement> GetControles() => _controles;
}