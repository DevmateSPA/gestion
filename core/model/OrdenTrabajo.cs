using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gestion.core.attributes;
using Gestion.core.attributes.validation;
using Gestion.core.interfaces.model;
using Gestion.core.model.detalles;

namespace Gestion.core.model;

public class OrdenTrabajo : ConDetalles<DetalleOrdenTrabajo>, IEmpresa
{
    private const int ORDEN_GRUPO_GENERAL = 10;
    private const int ORDEN_GRUPO_CLIENTE = ORDEN_GRUPO_GENERAL + 10;
    private const int ORDEN_GRUPO_TRABAJO = ORDEN_GRUPO_CLIENTE + 10;
    private const int ORDEN_GRUPO_PRODUCCION = ORDEN_GRUPO_TRABAJO + 10;
    private const int ORDEN_GRUPO_TAMAÑO = ORDEN_GRUPO_PRODUCCION + 10;
    private const int ORDEN_GRUPO_MONTAR = ORDEN_GRUPO_TAMAÑO + 10;
    private const int ORDEN_GRUPO_CLIENTE_PROPORCIONA = ORDEN_GRUPO_MONTAR + 10;
    private const int ORDEN_GRUPO_TIPO_IMPRESION = ORDEN_GRUPO_CLIENTE_PROPORCIONA + 10;
    private const int ORDEN_GRUPO_DATOS_MAQUINA = ORDEN_GRUPO_TIPO_IMPRESION + 10;
    private const int ORDEN_GRUPO_SOBRES = ORDEN_GRUPO_DATOS_MAQUINA + 10;
    private const int ORDEN_GRUPO_SACOS = ORDEN_GRUPO_SOBRES + 10;
    private const int ORDEN_GRUPO_TINTAS = ORDEN_GRUPO_SACOS + 10;
    private const int ORDEN_GRUPO_BLOCK = ORDEN_GRUPO_TINTAS + 10;
    private const int ORDEN_GRUPO_TALONARIOS = ORDEN_GRUPO_BLOCK + 10;
    private const int ORDEN_GRUPO_PAQUETE = ORDEN_GRUPO_TALONARIOS + 10;
    private const int ORDEN_GRUPO_RESMA = ORDEN_GRUPO_PAQUETE + 10;
    private const int ORDEN_GRUPO_FAJADO = ORDEN_GRUPO_RESMA + 10;
    private const int ORDEN_GRUPO_OPERARIOS = ORDEN_GRUPO_FAJADO + 10;

    // -- Grupo Datos del Documento
    public long Id { get; set; }
    [Required]
    [Grupo("Datos del Documento", ORDEN_GRUPO_GENERAL)]
    [Orden(0)]
    public string Folio { get; set; } = string.Empty;
    [Fecha]
    [Grupo("Datos del Documento", ORDEN_GRUPO_GENERAL)]
    [Orden(1)]
    public DateTime Fecha { get; set; } = DateTime.Now;

    // -- Grupo Cliente

    [Rut]
    [Nombre("Rut")]
    [Grupo("Cliente", ORDEN_GRUPO_CLIENTE)]
    [Orden(0)]
    public string RutCliente { get; set; } = string.Empty;
    [NoSaveDb]
    [OnlyRead]
    [Nombre("Razón Social")]
    [Grupo("Cliente", ORDEN_GRUPO_CLIENTE)]
    [Orden(1)]
    public string RazonSocial { get; set; } = "Por asignar";

    // -- Grupo Trabajo

    [Nombre("Descripción del Trabajo")]
    [Grupo("Trabajo", ORDEN_GRUPO_TRABAJO)]
    [Orden(0)]
    [TextArea]
    public string Descripcion { get; set; } = string.Empty;

    // -- Datos de Producción

    [Nombre("Cantidad")]
    [Grupo("Producción", ORDEN_GRUPO_PRODUCCION)]
    [Orden(0)]
    public int Cantidad { get; set; } = 0;
    [Nombre("Total Impresión")]
    [Grupo("Producción", ORDEN_GRUPO_PRODUCCION)]
    [Orden(1)]
    public int TotalImpresion { get; set; } = 0;
    [Nombre("Folio Inicio")]
    [Grupo("Producción", ORDEN_GRUPO_PRODUCCION)]
    [Orden(2)]
    public string FolioDesde { get; set; } = string.Empty;
    [Nombre("Folio Hasta")]
    [Grupo("Producción", ORDEN_GRUPO_PRODUCCION)]
    [Orden(3)]
    public string FolioHasta { get; set; } = string.Empty;

    [Fecha]
    [Nombre("Fecha Entrega")]
    [Grupo("Fechas", ORDEN_GRUPO_PRODUCCION+1)]
    [Orden(0)]
    public DateTime FechaEntrega { get; set; } = DateTime.Now;


    [Fecha]
    [Nombre("Fecha Término")]
    [Grupo("Fechas", ORDEN_GRUPO_PRODUCCION+1)]
    [Orden(1)]
    public DateTime FechaTermino { get; set; } = DateTime.Now;

    [Nombre("Observaciones Entrega")]
    [Grupo("Fechas", ORDEN_GRUPO_PRODUCCION+1)]
    [Orden(2)]
    [TextArea]
    public string Observacion7 { get; set; } = string.Empty;

    [Nombre("Observaciones Termino")]
    [Grupo("Fechas", ORDEN_GRUPO_PRODUCCION+1)]
    [Orden(3)]
    [TextArea]
    public string Observacion6 { get; set; } = string.Empty;

    // Guías de Despacho
    [Nombre("Guía Desde")]
    [Grupo("Guías de Despacho", ORDEN_GRUPO_PRODUCCION+2)]
    [Orden(0)]
    public string GuiaDesde { get; set; } = string.Empty;
    [Nombre("Guía Hasta")]
    [Grupo("Guías de Despacho", ORDEN_GRUPO_PRODUCCION+2)]
    [Orden(1)]
    public string GuiaHasta { get; set; } = string.Empty;

    [Nombre("Observaciones Guía")]
    [Grupo("Guías de Despacho", ORDEN_GRUPO_PRODUCCION+2)]
    [Orden(2)]
    [TextArea]
    public string Observacion8 { get; set; } = string.Empty;
    
    // Facturas

    [Nombre("Factura Desde")]
    [Grupo("Facturas", ORDEN_GRUPO_PRODUCCION+3)]
    [Orden(0)]
    public string FacturaDesde { get; set; } = string.Empty;

    [Nombre("Factura Hasta")]
    [Grupo("Facturas", ORDEN_GRUPO_PRODUCCION+3)]
    [Orden(1)]
    public string FacturaHasta { get; set; } = string.Empty;

    [Nombre("Observaciones Factura")]
    [Grupo("Facturas", ORDEN_GRUPO_PRODUCCION+3)]
    [Orden(2)]
    [TextArea]
    public string Observacion9 { get; set; } = string.Empty;

    // -- Grupo Tamaño

    [Nombre("Cortar Tamaño")]
    [Grupo("Tamaño", ORDEN_GRUPO_TAMAÑO)]
    [Orden(0)]
    public int CortarTamanio { get; set; } = 0;
    [Nombre("Cortar Tamaño N°")]
    [Grupo("Tamaño", ORDEN_GRUPO_TAMAÑO)]
    [Orden(1)]
    public string CortarTamanion { get; set; } = string.Empty;
    [Nombre("Cortar Tamaño Largo")]
    [Grupo("Tamaño", ORDEN_GRUPO_TAMAÑO)]
    [Orden(2)]
    public string CortarTamanioLargo { get; set; } = string.Empty;

    // -- Grupo Montar

    [Nombre("Montar")]
    [Grupo("Montar", ORDEN_GRUPO_MONTAR)]
    [Orden(0)]
    public int Montar { get; set; } = 0;
    [Nombre("Molde Tamaño")]
    [Grupo("Montar", ORDEN_GRUPO_MONTAR)]
    [Orden(1)]
    public int MoldeTamanio { get; set; } = 0;
    [Nombre("Largo Final")]
    [Grupo("Montar", ORDEN_GRUPO_MONTAR)]
    [Orden(2)]
    public string TamanioFinalAncho { get; set; } = string.Empty;
    [Nombre("Ancho Final")]
    [Grupo("Montar", ORDEN_GRUPO_MONTAR)]
    [Orden(3)]
    public string TamanioFinalLargo { get; set; } = string.Empty;

    // -- Cliente Proporciona
    [Visible(false)]
    [NotMapped]
    public string ClienteProporcionanada { get; set; } = string.Empty;
    [Visible(false)]
    [NotMapped]
    public string ClienteProporcionaOriginal { get; set; } = string.Empty;
    [Nombre("Película")]
    [Grupo("Cliente Proporciona", ORDEN_GRUPO_CLIENTE_PROPORCIONA)]
    [Orden(0)]
    public bool ClienteProporcionaPelicula { get; set; } = false;
    [Nombre("Plancha")]
    [Grupo("Cliente Proporciona", ORDEN_GRUPO_CLIENTE_PROPORCIONA)]
    [Orden(1)]
    public bool ClienteProporcionaPlancha { get; set; } = false;
    [Nombre("Papel")]
    [Grupo("Cliente Proporciona", ORDEN_GRUPO_CLIENTE_PROPORCIONA)]
    [Orden(2)]
    public bool ClienteProporcionaPapel { get; set; } = false;

    // -- Tipo de impresión

    [Nombre("Tipo de Impresión")]
    [Grupo("Tipo de Impresión", ORDEN_GRUPO_TIPO_IMPRESION)]
    [Orden(0)]
    [RadioGroup(
        "Solo tiro", "1", 
        "T/R M.P.", "2", 
        "T/R P.D.", "3"
    )]
    public string? TipoImpresion { get; set; } = null;

    // -- Datos de Máquina

    [Nombre("Máquina 1")]
    [Grupo("Datos de Máquina", ORDEN_GRUPO_DATOS_MAQUINA)]
    [Orden(0)]
    public string Maquina1 { get; set; } = string.Empty;
    [NoSaveDb]
    [Visible(false)]
    public string Maquina1descripcion { get; set; } = string.Empty;
    [Nombre("Máquina 2")]
    [Grupo("Datos de Máquina", ORDEN_GRUPO_DATOS_MAQUINA)]
    [Orden(1)]
    public string Maquina2 { get; set; } = string.Empty;
    [Nombre("Pin")]
    [Grupo("Datos de Máquina", ORDEN_GRUPO_DATOS_MAQUINA)]
    [Orden(2)]
    public string Pin { get; set; } = string.Empty;
    [Nombre("NVA")]
    [Grupo("Datos de Máquina", ORDEN_GRUPO_DATOS_MAQUINA)]
    [Orden(3)]
    public int Nva { get; set; } = 0;
    [Nombre("US")]
    [Grupo("Datos de Máquina", ORDEN_GRUPO_DATOS_MAQUINA)]
    [Orden(4)]
    public int Us { get; set; } = 0;
    [Nombre("CTP NVA")]
    [Grupo("Datos de Máquina", ORDEN_GRUPO_DATOS_MAQUINA)]
    [Orden(5)]
    public int CtpNva { get; set; } = 0;
    [Nombre("U")]
    [Grupo("Datos de Máquina", ORDEN_GRUPO_DATOS_MAQUINA)]
    [Orden(6)]
    public int U { get; set; } = 0;

    // -- Grupo Sobres

    [Nombre("Sobres")]
    [Grupo("Sobres", ORDEN_GRUPO_SOBRES)]
    [Orden(0)]
    [RadioGroup(
        "Cte.", "1",
        "Amer.", "2",
        "Amer. Especial", "3",
        "1/2 Oficio", "4",
        "Oficio", "5",
        "Otro", "6"
    )]
    public string? Sobres { get; set; } = null;

    // -- Grupo Sacos

    [Nombre("Sacos")]
    [Grupo("Sacos", ORDEN_GRUPO_SACOS)]
    [Orden(0)]
    [RadioGroup(
        "Amer.", "1",
        "Amer. Especial", "2",
        "1/2 Oficio", "3",
        "Oficio", "4",
        "Otro", "5"
    )]
    public string? Sacos { get; set; } = null;

    // -- Grupo Tintas

    [Nombre("Tinta 1")]
    [Grupo("Tintas", ORDEN_GRUPO_TINTAS)]
    [Orden(0)]
    [RadioGroup(
        "Tricromía", "1",
        "Pantone", "2",
        "S/Muestra", "3"
    )]
    public string? Tintas1 { get; set; } = null;

    [Nombre("Tinta 2")]
    [Grupo("Tintas", ORDEN_GRUPO_TINTAS)]
    [Orden(1)]
    [RadioGroup(
        "Tricromía", "1",
        "Pantone", "2",
        "S/Muestra", "3"
    )]
    public string? Tintas2 { get; set; } = null;

    [Nombre("Tinta 3")]
    [Grupo("Tintas", ORDEN_GRUPO_TINTAS)]
    [Orden(2)]
    [RadioGroup(
        "Tricromía", "1",
        "Pantone", "2",
        "S/Muestra", "3"
    )]
    public string? Tintas3 { get; set; } = null;

    [Nombre("Tinta 4")]
    [Grupo("Tintas", ORDEN_GRUPO_TINTAS)]
    [Orden(3)]
    [RadioGroup(
        "Tricromía", "1",
        "Pantone", "2",
        "S/Muestra", "3"
    )]
    public string? Tintas4 { get; set; } = null;

    // -- Grupo Block

    [Nombre("Block de")]
    [Grupo("Block", ORDEN_GRUPO_BLOCK)]
    [Orden(0)]
    public int Blockde { get; set; } = 0;
    [Nombre("Ancho")]
    [Grupo("Block", ORDEN_GRUPO_BLOCK)]
    [Orden(1)]
    public int BlockAncho { get; set; } = 0;
    [Nombre("Largo")]
    [Grupo("Block", ORDEN_GRUPO_BLOCK)]
    [Orden(2)]
    public int BlockLargo { get; set; } = 0;
    [Nombre("Observaciones Block")]
    [Grupo("Block", ORDEN_GRUPO_BLOCK)]
    [Orden(3)]
    [TextArea]
    public string Observacion1 { get; set; } = string.Empty;

    // -- Grupo Talonarios

    [Nombre("Talonario de")]
    [Grupo("Talonarios", ORDEN_GRUPO_TALONARIOS)]
    [Orden(0)]
    public int Talonariode { get; set; } = 0;
    [Nombre("Ancho")]
    [Grupo("Talonarios", ORDEN_GRUPO_TALONARIOS)]
    [Orden(1)]
    public int TalonarioAncho { get; set; } = 0;
    [Nombre("Largo")]
    [Grupo("Talonarios", ORDEN_GRUPO_TALONARIOS)]
    [Orden(2)]
    public int TalonarioLargo { get; set; } = 0;
    [Nombre("Observaciones Talonarios")]
    [Grupo("Talonarios", ORDEN_GRUPO_TALONARIOS)]
    [Orden(3)]
    [TextArea]
    public string Observacion2 { get; set; } = string.Empty;

    // -- Grupo Paquete

    [Nombre("Paquete de")]
    [Grupo("Paquete", ORDEN_GRUPO_PAQUETE)]
    [Orden(0)]
    public int Paquetede { get; set; } = 0;
    [Nombre("Cantidad Paquete")]
    [Grupo("Paquete", ORDEN_GRUPO_PAQUETE)]
    [Orden(1)]
    public int Paqueteca { get; set; } = 0;
    [Nombre("Observaciones Paquete")]
    [Grupo("Paquete", ORDEN_GRUPO_PAQUETE)]
    [Orden(2)]
    [TextArea]
    public string Observacion3 { get; set; } = string.Empty;

    // -- Grupo Resma

    [Nombre("Resma de")]
    [Grupo("Resma", ORDEN_GRUPO_RESMA)]
    [Orden(0)]
    public int Resmade { get; set; } = 0;
    [Nombre("Cantidad Resma")]
    [Grupo("Resma", ORDEN_GRUPO_RESMA)]
    [Orden(1)]
    public int Resmaca { get; set; } = 0;
    [Nombre("Observaciones Resma")]
    [Grupo("Resma", ORDEN_GRUPO_RESMA)]
    [Orden(2)]
    [TextArea]
    public string Observacion4 { get; set; } = string.Empty;

    // -- Grupo Fajado

    [Nombre("Fajado de")]
    [Grupo("Fajado", ORDEN_GRUPO_FAJADO)]
    [Orden(0)]
    public int Fajadode { get; set; } = 0;
    [Nombre("Cantidad Fajado")]
    [Grupo("Fajado", ORDEN_GRUPO_FAJADO)]
    [Orden(1)]
    public int Fajadoca { get; set; } = 0;
    [Nombre("Observaciones Fajado")]
    [Grupo("Fajado", ORDEN_GRUPO_FAJADO)]
    [Orden(2)]
    [TextArea]
    public string Observacion5 { get; set; } = string.Empty;

    // -- Grupo Operarios

    [Nombre("Operador 1")]
    [Grupo("Operarios", ORDEN_GRUPO_OPERARIOS)]
    [Orden(0)]
    public string Operador1 { get; set; } = string.Empty;
    [Nombre("Operador 2")]
    [Grupo("Operarios", ORDEN_GRUPO_OPERARIOS)]
    [Orden(1)]
    public string Operador2 { get; set; } = string.Empty;

    [Visible(false)]
    public long Empresa { get; set; } = 0;
}