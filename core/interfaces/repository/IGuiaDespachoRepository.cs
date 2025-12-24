using Gestion.core.model;

namespace Gestion.core.interfaces.repository;

public interface IGuiaDespachoRepository : IBaseRepository<GuiaDespacho>
{
    Task<bool> ExisteFolio(string folio, long empresaId);
}