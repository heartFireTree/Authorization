using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyTest.Extension
{
    public static class ModelExtension
    {
        /// <summary>
        /// 反射实现两个类的对象之间相同属性的值的复制
        /// </summary>
        /// <typeparam name="Tag">返回的实体</typeparam>
        /// <typeparam name="Source">数据源实体</typeparam>
        /// <param name="tag">目标实体</param>
        /// <param name="sourct">数据源实体</param>
        /// <param name="ignoreId">忽略ID字段</param>
        /// <param name="ignoreEmpty">忽略空字段</param>
        /// <returns></returns>
        public static void CopyTo<Source, Tag>(this Source sourct, Tag tag, bool ignoreId = true, bool ignoreEmpty = false)
        {
            var Types = sourct.GetType();
            var TypeTag = typeof(Tag);
            foreach (PropertyInfo source in Types.GetProperties())
            {
                foreach (PropertyInfo tagProp in TypeTag.GetProperties())
                {
                    if (!(ignoreId && tagProp.Name.Equals("id", StringComparison.OrdinalIgnoreCase)) && tagProp.Name.Equals(source.Name, StringComparison.OrdinalIgnoreCase) && tagProp.PropertyType == source.PropertyType)
                    {
                        if (tagProp.CanWrite && source.GetValue(sourct) != null)
                        {
                            tagProp.SetValue(tag, source.GetValue(sourct, null), null);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 对象转URI参数
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToUriParam(this object obj)
        {
            PropertyInfo[] propertis = obj.GetType().GetProperties();
            StringBuilder sb = new StringBuilder();
            foreach (var p in propertis)
            {
                var v = p.GetValue(obj, null);
                if (v == null)
                    continue;

                sb.Append(p.Name);
                sb.Append("=");
                sb.Append(HttpUtility.UrlEncode(v.ToString()));
                sb.Append("&");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }
}
