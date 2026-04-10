using Application.DTOs;
using Application.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BusinessScenarioAPI.Controllers
{
    [ApiController]
    [Route("api/BusinessCustomer")]
    public class BusinessCustomerController : ControllerBase
    {
        private readonly IMediator mediator;

        public BusinessCustomerController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerCommand command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search = null)
        {
            var result = await mediator.Send(new GetCustomersQuery(search));
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            
            var repo = HttpContext.RequestServices.GetService(typeof(Domain.Interfaces.ICustomerRepository)) as Domain.Interfaces.ICustomerRepository;
            if (repo == null) return StatusCode(500);

            await repo.Delete(id);
            return NoContent();
        }
    }
}