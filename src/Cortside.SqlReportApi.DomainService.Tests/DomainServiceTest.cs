using System;
using System.Security.Claims;
using Cortside.SqlReportApi.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Data.Sqlite;
using Moq;
using System.Data.Common;
using Cortside.SqlReportApi.Domain;
using System.Collections.Generic;

namespace Cortside.SqlReportApi.DomainService.Tests {
    public abstract class DomainServiceTest<T> : IDisposable {

        protected T service;
        protected UnitTestFixture testFixture;
        protected readonly Mock<IHttpContextAccessor> httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        protected DbContextOptions<DatabaseContext> databaseContextOptions;
        private DatabaseContext databaseContextStub;

        public Report report;
        public ReportGroup group;
        public ReportArgument reportArgument;
        public ReportArgumentQuery argumentQuery;

        protected DomainServiceTest() {
            testFixture = new UnitTestFixture();

        }

        protected IDatabaseContext GetDatabaseContext() {
            databaseContextOptions = new DbContextOptionsBuilder<DatabaseContext>()
                    .UseSqlite(CreateInMemoryDatabase())
                    .Options;
            databaseContextStub = new DatabaseContext(databaseContextOptions, httpContextAccessorMock.Object);
            return databaseContextStub;
        }

        private DbConnection CreateInMemoryDatabase() {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();
            return connection;
        }

        public void SetupHttpUser(Claim claim) {
            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            Mock<ClaimsPrincipal> user = new Mock<ClaimsPrincipal>();
            if (claim != null) {
                httpContext.SetupGet(x => x.User).Returns(user.Object);
                this.httpContextAccessorMock.SetupGet(x => x.HttpContext).Returns(httpContext.Object);
                user.Setup(x => x.FindFirst(claim.Type)).Returns(claim);
            }
        }

        public void Dispose() {
            testFixture.TearDown();
        }
    }
}
