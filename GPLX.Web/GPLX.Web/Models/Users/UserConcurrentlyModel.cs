using System.Collections.Generic;
using GPLX.Database.Models;

namespace GPLX.Web.Models.Users
{
    public class UserConcurrentlyModel
    {
        public IList<UserConcurrently> UserConcurrently { get; set; }
        public bool AnyConcurrently { get; set; }
    }
}
