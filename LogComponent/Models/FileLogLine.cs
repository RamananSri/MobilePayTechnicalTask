using System;
using System.Text;

namespace LogComponent.Models
{
    public class FileLogLine : LogLine
    {
        public FileLogLine(string text, DateTime timestamp) : base(text, timestamp)
        { }

        public static string GetFormattedFileHeader()
        {
            var stringbuilder = new StringBuilder();
            stringbuilder.Append("Timestamp".PadRight(25, ' '));
            stringbuilder.Append("\t");
            stringbuilder.Append("Data".PadRight(15, ' '));
            stringbuilder.Append("\t");
            stringbuilder.Append("\t");

            return stringbuilder.ToString();
        }

        public override string GetLineText()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(Timestamp.ToString("yyyy-MM-dd HH:mm:ss:fff"));
            stringBuilder.Append("\t");
            stringBuilder.Append(base.GetLineText());
            stringBuilder.Append("\t");

            return stringBuilder.ToString();
        }
    }
}
