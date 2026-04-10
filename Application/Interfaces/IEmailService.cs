using Application.DTOs;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IEmailService
    {
        Task Send(string to, string subject, string body);
        Task SendAccountStatement(CustomerDto customer, AccountStatementDto statement);
    }
}
