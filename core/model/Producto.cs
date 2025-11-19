using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Producto : IModel
{
    public long Id { get; set; }
    [Nombre("Código")]
    public string Codigo { get; private set; } = string.Empty;
    public string Descripcion { get; private set; } = string.Empty;
    public int Grupo { get; private set; }
    [Nombre("Stock minimo")]
    public int Stmi { get; private set; }
    public int Papel { get; private set; }
    [Nombre("Dimensiones")]
    public int Dime { get; private set; }
    [Visible(false)]
    public int Enin { get; private set; }
    [Visible(false)]
    public int Punp { get; private set; }
    [Visible(false)]
    public int Sain { get; private set; }
    [Visible(false)]
    public int Saldo { get; private set; }
    [Visible(false)]
    public int Salida { get; private set; }
    [Visible(false)]
    public int Entrada { get; private set; }

    public Producto() {}

}