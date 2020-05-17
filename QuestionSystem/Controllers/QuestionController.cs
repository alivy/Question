using BLL;
using Model.DBModel;
using Model.ViewModel.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Model.ViewModel.Response;
using QuestionSystem.Cache;
using Newtonsoft.Json;
using RuanYun.Excel.Builder;
using System.Text;
using System.IO;
using Aspose.Cells;
using RuanYun.Excel;
//using QuestionSystem.Common;
using System.Web.Mvc;
using System.Configuration;

namespace QuestionSystem.Controllers
{
    public class QuestionController : Controller
    {
        //场景
        public static string Science = ConfigurationManager.AppSettings["science"];

        #region 订单管理


        /// <summary>
        /// 订单管理
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //var result = new BaseBLL<Order>().LoadEntities();
            return View();
        }




        /// <summary>
        /// 获取提交订单
        /// </summary>
        /// <returns></returns>
        public ActionResult GetOrder(string userName, DateTime? submitTime, DateTime? getTime, int page = 1, int rows = 5)
        {
            var orders = new BaseBLL<Order>().LoadEntities();
            int total = orders.Count();
            if (!string.IsNullOrWhiteSpace(userName))
            {
                orders = orders.Where(x => x.UserName.Contains(userName)).ToList();
            }
            if (submitTime != null)
            {
                var submitdate = submitTime.Value.ToString("yyyy-MM-dd");
                orders = orders.Where(x => x.SubmitTime.Value.ToString("yyyy-MM-dd") == submitdate).ToList();
            }
            if (getTime != null)
            {
                var getdate = getTime.Value.ToString("yyyy-MM-dd");
                orders = orders.Where(x => x.GetTime == getdate).ToList();
            }
            var prizes = new BaseBLL<Prize>().LoadEntities();
            if (page == 0 && rows == 0)
            {
                orders = orders.OrderBy(m => m.Id).ToList();
            }
            else
            {
                orders = orders.OrderBy(m => m.Id).Skip((page - 1) * rows).Take(rows).ToList();
            }
            var result = new List<OrderView>();
            foreach (var item in orders)
            {
                var orderView = new OrderView();
                orderView.Id = item.Id;
                orderView.OrderId = item.OrderId;
                orderView.Phone = item.Phone;
                orderView.PrizeCount = item.PrizeCount;
                orderView.PrizeName = prizes.FirstOrDefault(x => x.PrizeId == item.PrizeId)?.PrizeName ?? string.Empty;
                orderView.UserName = item.UserName;
                orderView.GetTime = item.GetTime;
                orderView.SubmitTime = item.SubmitTime.Value.ToString("yyyy-MM-dd");
                result.Add(orderView);
            }
            return Json(new { total = total, rows = result });
        }

        /// <summary>
        /// 获取提交订单
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet]
        public ActionResult CreateOrderExcel(string userName, DateTime? submitTime, DateTime? getTime)
        {
            var orders = new BaseBLL<Order>().LoadEntities();
            if (!string.IsNullOrWhiteSpace(userName))
            {
                orders = orders.Where(x => x.UserName.Contains(userName)).ToList();
            }
            if (submitTime != null)
            {
                var submitdate = submitTime.Value.ToString("yyyy-MM-dd");
                orders = orders.Where(x => x.SubmitTime.Value.ToString("yyyy-MM-dd") == submitdate).ToList();
            }
            if (getTime != null)
            {
                var getdate = getTime.Value.ToString("yyyy-MM-dd");
                orders = orders.Where(x => x.GetTime == getdate).ToList();
            }
            var prizes = new BaseBLL<Prize>().LoadEntities();
            var result = new List<OrderView>();
            foreach (var item in orders)
            {
                var orderView = new OrderView();
                orderView.Id = item.Id;
                orderView.OrderId = item.OrderId;
                orderView.Phone = item.Phone;
                orderView.PrizeCount = item.PrizeCount;
                orderView.PrizeName = prizes.FirstOrDefault(x => x.PrizeId == item.PrizeId)?.PrizeName ?? string.Empty;
                orderView.UserName = item.UserName;
                orderView.GetTime = item.GetTime;
                orderView.SubmitTime = item.SubmitTime.Value.ToString("yyyy-MM-dd");
                result.Add(orderView);
            }

            var excel = ExcelFactory.CreateBuilder();
            var sheet = excel.InsertSheet("sheet1");
            CreateTableHead(sheet);
            FillBaseContentTable(sheet, result);
            var streamFile = ConvertToStream(excel);
            return new FileStreamResult(streamFile, "application/ms-excel");
        }

        /// <summary>
        /// 构造总览的表头
        /// </summary>
        /// <param name="sheet">当前的sheet</param>
        /// <param name="headCount">表头需要合并的列数</param>
        /// <param name="headName">表头内容</param>
        public void CreateTableHead(ISheetBuilder sheet)
        {
            var sb = new StringBuilder();

            sheet.SetFont(0, 10, 13, true);
            sheet.SetRowHeight(0, 30);
            sheet.SetCellAlignment(0, 0, 1000, 10, CellAlignment.Center, CellAlignment.Center);
            sheet.SetFont(0, 0, 0, 7, 8, true);
            sheet.SetColumnWidth(0, 20);
            sheet.SetColumnWidth(1, 25);
            sheet.SetColumnWidth(2, 20);
            sheet.SetColumnWidth(3, 20);
            sheet.SetColumnWidth(4, 20);
            sheet.SetColumnWidth(5, 20);
            sheet.SetColumnWidth(6, 20);

            sheet.WriteText(0, 0, "订单编号");
            sheet.WriteText(0, 1, "奖品名称");
            sheet.WriteText(0, 2, "电话");
            sheet.WriteText(0, 3, "用户名");
            sheet.WriteText(0, 4, "领取数量");
            sheet.WriteText(0, 5, "预约领取时间");
            sheet.WriteText(0, 6, "提交时间");
        }
        /// <summary>
        /// 向excel填充内容(基础格式)
        /// </summary>
        /// <param name="sheet">当前的sheet</param>
        /// <param name="details">消费明细集合</param>
        public void FillBaseContentTable(ISheetBuilder sheet, List<OrderView> orderViews)
        {
            if (orderViews == null)
                return;
            for (int i = 0; i < orderViews.Count; i++)
            {
                var detail = orderViews[i];
                sheet.WriteText(i + 1, 0, detail.OrderId);
                sheet.WriteText(i + 1, 1, detail.PrizeName);
                sheet.WriteText(i + 1, 2, detail.Phone);
                sheet.WriteText(i + 1, 3, detail.UserName);
                sheet.WriteText(i + 1, 4, detail.PrizeCount);
                sheet.WriteText(i + 1, 5, detail.GetTime);
                sheet.WriteText(i + 1, 6, detail.SubmitTime);
            }
        }

        /// <summary>
        /// 将excel对象转换为数据流
        /// </summary>
        /// <param name="excel">excel对象</param>
        /// <returns>数据流</returns>
        public MemoryStream ConvertToStream(IExcelBuilder excel)
        {
            var stream = new MemoryStream();
            excel.Workbook.Save(stream, SaveFormat.Xlsx);
            stream.Position = 0;
            return stream;
        }
        #endregion

        #region 奖品管理



        public string[] ExtentsfileName = new string[] { ".jpeg", ".png", ".jpg" };
        public string prizeUrlPath = "/image/prize/";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Prize()
        {
            //var result = new BaseBLL<Prize>().LoadEntities();
            return View();
        }



        /// <summary>
        /// 奖品管理
        /// </summary>
        /// <returns></returns>
        public ActionResult CetPrize()
        {
            var result = new BaseBLL<Prize>().LoadEntities();
            return Json(new { total = result.Count, rows = result });
        }

        /// <summary>
        /// 修改奖品
        /// </summary>
        /// <param name="prize"></param>
        /// <returns></returns>
        public ActionResult EditPrize(Prize prize)
        {
            try
            {
                var result = new BaseBLL<Prize>().FirstOrDefault(x => x.Id == prize.Id);
                if (result == null)
                {
                    return Json(new { code = 1, msg = "修改失败" });
                }
                if (string.IsNullOrWhiteSpace(prize.PrizeImg) || string.IsNullOrWhiteSpace(prize.PrizeName) || prize.PrizeNum <= 0 || prize.PrizeScore <= 0)
                    return Json(new { code = 1, msg = "数据不合法" });
                //prize.PrizeImg = result.PrizeImg;
                new BaseBLL<Prize>().UpdateEntity(prize);
                return Json(new { code = 0, msg = "修改成功" });
            }
            catch (Exception e)
            {
                return Json(new { code = 1, msg = $"修改异常，请联系程序员修改,异常消息：{e.Message}" });
            }
        }



        /// <summary>
        /// 新增奖品
        /// </summary>
        /// <param name="prize"></param>
        /// <returns></returns>
        public ActionResult AddPrize(Prize prize)
        {
            try
            {
                var result = new BaseBLL<Prize>().FirstOrDefault(x => x.PrizeId == prize.PrizeId);
                if (result != null)
                    return Json(new { code = 1, msg = "编号已存在" });
                if (string.IsNullOrWhiteSpace(prize.PrizeImg) || string.IsNullOrWhiteSpace(prize.PrizeName) || prize.PrizeNum <= 0 || prize.PrizeScore <= 0)
                    return Json(new { code = 1, msg = "数据不合法" });
                new BaseBLL<Prize>().AddEntity(prize);
                return Json(new { code = 0, msg = "添加成功" });
            }
            catch (Exception e)
            {
                return Json(new { code = 1, msg = $"添加异常，请联系程序员修改,异常消息：{e.Message}" });
            }
        }

        /// <summary>
        /// 删除奖品
        /// </summary>
        /// <param name="prize"></param>
        /// <returns></returns>
        public ActionResult DelPrize(int Id)
        {
            try
            {
                var result = new BaseBLL<Prize>().FirstOrDefault(x => x.Id == Id);
                if (result == null)
                    return Json(new { code = 1, msg = "数据不存在" });
                new BaseBLL<Prize>().DeleteEntity(result);
                return Json(new { code = 0, msg = "删除成功" });
            }
            catch (Exception e)
            {
                return Json(new { code = 1, msg = $"删除异常，请联系程序员修改,异常消息：{e.Message}" });
            }
        }



        public ActionResult PrizeUploadFile(HttpPostedFileBase fileName, Prize prize)
        {
            var request = HttpContext.Request;
            if (request.Files.Count > 0)
            {
                var file = request.Files[0];
                var extenfilename = Path.GetExtension(file.FileName);
                string path = HttpContext.Server.MapPath(prizeUrlPath);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                if (ExtentsfileName.Contains(extenfilename.ToLower()))
                {
                    string urlfile = prizeUrlPath + DateTime.Now.ToFileTime() + extenfilename;
                    string filepath = HttpContext.Server.MapPath(urlfile);
                    file.SaveAs(filepath);
                    return Json(new { code = 0, Msg = "上传成功", Url = $"{Science}{urlfile}" });
                }
                else
                {
                    return Json(new { code = 0, Msg = "只允许上传指定格式文件" + string.Join(",", ExtentsfileName), Url = "" });
                }
            }
            else
            {
                return Json(new { code = 0, msg = "上传成功" }); ;
            }
        }

        #endregion

        #region 奖品管理




        public string questionUrlPath = "/image/question/";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Question()
        {
            //var result = new BaseBLL<Prize>().LoadEntities();
            return View();
        }

        /// <summary>
        /// 题目管理
        /// </summary>
        /// <returns></returns>
        public ActionResult CetQuestion(string qcontent, int page = 1, int rows = 5)
        {
            var orders = new BaseBLL<Question>().LoadEntities();
            int total = orders.Count();
            if (!string.IsNullOrWhiteSpace(qcontent))
            {
                orders = orders.Where(x => x.QuestionContent.Contains(qcontent)).ToList();
            }
            var prizes = new BaseBLL<Question>().LoadEntities();
            if (page == 0 && rows == 0)
            {
                orders = orders.OrderBy(m => m.QuestionId).ToList();
            }
            else
            {
                orders = orders.OrderBy(m => m.QuestionId).Skip((page - 1) * rows).Take(rows).ToList();
            }
            return Json(new { total, rows = orders });
        }


        /// <summary>
        /// 修改题目
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public ActionResult EditQuestion(Question question)
        {
            try
            {
                var result = new BaseBLL<Question>().FirstOrDefault(x => x.QuestionId == question.QuestionId);
                if (result == null)
                {
                    return Json(new { code = 1, msg = "修改失败" });
                }
                if (string.IsNullOrWhiteSpace(question.QuestionContent) || string.IsNullOrWhiteSpace(question.Answer) || string.IsNullOrWhiteSpace(question.QuestionOption)
                    || question.Score <= 0 || question.Censorship <= 0)
                    return Json(new { code = 1, msg = "数据不合法" });
                new BaseBLL<Question>().UpdateEntity(question);
                return Json(new { code = 0, msg = "修改成功" });
            }
            catch (Exception e)
            {
                return Json(new { code = 1, msg = $"修改异常，请联系程序员修改,异常消息：{e.Message}" });
            }
        }



        /// <summary>
        /// 新增题目
        /// </summary>
        /// <param name="prize"></param>
        /// <returns></returns>
        public ActionResult AddQuestion(Question question)
        {
            try
            {
                var result = new BaseBLL<Question>().FirstOrDefault(x => x.QuestionId == question.QuestionId);
                if (result != null)
                    return Json(new { code = 1, msg = "编号已存在" });
                if (string.IsNullOrWhiteSpace(question.QuestionContent) || string.IsNullOrWhiteSpace(question.Answer) || string.IsNullOrWhiteSpace(question.QuestionOption)
                  || question.Score <= 0 || question.Censorship <= 0)
                    return Json(new { code = 1, msg = "数据不合法" });
                question.QuestionIndex = 0;
                question.QuestionType = 0;
                question.ImgName = string.Empty;
                question.DifficultyLevel = 0;
                new BaseBLL<Question>().AddEntity(question);
                return Json(new { code = 0, msg = "添加成功" });
            }
            catch (Exception e)
            {
                return Json(new { code = 1, msg = $"添加异常，请联系程序员修改,异常消息：{e.Message}" });
            }
        }

        /// <summary>
        /// 删除题目
        /// </summary>
        /// <param name="prize"></param>
        /// <returns></returns>
        public ActionResult Delquestion(int Id)
        {
            try
            {
                var result = new BaseBLL<Question>().FirstOrDefault(x => x.QuestionId == Id);
                if (result == null)
                    return Json(new { code = 1, msg = "数据不存在" });
                new BaseBLL<Question>().DeleteEntity(result);
                return Json(new { code = 0, msg = "删除成功" });
            }
            catch (Exception e)
            {
                return Json(new { code = 1, msg = $"删除异常，请联系程序员修改,异常消息：{e.Message}" });
            }
        }

        /// <summary>
        /// 题目上传
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="prize"></param>
        /// <returns></returns>
        public ActionResult QuestionUploadFile(HttpPostedFileBase fileName, Prize prize)
        {
            var request = HttpContext.Request;
            if (request.Files.Count > 0)
            {
                var file = request.Files[0];
                var extenfilename = Path.GetExtension(file.FileName);
                string path = HttpContext.Server.MapPath(prizeUrlPath);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                if (ExtentsfileName.Contains(extenfilename.ToLower()))
                {
                    string urlfile = prizeUrlPath + DateTime.Now.ToFileTime() + extenfilename;
                    string filepath = HttpContext.Server.MapPath(urlfile);
                    file.SaveAs(filepath);
                    return Json(new { code = 0, Msg = "上传成功", Url = $"{Science}{urlfile}" });
                }
                else
                {
                    return Json(new { code = 0, Msg = "只允许上传指定格式文件" + string.Join(",", ExtentsfileName), Url = "" });
                }
            }
            else
            {
                return Json(new { code = 0, msg = "上传成功" }); ;
            }
        }

        #endregion

        #region 领取时间管理
        /// <summary>
        /// 领取时间主页
        /// </summary>
        /// <returns></returns>
        public ActionResult CetDate()
        {
            return View();
        }

        /// <summary>
        /// 领取时间管理
        /// </summary>
        /// <returns></returns>

        public ActionResult CetPrizeDate()
        {
            var result = new BaseBLL<GetPrizeTime>().LoadEntities();
            return Json(new { total = result.Count, rows = result });
        }


        /// <summary>
        /// 修改奖品领取时间管理
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public ActionResult EditPrizeCetDate(GetPrizeTime time)
        {
            var result = new BaseBLL<GetPrizeTime>().FirstOrDefault(x => x.Id == time.Id);
            new BaseBLL<GetPrizeTime>().UpdateEntity(time);
            return Json(new { code = 0, msg = "修改成功" });
        }

        /// <summary>
        /// 修改奖品领取时间管理
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public ActionResult AddCetDate(DateTime getTime)
        {
            try
            {
                var date = getTime.ToString("yyyy-MM-dd");
                new BaseBLL<GetPrizeTime>().AddEntity(new GetPrizeTime { GetTime = date });
                return Json(new { code = 0, msg = "添加成功" });
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// 删除时间
        /// </summary>
        /// <param name="prize"></param>
        /// <returns></returns>
        public ActionResult DelDate(int Id)
        {
            try
            {
                var result = new BaseBLL<GetPrizeTime>().FirstOrDefault(x => x.Id == Id);
                if (result == null)
                    return Json(new { code = 1, msg = "数据不存在" });
                new BaseBLL<GetPrizeTime>().DeleteEntity(result);
                return Json(new { code = 0, msg = "删除成功" });
            }
            catch (Exception e)
            {
                return Json(new { code = 1, msg = $"删除异常，请联系程序员修改,异常消息：{e.Message}" });
            }
        }

        #endregion

        #region H5 数据接口

        /// <summary>
        /// 获取题目数据
        /// </summary>
        /// <param name="checkpoint">关卡数</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetQuestions(int checkpoint)
        {
            var question = new List<Question>();
            var date = DateTime.Now;

            //var questionKey = $"question_{checkpoint}_{date.ToString("MMdd")}";
            //if (CacheManager.Contains(questionKey))
            //{
            //    var questionCache = CacheManager.GetData<List<Question>>(questionKey);
            //    question = questionCache.OrderBy(x => x.QuestionId).ToList();
            //}
            //else
            //{
            //    var result = new BaseBLL<Question>().LoadEntities(x => x.Censorship == checkpoint).ToList();
            //    if (!result.Any())
            //        return Json(ResultView.View(1, "该关卡无题目，请录入", question));
            //    //var easyQuestion = result.Where(x => x.DifficultyLevel == 1).ToList();
            //    //var hardQuestion = result.Where(x => x.DifficultyLevel == 0).ToList();
            //    //if (!easyQuestion.Any() || !hardQuestion.Any())
            //    //{
            //    //    return Json(ResultView.View(1, "题库缺少题目", question));
            //    //}
            //    var showQuestion = result.OrderBy(s => Guid.NewGuid()).Take(10);
            //    var showQuestions = showQuestion.OrderBy(x => x.QuestionId).ToList();
            //    question.AddRange(showQuestions);
            //    CacheManager.Add(questionKey, showQuestions, 48 * 60);
            //}

            var result = new BaseBLL<Question>().LoadEntities(x => x.Censorship == checkpoint).ToList();
            question = result.OrderBy(s => Guid.NewGuid()).Take(10).ToList();
            if (checkpoint == 3)
            {
                Random randObj = new Random();
                foreach (var item in question)
                {
                    item.ImgPath = $"http://www.ihavedream.top/image/twoCheck/" + randObj.Next(1, 10) + ".jpg";
                }
            }
            return Json(ResultView.View(0, "成功", question));
        }

        /// <summary>
        /// 获取奖品数据
        /// </summary>
        /// <param name="checkpoint">关卡数</param>
        /// <returns></returns>
        public ActionResult GetPrizes()
        {
            var result = new BaseBLL<Prize>().LoadEntities().ToList();
            if (!result.Any())
                return Json("");
            return Json(ResultView.View(0, "成功", result));
        }

        /// <summary>
        /// 提交订单 
        /// </summary>
        /// <param name="request">请求</param>
        /// <returns></returns>
        public ActionResult SubmitOrder(OrderRequest request)
        {
            try
            {
                var prize = new BaseBLL<Prize>().FirstOrDefault(x => x.PrizeId == request.PrizeId);
                if (prize == null)
                    return Json(ResultView.View(1, "奖品不存在"));
                if (prize.PrizeNum - request.PrizeCount <= 0)
                {
                    return Json(ResultView.View(1, "奖品数量不足"));
                }
                var date = DateTime.Now;

                if (date > Convert.ToDateTime(request.GetTime))
                {
                    return Json(ResultView.View(1, "领取时间错误"));
                }
                var order = new BaseBLL<Order>().FirstOrDefault(x => x.SubmitTime.Value.Month == date.Month && x.Phone == request.Phone);
                if (order != null)
                {
                    return Json(ResultView.View(1, "你已玩过，请下月再来"));
                }
                new BaseBLL<Order>().AddEntity(new Order
                {
                    OrderId = date.ToString("yyyyMMdd") + Guid.NewGuid().ToString("N"),
                    PrizeId = request.PrizeId,
                    Phone = request.Phone,
                    UserName = request.UserName,
                    PrizeCount = request.PrizeCount,
                    GetTime = request.GetTime,
                    SubmitTime = date
                });
                prize.PrizeNum = prize.PrizeNum - request.PrizeCount;
                new BaseBLL<Prize>().AddEntity(prize);
                return Json(ResultView.View(0, "成功"));
            }
            catch (Exception e)
            {

                return Json(ResultView.View(1, ""));
            }

        }



        /// <summary>
        /// 获取领取时间
        /// </summary>
        /// <param name="checkpoint">关卡数</param>
        /// <returns></returns>
        public ActionResult GetTime()
        {
            var result = new BaseBLL<GetPrizeTime>().LoadEntities().ToList();
            if (!result.Any())
                return Json(ResultView.View(1, "空数据", result));
            return Json(ResultView.View(0, "成功", result));
        }


        public ActionResult GetWeiXinShare()
        {
            var result = Weixin.WeiXinAPI.GetWeiXinConfig("");
            var jsonresult = new
            {
                appId = result.Item1,
                timestamp = result.Item2,
                nonceStr = result.Item3,
                signature = result.Item4
            };
            return Json(jsonresult);
        }

        #endregion
    }
}