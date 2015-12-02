using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Jagi.Mapping
{
    /// <summary>
    /// 可以直接使用，或者使用以下方式客製 Converter: 
    ///     class MapperConfig : MappingBaseConfig
    ///     {
    ///         public MapperConfig(Assembly assembly) : base(assembly)
    ///         {
    ///     
    ///         }
    ///     
    ///         public override void ConverterSettings()
    ///         {
    ///             Mapper.CreateMap<DateTime, string>().ConvertUsing<DateTimeToString>();
    ///     
    ///             base.ConverterSettings();
    ///         }
    ///     }
    /// </summary>
    public class MappingBaseConfig
    {
        private readonly Assembly _assembly;

        /// <summary>
        /// 傳入執行的 Assembly 呼叫方式：
        ///    Assembly assembly = Assembly.GetExecutingAssembly();
        ///    MappingBaseConfig config = new MappingBaseConfig(assembly);
        /// </summary>
        /// <param name="assembly"></param>
        public MappingBaseConfig(Assembly assembly)
        {
            _assembly = assembly;
        }

        /// <summary>
        /// 提供基本的設定： string to decimal?, string to DateTime? and int? to string
        /// 如果有需要其他的 Converter 可以 Override 此函數，自行設定。
        /// </summary>
        public virtual void ConverterSettings()
        {
            Mapper.CreateMap<string, decimal?>().ConvertUsing<StringToDecimalNull>();
            Mapper.CreateMap<string, DateTime?>().ConvertUsing<StringToDateTimeNull>();
            Mapper.CreateMap<int?, string>().ConvertUsing<NullableIntToString>();
            Mapper.CreateMap<int, string>().ConvertUsing<IntToString>();
            Mapper.CreateMap<string, int>().ConvertUsing<StringToInt>();
            Mapper.CreateMap<DateTime?, string>().ConvertUsing<DateTimeNullToString>();
            Mapper.CreateMap<DateTime, string>().ConvertUsing<DateTimeToString>();
        }

        /// <summary>
        /// 執行 AutoMapper 的設定，讓 IMapFrom & IMapCustomized 可以使用
        /// 請注意，如果有需要設定 Converter，可以 Override ConverterSetting()
        /// </summary>
        public virtual void Execute()
        {
            ConverterSettings();
            Type[] types;
            if (_assembly == null)
                types = Assembly.GetExecutingAssembly().GetExportedTypes();
            else
                types = _assembly.GetExportedTypes();

            LoadStarndardMappings(types);
            LoadCustomMappings(types);
        }

        private void LoadCustomMappings(IEnumerable<Type> types)
        {
            var maps = (from t in types
                        from i in t.GetInterfaces()
                        where typeof(IMapFromCustomized).IsAssignableFrom(t) &&
                            !t.IsAbstract && !t.IsInterface
                        select (IMapFromCustomized)Activator.CreateInstance(t)).ToArray();

            foreach (var map in maps)
                map.CreateMappings(Mapper.Configuration);
        }

        private void LoadStarndardMappings(IEnumerable<Type> types)
        {
            var maps = (from t in types
                        from i in t.GetInterfaces()
                        where i.IsGenericType &&
                              i.GetGenericTypeDefinition() == typeof(IMapFrom<>) &&
                              !t.IsAbstract && !t.IsInterface
                        select new
                        {
                            Source = i.GetGenericArguments()[0],
                            Destination = t
                        }).ToArray();
            foreach (var map in maps)
                Mapper.CreateMap(map.Source, map.Destination);
        }
    }
}
