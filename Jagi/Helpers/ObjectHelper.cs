using System;
using System.Linq;

namespace Jagi.Helpers
{
    public static class ObjectHelper
    {
        /// <summary>
        /// 由既有的物件欄位 Copy 到新的物件，主要比對欄位名稱，只處理相同的欄位名稱
        /// 屬性若是複雜的資料型態基本上也可以的
        /// 請注意，使用此函數必須要保證 T 可以使用 empty constructor 建立，否則會出錯
        /// 或使用 CopyTo(target) 處理非空的 constructor
        /// </summary>
        /// <typeparam name="T">必須要是 parameterless conscutor</typeparam>
        /// <param name="s">來源物件</param>
        /// <param name="excludeFields">排除的欄位陣列</param>
        /// <returns>新的物件</returns>
        public static T CopyTo<T>(this T s, string[] excludeFields = null)
            where T : new()
        {
            T result = Activator.CreateInstance<T>();
            s.CopyTo(result, excludeFields);
            return result;
        }

        /// <summary>
        /// CopyTo: Use exist object and move values
        /// </summary>
        /// <param name="S">Source object</param>
        /// <param name="T">Target object: 不一定要是新的物件，資料會被覆蓋</param>
        /// <param name="excludeFields">Target object exclude fields</param>
        public static void CopyTo(this object S, object T, string[] excludeFields = null)
        {
            foreach (var pS in S.GetType().GetProperties())
            {
                if (excludeFields != null)
                    if (Array.Exists(excludeFields, p => p.Equals(pS.Name)))
                        continue;

                var property = T.GetType().GetProperties().FirstOrDefault(p => p.Name == pS.Name);

                if (property != null)
                    (property.GetSetMethod()).Invoke(T, new object[] { pS.GetGetMethod().Invoke(S, null) });
            }
        }

        /// <summary>
        /// 由既有的物件欄位 Copy 到新的物件，主要比對欄位名稱，只處理相同的欄位名稱
        /// 屬性若是複雜的資料型態基本上也可以的
        /// 本複製方式會自動忽略 S 屬性是 null or empty 字串，不覆蓋到 T 的欄位內
        /// </summary>
        /// <typeparam name="T">必須要是 parameterless conscutor</typeparam>
        /// <param name="s">來源物件</param>
        /// <param name="excludeFields">排除的欄位陣列</param>
        /// <returns>新的物件</returns>
        public static T CopyToExcludeNull<T>(this T s, string[] excludeFields = null)
            where T : new()
        {
            T result = Activator.CreateInstance<T>();
            s.CopyToExcludeNull(result, excludeFields);
            return result;
        }

        /// <summary>
        /// 將資料複製由 S 到 T，用法： S.CopyToExcludeNull(T)
        /// 本複製方式會自動忽略 S 屬性是 null or empty 字串，不覆蓋到 T 的欄位內
        /// </summary>
        /// <param name="S">Source object</param>
        /// <param name="T">Target object: 不一定要是新的物件，資料會被覆蓋</param>
        /// <param name="excludeFields">Target object exclude fields</param>
        public static void CopyToExcludeNull(this object S, object T, string[] excludeFields = null)
        {
            foreach (var pS in S.GetType().GetProperties())
            {
                if (excludeFields != null)
                    if (Array.Exists(excludeFields, p => p.Equals(pS.Name)))
                        continue;

                object value = pS.GetValue(S);
                if (value == null || string.IsNullOrEmpty(value.ToString()))
                    continue;

                var property = T.GetType().GetProperties().FirstOrDefault(p => p.Name == pS.Name);

                if (property != null)
                    (property.GetSetMethod()).Invoke(T, new object[] { pS.GetGetMethod().Invoke(S, null) });
            }
        }
    }
}
