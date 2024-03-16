namespace EventCopilotBot.Services
{
    //using EventCopilotBot.Models;
    using API.Models;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;
    using Newtonsoft.Json;

    /// <summary>
    /// Provides methods to track events in Azure Application Insights.
    /// </summary>
    public class AppInsights : IDisposable
    {
        private TelemetryClient telemetryClient;
        TelemetryConfiguration telemetryConfiguration = TelemetryConfiguration.CreateDefault();
        private IConfiguration _configuration;
        private readonly ISecretManager _secretManager;

        public AppInsights(IConfiguration configuration, ISecretManager secretManager)
        {
            _configuration = configuration;
            _secretManager = secretManager;
        }

        /// <summary>
        /// Tracks an event in Application Insights.
        /// </summary>
        /// <param name="logtype">The type of event to track.</param>
        /// <param name="userID">The ID of the user associated with the event.</param>
        /// <param name="attendeeType">The type of attendee associated with the event.</param>
        /// <param name="timezone">The timezone of the user associated with the event.</param>
        /// <param name="sessionID">The ID of the session associated with the event.</param>
        /// <param name="conversationID">The ID of the conversation associated with the event.</param>
        /// <param name="messageID">The ID of the message associated with the event.</param>
        /// <param name="action">The action associated with the event.</param>
        /// <param name="feedback">The feedback associated with the event.</param>
        /// <param name="question">The question associated with the event.</param>
        /// <param name="chatHistory">The chat history associated with the event.</param>
        /// <param name="answer">The answer associated with the event.</param>
        /// <param name="intent">The intent associated with the event.</param>
        /// <param name="token">The token associated with the event.</param>
        /// <param name="reponseTime">The response time associated with the event.</param>
        /// <param name="header">The header associated with the event.</param>
        /// <param name="footer">The footer associated with the event.</param>
        /// <param name="metadata">The metadata associated with the event.</param>
        /// <param name="explanation">The explanation associated with the event.</param>
        public void TrackEvent(
            string logtype, string userID, string attendeeType, string timezone, string sessionID,
            string conversationID, string messageID, string action, string feedback, string question,
            List<ChatHistoryItem> chatHistory, string answer, string intent, string token,
            string responseTime, string header, string footer, string metadata, string explanation, string responseMode)
        {
            try
            {
                // Setup application insights telemetry client
                telemetryConfiguration.ConnectionString = _secretManager.GetSecretAsync("ApplicationInsightsKey").Result;
                telemetryClient = new TelemetryClient(telemetryConfiguration);

                var customEvent = new EventTelemetry(logtype);
                customEvent.Properties["UserID"] = userID;
                customEvent.Properties["AttendeeType"] = attendeeType;
                customEvent.Properties["Timezone"] = timezone;
                customEvent.Properties["SessionID"] = sessionID;
                customEvent.Properties["ConversationID"] = conversationID;
                customEvent.Properties["MessageID"] = messageID;
                customEvent.Properties["Question"] = question;
                customEvent.Properties["Answer"] = answer;
                customEvent.Properties["Token"] = token;
                customEvent.Properties["Intent"] = intent;
                customEvent.Properties["Header"] = header;
                customEvent.Properties["Footer"] = footer;
                customEvent.Properties["Metadata"] = metadata;
                customEvent.Properties["Explanation"] = explanation;
                customEvent.Properties["ResponseTime"] = responseTime;
                customEvent.Properties["ResponseMode"] = responseMode;
                customEvent.Properties["AppRegion"] = _configuration.GetValue<string>("AppRegion", defaultValue: "");


                if (logtype == "Response" || logtype == "Exception")
                {
                    string chatHistoryString = JsonConvert.SerializeObject(chatHistory);
                    customEvent.Properties["ChatHistory"] = chatHistoryString;
                }

                if (logtype == "Feedback")
                {
                    customEvent.Properties["FeedBackStatus"] = action;
                    customEvent.Properties["FeedBack"] = feedback;
                }

                telemetryClient.TrackEvent(customEvent);
                telemetryClient.Flush();
                customEvent = null;
            }
            catch (Exception ex)
            {
                return; // Propagate the exception to the caller for proper handling.
            }
        }

        public void ExceptionEvent(Exception ex, UserChat chat, string userID)
        {
            try
            {
                // Setup application insights telemetry client
                telemetryConfiguration.ConnectionString = _secretManager.GetSecretAsync("ApplicationInsightsKey").Result;
                telemetryClient = new TelemetryClient(telemetryConfiguration);

                var exceptionTelemetry = new ExceptionTelemetry(ex)
                {
                    SeverityLevel = SeverityLevel.Error,
                    HandledAt = ExceptionHandledAt.UserCode
                };

                exceptionTelemetry.Properties["SessionID"] = chat.sessionID;
                exceptionTelemetry.Properties["ConversationID"] = chat.conversationID;
                exceptionTelemetry.Properties["MessageID"] = chat.messageID;
                exceptionTelemetry.Properties["UserID"] = userID;
                exceptionTelemetry.Properties["AppRegion"] = _configuration.GetValue<string>("AppRegion");

                if (chat.logType == "FrontendException")
                {
                    exceptionTelemetry.Properties["LogType"] = chat.logType;
                    exceptionTelemetry.Properties["Message"] = chat.message;
                    exceptionTelemetry.Properties["StackTrace"] = chat.exception;
                }
                else
                {
                    exceptionTelemetry.Properties["Question"] = chat.question;
                    exceptionTelemetry.Properties["Timezone"] = chat.timezone;
                }

                telemetryClient.TrackException(exceptionTelemetry);
                telemetryClient.Flush();
                exceptionTelemetry = null;
            }
            catch (Exception e)
            {
                return; // Propagate the exception to the caller for proper handling.
            }
        }

        public void ExceptionEvent(Exception ex)
        {
            try
            {
                // Setup application insights telemetry client
                telemetryConfiguration.ConnectionString = _secretManager.GetSecretAsync("ApplicationInsightsKey").Result;
                telemetryClient = new TelemetryClient(telemetryConfiguration);

                var exceptionTelemetry = new ExceptionTelemetry(ex)
                {
                    SeverityLevel = SeverityLevel.Error,
                    HandledAt = ExceptionHandledAt.UserCode
                };

                telemetryClient.TrackException(exceptionTelemetry);
                telemetryClient.Flush();
                exceptionTelemetry = null;
            }
            catch
            {
                return; // Propagate the exception to the caller for proper handling.
            }
        }

        public void Dispose()
        {
            try
            {
                telemetryConfiguration.Dispose();
            }
            catch (Exception e)
            {
                return; // Propagate the exception to the caller for proper handling.
            }
        }
    }

}