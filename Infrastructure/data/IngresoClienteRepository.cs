using System.Data.Common;
using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class IngresoClienteRepository : BaseRepository<IngresoCliente>, IIngresoClienteRepository
{
    public IngresoClienteRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "vw_ingreso_cliente", "vw_ingreso_cliente") {}

    public async Task<List<IngresoCliente>> FindAllByEmpresaAndFecha(long empresaId, DateTime desde, DateTime hasta)
    {
        if (_viewName == null)
            throw new InvalidOperationException("La vista no está asignada para este repositorio.");

        DbParameter[] parameters =
        [
            new MySqlParameter("@empresa", empresaId),
        ];

        return await FindWhereFrom(
            tableOrView: _viewName,
            where: "empresa = @empresa",
            orderBy: "fecha DESC",
            limit: null,
            offset: null,
            parameters);
    }


    public async Task<List<IngresoCliente>> FindPageByEmpresaAndFecha(long empresaId, DateTime desde, DateTime hasta, int pageNumber, int pageSize)
    {
        DbParameter[] parameters =
        [
            new MySqlParameter("@empresa", empresaId),
        ];

        return await FindPageWhere(
            where: "empresa = @empresa AND fecha",
            orderBy: "fecha DESC",
            pageNumber: pageNumber,
            pageSize: pageSize,
            parameters);
    }
}