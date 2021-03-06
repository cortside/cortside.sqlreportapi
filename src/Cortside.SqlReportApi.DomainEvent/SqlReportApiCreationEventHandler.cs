using System;
using System.Threading.Tasks;
using Cortside.Common.DomainEvent;
using Cortside.DomainEvent.Events;
using Cortside.SqlReportApi.DomainService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Cortside.SqlReportApi.DomainEvent {

    /// <summary>
    /// Handles domain event <see cref="SqlReportApiCreationEvent"/>
    /// </summary>
    public class SqlReportApiCreationEventHandler : IDomainEventHandler<SqlReportApiCreationEvent> {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<SqlReportApiCreationEventHandler> logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public SqlReportApiCreationEventHandler(IServiceProvider serviceProvider, ILogger<SqlReportApiCreationEventHandler> logger) {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        /// <summary>
        /// Handle message event
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public async Task Handle(DomainEventMessage<SqlReportApiCreationEvent> @event) {
            using (LogContext.PushProperty("MessageId", @event.MessageId))
            using (LogContext.PushProperty("CorrelationId", @event.CorrelationId)) {
                await Handle(@event.Data);
            }
        }

        /// <summary>
        /// Handle message
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public async Task Handle(SqlReportApiCreationEvent @event) {
            using (LogContext.PushProperty("Parameter", @event.Parameter)) {
            }
        }
    }
}
