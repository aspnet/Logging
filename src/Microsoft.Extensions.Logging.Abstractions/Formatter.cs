using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Reflection;

namespace Microsoft.Extensions.Logging
{
    public static class Formatter
    {
        public static string PrintData(this object data)
        {
            StringBuilder builder = new StringBuilder();
            data.PrintData(builder);
            return builder.ToString();
        }
        public static void PrintData(this object data, StringBuilder builder)
        {
            if (data == null)
                return;
            if (data is IEnumerable<KeyValuePair<string, object>>)
            {
                var first = true;
                foreach (var kvp in (IEnumerable<KeyValuePair<string, object>>)data)
                {
                    if (!first) { builder.Append(" "); first = false; }
                    builder.Append(kvp.Key);
                    builder.Append(": ");
                    PrintData(kvp.Value, builder);
                }
            }
            else if (data is IEnumerable && !(data is String)) // String also implements IEnumerable
            {
                var list = data as IEnumerable;
                if (list != null)
                {
                    var first = true;
                    foreach (var elem in list)
                    {
                        if (!first) { builder.Append(", "); first = false; }
                        PrintData(elem, builder);
                    }
                }
            }
            else if (data is KeyValuePair<string, object>)
            {
                var kvp = (KeyValuePair<string, object>)data;
                builder.Append(kvp.Key);
                builder.Append(": ");
                PrintData(kvp.Value, builder);
            }
            else if (data.GetType().IsAnonymousType())
            {
                Type t = data.GetType();

                // Get a list of the properties
                IEnumerable<PropertyInfo> pList = t.GetTypeInfo().DeclaredProperties;

                var first = true;
                // Loop through the properties in the list
                foreach (PropertyInfo pi in pList)
                {
                    if (!first) { builder.Append(" "); first = false; }

                    // Get the value of the property
                    object o = pi.GetValue(data, null);
                    // Write out the property information
                    builder.Append(pi.Name);
                    builder.Append(": ");
                    PrintData(o, builder);
                }
            }
            else
            {
                builder.Append(data.ToString());
            }
        }
    }

    public static class TypeExtension
    {

        public static Boolean IsAnonymousType(this Type type)
        {
            //TODO: Fix this method...is this correct?
            //Boolean hasCompilerGeneratedAttribute = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Count() > 0;
            Boolean nameContainsAnonymousType = type.FullName.Contains("AnonymousType");

            return nameContainsAnonymousType;
        }
    }

}
