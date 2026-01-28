
using System.Data.Common;

namespace Gestion.core.interfaces.database;

public interface IDbConnectionFactory
{
    Task<DbConnection> CreateConnection();
}