using Gestion.presentation.views.util.buildersUi;
using Gestion.core.model;
using Gestion.core.model.detalles;

namespace Gestion.presentation.config;

/// <summary>
/// Configuración central de la capa de presentación.
/// 
/// Aquí se registran definiciones de UI reutilizables, como:
/// - Columnas de DataGrid asociadas a tipos de entidad
/// 
/// Este método debe ejecutarse una sola vez al iniciar la aplicación.
/// </summary>
public static class UiConfig
{
    /// <summary>
    /// Registra todas las configuraciones de interfaz necesarias
    /// para el correcto funcionamiento de los builders de UI.
    /// </summary>
    public static void Register()
    {
        RegisterDataGrids();
    }

    /// <summary>
    /// Registra las configuraciones de columnas de DataGrid
    /// asociadas a tipos de entidades específicas.
    /// 
    /// Estas configuraciones serán utilizadas automáticamente
    /// por <see cref="DataGridBuilder{T}"/> cuando se construyan
    /// tablas para dichas entidades.
    /// </summary>
    private static void RegisterDataGrids()
    {
        // DataGrid para FacturaCompraProducto
        DataGridColumnRegistry.Register<FacturaCompraProducto>(
            new() { Header = "Código",  Binding = "Producto",       Width = 100 },
            new() { Header = "Nombre",  Binding = "Productonombre", Width = 800 },
            new() { Header = "Cantidad",Binding = "Entrada",        Width = 100 }
        );

        // DataGrid para DetalleOrdenTrabajo
        DataGridColumnRegistry.Register<DetalleOrdenTrabajo>(
            new() { Header = "Tipo Papel",     Binding = "TipoPapel",  Width = 90  },
            new() { Header = "Cantidad",       Binding = "Cantidad",   Width = 100 },
            new() { Header = "Sobras",          Binding = "Sobras",     Width = 160 },
            new() { Header = "Sobr.",           Binding = "Sobr",       Width = 160 },
            new() { Header = "Total",           Binding = "Total",      Width = 160 },
            new() { Header = "Tamaño Pliegos",  Binding = "TamPliegos", Width = 160 },
            new() { Header = "Tamaño",          Binding = "Tamanio",    Width = 160 }
        );
    }
}