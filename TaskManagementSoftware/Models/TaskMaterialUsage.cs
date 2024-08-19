namespace TaskManagementSoftware.Models
{
    public class TaskMaterialUsage
    {
        public Guid ID { get; set; }
        public Guid TaskID { get; set; }
        public Task Task { get; set; }
        public Guid MaterialID { get; set; }
        public Material Material { get; set; }
        public int Amount { get; set; }
        public Unit UnitOfMeasurement { get; set; }
    }
}
