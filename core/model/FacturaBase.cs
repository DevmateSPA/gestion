using System.ComponentModel.DataAnnotations;
using Gestion.core.attributes;
using Gestion.core.attributes.validation;
using Gestion.core.interfaces.model;

public abstract class FacturaBase : IEmpresa
{
    public long Id { get; set; }

    // ===============================
    // DATOS DEL DOCUMENTO
    // ===============================

    [Nombre("Folio")]
    [Required]
    [Grupo("Datos del Documento", 1)]
    [Orden(0)]
    public string Folio { get; set; } = string.Empty;

    [Nombre("Fecha")]
    [Fecha]
    [Grupo("Datos del Documento", 1)]
    [Orden(1)]
    public DateTime Fecha { get; set; } = DateTime.Now;


    // ===============================
    // CLIENTE / PROVEEDOR
    // ===============================

    [Nombre("Rut")]
    [Rut]
    [Grupo("Cliente", 2)]
    [Orden(0)]
    public string RutCliente { get; set; } = string.Empty;

    [Nombre("Nombre")]
    [Required]
    [Grupo("Cliente", 2)]
    [Orden(1)]
    public string NombreCliente { get; set; } = string.Empty;

    [Nombre("Dirección")]
    [Grupo("Cliente", 2)]
    [Orden(2)]
    public string DireccionCliente { get; set; } = string.Empty;


    // ===============================
    // MONTOS
    // ===============================

    [Nombre("Neto")]
    [Grupo("Totales", 3)]
    [Orden(0)]
    public long Neto { get; set; }

    [Nombre("IVA")]
    [Grupo("Totales", 3)]
    [Orden(1)]
    public long Iva { get; set; }

    [Nombre("Total")]
    [Grupo("Totales", 3)]
    [Orden(2)]
    public long Total { get; set; }


    // ===============================
    // OBSERVACIONES
    // ===============================

    [Nombre("Observaciones")]
    [Grupo("Observaciones", 4)]
    [Orden(0)]
    public string Observacion { get; set; } = string.Empty;


    // ===============================
    // CONTROL
    // ===============================

    [Visible(false)]
    public long Empresa { get; set; }
}