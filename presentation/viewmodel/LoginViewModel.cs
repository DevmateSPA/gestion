using System.Windows;
using Gestion.core.interfaces;

public class LoginViewModel
{
    private readonly IUsuarioRepository _usuarioRepository;

    public LoginViewModel(IUsuarioRepository usuarioRepository)
    {
        this._usuarioRepository = usuarioRepository;
    }

    public async Task IniciarSesion(string nombreUsuario, string contraseña)
    {
        var usuario = await this._usuarioRepository.GetByNombre(nombreUsuario);

        if (usuario is null)
        {
            MessageBox.Show("Usuario no encontrado");
            return;
        }

        if (usuario.Contraseña != contraseña)
        {
            MessageBox.Show("Contraseña Incorrecta");
            return;
        }

        MessageBox.Show("BIenvenido, {usuario.NombreUsuario}!");
    }
}