using MetodoSaw.Models;
using Microsoft.AspNetCore.Mvc;

namespace MetodoSaw.Controllers
{
    [ApiController]
    [Route("SAW")]
    public class SAWController: ControllerBase
    {
        [HttpPost]
        [Route("resolver")]
        public dynamic Maximizar(ModeloSAW modelo)
        {
            if (modelo.validarPesos())
            {
                return modelo.resolver();
            }
            else
            {
                return "no";
            }
        }

    }
}
