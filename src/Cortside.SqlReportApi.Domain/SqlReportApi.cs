using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cortside.SqlReportApi.Domain {

    [Table("SqlReportApi")]
    public class SqlReportApi : AuditableEntity {

        public SqlReportApi() {

        }

        /// <summary>
        /// Unique identifier for a SqlReportApi
        /// </summary>
        [Key]
        public Guid SqlReportApiId { get; set; }


    }
}
