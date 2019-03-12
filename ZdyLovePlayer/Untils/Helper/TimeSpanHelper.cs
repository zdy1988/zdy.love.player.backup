using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdyLovePlayer.Untils
{
    public class TimeSpanHelper
    {
        public static string FormatString(TimeSpan aTimeSpan)
        {
            string newFormat = aTimeSpan.ToString("d'd 'h'h 'm'm 's's'");
            // 1d 3h 43m 23s

            return newFormat;
        }

        public static string TimeSpanInWords(TimeSpan aTimeSpan)
        {
            List<string> timeStrings = new List<string>();

            int[] timeParts = new[] { aTimeSpan.Days, aTimeSpan.Hours, aTimeSpan.Minutes, aTimeSpan.Seconds };
            string[] timeUnits = new[] { "天", "时", "分", "秒" };

            for (int i = 0; i < timeParts.Length; i++)
            {
                if (timeParts[i] > 0)
                {
                    timeStrings.Add($"{timeParts[i]} {timeUnits[i]}");
                }
            }

            return timeStrings.Count != 0 ? string.Join(" ", timeStrings.ToArray()) : "0 秒";
        }
    }
}
