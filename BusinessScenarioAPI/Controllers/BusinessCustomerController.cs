using Application.DTOs;
using Application.Services;
using AutoMapper;
using Domain.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessScenarioAPI.Controllers
{
    [ApiController]
    [Route("api/BusinessCustomer")]
    [Authorize]
    public class BusinessCustomerController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly ICustomerRepository repository;
        private readonly IMapper mapper;

        public BusinessCustomerController(IMediator mediator, ICustomerRepository repository, IMapper mapper)
        {
            this.mediator = mediator;
            this.repository = repository;
            this.mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await repository.GetById(id);
            if (customer == null) return NotFound();
            return Ok(mapper.Map<CustomerDto>(customer));
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] CustomerDto dto)
        {
            var customer = await repository.GetById(id);
            if (customer == null) return NotFound();

            customer.Name = dto.Name;
            customer.Gender = dto.Gender;
            customer.DateOfBirth = dto.DateOfBirth;
            customer.Email = dto.Email;
            customer.Phone = dto.Phone;
            customer.Photo = dto.Photo;
            customer.Address = dto.Address;
            customer.Balance = dto.Balance;
            customer.CustmerType = dto.CustmerType;

            await repository.Update(customer);
            return Ok(mapper.Map<CustomerDto>(customer));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await repository.GetById(id);
            if (customer == null) return NotFound();

            await repository.Delete(id);
            return NoContent();
        }
    }
}