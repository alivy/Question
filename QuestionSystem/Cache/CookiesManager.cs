using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace QuestionSystem.Cache
{
    /// <summary>
    ///  Cookies管理类
    /// </summary>
    public class CookiesManager
    {
        /// <summary>
        /// 向当前会话状态集合添加一个新项
        /// </summary>
        /// <param name="name">会话名称</param>
        /// <param name="value">会话值</param>
        /// <param name="dtTime">过期时间</param>
        public static void Add(string name, string value, DateTime dtTime)
        {
            //添加Cookie
            HttpCookie m_cookie = new HttpCookie(name);
            m_cookie.Value = value;
            m_cookie.Expires = dtTime;
            m_cookie.HttpOnly = true;
            HttpContext.Current.Response.Cookies.Add(m_cookie);
        }

        /// <summary>
        /// 根据会话名称获取会话值
        /// </summary>
        /// <param name="name">会话名称</param>
        /// <returns>返回会话值</returns>
        public static object Get(string name)
        {
            string obj = null;
            if (HttpContext.Current.Request.Cookies[name] != null)
            {
                obj = HttpContext.Current.Server.HtmlEncode(HttpContext.Current.Request.Cookies[name].Value);
            }
            return obj;
        }


        /// <summary>
        /// 根据会话名称删除会话
        /// </summary>
        /// <param name="name">会话名称</param>
        /// <param name="value">值</param>
        /// <param name="dtTime">过期时间</param>
        public static void Remove(string name, string value, DateTime dtTime)
        {
            HttpCookie m_cookie = HttpContext.Current.Request.Cookies[name];
            m_cookie.Values.Remove(value);
            m_cookie.Expires = dtTime;
            HttpContext.Current.Response.Cookies.Add(m_cookie);
        }
    }

}
