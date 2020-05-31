namespace LogTest
{
    public interface ILogger
    {
        /// <summary>
        /// Stop the logging. If any outstanding messages these will not be written to the log
        /// </summary>
        void StopWithoutFlush();

        /// <summary>
        /// Stop the logging. The call will not return until all messages have been written to log.
        /// </summary>
        void StopWithFlush();

        /// <summary>
        /// Write a message to the log.
        /// </summary>
        /// <param name="message">The message written to the log</param>
        void WriteToLog(string message);
    }
}
