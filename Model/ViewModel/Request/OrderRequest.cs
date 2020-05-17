using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel.Request
{
    /// <summary>
    /// 订单请求
    /// </summary>
    public class OrderRequest
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }


        /// <summary>
        /// 奖品id
        /// </summary>
        public int PrizeId { get; set; }

        /// <summary>
        /// 兑换奖品数量
        /// </summary>
        public int PrizeCount { get; set; }

        /// <summary>
        /// 领取时间
        /// </summary>
        public string GetTime { get; set; }
    }
}
