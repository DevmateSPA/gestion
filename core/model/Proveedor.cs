using Gestion.core.attributes;
using Gestion.core.attributes.validation;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Proveedor : IEmpresa
{
    public long Id { get; set; }

    // ===============================
    // DATOS GENERALES
    // ===============================

    [Nombre("Rut Proveedor")]
    [Rut]
    [Grupo("Datos del Proveedor", 1)]
    [Orden(0)]
    public string Rut { get; set; } = string.Empty;

    [Nombre("Razón Social")]
    [Grupo("Datos del Proveedor", 1)]
    [Orden(1)]
    public string Razon_Social { get; set; } = string.Empty;

    [Nombre("Giro Comercial")]
    [Grupo("Datos del Proveedor", 1)]
    [Orden(2)]
    public string Giro { get; set; } = string.Empty;

    [Nombre("Dirección")]
    [Grupo("Datos del Proveedor", 1)]
    [Orden(3)]
    public string Direccion { get; set; } = string.Empty;

    [Grupo("Datos del Proveedor", 1)]
    [Orden(4)]
    public string Ciudad { get; set; } = string.Empty;

    [Nombre("Teléfono")]
    [Grupo("Contacto", 2)]
    [Orden(0)]
    public string Telefono { get; set; } = string.Empty;

    [Grupo("Contacto", 2)]
    [Orden(1)]
    public string Fax { get; set; } = string.Empty;


    // ===============================
    // OBSERVACIONES
    // ===============================

    [Nombre("Observaciones 1")]
    [Grupo("Observaciones", 3)]
    [Orden(0)]
    public string Observacion1 { get; set; } = string.Empty;

    [Nombre("Observaciones 2")]
    [Grupo("Observaciones", 3)]
    [Orden(2)]
    public string Observacion2 { get; set; } = string.Empty;


    // ===============================
    // SALDOS INICIALES
    // ===============================

    [Nombre("Debe")]
    [Grupo("Saldos Iniciales", 4)]
    [Orden(0)]
    public int Debi { get; set; }

    [Nombre("Haber")]
    [Grupo("Saldos Iniciales", 4)]
    [Orden(1)]
    public int Habi { get; set; }


    // ===============================
    // MOVIMIENTOS / CONTROL
    // ===============================

    [Grupo("Saldos", 5)]
    [Orden(0)]
    public long Debe { get; set; }

    [Nombre("Haber")]
    [Grupo("Saldos", 5)]
    [Orden(1)]
    public long Habe { get; set; }

    [Grupo("Saldos", 5)]
    [Orden(2)]
    public long Saldo { get; set; }

    [Visible(false)]
    public long Empresa { get; set; }
}