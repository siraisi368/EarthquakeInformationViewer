using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EarthquakeInformationViewer
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Accuracy
    {
        public string Epicenter { get; set; }
        public string Depth { get; set; }
        public string Magnitude { get; set; }
    }

    public class Issue
    {
        public string Source { get; set; }
        public string Status { get; set; }
    }

    public class MaxIntChange
    {
        public string String { get; set; }
        public string Reason { get; set; }
    }

    public class WolfxEEWAPI
    {
        public string Title { get; set; }
        public string CodeType { get; set; }
        public Issue Issue { get; set; }
        public string EventID { get; set; }
        public int Serial { get; set; }
        public string AnnouncedTime { get; set; }
        public string OriginTime { get; set; }
        public string Hypocenter { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Magunitude { get; set; }
        public int Depth { get; set; }
        public string MaxIntensity { get; set; }
        public Accuracy Accuracy { get; set; }
        public MaxIntChange MaxIntChange { get; set; }
        public List<object> WarnArea { get; set; }
        public bool isSea { get; set; }
        public bool isTraining { get; set; }
        public bool isAssumption { get; set; }
        public bool isWarn { get; set; }
        public bool isFinal { get; set; }
        public bool isCancel { get; set; }
        public string OriginalText { get; set; }
        public string Pond { get; set; }
    }


}
