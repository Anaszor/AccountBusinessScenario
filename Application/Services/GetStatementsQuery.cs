using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
namespace Application.Services
{
    public record GetStatementsQuery(string CustomerId, string Month)
    : IRequest<List<AccountStatementDto>>;
}
