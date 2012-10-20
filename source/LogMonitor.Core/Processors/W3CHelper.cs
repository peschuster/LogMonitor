using System;

namespace LogMonitor.Processors
{
    public static class W3CHelper
    {
        public static DateTime? GetTimestamp(string[] fieldNames, string[] line)
        {
            if (fieldNames == null || line == null)
                return null;

            int dateIndex = Array.IndexOf(fieldNames, W3CFields.Date);
            int timeIndex = Array.IndexOf(fieldNames, W3CFields.Time);

            if (dateIndex < 0 || line.Length <= dateIndex)
                return null;

            DateTime date;

            if (timeIndex < 0 && DateTime.TryParse(line[dateIndex], out date))
            {
                return date;
            }
            else if (DateTime.TryParse(line[dateIndex], out date))
            {
                DateTime date2;

                if (line.Length <= timeIndex && DateTime.TryParse(line[timeIndex], out date2))
                {
                    return date.Date.Add(date2 - date2.Date);
                }

                return date;
            }

            return null;
        }
    }
}