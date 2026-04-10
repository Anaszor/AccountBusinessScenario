using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer> Add(Customer customer);
        Task<List<Customer>> GetAll(string? search = null);
        Task<Customer?> GetById(int id);
        Task Delete(int id);
    }
}