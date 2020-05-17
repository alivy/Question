using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;

namespace QuestionSystem.Weixin
{
    public class TokenHelper
    {
        /// <summary>
        /// 获取AccessToken
        /// </summary>
        /// <param name="grant_type"></param>
        /// <param name="appid"></param>
        /// <param name="secrect"></param>
        /// <returns>access_toke</returns>
        public static string GetAccessToken(string appid, string secrect)
        {
            var url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type={0}&appid={1}&secret={2}", "client_credential", appid, secrect);
            var client = new HttpClient();

            var result = client.GetAsync(url).Result;

            StringBuilder sb3 = new StringBuilder();
            sb3.AppendLine("*****************GetAccessToken*******************");
            sb3.AppendLine("是否成功:" + result.IsSuccessStatusCode);
            sb3.AppendLine("内容:" + result.Content.ReadAsStringAsync().Result);
            sb3.AppendLine("************************************");
            //Logger.WirteMessageLog(sb3.ToString());

            if (!result.IsSuccessStatusCode) return string.Empty;
            var token = DynamicJson.Parse(result.Content.ReadAsStringAsync().Result).access_token;//access_token
            return token;
        }

        /// <summary>
        /// 获取jsapi_ticket
        /// jsapi_ticket是公众号用于调用微信JS接口的临时票据。
        /// 正常情况下，jsapi_ticket的有效期为7200秒，通过access_token来获取。
        /// 由于获取jsapi_ticket的api调用次数非常有限，频繁刷新jsapi_ticket会导致api调用受限，影响自身业务，开发者必须在自己的服务全局缓存jsapi_ticket 。
        /// jsapi_ticket验证地址 http://mp.weixin.qq.com/debug/cgi-bin/sandbox?t=jsapisign
        /// </summary>
        /// <param name="access_token">获取的access_token,也可以通过TokenHelper获取</param>
        public static string GetTickect(string access_token)
        {
            var url = string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi", access_token);
            var jsTicket = string.Empty;// System.Web.HttpContext.Current.Cache.Get("jsTicket") as string;
            //if (string.IsNullOrEmpty(jsTicket))
            //{
            var client = new HttpClient();
            var result = client.GetAsync(url).Result;
            if (!result.IsSuccessStatusCode) return string.Empty;
            jsTicket = result.Content.ReadAsStringAsync().Result;
            var token = DynamicJson.Parse(jsTicket).ticket;
            //System.Web.HttpContext.Current.Cache.Add("jsTicket",jsTicket,)
            //}
            StringBuilder sb3 = new StringBuilder();
            sb3.AppendLine("*****************GetTickect*******************");
            sb3.AppendLine("jsTicket:" + jsTicket);
            sb3.AppendLine("内容:" + result.Content.ReadAsStringAsync().Result);
            sb3.AppendLine("************************************");
            //Logger.WirteMessageLog(sb3.ToString());
            return token;
        }

        /// <summary>
        /// 签名算法
        /// </summary>
        /// <param name="jsapi_ticket">jsapi_ticket</param>
        /// <param name="noncestr">随机字符串(必须与wx.config中的nonceStr相同)</param>
        /// <param name="timestamp">时间戳(必须与wx.config中的timestamp相同)</param>
        /// <param name="url">当前网页的URL，不包含#及其后面部分(必须是调用JS接口页面的完整URL)</param>
        /// <returns></returns>
        public static string GetSignature(string jsapi_ticket, string noncestr, long timestamp, string url, out string string1)
        {
            var string1Builder = new StringBuilder();
            string1Builder.Append("jsapi_ticket=").Append(jsapi_ticket).Append("&")
                          .Append("noncestr=").Append(noncestr).Append("&")
                          .Append("timestamp=").Append(timestamp).Append("&")
                          .Append("url=").Append(url.IndexOf("#") >= 0 ? url.Substring(0, url.IndexOf("#")) : url);
            string1 = string1Builder.ToString();
            return Utils.Sha1(string1);
        }
    }
}