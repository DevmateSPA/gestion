using System.ComponentModel.DataAnnotations;
using Gestion.core.attributes.validation;
using Gestion.core.interfaces.model;
using Gestion.core.model.detalles;

namespace Gestion.core.model;

public class OrdenTrabajo : ConDetalles<DetalleOrdenTrabajo>, IEmpresa
{
    public long Id { get; set; }
    [Required]
    public string Folio { get; set; } = string.Empty;
    [Fecha]
    public DateTime Fecha { get; set; } = DateTime.Now;
    [Rut]
    public string RutCliente { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int Cantidad { get; set; } = 0;
    public int TotalImpresion { get; set; } = 0;
    public string FolioDesde { get; set; } = string.Empty;
    public string FolioHasta { get; set; } = string.Empty;
    public int CortarTamanio { get; set; } = 0;
    public string CortarTamanion { get; set; } = string.Empty;
    public string CortarTamanioLargo { get; set; } = string.Empty;
    public int Montar { get; set; } = 0;
    public int MoldeTamanio { get; set; } = 0;
    public string TamanioFinalAncho { get; set; } = string.Empty;
    public string TamanioFinalLargo { get; set; } = string.Empty;
    public string ClienteProporcionanada { get; set; } = string.Empty;
    public string ClienteProporcionaOriginal { get; set; } = string.Empty;
    public bool ClienteProporcionaPelicula { get; set; } = false;
    public bool ClienteProporcionaPlancha { get; set; } = false;
    public bool ClienteProporcionaPapel { get; set; } = false;
    public string TipoImpresion { get; set; } = string.Empty;
    public string Maquina1 { get; set; } = string.Empty;
    public string Maquina2 { get; set; } = string.Empty;
    public string Pin { get; set; } = string.Empty;
    public int Nva { get; set; } = 0;
    public int Us { get; set; } = 0;
    public int CtpNva { get; set; } = 0;
    public int U { get; set; } = 0;
    public string Sobres { get; set; } = string.Empty;
    public string Sacos { get; set; } = string.Empty;
    public string Tintas1 { get; set; } = string.Empty;
    public string Tintas2 { get; set; } = string.Empty;
    public string Tintas3 { get; set; } = string.Empty;
    public string Tintas4 { get; set; } = string.Empty;
    public long Empresa { get; set; } = 0;
}