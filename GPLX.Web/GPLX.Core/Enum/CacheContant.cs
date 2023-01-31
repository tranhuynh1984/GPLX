using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Core.Enum
{
    public static class CacheContant
    {
        public const int EXPIRATION_DEFAULT = 3600;

        public const int EXPIRATION_DASHBOARD_DEFAULT = 600;

        public const int EXPIRATION_5m = 300;

        public const int EXPIRATION_10m = 600;

        public const int EXPIRATION_15m = 900;

        public const int EXPIRATION_30m = 1800;

        public const int EXPIRATION_45m = 2700;

        public const int EXPIRATION_1H = 3600;

        public const int EXPIRATION_12H = 43200;

        public const int EXPIRATION_1D = 86400;

        public const int EXPIRATION_1W = 604800;

        public const string PrefixGroupByCodeCache = "GroupByCode";


        public const string PrefixPermissionCache = "GroupsPermission";
        public const string PrefixAuthorizeCache = "Authorize";
        public const string PrefixUsersCache = "Users";
        public const string PrefixUserManagesCache = "UserManages";
        public const string UserConcurrently = "UserConcurrently";
        public const string PrefixUnitCache = "Units";

        public const string PrefixGroupCache = "Groups";
        public const string PrefixStatusesCache = "Statuses";
        public const string PrefixStatusesForSubjectCache = "StatusesForSubject";
        public const string PrefixStatusesUsedCache = "StatusesUsed";
        public const string PrefixRevenuePlanContents = "RevenuePlanContents";
        public const string PrefixProfitPlanContents = "PrefixProfitPlanContents";
        public const string PrefixInvestmentPlanContents = "InvestmentPlanContents";
        public const string PrefixFunctionsCache = "Functions";
    }
}
