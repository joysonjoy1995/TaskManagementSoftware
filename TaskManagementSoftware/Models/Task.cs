namespace TaskManagementSoftware.Models
{
    public class Task
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int TotalDuration { get; set; }

        public ICollection<TaskMaterialUsage> TaskMaterialUsages { get; set; } = new HashSet<TaskMaterialUsage>();
    }
}
