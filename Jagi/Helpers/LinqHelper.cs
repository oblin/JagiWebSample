using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;

namespace Jagi.Helpers
{
    public static class LinqHelper
    {
        /// <summary>
        /// 檢測 IQueryable 物件中，傳回特定的欄位 {propertyName} 值內含 {keyword} 值的條件的物件
        /// 請注意，欄位必須要是字串，此功能相當於 samples.Where(p => p.Text.Contains(keyword))
        /// 但主要用途在於可以指定 propertyName，對於動態需求比較好用
        /// </summary>
        /// <typeparam name="T">必須要是IQuerable的物件</typeparam>
        /// <param name="source">必須要是繼承 IQuerable 的相關陣列</param>
        /// <param name="propertyName">物件 T 中的指定欄位名稱</param>
        /// <param name="keyword">符合指定欄位名稱的值，必須要是字串</param>
        /// <param name="ignoreCase">是否忽略大小寫，預設 true = 忽略</param>
        /// <returns></returns>
        public static IQueryable<T> Has<T>(this IQueryable<T> source, string propertyName, string keyword, bool ignoreCase = true)
        {
            if (source == null || string.IsNullOrEmpty(propertyName) || string.IsNullOrEmpty(keyword))
            {
                return source;
            }
            if (ignoreCase)
                keyword = keyword.ToLower();

            var parameter = Expression.Parameter(source.ElementType, String.Empty);
            var property = Expression.Property(parameter, propertyName);

            var CONTAINS_METHOD = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var TO_LOWER_METHOD = typeof(string).GetMethod("ToLower", new Type[] { });

            var toLowerExpression = Expression.Call(property, TO_LOWER_METHOD);
            
            var termConstant = Expression.Constant(keyword, typeof(string));

            MethodCallExpression containsExpression = GetMethodCallExpression(ignoreCase, property, CONTAINS_METHOD, toLowerExpression, termConstant);

            var predicate = Expression.Lambda<Func<T, bool>>(containsExpression, parameter);

            var methodCallExpression = Expression.Call(typeof(Queryable), "Where",
                                        new Type[] { source.ElementType },
                                        source.Expression, Expression.Quote(predicate));

            return source.Provider.CreateQuery<T>(methodCallExpression);
        }

        /// <summary>
        /// 檢測任意陣列物件中，傳回特定的欄位 {propertyName} 值內含 {keyword} 值的條件的物件
        /// 請注意，欄位必須要是字串，此功能相當於 samples.Where(p => p.Text.Contains(keyword))
        /// 但主要用途在於可以指定 propertyName，對於動態需求比較好用
        /// </summary>
        /// <typeparam name="T">任意物件 T</typeparam>
        /// <param name="source">可以是 List, IEnumerable...</param>
        /// <param name="propertyName">物件 T 中的指定欄位名稱</param>
        /// <param name="keyword">符合指定欄位名稱的值，必須要是字串</param>
        /// <param name="ignoreCase">是否忽略大小寫，預設 true = 忽略</param>
        /// <returns></returns>
        public static IEnumerable<T> Has<T>(this IEnumerable<T> source, string propertyName, string keyword, bool ignoreCase = true)
        {
            return source.AsQueryable().Has<T>(propertyName, keyword, ignoreCase);
        }

        /// <summary>
        /// 檢測任意陣列物件中，傳回特定的欄位 {propertyName} 值等於 {keyword} 值的條件的物件
        /// 請注意，欄位必須要是字串，此功能相當於 samples.Where(p => p.Text == keyword)
        /// 但主要用途在於可以指定 propertyName，對於動態需求比較好用
        /// 例外狀況：
        ///     1. Nullable 物件尚未處理
        ///     2. 不可以比較 Null value
        /// </summary>
        /// <typeparam name="T">任意物件 T</typeparam>
        /// <param name="source">可以是 List, IEnumerable...</param>
        /// <param name="propertyName">物件 T 中的指定欄位名稱</param>
        /// <param name="keyword">符合指定欄位名稱的值，必須要是 primitive type: string, int, bool...</param>
        /// <param name="ignoreCase">是否忽略大小寫，預設 true = 忽略</param>
        /// <returns></returns>
        public static IEnumerable<T> Equal<T>(this IEnumerable<T> source, string propertyName, object keyword, bool ignoreCase = true, bool isNullable = false)
        {
            if (!(keyword is IConvertible))
                throw new ArgumentOutOfRangeException("傳入型別必須要是 primitives Type!");

            if (isNullable)
                return NullableEqual<T>(source.AsQueryable(), propertyName, keyword, keyword.GetType(), ignoreCase);
            else
                return Equal<T>(source.AsQueryable(), propertyName, keyword, keyword.GetType(), ignoreCase);
        }

        private static IEnumerable<T> NullableEqual<T>(IQueryable<T> queryable, string propertyName, object keyword, Type type, bool ignoreCase)
        {
            throw new NotImplementedException();
        }

        public static IQueryable<T> Equal<T>(this IQueryable<T> source, string propertyName, object key, Type type, bool ignoreCase = true)
        {
            if (source == null || string.IsNullOrEmpty(propertyName))
            {
                return source;
            }

            if (type == null)
                type = typeof(string);

            var keyword = ConvertToType(key, type, ignoreCase);

            var parameter = Expression.Parameter(source.ElementType, String.Empty);
            var property = Expression.Property(parameter, propertyName);

            var EQUAL_METHOD = type.GetMethod("Equals", new[] { type });

            var termConstant = Expression.Constant(keyword, type);

            MethodCallExpression equalExpression;
            if (type == typeof(string))
            {
                var TO_LOWER_METHOD = typeof(string).GetMethod("ToLower", new Type[] { });
                var toLowerExpression = Expression.Call(property, TO_LOWER_METHOD);
                equalExpression = GetMethodCallExpression(ignoreCase, property, EQUAL_METHOD, toLowerExpression, termConstant);
            }
            else
            {
                equalExpression = Expression.Call(property, EQUAL_METHOD, termConstant);
            }


            var predicate = Expression.Lambda<Func<T, bool>>(equalExpression, parameter);

            var methodCallExpression = Expression.Call(typeof(Queryable), "Where",
                                        new Type[] { source.ElementType },
                                        source.Expression, Expression.Quote(predicate));

            return source.Provider.CreateQuery<T>(methodCallExpression);
        }

        /// <summary>
        /// 大於等於，使用於日期區間
        /// </summary>
        /// <typeparam name="T">任意物件 T</typeparam>
        /// <param name="source">可以是 List, IEnumerable...</param>
        /// <param name="propertyName">物件 T 中的指定欄位名稱</param>
        /// <param name="keyword">特定的日期區間，若此資料為NULL，則不允以計算</param>
        /// <returns></returns>
        public static IEnumerable<T> DateGreaterThanOrEqual<T>(this IEnumerable<T> source, string propertyName, DateTime? keyword)
        {
            return source.AsQueryable().DateGreaterThanOrEqual<T>(propertyName, keyword);
        }

        /// <summary>
        /// 小於等於，使用於日期區間
        /// </summary>
        /// <typeparam name="T">任意物件 T</typeparam>
        /// <param name="source">可以是 List, IEnumerable...</param>
        /// <param name="propertyName">物件 T 中的指定欄位名稱</param>
        /// <param name="keyword">特定的日期區間，若此資料為NULL，則不允以計算</param>
        /// <returns></returns>
        public static IEnumerable<T> DateLessThanOrEqual<T>(this IEnumerable<T> source, string propertyName, DateTime? keyword)
        {
            return source.AsQueryable().DateLessThanOrEqual<T>(propertyName, keyword);
        }
        
        /// <summary>
        /// 大於等於，使用於日期區間
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        /// <param name="keyword">特定的日期區間，若此資料為NULL，則不允以計算</param>
        /// <returns></returns>
        public static IQueryable<T> DateGreaterThanOrEqual<T>(this IQueryable<T> source, string propertyName, DateTime? keyword)
        {
            if (source == null || string.IsNullOrEmpty(propertyName))
            {
                return source;
            }

            if (keyword == null)
                return source;

            var parameter = Expression.Parameter(source.ElementType, String.Empty);
            var property = Expression.Property(parameter, propertyName);

            var termConstant = Expression.Constant((DateTime)keyword, typeof(DateTime));
            var notEqualExpression = Expression.GreaterThanOrEqual(property, termConstant);

            var predicate = Expression.Lambda<Func<T, bool>>(notEqualExpression, parameter);

            var methodCallExpression = Expression.Call(typeof(Queryable), "Where",
                                        new Type[] { source.ElementType },
                                        source.Expression, Expression.Quote(predicate));

            return source.Provider.CreateQuery<T>(methodCallExpression);
        }

        /// <summary>
        /// 小於等於，使用於日期區間
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        /// <param name="keyword">特定的日期區間，若此資料為NULL，則不允以計算</param>
        /// <returns></returns>
        public static IQueryable<T> DateLessThanOrEqual<T>(this IQueryable<T> source, string propertyName, DateTime? keyword)
        {
            if (source == null || string.IsNullOrEmpty(propertyName))
            {
                return source;
            }

            if (keyword == null)
                return source;

            var parameter = Expression.Parameter(source.ElementType, String.Empty);
            var property = Expression.Property(parameter, propertyName);

            var termConstant = Expression.Constant((DateTime)keyword, typeof(DateTime));
            var notEqualExpression = Expression.LessThanOrEqual(property, termConstant);

            var predicate = Expression.Lambda<Func<T, bool>>(notEqualExpression, parameter);

            var methodCallExpression = Expression.Call(typeof(Queryable), "Where",
                                        new Type[] { source.ElementType },
                                        source.Expression, Expression.Quote(predicate));

            return source.Provider.CreateQuery<T>(methodCallExpression);
        }

        /// <summary>
        /// 架構在 TSource 建立一個新的 TDest 陣列，兩個 Type 不需要一樣，會自動判讀欄位名稱複製資料
        /// 使用方式： var result = samples.CopyTo<TSource, TDest>();
        /// </summary>
        /// <typeparam name="TSource">傳入的型態</typeparam>
        /// <typeparam name="TDest">產生的新陣列型態</typeparam>
        /// <param name="source"></param>
        /// <returns>新的陣列</returns>
        public static IEnumerable<TDest> CopyTo<TSource, TDest>(this IEnumerable<TSource> source)
        {
            List<TDest> destinationList = new List<TDest>();
            List<TSource> sourceList = source.ToList<TSource>();

            var sourceType = typeof(TSource);
            var destType = typeof(TDest);
            foreach (TSource sourceElement in sourceList)
            {
                TDest destElement = Activator.CreateInstance<TDest>();
                sourceElement.CopyTo(destElement);
                destinationList.Add(destElement);
            }

            return destinationList;
        }

        /// <summary>
        /// 依據欄位與排序順序，針對任意的 IQueryable or DbSet 項目進行排序
        /// </summary>
        /// <typeparam name="T">任意的型態</typeparam>
        /// <param name="set"></param>
        /// <param name="fieldName">排序的欄位，如果不指定，則使用 T 的第一個欄位；建議一定要指定</param>
        /// <param name="order">只能是 "ASC" or "DESC" 這兩種字串</param>
        /// <returns>傳回 IOrderedQueryable<T> 的型態</returns>
        public static IQueryable<T> OrderByFieldName<T>(this IQueryable<T> set, string fieldName, string order = null)
        {
            order = order ?? "ASC";
            fieldName = fieldName ?? typeof(T).GetProperties().First().Name;

            return set.OrderBy(fieldName + " " + order);
        }

        public static IQueryable<T> OrderByFieldName<T>(this IEnumerable<T> set, string fieldName, string order = null)
        {
            return OrderByFieldName(set.AsQueryable(), fieldName, order);
        }

        /// <summary>
        /// 指定任意欄位名稱，回傳符合 searchKeyword 的結果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="set"></param>
        /// <param name="searchField">欄位名稱</param>
        /// <param name="searchKeyword">符合開頭的文字內容</param>
        /// <returns></returns>
        public static IQueryable<T> StartWithFieldName<T>(this IQueryable<T> set, 
            string searchField, string searchKeyword)
        {
            if (!string.IsNullOrEmpty(searchField) && !string.IsNullOrEmpty(searchKeyword))
            {
                set = set.Where(searchField + ".StartsWith(@0)", searchKeyword);
            }

            return set;
        }

        public static IQueryable<T> StartWithFieldName<T>(this IEnumerable<T> set, 
            string searchField, string searchKeyword)
        {
            return StartWithFieldName(set.AsQueryable(), searchField, searchKeyword);
        }

        private static object ConvertToType(object key, Type type, bool ignoreCase)
        {
            if (type == typeof(string))
            {
                if (ignoreCase)
                {
                    return key.ToString().ToLower();
                }
                else
                    return key.ToString();
            }

            return key;
        }

        private static MethodCallExpression GetMethodCallExpression(bool ignoreCase, MemberExpression property, System.Reflection.MethodInfo Method, MethodCallExpression toLowerExpression, ConstantExpression termConstant)
        {
            MethodCallExpression containsExpression;

            if (ignoreCase)
                containsExpression = Expression.Call(toLowerExpression, Method, termConstant);
            else
                containsExpression = Expression.Call(property, Method, termConstant);
            return containsExpression;
        }
    }
}
