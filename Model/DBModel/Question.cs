namespace Model.DBModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Question")]
    public partial class Question
    {
        public int QuestionId { get; set; }

        public string QuestionContent { get; set; }

        public int QuestionIndex { get; set; }

        public int QuestionType { get; set; }

        [StringLength(500)]
        public string ImgPath { get; set; }

        [StringLength(500)]
        public string ImgName { get; set; }

        [Required]
        [StringLength(500)]
        public string Answer { get; set; }

        public int DifficultyLevel { get; set; }

        public int? Score { get; set; }

        public DateTime? CreateDate { get; set; }

        public int? Censorship { get; set; }

        [StringLength(200)]
        public string QuestionOption { get; set; }
    }
}
