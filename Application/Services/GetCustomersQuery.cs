using Application.DTOs;
using MediatR;
using System.Collections.Generic;

namespace Application.Services
{
    public class GetCustomersQuery : IRequest<List<CustomerDto>>
    {
        public string? Search { get; }

        public GetCustomersQuery(string? search)
        {
            Search = search;
        }
    }
}