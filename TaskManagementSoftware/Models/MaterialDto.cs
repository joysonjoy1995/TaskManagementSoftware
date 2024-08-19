namespace TaskManagementSoftware.Models
{
    public class MaterialDto
    {
        public string PartNumber { get; set; }
        public int ManufacturerCode { get; set; }
        public int Price { get; set; }
        public string UnitOfIssue { get; set; } // This is a string for parsing purposes
    }
}
