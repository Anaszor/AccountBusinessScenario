using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BusinessScenarioAPI.Controllers
{
    [ApiController]
    [Route("api/statements")]
    [Authorize]
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

        [HttpGet("my-statements")]
        public async Task<IActionResult> GetMyStatements([FromQuery] string month)
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized(new { Message = "No email claim found in token." });
            }

            var customerRepo = HttpContext.RequestServices.GetService(typeof(Domain.Interfaces.ICustomerRepository)) as Domain.Interfaces.ICustomerRepository;
            if (customerRepo == null) return StatusCode(500, new { Message = "Repository service unavailable." });

            var customers = await customerRepo.GetAll(email);
            var customer = customers.FirstOrDefault(c => c.Email.Equals(email, System.StringComparison.OrdinalIgnoreCase));

            if (customer == null)
            {
                return BadRequest(new { Message = $"Your user account ({email}) is not linked to any business customer record. Please contact an Admin to create a business customer for this email." });
            }

            var result = await mediator.Send(new GetStatementsQuery(customer.Id.ToString(), month));
            return Ok(result);
        }

        [HttpPost("generate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Generate([FromBody] GenerateStatementsCommand command)
        {
            await mediator.Send(command);
            return Ok();
        }
    }
}
