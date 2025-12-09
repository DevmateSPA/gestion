using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Producto : IEmpresa
{
    public long Id { get; set; }
    [Nombre("Código")]
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int Grupo { get; set; }
    [Nombre("Stock minimo")]
    public int Stmi { get; set; }
    public int Papel { get; set; }
    [Nombre("Dimensiones")]
    public int Dime { get; set; }
    [Visible(false)]
    public int Enin { get; set; }
    [Visible(false)]
    public int Punp { get; set; }
    [Visible(false)]
    public int Sain { get; set; }
    [Visible(false)]
    public int Saldo { get; set; }
    [Visible(false)]
    public int Salida { get; set; }
    [Visible(false)]
    public int Entrada { get; set; }
    [Visible(false)]
    public long Empresa { get; set; }
}