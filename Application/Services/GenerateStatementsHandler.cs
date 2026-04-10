using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Application.Interfaces;
using Application.DTOs;
using Domain.Interfaces;
using MediatR;

namespace Application.Services
{
    public class GenerateStatementsHandler : IRequestHandler<GenerateStatementsCommand>
    {
        private readonly IStatementRepository repository;
        private readonly IEmailService emailService;
        private readonly ICustomerRepository customerRepository;

        public GenerateStatementsHandler(
            IStatementRepository repository,
            IEmailService emailService,
            ICustomerRepository customerRepository)
        {
            this.repository = repository;
            this.emailService = emailService;
            this.customerRepository = customerRepository;
        }

        public async Task Handle(GenerateStatementsCommand request, CancellationToken cancellationToken)
        {
            var statements = await repository.Generate(request.Month);

            foreach (var s in statements)
            {
                CustomerDto? customerDto = null;

                if (int.TryParse(s.CustomerId, out var custId))
                {
                    var customer = await customerRepository.GetById(custId);
                    if (customer != null)
                    {
                        customerDto = new CustomerDto
                        {
                            Id = customer.Id,
                            Name = customer.Name ?? string.Empty,
                            Gender = customer.Gender ?? string.Empty,
                            DateOfBirth = customer.DateOfBirth ?? string.Empty,
                            Email = customer.Email ?? string.Empty,
                            Phone = customer.Phone ?? string.Empty,
                            Photo = customer.Photo,
                            Address = customer.Address ?? string.Empty,
                            Balance = customer.Balance
                        };
                    }
                }

                var statementDto = new AccountStatementDto
                {
                    Id = s.Id,
                    CustomerId = s.CustomerId,
                    Month = s.Month,
                    Balance = s.Balance,
                    Email = s.Email
                };

                if (customerDto != null)
                {
                    await emailService.SendAccountStatement(customerDto, statementDto);
                }
                else
                {
                    await emailService.Send(s.Email, "Statement", $"Balance: {s.Balance}");
                }
            }
        }
    }
}
