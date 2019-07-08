using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using DynamoDBCrud.Core.Contracts;
using DynamoDBCrud.Core.Models;

namespace DynamoDBCrud.Core.Repositories
{

    public class ClienteRepositoryOMP: IClienteRepository
    {
        private readonly DynamoDBContext _context;
        public ClienteRepositoryOMP()
        {
            var credentials = 
                new BasicAWSCredentials("ACCESS_KEY", "SECRET_KEY");
            var client = new AmazonDynamoDBClient(credentials, RegionEndpoint.USEast1);
            _context = new DynamoDBContext(client);
        }

        public async Task<IEnumerable<Cliente>> GetAllClientes()
        {
            return await _context
                .ScanAsync<Cliente>(new List<ScanCondition>()).GetRemainingAsync();
        }
    }
}