using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Aspose.Cells;
using GPLX.Infrastructure.Constants;

namespace GPLX.Infrastructure.Extensions
{
    public static class TypeConverterExtension
    {

        public static DateTime ToDateTime(this string value)
            => DateTime.TryParse(value, out var result) ? result : default;

        public static DateTime ToDateTime(this string value, string format, CultureInfo culture)
            => DateTime.TryParseExact(value, format, culture, DateTimeStyles.None, out var result) ? result : default;
        public static DateTime ToDateTime(this string value, string format, CultureInfo culture, DateTime def)
            => DateTime.TryParseExact(value, format, culture, DateTimeStyles.None, out var result) ? result : def;

        public static DateTime? ToNullableDateTime(this string value)
              => !string.IsNullOrEmpty((value ?? "").Trim()) ? value.ToDateTime() : (DateTime?)null;

        public static short ToInt16(this string value)
            => short.TryParse(value, out var result) ? result : default;

        public static int ToInt32(this string value)
            => int.TryParse(value, out var result) ? result : default;

        public static int? ToNullableInt32(this string value)
            => !string.IsNullOrEmpty(value) ? value.ToInt32() : (int?)null;

        public static long ToInt64(this string value)
            => long.TryParse(value, out var result) ? result : default;

        public static long ToInt64Money(this string value)
            => long.TryParse((value ?? "0").Replace(",", "").Replace(".", ""), out var result) ? result > 0 ? result : 0 : default;

        public static long? ToNullableInt64(this string value)
        => !string.IsNullOrEmpty(value) ? value.ToInt64() : (long?)null;

        public static bool ToBoolean(this string value)
            => bool.TryParse(value, out var result) ? result : default;

        public static float ToFloat(this string value)
            => float.TryParse(value, out var result) ? result : default;

        public static decimal ToDecimal(this string value)
            => decimal.TryParse(value, out var result) ? result : default;

        public static double ToDouble(this string value)
           => double.TryParse(value, out var result) ? result : default;

        public static long ToLong(this string value)
        {
            string reValue = value;
            if (string.IsNullOrEmpty(reValue))
                return 0;
            var rgxDots = new Regex("[^0-9]");
            reValue = rgxDots.Replace(reValue, string.Empty);
            return long.TryParse(reValue, out var result) ? result : default;
        }
        public static double ToDouble(this object value)
            => value != null ? double.TryParse(value.ToString(), out var result) ? result : default : 0;

        public static bool IsNumber(this string value)
            => Regex.IsMatch(value, @"^\d+$");

        public static bool IsWholeNumber(this string value)
            => long.TryParse(value, out _);

        public static bool IsDecimalNumber(this string value)
         => decimal.TryParse(value, out _);

        public static bool IsBoolean(this string value)
           => bool.TryParse(value, out var _);

        public static List<int> StringToListInt(this string value, string separator)
        {
            var response = new List<int>();
            if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(separator))
                return response;
            var dataSplit = value.Split(separator);
            foreach (var s in dataSplit)
                response.Add(s.ToInt32());
            return response;
        }


        public static string StringToNonUnicode(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            string[] unicodeChars = { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
                "đ",
                "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
                "í","ì","ỉ","ĩ","ị",
                "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
                "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
                "ý","ỳ","ỷ","ỹ","ỵ",};
            string[] normalizeChars = { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
                "d",
                "e","e","e","e","e","e","e","e","e","e","e",
                "i","i","i","i","i",
                "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
                "u","u","u","u","u","u","u","u","u","u","u",
                "y","y","y","y","y",};
            for (int i = 0; i < unicodeChars.Length; i++)
            {
                text = text.Replace(unicodeChars[i], normalizeChars[i]);
                text = text.Replace(unicodeChars[i].ToUpper(), normalizeChars[i].ToUpper());
            }
            return text;
        }

        public static string StringRemoveSpecial(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            var rgxNonSpecial = new Regex("[^a-zA-Z0-9\\s_-]", RegexOptions.Multiline);
            var normalize = rgxNonSpecial.Replace(text, string.Empty);
            normalize = Regex.Replace(normalize, @"\t|\n|\r", "");
            return normalize;
        }

        public static string StringAesFromParams(this string text) => text.Replace("-", "+");
        public static Tuple<bool, string> ValidatorCell(this ExcelFormValidator validator, object value, bool isHeader = false, object rowParse = null, Type parseType = null, Cell cellVal = null)
        {
            try
            {
                bool isValidCell = true;
                string msgValidate = string.Empty;
                object parseValue = null;
                if (validator != null)
                {
                    if (isHeader)
                    {
                        string colName = (string)value;
                        //if (!string.Equals(validator.Name, colName, StringComparison.CurrentCultureIgnoreCase))
                        if (!colName.StartsWith(validator.Name, StringComparison.CurrentCultureIgnoreCase))
                            return new Tuple<bool, string>(false, $"Cột dữ liệu không đúng <require: {validator.Name}>");
                        return new Tuple<bool, string>(true, string.Empty);
                    }

                    switch (validator.Type)
                    {
                        case "string":
                            string valueString = (string)value;
                            parseValue = valueString;
                            if (validator.Required && !string.IsNullOrEmpty(valueString))
                            {
                                if (!string.IsNullOrEmpty(validator.Format))
                                {
                                    isValidCell = new Regex(validator.Format, RegexOptions.IgnoreCase).IsMatch(valueString);
                                    msgValidate = !isValidCell ? $"Dữ liệu không đúng định dạng: <{validator.Format}>" : string.Empty;

                                }
                            }
                            else if (validator.Required && string.IsNullOrEmpty(valueString))
                            {
                                isValidCell = false;
                                msgValidate = "Ô dữ liệu bắt buộc nhập";
                            }
                            break;
                        case "int":
                            string valueNumber = (string)value;
                            if (validator.Required && string.IsNullOrEmpty(valueNumber))
                            {
                                isValidCell = false;
                                msgValidate = "Ô dữ liệu bắt buộc nhập";
                            }
                            else if (validator.Required && !string.IsNullOrEmpty(valueNumber))
                            {
                                if (!string.IsNullOrEmpty(validator.Format) &&
                                    !new Regex(validator.Format).IsMatch(validator.Format))
                                {
                                    isValidCell = false;
                                    msgValidate = $"Dữ liệu không đúng định dạng: <{validator.Format}>";
                                }
                                else
                                {
                                    int valueParse = valueNumber.ToInt32();
                                    parseValue = valueParse;
                                    if (valueParse < validator.Min && !validator.NoMin)
                                    {
                                        isValidCell = false;
                                        msgValidate = $"Dữ liệu nhập vào không hợp lệ <min: {validator.Min}>";
                                    }
                                }
                            }


                            break;
                        case "float":
                            string floatNumber = (string)value;
                            if (validator.Required && string.IsNullOrEmpty(floatNumber))
                            {
                                isValidCell = false;
                                msgValidate = "Ô dữ liệu bắt buộc nhập";
                            }
                            else if (validator.Required && !string.IsNullOrEmpty(floatNumber))
                            {
                                if (!string.IsNullOrEmpty(validator.Format) &&
                                    !new Regex(validator.Format).IsMatch(validator.Format))
                                {
                                    isValidCell = false;
                                    msgValidate = $"Dữ liệu không đúng định dạng: <{validator.Format}>";
                                }
                                else
                                {
                                    float floatParse = floatNumber.ToFloat();
                                    parseValue = floatParse;
                                    if (floatParse < validator.Min && !validator.NoMin)
                                    {
                                        isValidCell = false;
                                        msgValidate = $"Dữ liệu nhập vào không hợp lệ <min: {validator.Min}>";
                                    }
                                }
                            }
                            break;
                        case "date":
                            string valueDate = (string)value;
                            if (validator.Required && string.IsNullOrEmpty(valueDate))
                            {
                                isValidCell = false;
                                msgValidate = "Ô dữ liệu bắt buộc nhập";
                            }
                            else if (validator.Required && !string.IsNullOrEmpty(valueDate))
                            {
                                if (string.IsNullOrEmpty(validator.Format))
                                {
                                    isValidCell = false;
                                    msgValidate = $"Trường dữ liệu chưa được cấu hình: <{validator.FieldNameMapper}>";
                                }
                                else
                                {
                                    try
                                    {
                                        if (cellVal != null && cellVal.DateTimeValue != DateTime.MinValue)
                                            parseValue = cellVal.DateTimeValue;
                                        else
                                        {
                                            var dataAs = DateTime.MinValue;
                                            if (cellVal != null)
                                                dataAs = (DateTime)cellVal.Value;
                                            if (dataAs != DateTime.MinValue)
                                                parseValue = dataAs;
                                            else
                                            {
                                                DateTime dateParse = valueDate.ToDateTime(validator.Format, new CultureInfo(!string.IsNullOrEmpty(validator.Culture) ? validator.Culture : "en-US"), DateTime.MinValue);
                                                parseValue = dateParse;
                                                if (dateParse == DateTime.MinValue)
                                                {
                                                    isValidCell = false;
                                                    msgValidate = $"Dữ liệu nhập vào không hợp lệ <{validator.Format}>";
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        DateTime dateParse = valueDate.ToDateTime(validator.Format, new CultureInfo(!string.IsNullOrEmpty(validator.Culture) ? validator.Culture : "en-US"), DateTime.MinValue);
                                        parseValue = dateParse;
                                        if (dateParse == DateTime.MinValue)
                                        {
                                            isValidCell = false;
                                            msgValidate = $"Dữ liệu nhập vào không hợp lệ <{validator.Format}>";
                                        }
                                    }
                                }
                            }
                            break;
                        default:
                            isValidCell = false;
                            msgValidate = $"Kiểu dữ liệu không được hỗ trợ <{value}>";
                            break;
                    }

                    if (isValidCell && parseValue != null && rowParse != null && parseType != null)
                    {
                        if (!string.IsNullOrEmpty(validator.FieldNameMapper))
                        {
                            var field = parseType.GetProperty(validator.FieldNameMapper, BindingFlags.Instance | BindingFlags.Public);
                            if (field != null)
                            {
                                Type t = Nullable.GetUnderlyingType(field.PropertyType) ?? field.PropertyType;
                                object safeValue = Convert.ChangeType(parseValue, t);
                                field.SetValue(rowParse, safeValue);
                            }
                        }
                    }
                }
                else
                {
                    isValidCell = false;
                    msgValidate = "Cột dữ liệu chưa được cấu hình";
                }
                return new Tuple<bool, string>(isValidCell, msgValidate);
            }
            catch (Exception)
            {
                return new Tuple<bool, string>(false, "Dữ liệu không đúng định dạng");
            }
        }

        public static string CreateHostFileView(this string value, string host, string paramValues = null)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            var rgxDuplicateSlash = new Regex("(/)+");
            string fileHost = $"{host}/{rgxDuplicateSlash.Replace(value.Replace("\\", "/"), "/")}{(!string.IsNullOrEmpty(paramValues) ? $"?{paramValues}" : "")}";
            return fileHost;
        }


        public static string CreateDatePhysicalPathFileToUser(this string user, string subFolder = null)
        {
            var rgxNonSpecial = new Regex("[^a-zA-Z0-9]");
            var u = user.StringToNonUnicode().ToLower().Replace(" ", string.Empty);
            u = rgxNonSpecial.Replace(u, string.Empty);
            u = Regex.Replace(u, @"\t|\n|\r", "");
            return (!string.IsNullOrEmpty(subFolder) ?
                Path.Combine($"{u}", DateTime.Now.ToString("yyyyMMdd"), subFolder) :
                Path.Combine($"{u}", DateTime.Now.ToString("yyyyMMdd"))).Replace("\\", "/");
        }

        public static string CreateEstimateRequestCode(this int value, string unitCode)
        {
            string prefix = "000000";
            string costCodeFormat = "{0}-{1}{2}";
            string stringOfTotal = value.ToString();
            prefix = stringOfTotal.Length < prefix.Length ? prefix.Substring(0, prefix.Length - stringOfTotal.Length) : string.Empty;
            return string.Format(costCodeFormat, unitCode, prefix, stringOfTotal);
        }

        public static string NormalizePath(this string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            var rgxDuplicateSlash = new Regex("(/)+");
            return rgxDuplicateSlash.Replace(value.Replace("\\", "/"), "/");
        }

        public static string CreateAbsolutePath(this string value, string pathPhysical)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            return $"{pathPhysical}{value}";
        }


        public static bool IsLastDayOfYear(this int year)
        {
            // ngày cuối của năm trước
            var newDate = new DateTime(year, 1, 1).AddDays(-1);
            var now = DateTime.Now;
            if (newDate.Day == now.Day && newDate.Month == now.Month)
                return true;
            return false;
        }
    }
}
