using System.ComponentModel.DataAnnotations;
using Gestion.core.attributes;
using Gestion.core.attributes.validation;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Venta : IEmpresa
{

    private const int GRUPO_DOCUMENTO = 10;
    private const int GRUPO_FACTURAS = 20;
    private const int GRUPO_DETALLE = 30;

    public long Id { get; set; }

    // ===============================
    // DATOS DEL DOCUMENTO
    // ===============================

    [Required]
    [Nombre("Folio")]
    [Grupo("Datos del Documento", GRUPO_DOCUMENTO)]
    [Orden(1)]
    public string Folio { get; set; } = string.Empty;

    [Nombre("Glosa")]
    [Grupo("Datos del Documento", GRUPO_DOCUMENTO)]
    [Orden(2)]
    public string Glosa { get; set; } = string.Empty;

    [Fecha]
    [Nombre("Fecha")]
    [Grupo("Datos del Documento", GRUPO_DOCUMENTO)]
    [Orden(3)]
    public DateTime Fecha { get; set; }

    [Nombre("Cuenta")]
    [Grupo("Datos del Documento", GRUPO_DOCUMENTO)]
    [Orden(3)]
    public string Cuenta { get; set; } = string.Empty;    
    [Nombre("Rut cliente")]
    [Grupo("Datos del Documento", GRUPO_DOCUMENTO)]
    [Orden(4)]
    public string Rutcliente { get; set; } = string.Empty;    

    [Nombre("Factura 1")]
    [Grupo("Documentos asociados", GRUPO_FACTURAS)]
    [Orden(0)]
    [Searchable("FOLIO_FACTURA")]
    public string Factura01 { get; set; } = string.Empty;    
    [Nombre("Monto 1")]
    [Grupo("Documentos asociados", GRUPO_FACTURAS)]
    [Orden(1)]
    public int Monto01 { get; set; } = 0;    
    [Nombre("Orden trabajo 1")]
    [Grupo("Documentos asociados", GRUPO_FACTURAS)]
    [Orden(2)]
    [Searchable("FOLIO_OT")]
    public string Ot01 { get; set; } = string.Empty;    
    [Nombre("Factura 2")]
    [Grupo("Documentos asociados", GRUPO_FACTURAS)]
    [Orden(3)]
    [Searchable("FOLIO_FACTURA")]
    public string Factura02 { get; set; } = string.Empty;    
    [Nombre("Monto 2")]
    [Grupo("Documentos asociados", GRUPO_FACTURAS)]
    [Orden(4)]
    public int Monto02 { get; set; } = 0;    
    [Nombre("Orden trabajo 2")]
    [Grupo("Documentos asociados", GRUPO_FACTURAS)]
    [Orden(5)]
    [Searchable("FOLIO_OT")]
    public string Ot02 { get; set; } = string.Empty;    
    [Nombre("Factura 3")]
    [Grupo("Documentos asociados", GRUPO_FACTURAS)]
    [Orden(6)]
    [Searchable("FOLIO_FACTURA")]
    public string Factura03 { get; set; } = string.Empty;    
    [Nombre("Monto 3")]
    [Grupo("Documentos asociados", GRUPO_FACTURAS)]
    [Orden(7)]
    public int Monto03 { get; set; } = 0;    
    [Nombre("Orden trabajo 3")]
    [Grupo("Documentos asociados", GRUPO_FACTURAS)]
    [Orden(8)]
    [Searchable("FOLIO_OT")]
    public string Ot03 { get; set; } = string.Empty;    
    [Nombre("Factura 4")]
    [Grupo("Documentos asociados", GRUPO_FACTURAS)]
    [Orden(9)]
    [Searchable("FOLIO_FACTURA")]
    public string Factura04 { get; set; } = string.Empty;
    [Nombre("Monto 4")]
    [Grupo("Documentos asociados", GRUPO_FACTURAS)]
    [Orden(10)]
    public int Monto04 { get; set; } = 0;    
    [Nombre("Orden trabajo 4")]
    [Grupo("Documentos asociados", GRUPO_FACTURAS)]
    [Orden(11)]
    [Searchable("FOLIO_OT")]
    public string Ot04 { get; set; } = string.Empty;    
    [Nombre("Factura 5")]
    [Grupo("Documentos asociados", GRUPO_FACTURAS)]
    [Orden(12)]
    [Searchable("FOLIO_FACTURA")]
    public string Factura05 { get; set; } = string.Empty;
    [Nombre("Monto 5")]
    [Grupo("Documentos asociados", GRUPO_FACTURAS)]
    [Orden(13)]
    public int Monto05 { get; set; } = 0;    
    [Nombre("Orden trabajo 5")]
    [Grupo("Documentos asociados", GRUPO_FACTURAS)]
    [Orden(14)]
    [Searchable("FOLIO_OT")]
    public string Ot05 { get; set; } = string.Empty;    
    [Nombre("Factura 6")]
    [Grupo("Documentos asociados", GRUPO_FACTURAS)]
    [Orden(15)]
    [Searchable("FOLIO_FACTURA")]
    public string Factura06 { get; set; } = string.Empty; 
    [Nombre("Monto 6")]
    [Grupo("Documentos asociados", GRUPO_FACTURAS)]
    [Orden(16)]
    public int Monto06 { get; set; } = 0;    
    [Nombre("Orden trabajo 6")]
    [Grupo("Documentos asociados", GRUPO_FACTURAS)]
    [Orden(17)]
    [Searchable("FOLIO_OT")]
    public string Ot06 { get; set; } = string.Empty;    
    [Nombre("Factura 7")]
    [Grupo("Documentos asociados", GRUPO_FACTURAS)]
    [Orden(18)]
    [Searchable("FOLIO_FACTURA")]
    public string Factura07 { get; set; } = string.Empty;
    [Nombre("Monto 7")]
    [Grupo("Documentos asociados", GRUPO_FACTURAS)]
    [Orden(19)]
    public int Monto07 { get; set; } = 0;    
    [Nombre("Orden trabajo 7")]
    [Grupo("Documentos asociados", GRUPO_FACTURAS)]
    [Orden(20)]
    [Searchable("FOLIO_OT")]
    public string Ot07 { get; set; } = string.Empty;    
    [Nombre("Factura 8")]
    [Grupo("Documentos asociados", GRUPO_FACTURAS)]
    [Orden(21)]
    [Searchable("FOLIO_FACTURA")]
    public string Factura08 { get; set; } = string.Empty;    
    [Nombre("Monto 8")]
    [Grupo("Documentos asociados", GRUPO_FACTURAS)]
    [Orden(22)]
    public int Monto08 { get; set; } = 0;    
    [Nombre("Orden trabajo 8")]
    [Grupo("Documentos asociados", GRUPO_FACTURAS)]
    [Orden(23)]
    [Searchable("FOLIO_OT")]
    public string Ot08 { get; set; } = string.Empty;    
    [Nombre("Factura 9")]
    [Grupo("Documentos asociados", GRUPO_FACTURAS)]
    [Orden(24)]
    [Searchable("FOLIO_FACTURA")]
    public string Factura09 { get; set; } = string.Empty;    
    [Nombre("Monto 9")]
    [Grupo("Documentos asociados", GRUPO_FACTURAS)]
    [Orden(25)]
    public int Monto09 { get; set; } = 0;    
    [Nombre("Orden trabajo 9")]
    [Grupo("Documentos asociados", GRUPO_FACTURAS)]
    [Orden(26)]
    [Searchable("FOLIO_OT")]
    public string Ot09 { get; set; } = string.Empty;    
    [Nombre("Factura 10")]
    [Grupo("Documentos asociados", GRUPO_FACTURAS)]
    [Orden(27)]
    [Searchable("FOLIO_FACTURA")]
    public string Factura10 { get; set; } = string.Empty;    
    [Nombre("Monto 10")]
    [Grupo("Documentos asociados", GRUPO_FACTURAS)]
    [Orden(28)] 
    public int Monto10 { get; set; } = 0;    
    [Nombre("Orden trabajo 10")]
    [Grupo("Documentos asociados", GRUPO_FACTURAS)]
    [Orden(29)]
    [Searchable("FOLIO_OT")]
    public string Ot10{ get; set; } = string.Empty;    
    [Nombre("Total")]
    [Grupo("Datos Bancarios", GRUPO_DETALLE)]
    [Orden(0)]
    [NoSaveDb]
    public int Total{ get; set; } = 0;    
    [Nombre("Forma")]
    [Grupo("Datos Bancarios", GRUPO_DETALLE)]
    [Orden(1)]
    public string Forma{ get; set; } = string.Empty;    
    [Nombre("Banco")]
    [Grupo("Datos Bancarios", GRUPO_DETALLE)]
    [Orden(2)]
    public string Banco{ get; set; } = string.Empty;   
    [Nombre("Cheque")]
    [Grupo("Datos Bancarios", GRUPO_DETALLE)]
    [Orden(3)]
    public string Cheque{ get; set; } = string.Empty; 
    [Nombre("Vencimiento")]
    [Grupo("Datos Bancarios", GRUPO_DETALLE)]
    [Orden(4)]
    [Fecha]
    public DateTime Fechavencimiento{ get; set; }
    [Visible(false)]
    public long Empresa { get; set; }
}