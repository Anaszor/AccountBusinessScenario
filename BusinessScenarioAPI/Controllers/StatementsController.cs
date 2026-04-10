using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BusinessScenarioAPI.Controllers
{
    [ApiController]
    [Route("api/statements")]
    public class StatementsController : ControllerBase
    {
        private readonly IMediator mediator;

        public StatementsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<AccountStatementDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<AccountStatementDto>>> Get([FromQuery] string customerId, [FromQuery] string month)
        {
            var result = await mediator.Send(new GetStatementsQuery(customerId, month));
            return Ok(result);
        }

        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] GenerateStatementsCommand command)
        {
            await mediator.Send(command);
            return Ok();
        }
    }
}
