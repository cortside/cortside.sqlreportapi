using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Cortside.Common.Security;
using Cortside.SqlReportApi.WebApi.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PolicyServer.Runtime.Client;

namespace Cortside.SqlReportApi.WebApi.Controllers {
    /// <summary>
    /// provides resources from the policy server
    /// </summary>
    [Route("api/v{version:apiVersion}/authorization")]
    [ApiController]
    [ApiVersion("1")]
    [Produces("application/json")]
    [Authorize]
    public class AuthorizationController : ControllerBase {
        private readonly ILogger<AuthorizationController> logger;
        private readonly IPolicyServerRuntimeClient policyClient;
        private readonly IConfiguration configuration;

        /// <summary>
        /// Authorization controller
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="policyClient"></param>
        /// <param name="configuration"></param>
        public AuthorizationController(ILogger<AuthorizationController> logger, IPolicyServerRuntimeClient policyClient, IConfiguration configuration) {
            this.logger = logger;
            this.policyClient = policyClient;
            this.configuration = configuration;
        }

        /// <summary>
        /// Gets the list if permissions associated with the caller, determined by their bearer token
        /// </summary>
        /// <returns>The list of permissions</returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(AuthorizationModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetPermissionsAsync() {
            logger.LogInformation("Retrieving authorization permissions for user.");
            var authProperties = await policyClient.EvaluateAsync(User).ConfigureAwait(false);
            AuthorizationModel responseModel = new AuthorizationModel() {
                Permissions = authProperties.Permissions.ToList()
            };
            var permissionsPrefix = configuration.GetSection("PolicyServer").GetValue<string>("BasePolicyPrefix");
            responseModel.Permissions = responseModel.Permissions.ConvertAll(p => $"{permissionsPrefix}.{p}");
            responseModel.Principal = SubjectPrincipal.From(ControllerContext.HttpContext.User);
            return Ok(responseModel);
        }
    }
}
