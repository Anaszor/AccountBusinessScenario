using Application.DTOs;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class GetStatementsHandler : IRequestHandler<GetStatementsQuery, List<AccountStatementDto>>
    {
        private readonly IStatementRepository repository;

        public GetStatementsHandler(IStatementRepository repository)
        {
            this.repository = repository;
        }

        public async Task<List<AccountStatementDto>> Handle(GetStatementsQuery request, CancellationToken cancellationToken)
        {
            var data = await repository.Get(request.CustomerId, request.Month);

            return data.Select(x => new AccountStatementDto
            {
                Id = x.Id,
                CustomerId = x.CustomerId,
                Month = x.Month,
                Balance = x.Balance,
                Email = x.Email
            }).ToList();
        }
    }
}
