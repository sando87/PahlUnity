using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace PahlUnity
{
    public static class CSVParser<T> where T : new()
    {
        public static T[] Parse(string csv)
        {
            List<List<string>> table = ParseTable(csv);

            if (table.Count <= 1)
                return Array.Empty<T>();

            List<T> result = new List<T>();

            // header
            List<string> header = table[0];

            // field/property cache
            Dictionary<int, MemberInfo> memberMap =
                BuildMemberMap(header);

            // rows
            for (int row = 1; row < table.Count; row++)
            {
                List<string> rowData = table[row];

                // 빈 row skip
                if (IsEmptyRow(rowData))
                    continue;

                T obj = new T();

                foreach (var pair in memberMap)
                {
                    int col = pair.Key;

                    if (col >= rowData.Count)
                        continue;

                    string raw = rowData[col];

                    MemberInfo member = pair.Value;

                    try
                    {
                        SetMemberValue(obj, member, raw);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(
                            $"CSV Parse Error " +
                            $"Type={typeof(T).Name} " +
                            $"Row={row} Col={col} " +
                            $"Value={raw}\n{e}");
                    }
                }

                result.Add(obj);
            }

            return result.ToArray();
        }

        // =========================================================
        // CSV TABLE PARSER
        // =========================================================

        private static List<List<string>> ParseTable(string csv)
        {
            List<List<string>> result = new List<List<string>>();

            List<string> row = new List<string>();
            StringBuilder cell = new StringBuilder();

            bool inQuotes = false;

            for (int i = 0; i < csv.Length; i++)
            {
                char c = csv[i];

                // quote
                if (c == '"')
                {
                    // escaped quote ""
                    if (inQuotes &&
                        i + 1 < csv.Length &&
                        csv[i + 1] == '"')
                    {
                        cell.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }

                    continue;
                }

                // comma
                if (c == ',' && !inQuotes)
                {
                    row.Add(cell.ToString().Trim());
                    cell.Clear();
                    continue;
                }

                // newline
                if ((c == '\n' || c == '\r') && !inQuotes)
                {
                    // \r\n
                    if (c == '\r' &&
                        i + 1 < csv.Length &&
                        csv[i + 1] == '\n')
                    {
                        i++;
                    }

                    row.Add(cell.ToString().Trim());
                    cell.Clear();

                    result.Add(row);

                    row = new List<string>();

                    continue;
                }

                cell.Append(c);
            }

            // last cell
            row.Add(cell.ToString().Trim());

            // last row
            if (row.Count > 0)
            {
                result.Add(row);
            }

            return result;
        }

        // =========================================================
        // MEMBER MAP
        // =========================================================

        private static Dictionary<int, MemberInfo> BuildMemberMap(
            List<string> header)
        {
            Dictionary<int, MemberInfo> map =
                new Dictionary<int, MemberInfo>();

            Type type = typeof(T);

            BindingFlags flags =
                BindingFlags.Public |
                BindingFlags.Instance;

            for (int i = 0; i < header.Count; i++)
            {
                string name = header[i];

                // field 우선
                FieldInfo field = type.GetField(name, flags);

                if (field != null)
                {
                    map[i] = field;
                    continue;
                }

                // property
                PropertyInfo property =
                    type.GetProperty(name, flags);

                if (property != null &&
                    property.CanWrite)
                {
                    map[i] = property;
                }
            }

            return map;
        }

        // =========================================================
        // VALUE SET
        // =========================================================

        private static void SetMemberValue(
            T obj,
            MemberInfo member,
            string raw)
        {
            Type valueType;

            if (member is FieldInfo field)
            {
                valueType = field.FieldType;

                object value = ConvertValue(raw, valueType);

                field.SetValue(obj, value);

                return;
            }

            if (member is PropertyInfo property)
            {
                valueType = property.PropertyType;

                object value = ConvertValue(raw, valueType);

                property.SetValue(obj, value);

                return;
            }
        }

        private static object ConvertValue(string raw, Type type)
        {
            if (type == typeof(string))
                return raw;

            if (string.IsNullOrEmpty(raw))
            {
                if (type.IsValueType)
                    return Activator.CreateInstance(type);

                return null;
            }

            if (type == typeof(int))
            {
                return int.Parse(
                    raw,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture);
            }

            if (type == typeof(float))
            {
                return float.Parse(
                    raw,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture);
            }

            if (type == typeof(double))
            {
                return double.Parse(
                    raw,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture);
            }

            if (type == typeof(decimal))
            {
                return decimal.Parse(
                    raw,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture);
            }

            if (type == typeof(bool))
                return bool.Parse(raw);

            if (type == typeof(long))
            {
                return long.Parse(
                    raw,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture);
            }

            if (type == typeof(short))
            {
                return short.Parse(
                    raw,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture);
            }

            if (type == typeof(byte))
            {
                return byte.Parse(
                    raw,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture);
            }

            if (type.IsEnum)
            {
                return Enum.Parse(type, raw);
            }

            return Convert.ChangeType(
                raw,
                type,
                CultureInfo.InvariantCulture);
        }

        // =========================================================
        // UTIL
        // =========================================================

        private static bool IsEmptyRow(List<string> row)
        {
            for (int i = 0; i < row.Count; i++)
            {
                if (!string.IsNullOrEmpty(row[i]))
                    return false;
            }

            return true;
        }
    }
}