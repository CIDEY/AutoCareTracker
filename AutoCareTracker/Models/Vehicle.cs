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
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Plate { get; set; }

        [Ignore]
        public string FullName => $"{Brand} {Model}";

        [Ignore]
        public string MainLetter => !string.IsNullOrWhiteSpace(Brand) ? Brand[0].ToString().ToUpper() : "?";

        [Ignore]
        public Color CircleColor
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Brand)) return Colors.Gray;
                int hash = Brand.GetHashCode();
                string[] colors = { "#4572ED", "#2AB27B", "#FF5252", "#FFAB40", "#7C4DFF", "#00B8D4" };
                return Color.FromArgb(colors[Math.Abs(hash) % colors.Length]);
            }
        }

        [Ignore]
        public string LogoUrl => !string.IsNullOrWhiteSpace(Brand)
            ? $"https://raw.githubusercontent.com/fawazahmed0/car-logos/master/logos/{Brand.Trim().ToLower().Replace(" ", "-")}/logo.png"
            : null;
    }
}
