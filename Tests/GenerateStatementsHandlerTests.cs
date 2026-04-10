using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Services;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using Xunit;

namespace Tests
{
    public class GenerateStatementsHandlerTests
    {
        [Fact]
        public async Task Handle_SendsEmailForEachGeneratedStatement()
        {
            // Arrange
            var repoMock = new Mock<IStatementRepository>();
            var emailMock = new Mock<IEmailService>();
            var customerRepoMock = new Mock<ICustomerRepository>();

            var generated = new List<AccountStatement>
            {
                new AccountStatement { Id = "s1", CustomerId = "c1", Month = "Jan", Balance = 5m, Email = "anas.zoriqi@gmial.com" },
                new AccountStatement { Id = "s2", CustomerId = "c2", Month = "Jan", Balance = 15m, Email = "staranas16@gmail.com" }
            };

            repoMock.Setup(r => r.Generate("Jan")).ReturnsAsync(generated);

            var handler = new GenerateStatementsHandler(repoMock.Object, emailMock.Object, customerRepoMock.Object);
            var command = new GenerateStatementsCommand("Jan");

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert: Verify email.Send called for each statement with expected values
            emailMock.Verify(e => e.Send("anas.zoriqi@gmial.com", "Statement", "Balance: 5"), Times.Once);
            emailMock.Verify(e => e.Send("staranas16@gmail.com", "Statement", "Balance: 15"), Times.Once);
        }

        [Fact]
        public async Task Handle_DoesNotThrow_WhenNoStatementsGenerated()
        {
            // Arrange
            var repoMock = new Mock<IStatementRepository>();
            var emailMock = new Mock<IEmailService>();
            var customerRepoMock = new Mock<ICustomerRepository>();

            repoMock.Setup(r => r.Generate(It.IsAny<string>()))
                    .ReturnsAsync(new List<AccountStatement>());

            var handler = new GenerateStatementsHandler(repoMock.Object, emailMock.Object, customerRepoMock.Object);
            var command = new GenerateStatementsCommand("Feb");

            // Act / Assert
            await handler.Handle(command, CancellationToken.None);

            // Ensure no calls to Send
            emailMock.Verify(e => e.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}