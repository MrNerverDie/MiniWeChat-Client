using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;
using System;

namespace MiniWeChat
{
    public class DateFormatTool : MonoBehaviour
    {
		public static string GetChatFrameTimeStr(long date)
        {
            DateTime dateTime = new DateTime(date);
            
            DateTime yesterday = DateTime.Today.Subtract(new TimeSpan(1, 0, 0, 0));
            
            if (dateTime.Year == DateTime.Today.Year &&
                dateTime.DayOfYear == DateTime.Today.DayOfYear)
            {
                if (dateTime.Hour < 5)
                {
                    return dateTime.ToString("午夜 HH:mm");                    
                }else if(dateTime.Hour < 11)
                {
                    return dateTime.ToString("上午 HH:mm");
                }
                else if (dateTime.Hour < 14)
                {
                    return dateTime.ToString("中午 HH:mm");
                }
                else if (dateTime.Hour < 18)
                {
                    return dateTime.ToString("下午 HH:mm");
                }
                else 
                {
                    return dateTime.ToString("晚上 HH:mm");
                }
            }
            else if (dateTime.Year == yesterday.Year &&
                dateTime.DayOfYear == yesterday.DayOfYear)
            {
                return dateTime.ToString("昨天 HH:mm");
            }
            else if (dateTime.Year == DateTime.Today.Year)
            {
                return dateTime.ToString("MM月dd日 HH:mm");
            }
            else
            {
                return dateTime.ToString("yyyy/MM/dd HH:mm:ss");
            }
        }
    }
}

