using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IStatementRepository
    {
        Task<List<AccountStatement>> Get(string customerId, string month);
        Task<List<AccountStatement>> Generate(string month);
    }
}
