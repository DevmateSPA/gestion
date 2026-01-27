using Gestion.core.exceptions;
using Gestion.core.interfaces.reglas;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.model.DTO;
using Gestion.core.reglas.common;

namespace Gestion.core.services;

public class UsuarioService : BaseService<Usuario>, IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    public UsuarioService(IUsuarioRepository usuarioRepository)
        :base(usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<Usuario?> GetByNombre(string nombreUsuario, long empresaId)
    {
        return await _usuarioRepository.GetByNombre(nombreUsuario, empresaId);
    }

    public async Task<List<TipoUsuarioDTO>> GetTipoList()
    {
        return await _usuarioRepository.GetTipoList();
    }

    protected override IEnumerable<IReglaNegocio<Usuario>> DefinirReglas(
        Usuario entity, 
        long? excludeId = null)
    {
        return
        [
            new RequeridoRegla<Usuario>(
                c => c.Nombre,
                "El nombre del usuario es obligatorio."),

            new RequeridoRegla<Usuario>(
                c => c.Clave,
                "La clave del usuario es obligatoria."),

        new NoValorPorDefectoRegla<Usuario, long>(
            u => u.Tipo,
            valorInvalido: 0,
            mensaje: "Debe seleccionar un tipo de usuario."),

            new UnicoRegla<Usuario>(
                existe: (u, id) =>
                    _usuarioRepository.ExistsByColumns(
                        [
                            ("nombre", u.Nombre),
                            ("empresa", u.Empresa)
                        ],
                        id),

                valor: u => u.Nombre,

                mensaje: "El nombre del usuario: {0}, ya existe para la empresa actual.")
        ];
    }
}