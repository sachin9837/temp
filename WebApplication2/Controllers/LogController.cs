namespace API.Controllers
{
    //  using EventCopilotBot.Models;
    using EventCopilotBot.Services;
    using API.Models;
    using Microsoft.AspNetCore.Mvc;
    /// <summary>
    /// Controller for logging custom events and exceptions to Azure Application Insights.
    /// </summary>

    //[Authorize]

    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ISecretManager _secretManager;
        private readonly IConfiguration _configuration;
        /// <summary>
        /// Controller for logging events.
        /// </summary>
        public LogController(IConfiguration configuration, ISecretManager secretManager)
        {
            _configuration = configuration;
            _secretManager = secretManager;
        }


        /// <summary>
        /// Logs a custom event to Application Insights.
        /// </summary>
        /// <returns>An ActionResult containing a string indicating the success of the operation.</returns>
        [HttpPost]
        [Route("api/logCustom")]
        public ActionResult<IEnumerable<string>> logCustom()
        {
            using (var appInsights = new AppInsights(_configuration, _secretManager))
            {
                //var userID = User.FindFirst("pdcId")?.Value;
                //var attendeeType = User.FindFirst("attendeeType")?.Value;
                //var sessionID = Request.Form["sessionID"];
                //var conversationID = Request.Form["conversationID"];
                //var messageID = Request.Form["messageID"];
                //var logtype = Request.Form["logType"];
                //var action = Request.Form["action"];
                //var feedback = Request.Form["feedback"];
                //var timezone = Request.Form["timezone"];
                var userID = "123456789";
                var attendeeType = "VIP";
                var sessionID = "session123";
                var conversationID = "conversation456";
                var messageID = "message789";
                var logtype = "custom";
                var action = "click";
                var feedback = "positive";
                var timezone = "UTC";


                appInsights.TrackEvent(logtype, userID, attendeeType, timezone, sessionID, conversationID, messageID, action, feedback, "-", new List<ChatHistoryItem>(), "", "", "", "", "", "", "", "", "");

                return new string[] { "Response Logged Successfully" };
            }
        }

        /// <summary>
        /// Logs an exception to Azure Application Insights.
        /// </summary>
        /// <param name="exep">The UserChat object containing the exception message and stack trace.</param>
        /// <returns>An ActionResult containing a string indicating whether the response was logged successfully.</returns>
        [HttpPost]
        [Route("api/logException")]
        public ActionResult<IEnumerable<string>> logException(UserChat exep)
        {
            using (var appInsights = new AppInsights(_configuration, _secretManager))
            {
                var userID = User.FindFirst("pdcId")?.Value;
                var customException = new Exception(exep.message);
                customException.Data["StackTrace"] = exep.exception;
                appInsights.ExceptionEvent(customException, exep, userID);
                return new string[] { "Response Logged Successfully" };
            }
        }
    }
}
