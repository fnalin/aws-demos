
using Microsoft.AspNetCore.Mvc;

namespace DynamoDBCrud.API.Controllers
{


    [Route("api/v1/[controller]")]
    public class TesteController {

        [HttpGet("ping")]
        public string Ping()=>"Pong";

     }

}