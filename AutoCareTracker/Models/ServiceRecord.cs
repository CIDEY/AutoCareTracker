using SQLite;

namespace AutoCareTracker.Models
{
    public class ServiceRecord
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string WorkType { get; set; } // Например: "Замена масла"
        public int Mileage { get; set; }    // Пробег
        public DateTime Date { get; set; }   // Дата обслуживания
        public double Cost { get; set; }     // Стоимость
        public string Notes { get; set; }    // Заметки
    }
}
