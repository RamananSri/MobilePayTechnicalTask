using System;
using System.Text;

namespace LogComponent.Models
{
    public class LogLine
    {
        public LogLine(string text, DateTime timestamp)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Timestamp = timestamp;
        }

        public string Text { get; }
        public DateTime Timestamp { get; }

        public virtual string GetLineText()
        {
            var sb = new StringBuilder();

            if (Text.Length > 0)
            {
                sb.Append(Text);
                sb.Append(". ");
            }

            return sb.ToString();
        }
    }
}