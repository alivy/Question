using EntityFramework.Extensions;
using Model.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace DAL
{
    /// <summary>
    /// Dal基类，继承该类就拥有增删改查的方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseDal<T> where T : class, new()
    {


        #region 获取符合条件的实体个数
        /// <summary>
        /// 获取符合条件的实体个数
        /// </summary>
        /// <param name="where">条件</param>
        /// <returns></returns>
        public int GetCount(Expression<Func<T, bool>> where = null)
        {
            using (var db = DBContext.CreateContext())
            {
                if (where == null)
                {
                    return db.Set<T>().Count();
                }
                return db.Set<T>().Count<T>(where);
            }
        }
        #endregion

        #region 获取实体集指定总和
        /// <summary>
        /// 获取实体集指定总和
        /// </summary>
        /// <param name="field">求和字段</param>
        /// <param name="where">条件</param>
        /// <returns></returns>
        public decimal? GetSum(Expression<Func<T, decimal?>> field, Expression<Func<T, bool>> where = null)
        {
            using (var db = DBContext.CreateContext())
            {
                if (where != null)
                    return db.Set<T>().Where(where).Sum(field);
                return db.Set<T>().Sum(field);
            }
        }
        #endregion

        #region 获取实体集
        /// <summary>
        /// 获取实体集
        /// </summary>
        /// <param name="where">条件</param>
        /// <returns></returns>
        public IList<T> LoadEntities(Expression<Func<T, bool>> where = null)
        {
            using (var db = DBContext.CreateContext())
            {
                if (where == null)
                {
                    return db.Set<T>().ToList();
                }
                return db.Set<T>().Where<T>(where).ToList();
            }
        }
        #endregion

        #region 获取实体集指定条数
        /// <summary>
        /// 获取实体集指定条数
        /// </summary>
        /// <param name="where">条件</param>
        /// <returns></returns>
        public IList<T> LoadEntities<S>(Expression<Func<T, bool>> where = null, Func<T, S> order = null, bool isAsc = false, int top = 0)
        {
            using (var db = DBContext.CreateContext())
            {
                var m_query = db.Set<T>();
                IQueryable<T> m_data = null;

                if (where != null)
                {
                    m_data = m_query.Where<T>(where).AsQueryable();
                }
                if (order != null)
                {
                    if (isAsc)
                    {
                        m_data = m_data.OrderBy<T, S>(order).AsQueryable();
                    }
                    else
                    {
                        m_data = m_data.OrderByDescending<T, S>(order).AsQueryable();
                    }
                }
                if (top > 0)
                {
                    m_data = m_data.Take(top);
                }


                return m_data.ToList();
            }
        }
        #endregion

        #region 数据分页
        /// <summary>
        /// 数据分页
        /// </summary>
        /// <param name="where">条件的lambda</param>
        /// <param name="order">排序的lambda</param>
        /// <param name="total">总数，返回值</param>
        /// <param name="pageSize">每一页数据的大小</param>
        /// <param name="pageIndex">当前页索引</param>
        /// <param name="isAsc">排序方式，true为正序，false为倒序</param>
        /// <returns></returns>
        public IList<T> GetPageList<S>(Expression<Func<T, bool>> where, Func<T, S> order,
            out int total, int pageSize = 10, int pageIndex = 1, bool isAsc = false)
        {
            using (DBContext db = DBContext.CreateContext())
            {
                var temp = db.Set<T>().Where<T>(where).AsQueryable();
                total = temp.Count();
                if (isAsc)
                {
                    return temp.OrderBy<T, S>(order)
                            .Skip<T>((pageIndex - 1) * pageSize)
                            .Take<T>(pageSize).ToList();
                }
                else
                {
                    return temp.OrderByDescending(order)
                        .Skip<T>((pageIndex - 1) * pageSize)
                        .Take<T>(pageSize).ToList();
                }
            }
        }
        #endregion

        #region 添加实体
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public int AddEntity(T entity)
        {
            using (var db = DBContext.CreateContext())
            {
                db.Set<T>().Add(entity);
                return db.SaveChanges();
            }
        }
        #endregion

        #region 更新实体
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">要更新的实体对象</param>
        /// <returns></returns>
        public int UpdateEntity(T entity)
        {
            using (var db = DBContext.CreateContext())
            {
                db.Entry<T>(entity).State = System.Data.Entity.EntityState.Modified;
                return db.SaveChanges();
            }
        }
        #endregion

        #region 更新指定属性的记录
        /// <summary>
        /// 更新指定属性的记录
        /// </summary>
        /// <param name="model">模型</param>
        /// <param name="propertys">属性</param>
        public int UpdateEntity(T model, string[] propertys)
        {
            using (var db = DBContext.CreateContext())
            {
                if (propertys == null || propertys.Length == 0)
                {
                    throw new Exception("当前更新的实体必须至少指定一个字段名称");
                }

                db.Configuration.ValidateOnSaveEnabled = false;

                DbEntityEntry entry = db.Entry(model);
                entry.State = System.Data.Entity.EntityState.Unchanged;
                foreach (var item in propertys)
                {
                    entry.Property(item).IsModified = true;
                }
                return db.SaveChanges();
            }
        }
        #endregion

        #region 删除实体
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity">要删除实体的对象，我们一般只需给
        /// 该实体的Id字段赋值即可，其他字段不用赋值</param>
        /// <returns></returns>
        public int DeleteEntity(T entity)
        {
            using (var db = DBContext.CreateContext())
            {
                db.Entry<T>(entity).State = System.Data.Entity.EntityState.Deleted;
                return db.SaveChanges();
            }
        }
        #endregion

        #region 获取第一个或者null的实体
        /// <summary>
        /// 获取第一个或者null的实体
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="order">排序</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        public T FirstOrDefault(Expression<Func<T, bool>> where = null)
        {
            using (var db = DBContext.CreateContext())
            {
                var m_query = db.Set<T>().AsQueryable<T>();
                if (where != null)
                {
                    m_query = m_query.Where(where);
                }
                return m_query.FirstOrDefault();
            }
        }



        #endregion

        #region 获取第一个实体
        /// <summary>
        /// 获取第一个或者null的实体
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="order">排序</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        public T First<S>(Expression<Func<T, bool>> where = null, Expression<Func<T, S>> order = null, bool isAsc = true)
        {
            using (var db = DBContext.CreateContext())
            {
                var m_query = db.Set<T>().AsQueryable<T>();
                if (where != null)
                {
                    m_query = m_query.Where(where);
                }
                if (order != null)
                {
                    if (isAsc)
                    {
                        m_query = m_query.OrderBy(order);
                    }
                    else
                    {
                        m_query = m_query.OrderByDescending(order);
                    }
                }
                return m_query.First();
            }
        }
        #endregion

        #region 获取一个实体,实体数量大于1或者不存在将抛出异常
        /// <summary>
        /// 获取一个实体,实体数量大于1或者不存在将抛出异常
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="order">排序</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        public T Single<S>(Expression<Func<T, bool>> where = null, Expression<Func<T, S>> order = null, bool isAsc = true)
        {
            using (var db = DBContext.CreateContext())
            {
                var m_query = db.Set<T>().AsQueryable<T>();
                if (where != null)
                {
                    m_query = m_query.Where(where);
                }
                if (order != null)
                {
                    if (isAsc)
                    {
                        m_query = m_query.OrderBy(order);
                    }
                    else
                    {
                        m_query = m_query.OrderByDescending(order);
                    }
                }
                return m_query.Single();
            }
        }
        #endregion

        #region 获取一个实体,实体数量大于1将抛出异常
        /// <summary>
        /// 获取一个实体,实体数量大于1将抛出异常
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="order">排序</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        public T SingleOrDefault<S>(Expression<Func<T, bool>> where = null, Expression<Func<T, S>> order = null, bool isAsc = true)
        {
            using (var db = DBContext.CreateContext())
            {
                var m_query = db.Set<T>().AsQueryable<T>();
                if (where != null)
                {
                    m_query = m_query.Where(where);
                }
                if (order != null)
                {
                    if (isAsc)
                    {
                        m_query = m_query.OrderBy(order);
                    }
                    else
                    {
                        m_query = m_query.OrderByDescending(order);
                    }
                }
                return m_query.SingleOrDefault();
            }
        }
        /// <summary>
        /// 获取一个实体,实体数量大于1将抛出异常
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="order">排序</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        public T SingleOrDefault(Expression<Func<T, bool>> where = null)
        {
            using (var db = DBContext.CreateContext())
            {
                var m_query = db.Set<T>().AsQueryable<T>();
                if (where != null)
                {
                    m_query = m_query.Where(where);
                }
                return m_query.SingleOrDefault();
            }
        }

        #endregion

        #region 获取指定类型的第一个实体
        /// <summary>
        /// 获取指定类型的第一个实体
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="order">排序</param>
        /// <param name="isAsc">是否升序，默认升序</param>
        /// <param name="selector">要返回对象的表达式</param>
        /// <returns></returns>
        public S GetFirstEntity<S, K>(Expression<Func<T, bool>> where = null, Expression<Func<T, K>> order = null,
            bool isAsc = true, Expression<Func<T, S>> selector = null) where S : class
        {
            using (var db = DBContext.CreateContext())
            {
                var query = db.Set<T>().AsQueryable();
                if (where != null)
                {
                    query = query.Where(where);
                }
                if (order != null)
                {
                    if (isAsc)
                    {
                        query = query.OrderBy(order);
                    }
                    else
                    {
                        query = query.OrderByDescending(order);
                    }
                }
                return query.Select(selector).First();
            }
        }
        #endregion

        #region 获取指定类型的第一个实体或null
        /// <summary>
        /// 获取指定类型的第一个实体或null
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="order">排序</param>
        /// <param name="isAsc">是否升序，默认升序</param>
        /// <param name="selector">要返回对象的表达式</param>
        /// <returns></returns>
        public S GetFirstOrDefaultEntity<S, K>(Expression<Func<T, bool>> where = null, Expression<Func<T, K>> order = null,
            bool isAsc = true, Expression<Func<T, S>> selector = null) where S : class
        {
            using (var db = DBContext.CreateContext())
            {
                var query = db.Set<T>().AsQueryable();
                if (where != null)
                {
                    query = query.Where(where);
                }
                if (order != null)
                {
                    if (isAsc)
                    {
                        query = query.OrderBy(order);
                    }
                    else
                    {
                        query = query.OrderByDescending(order);
                    }
                }
                return query.Select(selector).FirstOrDefault();
            }
        }
        #endregion

        #region 获取指定类型的一个实体或null
        /// <summary>
        /// 获取指定类型的一个实体或null
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="order">排序</param>
        /// <param name="isAsc">是否升序，默认升序</param>
        /// <param name="selector">要返回对象的表达式</param>
        /// <returns></returns>
        public S GetSingleOrDefaultEntity<S, K>(Expression<Func<T, bool>> where = null, Expression<Func<T, K>> order = null,
            bool isAsc = true, Expression<Func<T, S>> selector = null)
        {
            using (var db = DBContext.CreateContext())
            {
                var query = db.Set<T>().AsQueryable();
                if (where != null)
                {
                    query = query.Where(where);
                }
                if (order != null)
                {
                    if (isAsc)
                    {
                        query = query.OrderBy(order);
                    }
                    else
                    {
                        query = query.OrderByDescending(order);
                    }
                }
                return query.Select(selector).SingleOrDefault();
            }
        }
        #endregion

        #region 获取指定类型的一个实体
        /// <summary>
        /// 获取指定类型的一个实体
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="order">排序</param>
        /// <param name="isAsc">是否升序，默认升序</param>
        /// <param name="selector">要返回对象的表达式</param>
        /// <returns></returns>
        public S GetSingleEntity<S, K>(Expression<Func<T, bool>> where = null, Expression<Func<T, K>> order = null,
            bool isAsc = true, Expression<Func<T, S>> selector = null)
        {
            using (var db = DBContext.CreateContext())
            {
                var query = db.Set<T>().AsQueryable();
                if (where != null)
                {
                    query = query.Where(where);
                }
                if (order != null)
                {
                    if (isAsc)
                    {
                        query = query.OrderBy(order);
                    }
                    else
                    {
                        query = query.OrderByDescending(order);
                    }
                }
                return query.Select(selector).Single();
            }
        }
        #endregion

        #region 返回指定类型的集合
        /// <summary>
        /// 返回指定类型的集合
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="order">排序</param>
        /// <param name="isAsc">是否升序</param>
        /// <param name="selector">返回指定对象</param>
        /// <returns></returns>
        public IList<S> GetEntities<S, K>(Expression<Func<T, bool>> where = null, Expression<Func<T, K>> order = null,
                  bool isAsc = true, Expression<Func<T, S>> selector = null)
        {
            using (var db = DBContext.CreateContext())
            {
                var query = db.Set<T>().AsQueryable();
                if (where != null)
                {
                    query = query.Where(where);
                }
                if (order != null)
                {
                    if (isAsc)
                    {
                        query = query.OrderBy(order);
                    }
                    else
                    {
                        query = query.OrderByDescending(order);
                    }
                }
                return query.Select(selector).ToList();
            }
        }
        #endregion

        #region 返回指定类型的分页
        /// <summary>
        /// 返回指定类型的分页
        /// </summary>
        /// <param name="where">条件，不能为null</param>
        /// <param name="order">排序，不能为null</param>
        /// <param name="selector">返回对象，不能为null</param>
        /// <param name="total">总数</param>
        /// <param name="isAsc">是否升序，默认升序</param>
        /// <param name="pageSize">每页数据大小，默认20</param>
        /// <param name="pageIndex">当前页数，默认1</param>
        /// <returns></returns>
        public IList<S> GetEntityPageList<S, K>(Expression<Func<T, bool>> where, Expression<Func<T, K>> order,
            Expression<Func<T, S>> selector, out int total, bool isAsc = true, int pageSize = 20, int pageIndex = 1)
            where S : class, new()
        {
            using (var db = DBContext.CreateContext())
            {
                var query = db.Set<T>().AsQueryable();
                query = query.Where(where);
                if (isAsc)
                {
                    query = query.OrderBy(order);
                }
                else
                {
                    query = query.OrderByDescending(order);
                }
                total = query.Count();
                return query.Skip<T>((pageIndex - 1) * pageSize)
                            .Take<T>(pageSize)
                            .Select(selector)
                            .ToList();
            }
        }
        #endregion

        #region 执行sql语句，返回指定的类型
        /// <summary>
        /// 执行sql语句，返回指定的类型
        /// </summary>
        /// <param name="sql">要执行的sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public IList<S> ExecuteSql<S>(string sql, params SqlParameter[] parameters) where S : class
        {
            using (var db = DBContext.CreateContext())
            {
                return db.Database.SqlQuery<S>(sql, parameters).ToList();
            }
        }

        #endregion

        #region 执行sql语句，返回Int型
        public Int32 Executesql(string sql)
        {
            using (var db = DBContext.CreateContext())
            {
                return db.Database.ExecuteSqlCommand(sql);
            }
        }
        #endregion

        #region 获取一个DataSet
        /// <summary>
        /// 获取一个DataSet
        /// </summary>
        /// <param name="sql">要执行的sql语句</param>
        /// <param name="cmdType">执行类型</param>
        /// <param name="sqlParams">传递的参数</param>
        /// <returns></returns>
        public System.Data.DataSet GetDataSet(string sql, System.Data.CommandType cmdType, params SqlParameter[] sqlParams)
        {
            using (var db = DBContext.CreateContext())
            {
                var constr = db.Database.Connection.ConnectionString;
                System.Data.DataSet ds = new System.Data.DataSet();
                using (SqlDataAdapter adapter = new SqlDataAdapter(sql, constr))
                {
                    adapter.SelectCommand.CommandType = cmdType;
                    if (sqlParams != null)
                    {
                        adapter.SelectCommand.Parameters.AddRange(sqlParams);
                    }
                    adapter.Fill(ds);
                    return ds;
                }
            }
        }

        #endregion

        #region 获取脉冲号
        public int GetPulse(string name)
        {
            int result = 0;
            SqlParameter[] m_parms = new SqlParameter[2]{
                new SqlParameter("@Name",name),
                new SqlParameter("@pnum",result)
            };
            m_parms[1].Direction = System.Data.ParameterDirection.Output;
            using (var db = DBContext.CreateContext())
            {
                db.Database.ExecuteSqlCommand("EXEC Pro_GetPulse @Name,@pnum out", m_parms);
                return result = Convert.ToInt32(m_parms[1].Value);
            }
        }
        #endregion

        #region 批量插入
        /// <summary>  
        /// 批量插入  
        /// </summary>  
        /// <typeparam name="T">泛型集合的类型</typeparam>  
        /// <param name="tableName">将泛型集合插入到本地数据库表的表名</param>  
        /// <param name="list">要插入大泛型集合</param>  
        public void BulkInsert<S>(string tableName, List<S> list)
        {
            using (var db = DBContext.CreateContext())
            {
                //db.Database.Connection.Open(); //打开Coonnection连接  
                using (var bulkCopy = new SqlBulkCopy(db.Database.Connection.ConnectionString))
                {
                    bulkCopy.BatchSize = list.Count;
                    bulkCopy.DestinationTableName = tableName;

                    var table = new DataTable();
                    var props = TypeDescriptor.GetProperties(typeof(T))
                        .Cast<PropertyDescriptor>()
                        .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                        .ToArray();

                    foreach (var propertyInfo in props)
                    {
                        bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                        table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                    }

                    var values = new object[props.Length];
                    foreach (var item in list)
                    {
                        for (var i = 0; i < values.Length; i++)
                        {
                            values[i] = props[i].GetValue(item);
                        }

                        //需要匹配指定列的情况下
                        //bulkCopy.ColumnMappings.Add("ProductID", "ProductID");
                        table.Rows.Add(values);
                    }
                    bulkCopy.WriteToServer(table);
                }
            }
        }

        /// <summary>
        ///  批量插入 
        /// </summary>
        /// <param name="dt">表</param>
        /// <param name="tableName">本地数据库表的表名</param>
        public void BulkInsert(string tableName, DataTable dt)
        {
            using (var db = DBContext.CreateContext())
            {
                //db.Database.Connection.Open(); //打开Coonnection连接  
                using (var bulkCopy = new SqlBulkCopy(db.Database.Connection.ConnectionString))
                {
                    bulkCopy.BatchSize = dt.Rows.Count;
                    bulkCopy.DestinationTableName = tableName;
                    bulkCopy.WriteToServer(dt);
                }
            }
        }
        #endregion

        #region 使用Z.EntityFramework.Extensions批量处理
        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int BulkUpdate(List<T> entity)
        {
            using (var db = DBContext.CreateContext())
            {
                db.Set<T>().BulkUpdate(entity);
                return db.SaveChanges();
            }
        }


        /// <summary>
        /// 批量更新,可协变
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int BulkUpdate(IEnumerable<T> entity)
        {
            using (var db = DBContext.CreateContext())
            {
                db.Set<T>().BulkUpdate(entity);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int BulkInsert(IEnumerable<T> entity)
        {
            using (var db = DBContext.CreateContext())
            {
                db.Set<T>().BulkInsert(entity);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int BulkDelete(IEnumerable<T> entity)
        {
            using (var db = DBContext.CreateContext())
            {
                db.Set<T>().BulkDelete(entity);
                return db.SaveChanges();
            }
        }
        #endregion

        #region 使用EntityFramework.Extended批量处理

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public int BulkDelete(Expression<Func<T, bool>> where = null)
        {
            using (var db = DBContext.CreateContext())
            {
                if (where == null)
                {
                    db.Set<T>().Delete();
                }
                db.Set<T>().Where(where).Delete();

                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public int BulkUpdate(Expression<Func<T, T>> entity, Expression<Func<T, bool>> where = null)
        {
            using (var db = DBContext.CreateContext())
            {
                if (where == null)
                {
                    db.Set<T>().Update(entity);
                }
                db.Set<T>().Where(where).Update(entity);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public int BulkInsert2(IEnumerable<T> entity)
        {
            using (var db = DBContext.CreateContext())
            {
                db.Set<T>().AddRange(entity);
                return db.SaveChanges();
            }
        }
        #endregion

    }
}
