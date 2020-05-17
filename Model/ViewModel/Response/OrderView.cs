using Model.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel.Response
{
    public class OrderView
    {

        public int Id { get; set; }

        public string OrderId { get; set; }

        public int PrizeId { get; set; }


        public string Phone { get; set; }

        public string UserName { get; set; }

        public int PrizeCount { get; set; }


        public string GetTime { get; set; }

        public string SubmitTime { get; set; }

        public string PrizeName { get; set; }

    }
}
