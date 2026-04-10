using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext context;

        public CustomerRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Customer> Add(Customer customer)
        {
            context.Customers.Add(customer);
            await context.SaveChangesAsync();
            return customer;
        }

        public async Task<List<Customer>> GetAll(string? search = null)
        {
            var query = context.Customers.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                query = query.Where(c =>
                    c.Name.Contains(s) ||
                    c.Email.Contains(s) ||
                    c.Phone.Contains(s) ||
                    c.Address.Contains(s));
            }
            return await query.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<Customer?> GetById(int id)
        {
            return await context.Customers.FindAsync(id);
        }

        public async Task Delete(int id)
        {
            var entity = await context.Customers.FindAsync(id);
            if (entity != null)
            {
                context.Customers.Remove(entity);
                await context.SaveChangesAsync();
            }
        }
    }
}