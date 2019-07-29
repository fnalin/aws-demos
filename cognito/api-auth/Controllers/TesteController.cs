using Microsoft.AspNetCore.Mvc;

namespace fansoftapi.Controllers {

    public class TesteController 
    {

        [HttpGet("ping")]
        public string Ping()=>"Pong";

    }

}