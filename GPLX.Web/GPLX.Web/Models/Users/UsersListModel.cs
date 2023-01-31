using System.Collections.Generic;
using GPLX.Database.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GPLX.Web.Models.Users
{
    public class UsersListModel
    {
        public IList<SelectListItem> Groups { get; set; }
        public IList<SelectListItem> Departments { get; set; }
        public IList<SelectListItem> Units { get; set; }
        public IList<SelectListItem> UnitChanges { get; set; }

        public IList<SelectListItem> CreateDefaults(string text)
        {
            return new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = text,
                    Value = string.Empty
                }
            };
        }

        public IList<UserConcurrently> CurrentlySetting { get; set; }
    }
}
