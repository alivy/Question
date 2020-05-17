namespace Model.DBModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Prize")]
    public partial class Prize
    {
        public int Id { get; set; }

        public int PrizeId { get; set; }

        [Required]
        [StringLength(50)]
        public string PrizeName { get; set; }

        [Required]
        [StringLength(200)]
        public string PrizeImg { get; set; }

        public int PrizeNum { get; set; }

        public int PrizeScore { get; set; }
        
    }
}
