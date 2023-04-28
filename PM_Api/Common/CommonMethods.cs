using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PM_Api.Common
{
    /// <summary>
    /// 缓存帮助类
    /// </summary>
    public class CommonMethods
    {
        #region  获取当前应用程序指定CacheKey的Cache值
        /// <summary>
        /// 获取当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <returns></returns>y
        public static object GetCache(string CacheKey)
        {
            try
            {
                System.Web.Caching.Cache objCache = HttpRuntime.Cache;
                Object value = objCache[CacheKey];
                if (value != null)
                {
                    return value;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }

        }
        #endregion

        #region 设置当前应用程序指定CacheKey的Cache值
        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <param name="objObject"></param>
        public static void SetCache(string CacheKey, object objObject)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(CacheKey, objObject);
        }
        #endregion

        #region  设置绝对过期时间Cache值
        /// <summary>        
        /// 设置绝对的过期时间Cache值
        /// </summary>        
        /// <param name="objectkey"></param>        
        /// <param name="objObject"></param>        
        /// <param name="Seconds">秒</param>        

        public static void SetCacheAbsoluteTime(string objectkey, object objObject, int Seconds)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(objectkey, objObject, null, DateTime.Now.AddSeconds(Seconds), TimeSpan.Zero);
        }
        #endregion

        #region  设置相对过期时间Cache值(即:访问激活后不过期)
        /// <summary>        
        /// 设置相对过期时间Cache值(即:访问激活后不过期)
        /// </summary>        
        /// <param name="objectkey"></param>        
        /// <param name="objObject"></param>        
        /// <param name="timeSpan">超过多少时间不调用就失效，单位是秒</param>        

        public static void SetCacheRelativeTime(string objectkey, object objObject, int timeSpan)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(objectkey, objObject, null, DateTime.MaxValue, TimeSpan.FromSeconds(timeSpan));
        }
        #endregion

        #region  清除单一键缓存
        /// <summary>
        /// 清除单一键缓存
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveKeyCache(string CacheKey)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Remove(CacheKey);
        }
        #endregion

        #region  清除所有缓存
        /// <summary>
        /// 清除所有缓存
        /// </summary>
        public static void RemoveAllCache()
        {
            System.Web.Caching.Cache _cache = HttpRuntime.Cache;
            IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
            if (_cache.Count > 0)
            {
                ArrayList al = new ArrayList();
                while (CacheEnum.MoveNext())
                {
                    al.Add(CacheEnum.Key);
                }
                foreach (string key in al)
                {
                    _cache.Remove(key);
                }
            }
        }
        #endregion

        #region  以列表形式返回已存在的所有缓存 
        /// <summary>
        /// 以列表形式返回已存在的所有缓存 
        /// </summary>
        /// <returns></returns> 
        public static ArrayList ShowAllCache()
        {
            ArrayList al = new ArrayList();
            System.Web.Caching.Cache _cache = HttpRuntime.Cache;
            if (_cache.Count > 0)
            {
                IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
                while (CacheEnum.MoveNext())
                {
                    al.Add(CacheEnum.Key);
                }
            }
            return al;
        }
        #endregion
    }
}