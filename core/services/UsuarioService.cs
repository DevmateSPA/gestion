using Gestion.core.exceptions;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.model.DTO;

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

    protected override Task<List<string>> ValidarReglasNegocio(Usuario entity, long? excludeId = null)
    {
        List<string> erroresEncontrados = [];

        if (string.IsNullOrWhiteSpace(entity.Nombre) ||
            string.IsNullOrWhiteSpace(entity.Clave))
        {
            erroresEncontrados.Add("Ingrese todos los campos necesarios");
        }

        return Task.FromResult(erroresEncontrados);
    }
}