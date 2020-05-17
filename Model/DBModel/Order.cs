namespace Model.DBModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Order")]
    public partial class Order
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string OrderId { get; set; }

        public int PrizeId { get; set; }

        [Required]
        [StringLength(50)]
        public string Phone { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        public int PrizeCount { get; set; }

        [StringLength(50)]
        public string GetTime { get; set; }

        public DateTime? SubmitTime { get; set; }
    }
}
