using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace DotnetPN.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SampleController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
    }
}
