namespace Model.DBModel
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DBContext : DbContext
    {
        public DBContext() : base("name=AnswerActivityDB")
        {
        }

        /// <summary>
        /// ´´½¨DBContext
        /// </summary>
        /// <returns></returns>
        public static DBContext CreateContext()
        {
            return new DBContext();
        }

        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<Prize> Prize { get; set; }
        public virtual DbSet<Question> Question { get; set; }

        public virtual DbSet<GetPrizeTime> GetPrizeTime { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .Property(e => e.Phone)
                .IsUnicode(false);
        }
    }
}
