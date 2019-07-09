using System.Threading.Tasks;
using DynamoDBCrud.Core.Contracts;
using DynamoDBCrud.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace DynamoDBCrud.API.Controllers
{
    [Route("api/v1/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteRepository _clienteRepository;
        public ClientesController(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {

            var data = await _clienteRepository.GetAllClientesAsync();
            return Ok(data);
        }

        [HttpGet("{email}/{nome}")]
        public async Task<IActionResult> GetByPK(string email, string nome)
        {
            // a consulta na fila é case sensitive
            var data = await _clienteRepository.GetClienteAsync(email, nome);
            return Ok(data);
        }

        [HttpGet("query/{email}/{sobrenome}")]
        public async Task<IActionResult> GetBySobrenome(string email, string sobrenome)
        {
            // pesquisando pela Partition Key e outro campo
            var data = await _clienteRepository.GetAllClientesBySobreNomeAsync(email, sobrenome);
            return Ok(data);
        }

         [HttpGet("gsi/{sobrenome}")]
        public async Task<IActionResult> GetBySobrenome(string sobrenome)
        {
            // pesquisando pela Partition Key e outro campo
            var data = await _clienteRepository.GetAllClientesBySobreNomeWithOutPKAsync(sobrenome);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody]Cliente cliente)
        {
            await _clienteRepository.SaveClienteAsync(cliente);
            return Ok(cliente);
        }

        [HttpPut("{email}/{nome}")]
        public async Task<IActionResult> Update(string email, string nome, [FromBody]Cliente cliente)
        {
            var data = await _clienteRepository.GetClienteAsync(email, nome);

            if (data == null) return BadRequest("Cliente não localizado");

            data.Sobrenome = cliente.Sobrenome;

            await _clienteRepository.SaveClienteAsync(data);
            return Ok();
        }

        [HttpDelete("{email}/{nome}")]
        public async Task<IActionResult> Delete(string email, string nome)
        {
            await _clienteRepository.DeleteClienteAsync(email, nome);
            return NoContent();
        }
    }
}