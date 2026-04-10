using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class StatementRepository : IStatementRepository
    {
        private readonly ApplicationDbContext context;

        public StatementRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<List<AccountStatement>> Get(string customerId, string month)
        {
            return await context.AccountStatements
                .Where(x => x.CustomerId == customerId && x.Month == month)
                .ToListAsync();
        }

        public async Task<List<AccountStatement>> Generate(string month)
        {
            var customers = await context.Customers.ToListAsync();

            var existingCustomerIds = await context.AccountStatements
                .Where(s => s.Month == month)
                .Select(s => s.CustomerId)
                .ToListAsync();

            var list = customers
                .Where(c => !existingCustomerIds.Contains(c.Id.ToString()))
                .Select(c => new AccountStatement
                {
                    Id = Guid.NewGuid().ToString(),
                    CustomerId = c.Id.ToString(),
                    Month = month,
                    Balance = c.Balance,
                    Email = c.Email
                })
                .ToList();

            if (list.Count > 0)
            {
                context.AccountStatements.AddRange(list);
                await context.SaveChangesAsync();
            }

            return list;
        }
    }
}
