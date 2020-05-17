using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Model.DBModel
{
    ///// <summary>
    ///// 数据库连接
    ///// </summary>
    //public partial class DBContext : DbContext
    //{
    //    public DBContext() : base("name=AnswerActivityDB")
    //    {
    //    }
    //    /// <summary>
    //    /// 创建DBContext
    //    /// </summary>
    //    /// <returns></returns>
    //    public static DBContext CreateContext()
    //    {
    //        return new DBContext();
    //    }


    //    public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
    //    public virtual DbSet<Order> Order { get; set; }
    //    public virtual DbSet<Prize> Prize { get; set; }
    //    public virtual DbSet<Question> Question { get; set; }
    //    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    //    {
    //        modelBuilder.Entity<Order>()
    //            .Property(e => e.Phone)
    //            .IsUnicode(false);
    //    }
    //}
}
