using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class AccountStatementDto
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string Month { get; set; }
        public decimal Balance { get; set; }
        public string Email { get; set; }
    }
}
