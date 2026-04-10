using Application.DTOs;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services
{
    public class GetCustomersHandler : IRequestHandler<GetCustomersQuery, List<CustomerDto>>
    {
        private readonly ICustomerRepository repository;
        private readonly IMapper mapper;

        public GetCustomersHandler(ICustomerRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<List<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
        {
            var items = await repository.GetAll(request.Search);
            return mapper.Map<List<CustomerDto>>(items);
        }
    }
}