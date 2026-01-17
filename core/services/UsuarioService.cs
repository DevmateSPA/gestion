using Gestion.core.exceptions;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class UsuarioService : BaseService<Usuario>, IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    public UsuarioService(IUsuarioRepository usuarioRepository)
        :base(usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    protected override Task<List<string>> ValidarReglasNegocio(Usuario entity, long? excludeId = null)
    {
        List<string> erroresEncontrados = [];

        if (string.IsNullOrWhiteSpace(entity.Nombre) ||
            string.IsNullOrWhiteSpace(entity.Clave) ||
            string.IsNullOrWhiteSpace(entity.Tipo))
        {
            erroresEncontrados.Add("Ingrese todos los campos necesarios");
        }

        return Task.FromResult(erroresEncontrados);
    }
}