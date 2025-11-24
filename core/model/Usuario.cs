using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Usuario : IModel
{
    public long Id { get; set; }
    public string NombreUsuario { get; private set; } = string.Empty;
    public string Contraseña { get; private set; } = string.Empty;
    [Visible(false)]
    public int Empresa { get; set; }

    public Usuario(long id, string nombreUsuario, string contraseña)
    {
        this.Id = Id;
        this.NombreUsuario = nombreUsuario;
        this.Contraseña = contraseña;
    }

    public Usuario() {}
}