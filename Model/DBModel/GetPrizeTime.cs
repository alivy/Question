using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DBModel
{
    [Table("GetPrizeTime")]
    public class GetPrizeTime
    {
        public int Id { get; set; }
        /// <summary>
        /// 领取时间
        /// </summary>
        [Required]
        [StringLength(50)]
        public string GetTime { get; set; }
    }
}
