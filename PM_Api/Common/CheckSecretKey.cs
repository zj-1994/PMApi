using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;

namespace PM_Api.Common
{
    /// <summary>
    /// 接口验证帮助类
    /// </summary>
    public class CheckSecretKey
    {
        #region 对比密匙
        /// <summary>
        /// 对比密匙
        /// </summary>
        /// <param name="paramData"></param>
        /// <returns></returns>
        public static object ContrastSecretKey(string paramData)
        {
            try
            {
                string timespan = HttpContext.Current.Request.Headers["timespan"];
                string nonce = HttpContext.Current.Request.Headers["nonce"];
                string userId = HttpContext.Current.Request.Headers["userId"];
                string token = HttpContext.Current.Request.Headers["token"];
                string signature = HttpContext.Current.Request.Headers["signature"];

                //判断请求头是否包含以下参数
                if (string.IsNullOrEmpty(timespan))
                {
                    return new { code = 300, msg = "timespan(时间戳)不可为空" };
                }
                if (DateDiff(DateTime.Now, StampToDateTime(timespan)) >= 30)
                {
                    return new { code = 301, msg = "timespan(时间戳)验证失败" };
                }
                if (string.IsNullOrEmpty(nonce))
                {
                    return new { code = 302, msg = "nonce(随机数)不可为空" };
                }
                if (string.IsNullOrEmpty(userId) || userId=="null")
                {
                    return new { code = 303, msg = "userId(用户Id)不可为空" };
                }
                if (string.IsNullOrEmpty(signature))
                {
                    return new { code = 304, msg = "signature(签名参数)不可为空" };
                }
                if (string.IsNullOrEmpty(token))
                {
                    return new { code = 305, msg = "token不可为空" };
                }

                //判断token是否有效
                var SignToken = CommonMethods.GetCache(userId);
                if (SignToken == null)
                {
                    return new { code = 306, msg = "token验证失败,请重新登录" };
                }
                else
                {
                    if (SignToken.ToString() != token)
                    {
                        return new { code = 307, msg = "此账号异地登录,请重新登录" };
                    }
                }

                //根据请求类型拼接参数
                paramData = paramData.Replace("JsonParm", ""); 
                string[] arr = Regex.Matches(paramData, @"[0-9a-zA-Z\u4e00-\u9fa5]", RegexOptions.IgnoreCase | RegexOptions.Multiline).Cast<Match>().Select(m => m.Value.ToUpper()).ToArray();
                Array.Sort(arr);
                var xxx = string.Join("", arr);
                string newKeys = GetMD5(timespan + nonce + userId + token + string.Join("", arr)).ToUpper();
                if (newKeys != signature)
                {
                    return new { code = 308, msg = "signature(签名参数)验证失败" };
                };
                return true;
            }
            catch (Exception ex)
            {
                return new { code = 500, msg = "Token验证失败", error = ex };
            }
        }
        #endregion

        #region MD5加密
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="myString"></param>
        /// <returns></returns>
        public static string GetMD5(string myString)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            //byte[] fromData = System.Text.Encoding.Unicode.GetBytes(myString);
            byte[] fromData = System.Text.Encoding.UTF8.GetBytes(myString);//
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                //这个是很常见的错误，你字节转换成字符串的时候要保证是2位宽度啊，某个字节为0转换成字符串的时候必须是00的，否则就会丢失位数啊。不仅是0，1～9也一样。
                //byte2String += targetData[i].ToString("x");//这个会丢失
                byte2String = byte2String + targetData[i].ToString("x2");
            }
            return byte2String;
        }
        #endregion

        #region 时间戳转为C#格式时间
        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime StampToDateTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
        #endregion

        #region 计算两个日期的时间间隔,返回的是时间间隔的日期差的绝对值.
        /// <summary>
        /// 计算两个日期的时间间隔,返回的是时间间隔的日期差的绝对值.
        /// </summary>
        /// <param name="DateTime1">第一个日期和时间</param>
        /// <param name="DateTime2">第二个日期和时间</param>
        /// <returns></returns>
        public static int DateDiff(DateTime DateTime1, DateTime DateTime2)
        {
            string dateDiff = null;
            try
            {
                TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
                TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
                TimeSpan ts = ts1.Subtract(ts2).Duration();
                dateDiff = ts.Days.ToString() + "天" + ts.Hours.ToString() + "小时" + ts.Minutes.ToString() + "分钟" + ts.Seconds.ToString() + "秒";
                return ts.Seconds;
            }
            catch
            {
                return 0;
            }

        }
        #endregion
    }
}