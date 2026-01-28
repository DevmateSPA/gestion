using Gestion.core.model;

namespace Gestion.core.interfaces.service;
public interface INotaCreditoService : IBaseService<NotaCredito>
{
    Task<string> GetSiguienteFolio(long empresaId);
}