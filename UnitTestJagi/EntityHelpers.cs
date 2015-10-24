using Jagi.Database;
using Jagi.Interface;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestJagi
{
    public static class EntityHelpers
    {
        /// <summary>
        /// 提供給所有 IQueryable 的物件使用
        /// </summary>
        /// <typeparam name="T">任何實作 IQueryable 的 DbSet 均可</typeparam>
        /// <param name="set">資料的提供源</param>
        /// <returns></returns>
        public static DbSet<T> MockDbSet<T>(this IQueryable<T> set)
            where T : class
        {
            var mockSet = Substitute.For<DbSet<T>, IQueryable<T>>();
            ((IQueryable<T>)mockSet).Provider.Returns(set.Provider);
            ((IQueryable<T>)mockSet).Expression.Returns(set.Expression);
            ((IQueryable<T>)mockSet).ElementType.Returns(set.ElementType);
            ((IQueryable<T>)mockSet).GetEnumerator().Returns(set.GetEnumerator());
            mockSet.AsNoTracking().Returns(mockSet);

            return mockSet;
        }

        /// <summary>
        /// 特別提供給實作 Entity abstract type 使用；重點在於可以測試 Find() method 
        /// (透過 set.SingleOrDefault(p => p.Id == id);)
        /// </summary>
        /// <typeparam name="T">任何實作 Entity abstract class 的 IQueryable object</typeparam>
        /// <param name="set">資料的提供源</param>
        /// <returns></returns>
        public static DbSet<T> MockEntityQueryable<T>(this IQueryable<T> set)
            where T : Entity
        {
            var mockSet = Substitute.For<DbSet<T>, IQueryable<T>>();
            ((IQueryable<T>)mockSet).Provider.Returns(set.Provider);
            ((IQueryable<T>)mockSet).Expression.Returns(set.Expression);
            ((IQueryable<T>)mockSet).ElementType.Returns(set.ElementType);
            ((IQueryable<T>)mockSet).GetEnumerator().Returns(set.GetEnumerator());

            mockSet.Find(Arg.Any<int>()).Returns(x =>
            {
                int id = Convert.ToInt32(((object[])(x[0]))[0]);
                if (id == 0)
                    return null;
                var item = set.SingleOrDefault(p => p.Id == id);
                return item;
            });

            return mockSet;
        }

        /// <summary>
        /// 主要是 implement Find() method; 因為 IQueryable 並沒有對應的 Find() method 實做，
        /// 因此使用 SingleOrDefault() 取代
        /// 
        /// 另外注意 Returns 中，使用 Convert.ToInt32(((object[])(x[0]))[0])
        /// 主要原因是 x 是 NSubstitute 的 CallInfo，採用 boxing 方式變成 object array
        /// 因此必須要先轉換成 object array 之後，才能正確取得傳入參數
        /// 
        /// 另外；請注意，因為 GetEnumerator() 只有一次，因此一但使用 ToList() 之後，下次再進行 ToList 會是空值
        /// </summary>
        /// <typeparam name="T">必須要是 Entity，主要原因為實做 Find() 時，使用 Id 進行判斷</typeparam>
        /// <param name="set">必須要是 IDbSet<T>，因此要變更呼叫的型態</param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IDbSet<T> MockDbSet<T>(this IDbSet<T> set, IQueryable<T> data)
            where T : class
        {
            set.Provider.Returns(data.Provider);
            set.Expression.Returns(data.Expression);
            set.ElementType.Returns(data.ElementType);
            set.GetEnumerator().Returns(data.GetEnumerator());
            return set;
        }

        public static DataContext MockDataContext<T>(this DbSet<T> mockSet)
            where T : class
        {
            var mockContext = Substitute.For<DataContext>();
            mockContext.Set<T>().Returns(mockSet);
            return mockContext;
        }

        /// <summary>
        /// 指定兩項 Generic Type
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="set1"></param>
        /// <param name="set2"></param>
        /// <returns></returns>
        public static DataContext SetMockDataContext<T1, T2>(DbSet<T1> set1, DbSet<T2> set2)
            where T1 : class
            where T2 : class
        {
            var mockContext = Substitute.For<DataContext>();
            mockContext.Set<T1>().Returns(set1);
            mockContext.Set<T2>().Returns(set2);
            return mockContext;
        }
    }
}
