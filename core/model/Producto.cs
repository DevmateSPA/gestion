using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Producto : IModel
{
    public int Id { get; set; }
    public int Codigo { get; private set; }
    public string Descripcion { get; private set; } = string.Empty;
    public int Dime { get; private set; }
    public int Enin { get; private set; }
    public int Entrada { get; private set; }
    public int Grupo { get; private set; }
    public int Papel { get; private set; }
    public int Punp { get; private set; }
    public int Sain { get; private set; }
    public int Saldo { get; private set; }
    public int Salida { get; private set; }
    public int Stmi { get; private set; }

    public Producto() {}

}