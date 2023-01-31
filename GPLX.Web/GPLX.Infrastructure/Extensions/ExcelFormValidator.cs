using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Aspose.Cells;
using GPLX.Core.DTO.Entities;
using GPLX.Core.DTO.Response;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace GPLX.Infrastructure.Extensions
{
    public class ExcelFormValidator
    {
        public int Position { get; set; }
        public string Name { get; set; }
        public bool Required { get; set; }
        public string Type { get; set; }
        public string Format { get; set; }
        public int Min { get; set; }
        public bool NoMin { get; set; }

        public string FieldNameMapper { get; set; }
        public string Culture { get; set; }
        public string Replace { get; set; }
    }

    public class ExcelFormValidation
    {

        private const string RuleRequire = "require";
        private const string RuleMin = "min";
        private const string RuleMax = "max";
        private const string RuleEqual = "equals";
        private const string RuleContains = "contains";
        private const string RuleStartWith = "startwith";

        private readonly Regex RuleValue = new Regex("(\\()+([0-9a-zA-Z-_=+])+(\\))");

        public bool ReadWithoutValidColumnName { get; set; }
        public int StartAtRow { get; set; }
        public BreakInfo BreakInfo { get; set; }
        public List<ExcelFormValidator> ColumnConfigs { get; set; }
        private IConfiguration Configuration { get; set; }

        public ExcelFormValidation(IConfiguration configuration)
        {
            Configuration = configuration;
            Data = new List<object>();
            Errors = new List<ExcelValidatorError>();
        }

        /// <summary>
        /// Do not remove
        /// </summary>
        public ExcelFormValidation()
        {
            Data = new List<object>();
            Errors = new List<ExcelValidatorError>();
        }

        /// <summary>
        /// Cấu hình những file bắt đầu là header dòng đầu tiên
        /// </summary>
        /// <param name="tag"></param>
        public void LoadFromConfig(string tag)
        {
            var fromConfig = Configuration.GetSection(tag).Get<ExcelFormValidator[]>();
            ColumnConfigs = fromConfig.ToList();
        }

        /// <summary>
        /// Load các file có cấu trúc khác
        /// </summary>
        public ExcelFormValidation Load(string tag)
        {
            var configuration = Configuration.GetSection(tag).Get<ExcelFormValidation>();
            configuration.Configuration = Configuration;
            return configuration;
        }

        public Tuple<bool, string> ValidCell(ExcelFormValidator validator, object value, bool isHeader = false, object rowParse = null, Type parseType = null, Cell cellVal = null)
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
                        var columnNames = validator.Name.Split(';');
                        if (!columnNames.Any(x => colName.StartsWith(x, StringComparison.CurrentCultureIgnoreCase) && !string.IsNullOrEmpty(x)))
                            return new Tuple<bool, string>(false, $"Cột dữ liệu không đúng <b>{validator.Name}</b>");
                        return new Tuple<bool, string>(true, string.Empty);
                    }

                    switch (validator.Type)
                    {
                        case "string":
                            string valueString = (string)value;
                            parseValue = valueString;
                            if (!string.IsNullOrEmpty(valueString))
                            {
                                if (!string.IsNullOrEmpty(validator.Format))
                                {
                                    isValidCell = new Regex(validator.Format, RegexOptions.IgnoreCase).IsMatch(valueString);
                                    msgValidate = !isValidCell ? $"Dữ liệu không đúng định dạng: <b>{validator.Format}</b>" : string.Empty;

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
                            try
                            {
                                if (cellVal?.Value != null && cellVal.Type == CellValueType.IsNumeric)
                                    valueNumber = cellVal.IntValue.ToString("N0");
                            }
                            catch (Exception e)
                            {
                                Log.Error(e, "get cell Int {0}", e.Message);
                            }
                            if (validator.Required && string.IsNullOrEmpty(valueNumber))
                            {
                                isValidCell = false;
                                msgValidate = "Ô dữ liệu bắt buộc nhập";
                            }
                            else if (!string.IsNullOrEmpty(valueNumber))
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
                                        msgValidate = $"Dữ liệu nhập vào không hợp lệ <b>{validator.Min}</b>";
                                    }
                                }
                            }


                            break;
                        case "float":
                            string floatNumber = (string)value;
                            bool isNegative = false;
                            bool isGetNatureValue = false;
                            try
                            {
                                if (cellVal.Value != null && cellVal.Type == CellValueType.IsNumeric)
                                {
                                    double fVal = cellVal.DoubleValue;
                                    if (floatNumber.EndsWith("%"))
                                        fVal = Math.Round(fVal * 100, MidpointRounding.AwayFromZero);

                                    floatNumber = fVal.ToString("N");
                                    // kiểm tra giá trị âm
                                    isNegative = fVal < 0;
                                    isGetNatureValue = true;
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Warning(e, "get cell Float {0}", e.Message);
                                isGetNatureValue = false;
                            }

                            // trường hợp custom format của excel
                            // giá trị = 0
                            if (floatNumber.Trim() == "-")
                                floatNumber = "0";
                            if (floatNumber.StartsWith("(") && !isNegative)
                                isNegative = true;


                            if (!string.IsNullOrEmpty(validator.Replace))
                            {
                                var rplRegex = new Regex(validator.Replace);
                                floatNumber = rplRegex.Replace(floatNumber, string.Empty);
                            }

                            if (validator.Required && string.IsNullOrEmpty(floatNumber))
                            {
                                isValidCell = false;
                                msgValidate = "Ô dữ liệu bắt buộc nhập";
                            }
                            else if (!string.IsNullOrEmpty(floatNumber))
                            {
                                if (!string.IsNullOrEmpty(validator.Format) && !new Regex(validator.Format).IsMatch(floatNumber))
                                {
                                    isValidCell = false;
                                    msgValidate = $"Dữ liệu không đúng định dạng: <b>{validator.Format}</b>";
                                }
                                else
                                {
                                    double floatParse = floatNumber.ToDouble();
                                    if (isNegative && !isGetNatureValue)
                                        floatParse = floatParse * -1;
                                    floatParse = Math.Round(floatParse, MidpointRounding.AwayFromZero);
                                    parseValue = floatParse;
                                    if (floatParse < validator.Min && !validator.NoMin)
                                    {
                                        isValidCell = false;
                                        msgValidate = $"Dữ liệu nhập vào không hợp lệ <b> {validator.Min}</b>";
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
                            else if (!string.IsNullOrEmpty(valueDate))
                            {
                                if (string.IsNullOrEmpty(validator.Format))
                                {
                                    isValidCell = false;
                                    msgValidate = $"Trường dữ liệu chưa được cấu hình: <b>{validator.FieldNameMapper}</b>";
                                }
                                else
                                {
                                    try
                                    {
                                        if (cellVal?.Value != null && cellVal.Type == CellValueType.IsDateTime && cellVal.DateTimeValue != DateTime.MinValue)
                                            parseValue = cellVal.DateTimeValue;
                                        else
                                        {
                                            var dataAs = DateTime.MinValue;
                                            if (cellVal?.Value != null)
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
                                                    msgValidate = $"Dữ liệu nhập vào không hợp lệ <b>{validator.Format}</b>";
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
                                            msgValidate = $"Dữ liệu nhập vào không hợp lệ <b>{validator.Format}</b>";
                                        }
                                    }
                                }
                            }
                            break;
                        default:
                            isValidCell = false;
                            msgValidate = $"Kiểu dữ liệu không được hỗ trợ <b>{value}</b>";
                            break;
                    }

                    if (isValidCell && parseValue != null && rowParse != null && parseType != null)
                    {
                        //var rowFields = rowParse.GetType().GetFields();
                        if (!string.IsNullOrEmpty(validator.FieldNameMapper))
                        {
                            var field = parseType.GetProperty(validator.FieldNameMapper, BindingFlags.Instance | BindingFlags.Public);
                            //var field = rowFields.FirstOrDefault(x => x.Name == validator.FieldNameMapper);
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

        /// <summary>
        /// Đọc dữ liệu từ cell
        /// </summary>
        /// <param name="validator">Cấu hình cột</param>
        /// <param name="atRowHeader">Cột tiêu đề</param>
        /// <param name="cellVal">Aspose cell</param>
        /// <returns></returns>
        public ExcelCell ReadCellAt(ExcelFormValidator validator, Cell cellVal, bool atRowHeader = false)
        {
            try
            {
                var stringCellVal = cellVal.StringValue;
                var returnCell = new ExcelCell { CellName = cellVal.Name, FieldMapper = validator?.FieldNameMapper };
                bool isValidCell = true;
                string msgValidate = string.Empty;
                object parseValue = null;
                if (validator != null)
                {
                    if (atRowHeader)
                    {
                        string colName = stringCellVal;
                        var columnNames = validator.Name.Split(';');
                        if (!columnNames.Any(x =>
                            colName.StartsWith(x, StringComparison.CurrentCultureIgnoreCase) &&
                            !string.IsNullOrEmpty(x)))
                        {
                            returnCell.CellName = cellVal.Name;
                            returnCell.IsNotValidCell = true;
                            returnCell.ErrorMessage = $"Tên cột dữ liệu <b>{validator.Name}</b> không đúng. Yêu cầu <b>{columnNames[0]}</b>";
                        }

                        returnCell.IsHeader = true;
                        return returnCell;
                    }

                    switch (validator.Type)
                    {
                        case "string":
                            string valueString = stringCellVal;
                            parseValue = valueString;
                            if (!string.IsNullOrEmpty(valueString))
                            {
                                if (!string.IsNullOrEmpty(validator.Format))
                                {
                                    isValidCell = new Regex(validator.Format, RegexOptions.IgnoreCase).IsMatch(valueString);
                                    msgValidate = !isValidCell ? $"Dữ liệu không đúng định dạng: {validator.Format}" : string.Empty;
                                }
                            }
                            else if (validator.Required && string.IsNullOrEmpty(valueString))
                            {
                                isValidCell = false;
                                msgValidate = "Ô dữ liệu bắt buộc nhập";
                            }
                            break;
                        case "int":
                            string valueNumber = stringCellVal;
                            try
                            {
                                
                                if (cellVal.Value != null && cellVal.Type == CellValueType.IsNumeric)
                                    valueNumber = cellVal.IntValue.ToString("N0");
                            }
                            catch (Exception e)
                            {
                                Log.Error(e, "get cell Float {0}", e.Message);
                            }
                            if (validator.Required && string.IsNullOrEmpty(valueNumber))
                            {
                                isValidCell = false;
                                msgValidate = "Ô dữ liệu bắt buộc nhập";
                            }
                            else if (!string.IsNullOrEmpty(valueNumber))
                            {
                                if (!string.IsNullOrEmpty(validator.Format) &&
                                    !new Regex(validator.Format).IsMatch(validator.Format))
                                {
                                    isValidCell = false;
                                    msgValidate = $"Dữ liệu không đúng định dạng: {validator.Format}";
                                }
                                else
                                {
                                    int valueParse = valueNumber.ToInt32();
                                    parseValue = valueParse;
                                    if (valueParse < validator.Min && !validator.NoMin)
                                    {
                                        isValidCell = false;
                                        msgValidate = $"Dữ liệu nhập vào không hợp lệ {validator.Min}";
                                    }
                                }
                            }


                            break;
                        case "float":
                            string floatNumber = stringCellVal;
                            bool isNegative = false;
                            bool isGetNatureValue = false;
                            try
                            {
                                if (cellVal.Value != null && cellVal.Type == CellValueType.IsNumeric)
                                {
                                    double fVal = cellVal.DoubleValue;
                                    if (floatNumber.EndsWith("%"))
                                        fVal = Math.Round(fVal * 100, MidpointRounding.AwayFromZero);
                                    floatNumber = fVal.ToString("N");
                                    // kiểm tra giá trị âm
                                    isNegative = fVal < 0;
                                    isGetNatureValue = true;
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Warning(e, "get cell Float {0}", e.Message);
                                isGetNatureValue = false;
                            }

                            // trường hợp custom format của excel
                            // giá trị = 0
                            if (floatNumber.Trim() == "-")
                            {
                                floatNumber = "0";
                            }

                            if (floatNumber.StartsWith("(") && !isNegative)
                            {
                                isNegative = true;
                            }

                            if (!string.IsNullOrEmpty(validator.Replace))
                            {
                                var rplRegex = new Regex(validator.Replace);
                                floatNumber = rplRegex.Replace(floatNumber, string.Empty);
                            }

                            if (validator.Required && string.IsNullOrEmpty(floatNumber))
                            {
                                isValidCell = false;
                                msgValidate = "Ô dữ liệu bắt buộc nhập";
                            }
                            else if (!string.IsNullOrEmpty(floatNumber))
                            {
                                if (!string.IsNullOrEmpty(validator.Format) && !new Regex(validator.Format).IsMatch(floatNumber))
                                {
                                    isValidCell = false;
                                    msgValidate = $"Dữ liệu không đúng định dạng: {validator.Format}";
                                }
                                else
                                {
                                    double floatParse = floatNumber.ToDouble();
                                    if (isNegative && !isGetNatureValue)
                                        floatParse = floatParse * -1;
                                    parseValue = floatParse;
                                    if (floatParse < validator.Min && !validator.NoMin)
                                    {
                                        isValidCell = false;
                                        msgValidate = $"Dữ liệu nhập vào không hợp lệ {validator.Min}";
                                    }
                                }
                            }
                            break;
                        case "date":
                            string valueDate = stringCellVal;

                            if (validator.Required && string.IsNullOrEmpty(valueDate))
                            {
                                isValidCell = false;
                                msgValidate = "Ô dữ liệu bắt buộc nhập";
                            }
                            else if (!string.IsNullOrEmpty(valueDate))
                            {
                                if (string.IsNullOrEmpty(validator.Format))
                                {
                                    isValidCell = false;
                                    msgValidate = $"Trường dữ liệu chưa được cấu hình: {validator.FieldNameMapper}";
                                }
                                else
                                {
                                    try
                                    {
                                        if (cellVal?.Value != null && cellVal.Type == CellValueType.IsDateTime && cellVal.DateTimeValue != DateTime.MinValue)
                                            parseValue = cellVal.DateTimeValue;
                                        else
                                        {
                                            var dataAs = DateTime.MinValue;
                                            if (cellVal?.Value != null)
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
                                                    msgValidate = $"Dữ liệu nhập vào không hợp lệ {validator.Format}";
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
                                            msgValidate = $"Dữ liệu nhập vào không hợp lệ {validator.Format}";
                                        }
                                    }
                                }
                            }
                            break;
                        default:
                            isValidCell = false;
                            msgValidate = $"Kiểu dữ liệu không được hỗ trợ {stringCellVal}";
                            break;
                    }

                }
                else
                {
                    isValidCell = false;
                    msgValidate = $"Cột dữ liệu {stringCellVal} chưa được cấu hình";
                }
                returnCell.ReaderVal = parseValue;
                returnCell.IsNotValidCell = !isValidCell;
                returnCell.ErrorMessage = msgValidate;
                returnCell.StringCellValue = cellVal.StringValue;

                return returnCell;
            }
            catch (Exception)
            {
                return new ExcelCell
                {
                    IsNotValidCell = true,
                    ErrorMessage = "Dữ liệu không đúng định dạng",
                    CellName = cellVal.Name
                };
            }
        }

        /// <summary>
        /// set dữ liệu nhóm cho cell trong cùng row
        /// </summary>
        /// <param name="cells"></param>
        /// <param name="groupId"></param>
        /// <param name="groupName"></param>

        public Exception Read(Worksheet sheet, string jsonTagConfig, Type parseType)
        {
            var sheetConfig = Load(jsonTagConfig);
            if (!sheetConfig.ColumnConfigs.Any())
                return new Exception("missing sheet configuration");

            var cells = sheet.Cells;
            //int rowCounter = 0;
            foreach (Row cellsRow in cells.Rows)
            {
                if (cellsRow.IsHidden)
                    continue;
                var fCell = cellsRow.FirstCell.Row;
                var instanceOf = Activator.CreateInstance(parseType);
                //rowCounter++;
                if (fCell < sheetConfig.StartAtRow) { continue; }

                if (sheetConfig.BreakInfo.Value != null)
                {
                    var valAt = cellsRow.GetCellOrNull(sheetConfig.BreakInfo.Position);
                    if (valAt != null && valAt.Name.StartsWith($"{sheetConfig.BreakInfo.ColChar}")
                                      && valAt.Value?.Equals(sheetConfig.BreakInfo.Value) == true)
                        break;
                }
                for (int j = 0; j < sheetConfig.ColumnConfigs.Count; j++)
                {
                    var dataAt = cellsRow[j];
                    var xCellColumnValidator = sheetConfig.ColumnConfigs.FirstOrDefault(x => x.Position == j);
                    if (xCellColumnValidator != null)
                    {
                        bool header = !sheetConfig.ReadWithoutValidColumnName && fCell == 1 || fCell == sheetConfig.StartAtRow;
                        var cellValid = ValidCell(xCellColumnValidator, dataAt.StringValue, header, instanceOf, parseType, dataAt);
                        if (!cellValid.Item1)
                            AddError(new ExcelValidatorError { Column = dataAt.Name, Message = cellValid.Item2 });
                    }
                }
                if (sheetConfig.ReadWithoutValidColumnName || fCell > 1 && fCell > sheetConfig.StartAtRow)
                    AddData(instanceOf);

            }

            return null;
        }

        void AddError(ExcelValidatorError e)
        {
            if (Errors == null)
                Errors = new List<ExcelValidatorError>();
            Errors.Add(e);
        }

        void AddData(object o)
        {
            if (Data == null)
                Data = new List<object>();
            Data.Add(o);
        }

        public IList<ExcelValidatorError> Errors { get; set; }

        public IList<object> Data { get; set; }
    }


    /// <summary>
    /// Điều kiện khi gặp row có cell giá trị như trong cấu hình
    /// Sẽ thoát khỏi vòng lặp
    /// </summary>
    public class BreakInfo
    {
        public int Position { get; set; }
        public object Value { get; set; }
        public string ColChar { get; set; }
    }

    public class RowCellsValidate
    {
        private const string RuleRequire = "require";
        private const string RuleMin = "min";
        private const string RuleMinEqual = "minEqual";
        private const string RuleMax = "max";
        private const string RuleMaxEqual = "maxEqual";
        private const string RuleEqual = "equals";
        private const string RuleContains = "contains";
        private const string RuleStartWith = "startwith";

        private const string RuleEqualDataId = "id=";

        private readonly Regex _ruleValue = new Regex("(\\()+([0-9a-zA-Z-_=+:\\[\\]]+)+(\\))");

        public IList<ExcelRow> AllRows { get; set; }

        public bool ValidateCellInRow(IList<ExcelCell> cells, string rowRules)
        {
            try
            {
                var rules = rowRules?.Split(";");
                bool allCellValid = true;
                if (rules?.Length > 0)
                {
                    foreach (var cell in cells)
                    {
                        if (cell.IsHeader)
                            break;
                        foreach (var rule in rules)
                        {
                            string ruleValidMsg = _cellRuleValid(cell, rule);
                            if (!string.IsNullOrEmpty(ruleValidMsg))
                            {
                                cell.IsNotValidCell = true;
                                cell.ErrorMessage += $"{ruleValidMsg}\n";
                                allCellValid = false;
                            }
                        }
                    }
                }
                return allCellValid;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool ValidateRows()
        {
            bool isValid = true;
            if (AllRows.Count > 0)
            {
                foreach (var excelRow in AllRows)
                {
                    if (!ValidateCellInRow(excelRow.Cells, excelRow.Rules))
                        isValid = false;
                }
            }
            return isValid;
        }

        string _cellRuleValid(ExcelCell cell, string rule)
        {
            if (string.IsNullOrEmpty(rule))
                return string.Empty;
            string ruleValue = string.Empty;
            bool applyToRange = false;
            int startRange = 0, endRange = 0;
            if (_ruleValue.IsMatch(rule))
            {
                var matchGroups = _ruleValue.Match(rule);
                ruleValue = matchGroups.Groups[2].Value;

                //applyToRange = ruleValue.Contains(":") || ruleValue.Contains("[");
                // áp dụng cho dữ liệu tại 1 cột hoặc doanh sách cột (1:9)

                string rangeOnValue = ruleValue;
                if (ruleValue.Contains("["))
                {
                    var ruleValueSplit = ruleValue.Split("[");
                    rangeOnValue = ruleValueSplit[0];
                    ruleValue = ruleValueSplit[1].TrimEnd(']');
                }

                if (rangeOnValue.Contains(":"))
                {
                    var rangeSplit = rangeOnValue.Split(":");
                    startRange = rangeSplit[0].ToInt32();
                    endRange = rangeSplit[1].ToInt32();
                    applyToRange = true;
                }
                else
                {
                    if (!string.IsNullOrEmpty(rangeOnValue))
                    {
                        startRange = rangeOnValue.ToInt32();
                        endRange = rangeOnValue.ToInt32();
                        applyToRange = true;
                    }
                }
            }

            int indexOfRoundBracket = rule.IndexOf("(", StringComparison.CurrentCultureIgnoreCase);

            string cmRule = rule;
            if (indexOfRoundBracket != -1)
                cmRule = rule.Substring(0, indexOfRoundBracket);
            switch (cmRule)
            {
                case RuleRequire:
                    if (!applyToRange)
                    {
                        if (string.IsNullOrEmpty(cell.StringCellValue))
                            return $"Ô dữ liệu {cell.CellName} không được để trống!";
                        return string.Empty;
                    }

                    if (cell.CellPosition >= startRange && cell.CellPosition <= endRange)
                        if (string.IsNullOrEmpty(cell.StringCellValue))
                            return $"Ô dữ liệu {cell.CellName} không được để trống!";

                    return string.Empty;
                case RuleMin:
                    if (!applyToRange)
                    {
                        if (!_minRule(cell, ruleValue))
                            return $"Dữ liệu {cell.CellName} phải lớn hơn {ruleValue}!";
                        return string.Empty;
                    }

                    if (cell.CellPosition >= startRange && cell.CellPosition <= endRange)
                        if (!_minRule(cell, ruleValue))
                            return $"Dữ liệu {cell.CellName} phải lớn hơn {ruleValue}!";

                    return string.Empty;
                case RuleMinEqual:
                    if (!applyToRange)
                    {
                        if (!_minRule(cell, ruleValue, true))
                            return $"Dữ liệu {cell.CellName} phải lớn hơn hoặc bằng {ruleValue}!";
                        return string.Empty;
                    }

                    if (cell.CellPosition >= startRange && cell.CellPosition <= endRange)
                        if (!_minRule(cell, ruleValue, true))
                            return $"Dữ liệu {cell.CellName} phải lớn hơn hoặc bằng {ruleValue}!";

                    return string.Empty;
                case RuleMax:
                    if (!applyToRange)
                    {
                        if (!_maxRule(cell, ruleValue))
                            return $"Dữ liệu {cell.CellName} phải nhỏ hơn {ruleValue}!";
                        return string.Empty;
                    }

                    if (cell.CellPosition >= startRange && cell.CellPosition <= endRange)
                        if (!_maxRule(cell, ruleValue))
                            return $"Dữ liệu {cell.CellName} phải nhỏ hơn {ruleValue}!";

                    return string.Empty;
                case RuleMaxEqual:
                    if (!applyToRange)
                    {
                        if (!_maxRule(cell, ruleValue, true))
                            return $"Dữ liệu {cell.CellName} phải nhỏ hơn hoặc bằng {ruleValue}!";
                        return string.Empty;
                    }

                    if (cell.CellPosition >= startRange && cell.CellPosition <= endRange)
                        if (!_maxRule(cell, ruleValue, true))
                            return $"Dữ liệu {cell.CellName} phải nhỏ hơn hoặc bằng {ruleValue}!";

                    return string.Empty;
                //todo: 3 case này chưa sử dụng
                case RuleEqual:
                    int iOfIdGroup = rule.IndexOf(RuleEqualDataId, StringComparison.OrdinalIgnoreCase);
                    if (iOfIdGroup != -1)
                    {
                        int refIdEqual = rule.Substring(iOfIdGroup).ToInt32();
                        if (refIdEqual == 0)
                            return string.Empty;

                        var fRowEqual = AllRows.FirstOrDefault(c => c.GroupId == iOfIdGroup);
                        if (fRowEqual == null)
                            return string.Empty;

                        var cellRowEqual = fRowEqual.Cells.FirstOrDefault(c => c.CellPosition == cell.CellPosition);
                        if (cellRowEqual != null)
                        {
                            if (!cellRowEqual.ReaderVal.Equals(cell.ReaderVal))
                                return $"Dữ liệu ô {cell.CellName} không khớp với ô {cellRowEqual.CellName}";
                        }
                    }
                    break;
                case RuleContains:
                    break;
                case RuleStartWith:
                    break;
            }
            return string.Empty;
        }

        bool _minRule(ExcelCell cell, string minVal, bool equalMin = false)
        {
            if (cell.ReaderVal == null)
                return false;
            if (string.IsNullOrEmpty(minVal))
                return true;

            switch (cell.ReaderVal)
            {
                case int intValue:
                    if (equalMin)
                        return intValue >= minVal.ToInt32();
                    return intValue > minVal.ToInt32();
                case double doubleValue:
                    if (equalMin)
                        return doubleValue >= minVal.ToDouble();
                    return doubleValue > minVal.ToDouble();
                case DateTime dateValue:
                    DateTime minDateRule = minVal.ToDateTime("yyyy-MM-dd", CultureInfo.GetCultureInfo("en-US"));
                    if (equalMin)
                        return dateValue > DateTime.MinValue && (dateValue - minDateRule).Days >= 0;
                    return dateValue > DateTime.MinValue && dateValue > minDateRule;
                default:
                    return true;
            }
        }

        bool _maxRule(ExcelCell cell, string minVal, bool equalMax = false)
        {
            if (cell.ReaderVal == null)
                return false;
            if (string.IsNullOrEmpty(minVal))
                return true;

            switch (cell.ReaderVal)
            {
                case int intValue:
                    if (equalMax)
                        return intValue <= minVal.ToInt32();
                    return intValue < minVal.ToInt32();
                case double doubleValue:
                    if (equalMax)
                        return doubleValue <= minVal.ToDouble();
                    return doubleValue < minVal.ToDouble();
                case DateTime dateValue:
                    DateTime minDateRule = minVal.ToDateTime("yyyy-MM-dd", CultureInfo.GetCultureInfo("en-US"));
                    if (equalMax)
                        return dateValue < DateTime.MaxValue && dateValue <= minDateRule;
                    return dateValue < DateTime.MaxValue && dateValue < minDateRule;
                default:
                    return true;
            }
        }

        public string CreateErrorFile(Workbook wb, string writeFolder, string originalName)
        {
            try
            {
                Worksheet ws = wb.Worksheets[0];
                int columnErrors = 0;
                foreach (var excelRow in AllRows)
                {
                    int latestCol = excelRow.Cells.Max(x => x.CellPosition);
                    columnErrors = latestCol;
                    var allCellsErr = excelRow.Cells.Where(x => x.IsNotValidCell).ToList();
                    if (allCellsErr.Count > 0)
                    {
                        var res = allCellsErr.Aggregate("", (current, next) => current + next.ErrorMessage);
                        Cell c = ws.Cells[excelRow.RowIndex, latestCol + 1];
                        var styleErr = c.GetStyle();
                        styleErr.Font.Color = Color.Red;
                        styleErr.BackgroundColor = Color.White;

                        c.SetStyle(styleErr);

                        c.PutValue(res);

                        allCellsErr.ForEach(x =>
                        {
                            var style = ws.Cells[x.CellName].GetStyle();
                            style.Pattern = BackgroundType.Solid;
                            style.ForegroundColor = Color.FromArgb(252, 202, 205);
                            style.Font.Color = Color.FromArgb(146, 66, 77);

                            ws.Cells[x.CellName].SetStyle(style);
                        });

                        ws.AutoFitRow(excelRow.RowIndex);
                    }
                }
                ws.AutoFitColumn(columnErrors);

                string fileErrorName = originalName.Replace(".xlsx", $"_{DateTime.Now:yyyyMMddHHmmss}_loi.xlsx");
                string fullPath = $"{writeFolder}\\{fileErrorName}";
                if (!Directory.Exists(writeFolder))
                    Directory.CreateDirectory(writeFolder);
                if (File.Exists(fullPath))
                    File.Delete(fullPath);

                wb.Save(fullPath, SaveFormat.Xlsx);
                return fileErrorName;
            }
            catch (Exception e)
            {
                Log.Error(e, "error");
                return string.Empty;
            }
        }
        
        public string CreateErrorFile(Workbook wb, Worksheet ws, string writeFolder, string originalName)
        {
            try
            {
                int columnErrors = 0;
                foreach (var excelRow in AllRows)
                {
                    int latestCol = excelRow.Cells.Max(x => x.CellPosition);
                    columnErrors = latestCol;
                    var allCellsErr = excelRow.Cells.Where(x => x.IsNotValidCell).ToList();
                    if (allCellsErr.Count > 0)
                    {
                        var res = allCellsErr.Aggregate("", (current, next) => current + next.ErrorMessage);
                        Cell c = ws.Cells[excelRow.RowIndex, latestCol + 1];
                        var styleErr = c.GetStyle();
                        styleErr.Font.Color = Color.Red;
                        styleErr.BackgroundColor = Color.White;

                        c.SetStyle(styleErr);

                        c.PutValue(res);

                        allCellsErr.ForEach(x =>
                        {
                            var style = ws.Cells[x.CellName].GetStyle();
                            style.Pattern = BackgroundType.Solid;
                            style.ForegroundColor = Color.FromArgb(252, 202, 205);
                            style.Font.Color = Color.FromArgb(146, 66, 77);

                            ws.Cells[x.CellName].SetStyle(style);
                        });

                        ws.AutoFitRow(excelRow.RowIndex);
                    }
                }
                ws.AutoFitColumn(columnErrors);

                string fileErrorName = originalName.Replace(".xlsx", $"_{DateTime.Now:yyyyMMddHHmmss}_loi.xlsx");
                string fullPath = $"{writeFolder}\\{fileErrorName}";
                if (!Directory.Exists(writeFolder))
                    Directory.CreateDirectory(writeFolder);
                if (File.Exists(fullPath))
                    File.Delete(fullPath);

                wb.Save(fullPath, SaveFormat.Xlsx);
                return fileErrorName;
            }
            catch (Exception e)
            {
                Log.Error(e, "error");
                return string.Empty;
            }
        }
    }
}
