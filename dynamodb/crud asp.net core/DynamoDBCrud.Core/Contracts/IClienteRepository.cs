using System.Collections.Generic;
using System.Threading.Tasks;
using DynamoDBCrud.Core.Models;

namespace DynamoDBCrud.Core.Contracts
{

    public interface IClienteRepository
    {
        Task<IEnumerable<Cliente>> GetAllClientesAsync();
        
        // Usando a partition e sort key 
        Task<Cliente> GetClienteAsync(string email, string nome);
        Task<IEnumerable<Cliente>> GetAllClientesBySobreNomeAsync(string email, string sobrenome);
        Task<IEnumerable<Cliente>> GetAllClientesBySobreNomeWithOutPKAsync(string sobrenome);

        Task SaveClienteAsync(Cliente cliente);
        Task DeleteClienteAsync(string email, string nome);
    }
}