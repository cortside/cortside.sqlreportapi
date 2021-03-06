using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cortside.Common.DomainEvent;
using Cortside.SqlReportApi.Data;
using Cortside.SqlReportApi.Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Cortside.SqlReportApi.DomainService.Tests {

    public class SqlReportApierviceTest : DomainServiceTest<ISqlReportService> {
        private readonly IDatabaseContext databaseContext;
        private readonly Mock<IDomainEventPublisher> domainEventPublisherMock;
        private readonly Mock<ILogger<SqlReportService>> loggerMock;
        private readonly Mock<IHttpContextAccessor> httpAccessorMock;
        private readonly ITestOutputHelper testOutputHelper;
        private readonly ISqlReportService service;

        public SqlReportApierviceTest(ITestOutputHelper testOutputHelper) : base() {
            databaseContext = GetDatabaseContext();
            domainEventPublisherMock = testFixture.Mock<IDomainEventPublisher>();
            loggerMock = testFixture.Mock<ILogger<SqlReportService>>();
            httpAccessorMock = testFixture.Mock<IHttpContextAccessor>();
            this.testOutputHelper = testOutputHelper;

            service = new SqlReportService(databaseContext, loggerMock.Object);
        }

        private void Seed() {
            using (var context = new DatabaseContext(databaseContextOptions, httpAccessorMock.Object)) {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                argumentQuery = new ReportArgumentQuery() { ArgQuery = "query", ReportArgumentQueryId = 1 };
                reportArgument = new ReportArgument { Name = "date", ReportArgumentId = 1, ArgType = "DateTime", ReportId = 1 };
                group = new ReportGroup() { Name = "groupName", ReportGroupId = 1 };
                report = new Report() {
                    Description = "someDescription",
                    Name = "someName",
                    ReportId = reportArgument.ReportId,
                    ReportArguments = new List<ReportArgument>() { reportArgument },
                    ReportGroupId = group.ReportGroupId
                };

                context.AddRange(argumentQuery, reportArgument, group, report);
                context.SaveChanges();
            }
        }


        [Fact]
        public async Task ShouldGetReport() {
            //arrange
            using (var context = new DatabaseContext(databaseContextOptions, httpAccessorMock.Object)) {
                Seed();
                //act
                var result = service.GetReport(report.Name);

                //assert
                result.Should().BeEquivalentTo(report);
            }
        }

        [Fact]
        public async Task ShouldGetReports() {
            //arrange
            using (var context = new DatabaseContext(databaseContextOptions, httpAccessorMock.Object)) {
                Seed();

                //act
                var result = service.GetReports();

                //assert
                result.FirstOrDefault().Should().BeEquivalentTo(report);
            }
        }

        [Fact]
        public async Task ShouldGetReportGroup() {
            //arrange
            using (var context = new DatabaseContext(databaseContextOptions, httpAccessorMock.Object)) {
                Seed();

                //act
                var result = service.GetReportGroup(group.ReportGroupId);

                //assert
                result.Should().BeEquivalentTo(group);
            }
        }

        [Fact]
        public async Task ShouldGetReportGroups() {
            //arrange
            using (var context = new DatabaseContext(databaseContextOptions, httpAccessorMock.Object)) {
                Seed();

                //act
                var result = service.GetReportGroups();

                //assert
                result.FirstOrDefault().Should().BeEquivalentTo(group);
            }
        }

        [Fact]
        public async Task ShouldGetReportArgument() {
            //arrange
            using (var context = new DatabaseContext(databaseContextOptions, httpAccessorMock.Object)) {
                Seed();

                //act
                var result = service.GetReportArgument(reportArgument.ReportArgumentId);

                //assert
                result.Should().BeEquivalentTo(reportArgument);
            }
        }

        [Fact]
        public async Task ShouldGetReportArguments() {
            //arrange
            using (var context = new DatabaseContext(databaseContextOptions, httpAccessorMock.Object)) {
                Seed();

                //act
                var result = service.GetReportArguments();

                //assert
                result.FirstOrDefault().Should().BeEquivalentTo(reportArgument);
            }
        }

        [Fact]
        public async Task ShouldGetReportArgumentQuery() {
            //arrange
            using (var context = new DatabaseContext(databaseContextOptions, httpAccessorMock.Object)) {
                Seed();

                //act
                var result = service.GetReportArgumentQuery(argumentQuery.ReportArgumentQueryId);

                //assert
                result.Should().BeEquivalentTo(argumentQuery);
            }
        }

        [Fact]
        public async Task ShouldGetReportArgumentsQueries() {
            //arrange
            using (var context = new DatabaseContext(databaseContextOptions, httpAccessorMock.Object)) {
                Seed();

                //act
                var result = service.GetReportArgumentQueries();

                //assert
                result.FirstOrDefault().Should().BeEquivalentTo(argumentQuery);
            }
        }

        [Fact(Skip = "Sqlite doesn't support stored procs")]
        public async Task ShouldExecuteReport() {
            //arrange
            using (var context = new DatabaseContext(databaseContextOptions, httpAccessorMock.Object)) {
                Seed();

                var query = new QueryCollection(new Dictionary<string, StringValues>() {
                    { "date", DateTime.UtcNow.Date.ToString() }
                });

                //act
                var result = await service.ExecuteReport(report.Name, query, new List<string>());

                //assert
                result.Should().NotBeNull();
            }
        }
    }
}
