namespace TaskManagementSoftware.Models
{
    public class TaskMaterialUsageDto
    {
        public string TaskName { get; set; }
        public string MaterialName { get; set; }
        public int Amount { get; set; }
        public Unit UnitOfMeasurement { get; set; }
    }
}
