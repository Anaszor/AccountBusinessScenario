using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using Xunit;

namespace Tests
{
    public class GetStatementsHandlerTests
    {
        [Fact]
        public async Task Handle_ReturnsMappedDtos_WhenRepositoryReturnsStatements()
        {
            // Arrange
            var repoMock = new Mock<IStatementRepository>();
            var statements = new List<AccountStatement>
            {
                new AccountStatement { Id = "1", CustomerId = "C1", Month = "January", Balance = 10m, Email = "anas.zoriqi@gmial.com" },
                new AccountStatement { Id = "2", CustomerId = "C1", Month = "January", Balance = 20m, Email = "staranas16@gmail.com" }
            };
            repoMock.Setup(r => r.Get("C1", "January"))
                    .ReturnsAsync(statements);

            var handler = new GetStatementsHandler(repoMock.Object);
            var query = new GetStatementsQuery("C1", "January");

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("1", result.First().Id);
            Assert.Equal(10m, result.First().Balance);
            Assert.Equal("anas.zoriqi@gmial.com", result.First().Email);
        }

        [Fact]
        public async Task Handle_ReturnsEmptyList_WhenRepositoryReturnsEmpty()
        {
            // Arrange
            var repoMock = new Mock<IStatementRepository>();
            repoMock.Setup(r => r.Get(It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(new List<AccountStatement>());

            var handler = new GetStatementsHandler(repoMock.Object);
            var query = new GetStatementsQuery("X", "Y");

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}