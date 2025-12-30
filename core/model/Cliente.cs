using System.ComponentModel.DataAnnotations;
using Gestion.core.attributes;
using Gestion.core.attributes.validation;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Cliente : IEmpresa
{
    public long Id { get; set; }

    // ===============================
    // DATOS DEL CLIENTE
    // ===============================

    [Rut]
    [Grupo("Datos del Cliente", 1)]
    [Orden(0)]
    public string Rut { get; set; } = string.Empty;

    [Nombre("Razón Social")]
    [Grupo("Datos del Cliente", 1)]
    [Orden(1)]
    public string Razon_Social { get; set; } = string.Empty;

    [Grupo("Datos del Cliente", 1)]
    [Orden(2)]
    public string Giro { get; set; } = string.Empty;

    [Nombre("Dirección")]
    [Grupo("Datos del Cliente", 1)]
    [Orden(3)]
    public string Direccion { get; set; } = string.Empty;

    [Grupo("Datos del Cliente", 1)]
    [Orden(4)]
    public string Ciudad { get; set; } = string.Empty;


    // ===============================
    // CONTACTO
    // ===============================

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

    [Nombre("Observación")]
    [Grupo("Observaciones", 3)]
    [Orden(0)]
    public string Observaciones1 { get; set; } = string.Empty;


    // ===============================
    // SALDOS
    // ===============================

    [Nombre("DebeI")]
    [Grupo("Saldos Iniciales", 4)]
    [Orden(0)]
    public int Debi { get; set; }

    [Nombre("HabI")]
    [Grupo("Saldos Iniciales", 4)]
    [Orden(1)]
    public int Habi { get; set; }

    [Nombre("Debe")]
    [Grupo("Saldos", 4)]
    [Orden(2)]
    public long Debe { get; set; }

    [Nombre("Habe")]
    [Grupo("Saldos", 4)]
    [Orden(3)]
    public long Habe { get; set; }

    [Nombre("Saldo")]
    [Grupo("Saldos", 4)]
    [Orden(4)]
    public long Saldo { get; set; }


    // ===============================
    // CONTROL
    // ===============================

    [Visible(false)]
    public long Empresa { get; set; }
}