using System.ComponentModel.DataAnnotations;
using Gestion.core.attributes;
using Gestion.core.attributes.validation;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class DocumentoNulo : IEmpresa
{
    public long Id { get; set; }

    // ===============================
    // DATOS DEL DOCUMENTO
    // ===============================

    [Required]
    [Nombre("Tipo")]
    [Grupo("Datos del Documento", 1)]
    [Orden(0)]
    public string Tipo { get; set; } = string.Empty;

    [Required]
    [Nombre("Folio")]
    [Grupo("Datos del Documento", 1)]
    [Orden(1)]
    public string Folio { get; set; } = string.Empty;

    [Nombre("Glosa")]
    [Grupo("Datos del Documento", 1)]
    [Orden(2)]
    public string Glosa { get; set; } = string.Empty;

    [Fecha]
    [Nombre("Fecha")]
    [Grupo("Datos del Documento", 1)]
    [Orden(3)]
    public DateTime Fecha { get; set; }


    // ===============================
    // CONTROL
    // ===============================

    [Visible(false)]
    public long Empresa { get; set; }
}