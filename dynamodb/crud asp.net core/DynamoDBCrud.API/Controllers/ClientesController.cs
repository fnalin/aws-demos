using System.Threading.Tasks;
using DynamoDBCrud.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace DynamoDBCrud.API.Controllers {
    [Route("api/v1/[controller]")]
    public class ClientesController : ControllerBase 
    {
        private readonly IClienteRepository _clienteRepository;
        public ClientesController(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(){

            var data = await _clienteRepository.GetAllClientes();            
            return Ok(data);
        }
    }
}