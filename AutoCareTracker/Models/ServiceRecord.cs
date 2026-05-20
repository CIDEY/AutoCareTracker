using SQLite;

namespace AutoCareTracker.Models
{
    public class ServiceRecord
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Ссылка на автомобиль, к которому относится запись
        public int VehicleId { get; set; }

        public string WorkType { get; set; }
        public int Mileage { get; set; }
        public DateTime Date { get; set; }
        public double Cost { get; set; }
        public string Notes { get; set; }
    }
}
