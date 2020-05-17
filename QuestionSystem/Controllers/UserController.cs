using Model.ViewModel.Request;
using Model.ViewModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuestionSystem.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult LoginOn()
        {
            var result = Weixin.WeiXinAPI.GetWeiXinConfig("");
            var jsonresult = new
            {
                appId = result.Item1,
                timestamp = result.Item2,
                nonceStr = result.Item3,
                signature = result.Item4
            };
            ViewBag.appId = result.Item1;
            ViewBag.timestamp = result.Item2;
            ViewBag.nonceStr = result.Item3;
            ViewBag.signature = result.Item4;
            return View();
        }


        public ActionResult WebUserLogin(UserLoginRequest request)
        {
           
            if (request.UserName == "admin" && request.UserPwd == "123456")
            {
                var resultUrl = "Home/index";
                return Json(ResultView.View(0, resultUrl, resultUrl));
            }
            return Json(ResultView.View(1, "账号密码错误"));
        }
    }
}