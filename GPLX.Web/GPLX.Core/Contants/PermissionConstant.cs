using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Core.Contants
{
    public static class PermissionConstant
    {
        public const int VIEW = 2;
        public const int APPROVE = 4;
        public const int ADD = 8;
        public const int EDIT = 16;
        public const int DELETE = 32;


        public const string VIEW_KEY = "VIEW";
        public const string APPROVE_KEY = "APPROVE";
        public const string ADD_KEY = "ADD";
        public const string EDIT_KEY = "EDIT";
        public const string DELETE_KEY = "DELETE";


        public static Dictionary<string, int> PermissionKeyToValue = new Dictionary<string, int>
        {
            { VIEW_KEY, VIEW},
            { APPROVE_KEY, APPROVE},
            { ADD_KEY, ADD},
            { EDIT_KEY, EDIT},
            { DELETE_KEY, DELETE}
        };
    }
}
