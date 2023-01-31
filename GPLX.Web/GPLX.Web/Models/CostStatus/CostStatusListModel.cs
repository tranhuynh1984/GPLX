using System.Collections.Generic;

namespace GPLX.Web.Models.CostStatus
{
    public class CostStatusListModel
    {
        public List<KeyPair> Stats { get; set; }
        public IList<KeyPair> StatusForCostEstimateType { get; set; }
        public IList<KeyPair> StatusForSubject { get; set; }
        public IList<KeyPair> Types { get; set; }
    }

    public class KeyPair
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }
}
