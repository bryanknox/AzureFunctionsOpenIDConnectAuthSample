using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestFixtures.AzureFunctions
{
    /// <summary>
    /// A test fixture useful for testing ILogger related features where the log is simply
    /// an List<LogEntry>.
    /// </summary>
    public class ListLoggerFixture : ILogger
    {
        public ListLoggerFixture()
        {
            this.LogEntries = new List<LogEntry>();
        }

        public IList<LogEntry> LogEntries;

        // ILogger interface property.
        public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;

        // ILogger interface property.
        public bool IsEnabled(LogLevel logLevel) => false;

        // ILogger interface method.
        public void Log<TState>(LogLevel logLevel,
                                EventId eventId,
                                TState state,
                                Exception exception,
                                Func<TState, Exception, string> formatter)
        {

            string message = formatter(state, exception);
            this.LogEntries.Add(new LogEntry() {
                LogLevel = logLevel,
                Message = message
            });
        }

        /// <summary>
        /// Returns true if this.LogEntries contains any LogEntry with the given text.
        /// </summary>
        /// <param name="text">
        /// The text to match within a LogEntry's Message property.
        /// </param>
        /// <returns>
        /// True if a matching LogEntry was found, otherwise False.
        /// </returns>
        public bool HasLogEntryMessageContaining(string text)
        {
            return this.LogEntries
                .Where(le => le.Message.Contains(text, StringComparison.OrdinalIgnoreCase))
                .Any();
        }

        /// <summary>
        /// Returns true if this.LogEntries have the specified logLevel and contain the given text.
        /// </summary>
        /// <param name="logLevel">
        /// The LogLevel to match.
        /// </param>
        /// <param name="text">
        /// The text to match within a LogEntry's Message property.
        /// </param>
        /// <returns>
        /// True if a matching LogEntry was found, otherwise False.
        /// </returns>
        public bool HasLogEntryMessageContaining(LogLevel logLevel, string text)
        {
            return this.LogEntries
                .Where(le => le.LogLevel == logLevel
                    && le.Message.Contains(text, StringComparison.OrdinalIgnoreCase))
                .Any();
        }


        /// <summary>
        /// Returns true if this.LogEntries contains any LogEntry with the given
        /// LogLevel, and that has a Message containing JSON that DeepEquals the given
        /// JObject.
        /// </summary>
        public bool HasLogEntryMessageThatDeepEqualsJObject(LogLevel logLevel, JObject jObject)
        {
            return this.LogEntries
                .Where(le => {
                    if (le.LogLevel == logLevel)
                    {
                        JObject messageJObject = TryParse(le.Message);
                        if (messageJObject != null)
                        {
                            return JToken.DeepEquals(jObject, messageJObject);
                        }
                    }
                    return false;
                })
                .Any();
        }

        /// <summary>
        /// Returns true if this.LogEntries contains any LogEntry with the given
        /// LogLevel, and that has the given Message.
        /// </summary>
        public bool HasLogEntryMessageThatMatches(LogLevel logLevel, string message)
        {
            return this.LogEntries
                .Where(le => le.LogLevel == logLevel && le.Message == message)
                .Any();
        }

        /// <summary>
        /// Returns true if this.LogEntries contains any LogEntry with the given
        /// LogLevel, and where the Message starts with the given text.
        /// </summary>
        public bool HasLogEntryMessageThatStartsWith(LogLevel logLevel, string text)
        {
            return this.LogEntries
                .Where(le => le.LogLevel == logLevel 
                    && le.Message != null
                    && le.Message.StartsWith(text))
                .Any();
        }

        /// <summary>
        /// Returns true if this.LogEntries contains any LogEntry with the given
        /// LogLevel, and where the Message starts with the given text, followed by
        /// the given followedByText.
        /// </summary>
        public bool HasLogEntryMessageThatStartsWithFollowedBy(
            LogLevel logLevel,
            string text,
            string followedByText)
        {
            return this.LogEntries
                .Where(le => {
                    if (le.LogLevel == logLevel)
                    {
                        if (le.Message != null 
                            && le.Message.StartsWith(text) 
                            && le.Message.Length > text.Length)
                        {
                           return followedByText == le.Message.Substring(text.Length);
                        }
                    }
                    return false;
                })
                .Any();
        }

        /// <summary>
        /// Returns true if this.LogEntries contains any LogEntry with the given
        /// LogLevel, and where the Message starts with the given text, followed by
        /// the JSON text for the given JObject.
        /// </summary>
        public bool HasLogEntryMessageThatStartsWithFollowedByJSon(LogLevel logLevel, string text, JObject jObject)
        {
            return this.LogEntries
                .Where(le => {
                    if (le.LogLevel == logLevel)
                    {
                        if (le.Message != null
                            && le.Message.StartsWith(text)
                            && le.Message.Length > text.Length)
                        {
                            string followedByText = le.Message.Substring(text.Length);

                            JObject followedByTextJObject = TryParse(followedByText);
                            if (followedByTextJObject != null)
                            {
                                return JToken.DeepEquals(jObject, followedByTextJObject);
                            }
                        }
                    }
                    return false;
                })
                .Any();
        }

        private JObject TryParse(string maybeJson)
        {
            try
            {
                return JObject.Parse(maybeJson);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
