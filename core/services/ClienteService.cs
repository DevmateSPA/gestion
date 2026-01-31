using Gestion.core.interfaces.reglas;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.reglas.common;

namespace Gestion.core.services;

public class ClienteService : BaseService<Cliente>, IClienteService
{
    private readonly IClienteRepository _clienteRepository;
    public ClienteService(IClienteRepository clienteRepository)
        :base(clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<Cliente?> FindByRut(string rut, long empresaId)
    {
        return await _clienteRepository.FindByRut(rut, empresaId);
    }

    public async Task<List<string>> GetRutList(string busquedaRut, long empresaId)
    {
        return await _clienteRepository.GetRutList(busquedaRut, empresaId);
    }

    protected override IEnumerable<IReglaNegocio<Cliente>> DefinirReglas(
        Cliente entity,
        long? excludeId = null)
    {
        return
        [
            new RequeridoRegla<Cliente>(
                c => c.Rut,
                "El rut del cliente es obligatorio."),

            new UnicoRegla<Cliente>(
                existe: (cliente, id) =>
                    _clienteRepository.ExistsByColumns(
                        [
                            ("rut", cliente.Rut),
                            ("empresa", cliente.Empresa)
                        ],
                        id),

                valor: c => c.Rut,

                mensaje: "El rut del cliente: {0}, ya existe para la empresa actual.")
        ];
    }
}