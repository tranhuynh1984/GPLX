using System.Collections.Generic;
using System.Linq;
using GPLX.Core.Extensions;
using Newtonsoft.Json;

namespace GPLX.Core.DTO.Request.Users
{
    public class UsersConfigRequest
    {
        public string Record { get; set; }
        public int RawId => !string.IsNullOrEmpty(Record) && !string.IsNullOrEmpty(RequestPage) ? int.TryParse(Record.StringAesDecryption(RequestPage, true), out var i) ? i : -1 : -1;
        public string RequestPage { get; set; }
        public string GroupRecords { get; set; }
        public string Units { get; set; }
        public string DepartmentRecord { get; set; }
        public string PathSignature { get; set; }
        public string Currently { get; set; }

        public IList<CurrentlySetting> CurrentlySettings
        {
            get
            {
                if (!string.IsNullOrEmpty(Currently))
                {
                    var deserialize = JsonConvert.DeserializeObject<List<CurrentlySetting>>(Currently);
                    deserialize = deserialize.GroupBy(x => x.UnitCode).Select(c => c.First()).ToList();
                    return deserialize;
                }
                return null;
            }
        }

    }

    public class CurrentlySetting
    {
        public string UnitCode { get; set; }
        public string GroupId { get; set; }

    }
}
