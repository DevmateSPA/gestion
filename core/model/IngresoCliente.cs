using System.ComponentModel.DataAnnotations;
using Gestion.core.attributes;
using Gestion.core.attributes.validation;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class IngresoCliente : IEmpresa
{
    public long Id { get; set; }

    // ===============================
    // DATOS DEL CLIENTE
    // ===============================

    [Nombre("Rut Cliente")]
    [Rut]
    [Grupo("Datos del Cliente", 1)]
    [Orden(0)]
    public string Rut { get; set; } = string.Empty;

    [Nombre("Razón Social")]
    [Grupo("Datos del Cliente", 1)]
    [Orden(1)]
    public string Razon_Social { get; set; } = string.Empty;


    [Nombre("Saldo")]
    [Grupo("Saldos", 4)]
    [Orden(4)]
    public long Monto { get; set; }


    // ===============================
    // CONTROL
    // ===============================

    [Visible(false)]
    public long Empresa { get; set; }
}