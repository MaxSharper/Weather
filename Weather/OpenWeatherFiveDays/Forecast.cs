using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Weather.OpenWeatherFiveDays
{
    class Forecast
    {
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime Dt { get; set; }
        public MainDetail Main { get; set; }
        public List<Weather> Weather { get; set; }
        public Wind Wind { get; set; }
        public string Dt_txt { get; set; }
    }
}
