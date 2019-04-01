using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherUpdateClient.Model
{
    public class WeatherItem
    {
        public int Id { get; set; }
        public WeatherStatus Status { get; set; }
        public int Temperature { get; set; }
        public string City { get; set; }

    }

    public enum WeatherStatus
    {
        Sun,
        Fog,
        Rain
    }
}
