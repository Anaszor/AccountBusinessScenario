using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Application.Services
{

    public class GenerateStatementsValidator : AbstractValidator<GenerateStatementsCommand>
    {
        public GenerateStatementsValidator()
        {
            RuleFor(x => x.Month).NotEmpty();
        }
    }
}
