using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class BancoRepository : BaseRepository<Banco>, IBancoRepository
{
    public BancoRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "banco", "vw_banco") {}
        
    /// <summary>
    /// Verifica si ya existe un banco con el mismo código dentro de una empresa específica.
    /// </summary>
    /// <param name="codigo">
    /// Código del banco que se desea validar.
    /// </param>
    /// <param name="empresaId">
    /// Identificador de la empresa a la que pertenece el banco.
    /// </param>
    /// <returns>
    /// <c>true</c> si existe otro banco con el mismo <paramref name="codigo"/>
    /// asociado a la empresa indicada; de lo contrario, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// Esta validación aplica una regla de negocio de unicidad compuesta:
    /// <para>
    /// (<c>codigo</c>, <c>empresa</c>) debe ser único para una empresa en cuestión.
    /// </para>
    /// 
    /// Permite que distintas empresas tengan bancos con el mismo código,
    /// pero evita duplicados dentro de una misma empresa.
    /// 
    /// Este método se utiliza principalmente durante la creación o
    /// actualización de entidades <see cref="Banco"/>.
    /// </remarks>
    public async Task<bool> ExisteCodigo(
        string codigo,
        long empresaId,
        long? excludeId = null) => await ExistsByColumns(
            new Dictionary<string, object>
            {
                ["codigo"] = codigo,
                ["empresa"] = empresaId
            },
            excludeId);
}