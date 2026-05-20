using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCareTracker.Models
{
    public class Vehicle
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Brand { get; set; }   // Марка (напр. Toyota)
        public string Model { get; set; }   // Модель (напр. Camry)
        public string Plate { get; set; }   // Госномер

        // Вспомогательное свойство для отображения
        [Ignore]
        public string FullName => $"{Brand} {Model}";
    }
}
