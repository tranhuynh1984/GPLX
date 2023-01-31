namespace GPLX.Database.Models
{
    public class Functions
    {
        public int Id { get; set; }
        public string Unique { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }
        public string Url { get; set; }
        public int Order { get; set; }
        public string IconClass { get; set; }
        /// <summary>
        /// Hiển thị trên menu
        /// </summary>
        public int DisplayOnMenu { get; set; }
       
        /// <summary>
        /// Thuộc nhóm chức năng nào
        /// </summary>
        public string Module { get; set; }
    }
}
