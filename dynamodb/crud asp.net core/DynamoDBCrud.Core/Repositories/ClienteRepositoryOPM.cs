using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using DynamoDBCrud.Core.Contracts;
using DynamoDBCrud.Core.Models;

namespace DynamoDBCrud.Core.Repositories
{
    // Using Object Persistence Model
    // https://docs.aws.amazon.com/pt_br/amazondynamodb/latest/developerguide/DotNetSDKHighLevel.html
    // É o mais simples, somente permite o mapeamento de items, não podemos fazer criar, excluir e editar tables

    public class ClienteRepositoryOMP : IClienteRepository
    {
        private readonly DynamoDBContext _context;
        public ClienteRepositoryOMP()
        {
            var credentials =
                new BasicAWSCredentials("ACCESS_KEY", "SECRET_KEY");
            var client = new AmazonDynamoDBClient(credentials, RegionEndpoint.USEast1);
            _context = new DynamoDBContext(client);
        }

        public async Task<IEnumerable<Cliente>> GetAllClientesAsync()
        {
            return await _context
                .ScanAsync<Cliente>(new List<ScanCondition>()).GetRemainingAsync();
        }


        public async Task<Cliente> GetClienteAsync(string email, string nome)
        {
            // é necessário informar o nome pois ele faz parte da Primary Key, é o Sort Key
            return await _context
                            .LoadAsync<Cliente>(email, nome);

        }
        public async Task<IEnumerable<Cliente>> GetAllClientesBySobreNomeAsync(string email, string sobrenome)
        {
            var config = new DynamoDBOperationConfig()
            {
                // não pega do cache
                ConsistentRead = true,
                QueryFilter = new List<ScanCondition>{
                    new ScanCondition("sobrenome",ScanOperator.BeginsWith, sobrenome)
                }
            };

            return await _context
                        .QueryAsync<Cliente>(email, config).GetRemainingAsync();
        }



        public async Task<IEnumerable<Cliente>> GetAllClientesBySobreNomeWithOutPKAsync(string sobrenome)
        {
            var config = new DynamoDBOperationConfig()
            {
                // não é possível usar leituras consistentes no GSI
                // ConsistentRead = true,
                // precisa criar um GSI (Global Secondary Index) no DynamoDB
                IndexName = "sobrenome-index"
            };

            return await _context
                        .QueryAsync<Cliente>(sobrenome, config).GetRemainingAsync();
        }


        public async Task SaveClienteAsync(Cliente cliente)
        {
            await _context.SaveAsync(cliente);
        }

        public async Task DeleteClienteAsync(string email, string nome)
        {
            // é necessário informar o nome pois ele faz parte da Primary Key, é o Sort Key
            await _context.DeleteAsync<Cliente>(email, nome);
        }
    }
}