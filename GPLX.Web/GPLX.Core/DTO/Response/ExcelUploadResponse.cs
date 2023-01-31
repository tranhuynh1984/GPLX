using System.Collections.Generic;

namespace GPLX.Core.DTO.Response
{
    public class ExcelUploadResponse<T>
    {
        public ExcelUploadResponse()
        {
            Data = new List<T>();
            Errors = new List<ExcelValidatorError>();
        }

        public List<T> Data { get; set; } = new List<T>();

        public List<ExcelValidatorError> Errors { get; set; }

        public void Add(T item)
        {
            Data.Add(item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            Data.AddRange(items);
        }

        public void AddError(ExcelValidatorError e)
        {
            Errors.Add(e);
        }

        public void AddErrorRange(IEnumerable<ExcelValidatorError> es)
        {
            Errors.AddRange(es);
        }

        public bool IsValid => Errors.Count == 0;

        public int Code { get; set; }

        public object SpecifyFieldValue { get; set; }

        public string ExcelFileError { get; set; }

        public string Message { get; set; }
    }

    public class ExcelValidatorError
    {
        public string Column { get; set; }
        public string Message { get; set; }
        public string Cell { get; set; }
    }
}
