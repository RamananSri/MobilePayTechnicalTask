using System;
using System.Text;

namespace LogTest
{
    public class LogLine
    {
        public LogLine()
        {
            Text = "";
        }

        public string Text { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual string LineText()
        {
            StringBuilder sb = new StringBuilder();

            if (Text.Length > 0)
            {
                sb.Append(Text);
                sb.Append(". ");
            }

            sb.Append(CreateLineText());

            return sb.ToString();
        }

        public string CreateLineText()
        {
            return "";
        }
    }
}