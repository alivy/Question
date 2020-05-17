using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel.Response
{
    /// <summary>
    /// 返回结果
    /// </summary>
    public class ResultView
    {

        /// <summary>
        /// 0成功 other 失败
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        ///消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public object Value { get; set; }


        public static ResultView View(int code, string message, object Data = null)
        {
            return new ResultView
            {
                Code = code,
                Message = message,
                Value = Data
            };
        }
    }
}
