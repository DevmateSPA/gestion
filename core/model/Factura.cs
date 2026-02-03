using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gestion.core.attributes;
using Gestion.core.attributes.validation;
using Gestion.core.interfaces.service;

namespace Gestion.core.model;

// Implementa la lista de detalles, implementa la interfaz IModel
public class Factura : TotalBase
{
    private const int GRUPO_DOCUMENTO = 10;
    private const int GRUPO_CLIENTE = 20;
    private const int GRUPO_GUIAS = 40;
    private const int GRUPO_ORDENES = 50;

    [Nombre("Folio")]
    [Required]
    [Grupo("Datos del Documento", GRUPO_DOCUMENTO)]
    [Orden(0)]
    public string Folio { get; set; } = string.Empty;

    [Nombre("Fecha")]
    [Grupo("Datos del Documento", GRUPO_DOCUMENTO)]
    [Orden(1)]
    public DateTime Fecha { get; set; } = DateTime.Today;

    [Nombre("Fecha de Vencimiento")]
    [Grupo("Datos del Documento", GRUPO_DOCUMENTO)]
    [Orden(2)]
    public DateTime FechaVencimiento { get; set; } = DateTime.Today;

    [Nombre("Rut")]
    [Rut]
    [Grupo("Cliente", GRUPO_CLIENTE)]
    [Orden(0)]
    [Lookup(
        typeof(IClienteService),
        nameof(IClienteService.FindByRut),
        "Razon_Social:NombreCliente"
    )]
    [Searchable("RUT_CLIENTE")]
    public string RutCliente
    {
        get => _rutCliente;
        set
        {
            if (_rutCliente == value) return;
            _rutCliente = value; 
            OnPropertyChanged(nameof(RutCliente));
        }
    }

    private string _rutCliente = string.Empty;
    private string _nombreCliente = "Sin asignar";

    [Nombre("Nombre")]
    [Grupo("Cliente", GRUPO_CLIENTE)]
    [Orden(1)]
    [NoSaveDb]
    [OnlyRead]
    [TextArea]
    public string NombreCliente
    {
        get => _nombreCliente;
        set
        {
            if (_nombreCliente == value) return;
            _nombreCliente = value;
            OnPropertyChanged(nameof(NombreCliente));
        }
    }

    [Nombre("Crédito")]
    [Grupo("Totales", GRUPO_TOTALES)]
    [Orden(5)]
    public long TipoCredito { get; set; }

    [Nombre("Haber")]
    [Grupo("Totales", GRUPO_TOTALES)]
    [Orden(6)]
    [Visible(false)]
    public int Habe { get; set; }

    [NotMapped]
    [Visible(false)]
    public decimal Debe => Total - Habe;

    [Nombre("Nota de Crédito")]
    [Grupo("Totales", GRUPO_TOTALES)]
    [Orden(7)]
    public long NotaCredito { get; set; }

    [Visible(false)]
    public string Tipo { get; set; } = string.Empty;

    private const string OrdenesDeTrabajo = "Ordenes de Trabajo";

    [Nombre("Orden 1")]
    [Grupo(OrdenesDeTrabajo, GRUPO_ORDENES)]
    [Orden(0)]
    [Searchable("FOLIO_OT")]
    public string Ot01 { get; set; } = string.Empty;

    [Nombre("Orden 2")]
    [Grupo(OrdenesDeTrabajo, GRUPO_ORDENES)]
    [Orden(1)]
    [Searchable("FOLIO_OT")]
    public string Ot02 { get; set; } = string.Empty;
    [Nombre("Orden 3")]
    [Grupo(OrdenesDeTrabajo, GRUPO_ORDENES)]
    [Orden(2)]
    [Searchable("FOLIO_OT")]
    public string Ot03 { get; set; } = string.Empty;
    [Nombre("Orden 4")]
    [Grupo(OrdenesDeTrabajo, GRUPO_ORDENES)]
    [Orden(3)]
    [Searchable("FOLIO_OT")]
    public string Ot04 { get; set; } = string.Empty;
    [Nombre("Orden 5")]
    [Grupo(OrdenesDeTrabajo, GRUPO_ORDENES)]
    [Orden(4)]
    [Searchable("FOLIO_OT")]
    public string Ot05 { get; set; } = string.Empty;

    [Visible(false)] public string Op01 { get; set; } = string.Empty;
    [Visible(false)] public string Op02 { get; set; } = string.Empty;
    [Visible(false)] public string Op03 { get; set; } = string.Empty;
    [Visible(false)] public string Op04 { get; set; } = string.Empty;
    [Visible(false)] public string Op05 { get; set; } = string.Empty;

    private const string GuiasDespacho = "Guías de Despacho";

    [Nombre("Guía 1")]
    [Grupo(GuiasDespacho, GRUPO_GUIAS)]
    [Orden(0)]
    [Searchable("FOLIO_GD")]
    public string Guia1 { get; set; } = string.Empty;

    [Nombre("Guía 2")]
    [Grupo(GuiasDespacho, GRUPO_GUIAS)]
    [Orden(1)]
    [Searchable("FOLIO_GD")]
    public string Guia2 { get; set; } = string.Empty;

    [Nombre("Guía 3")]
    [Grupo(GuiasDespacho, GRUPO_GUIAS)]
    [Orden(2)]
    [Searchable("FOLIO_GD")]
    public string Guia3 { get; set; } = string.Empty;

    [Nombre("Guía 4")]
    [Grupo(GuiasDespacho, GRUPO_GUIAS)]
    [Orden(3)]
    [Searchable("FOLIO_GD")]
    public string Guia4 { get; set; } = string.Empty;

    [Nombre("Guía 5")]
    [Grupo(GuiasDespacho, GRUPO_GUIAS)]
    [Orden(4)]
    [Searchable("FOLIO_GD")]
    public string Guia5 { get; set; } = string.Empty;

    [Nombre("Película")]
    [Orden(1)]
    public bool Peli { get; set; }
    [Nombre("Plan")]
    [Orden(2)]
    public bool Plan { get; set; }

    [Grupo("Detalles", GRUPO_TOTALES+2)]
    [Nombre("Detalle")]
    [TextArea]
    public string Memo { get; set; } = string.Empty;
}