using System;
using System.Collections.Generic;
using System.Text;

namespace App.Extensions.DateTimeExtensions
{
    public static partial class DateTimeExtension
    {
        /// <summary>
        /// 时间转换为 毫秒时间戳
        /// </summary>
        /// <param name="dateTime">时间</param>
        /// <returns></returns>
        public static double TransitionTimeStamp(this DateTime dateTime)
        {
            // 这里增加8小时是因为 UTC(协调世界时) 的时间差
            TimeSpan timeSub = dateTime - DateTime.UnixEpoch.AddHours(8);
            // 秒转换为毫秒
            return timeSub.TotalSeconds * 1000;
        }

        /// <summary>
        /// 毫秒时间戳 转换为时间
        /// </summary>
        /// <param name="timestamp">毫秒时间戳</param>
        /// <returns></returns>
        public static DateTime TransitionDateTime(this double timestamp)
        {
            TimeSpan timeSub = TimeSpan.FromMilliseconds(timestamp);
            // DateTime.UnixEpoch对应的时间的时间戳为0
            return DateTime.UnixEpoch.Add(timeSub).AddHours(8);
        }
    }
}
