using System.Collections.Generic;
using System.Threading.Tasks;
using DynamoDBCrud.Core.Models;

namespace DynamoDBCrud.Core.Contracts
{

    public interface IClienteRepository
    {
        Task<IEnumerable<Cliente>> GetAllClientes();
    }
}