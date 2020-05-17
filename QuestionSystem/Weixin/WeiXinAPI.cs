using QuestionSystem.Cache;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace QuestionSystem.Weixin
{
    public class WeiXinAPI
    {
        public static string weiXinAccessToken = "WeiXinAccessToken";
        public static string weiXinJsapiTicket = "weiXinJsapiTicket";
        /// <summary>
        /// 获取微信分享参数
        /// Item1:appId, Item2:timestamp,Item3:nonceStr,Item4:signature
        /// </summary>
        /// <param name="url">当前链接地址:Request.Url.PathAndQuery</param>
        /// <returns></returns>
        public static Tuple<string, long, string, string> GetWeiXinConfig(string url)
        {
            var appId = ConfigurationManager.AppSettings["appId"];
            var AppSecret = ConfigurationManager.AppSettings["AppSecret"];
            var domain = ConfigurationManager.AppSettings["WXHost"];
            var nonceStr = Utils.CreateNonce_str();
            var timestamp = Utils.CreateTimestamp();

            //获取token
            var Token = CacheManager.Contains(weiXinAccessToken) ? CacheManager.GetData<string>(weiXinAccessToken) : string.Empty;
            bool refreshTicket = false;
            if (string.IsNullOrEmpty(Token))
            {
                Token = TokenHelper.GetAccessToken(appId, AppSecret);
                CacheManager.Add(weiXinAccessToken, Token, 6000);
                refreshTicket = true;
            }

            //获取票据
            var jsTicket = CacheManager.Contains(weiXinJsapiTicket) ? CacheManager.GetData<string>(weiXinJsapiTicket) : string.Empty;
            if (string.IsNullOrEmpty(Token) || refreshTicket)
            {
                jsTicket = TokenHelper.GetTickect(Token);
                CacheManager.Add(weiXinJsapiTicket, jsTicket, 6000);
            }
            var string1 = "";
            //获取签名、
            var signature = TokenHelper.GetSignature(jsTicket, nonceStr, timestamp, domain, out string1);
            return new Tuple<string, long, string, string>(appId, timestamp, nonceStr, signature);
        }


    }
}