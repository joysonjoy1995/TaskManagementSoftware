namespace TaskManagementSoftware.Models
{
    public class Material
    {
        public Guid ID { get; set; }
        public string PartNumber { get; set; }
        public int ManufacturerCode { get; set; }
        public int Price { get; set; }
        public Unit UnitOfIssue { get; set; }

        // Navigation property for TaskMaterialUsage
        public ICollection<TaskMaterialUsage> TaskMaterialUsages { get; set; } = new HashSet<TaskMaterialUsage>();
    }
}
