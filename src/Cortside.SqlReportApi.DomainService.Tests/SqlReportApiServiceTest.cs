using Cortside.Common.DomainEvent;
using Cortside.SqlReportApi.Data;
using Moq;
using Xunit.Abstractions;

namespace Cortside.SqlReportApi.DomainService.Tests {
    public class SqlReportApierviceTest : DomainServiceTest<ISqlReportApiService> {

        private readonly IDatabaseContext databaseContext;
        private readonly Mock<IDomainEventPublisher> domainEventPublisherMock;
        private readonly ITestOutputHelper testOutputHelper;

        public SqlReportApierviceTest(ITestOutputHelper testOutputHelper) : base() {
            databaseContext = GetDatabaseContext();
            domainEventPublisherMock = testFixture.Mock<IDomainEventPublisher>();
            this.testOutputHelper = testOutputHelper;
        }

    }
}
