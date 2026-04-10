using Application.DTOs;
using AutoMapper;
using Domain.Interfaces;
using Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, CustomerDto>
    {
        private readonly ICustomerRepository repository;
        private readonly IMapper mapper;

        public CreateCustomerHandler(ICustomerRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<CustomerDto> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            var entity = new Customer
            {
                Name = request.Name,
                Gender = request.Gender,
                DateOfBirth = request.DateOfBirth,
                Email = request.Email,
                Phone = request.Phone,
                Photo = request.Photo,
                Address = request.Address,
                Balance = request.Balance
            };

            var added = await repository.Add(entity);
            return mapper.Map<CustomerDto>(added);
        }
    }
}