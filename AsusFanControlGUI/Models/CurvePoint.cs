using System.Runtime.Serialization;

namespace AsusFanControlGUI.Models
{
    [DataContract]
    public class CurvePoint
    {
        [DataMember(Name = "tempC")]
        public float TempC { get; set; }

        [DataMember(Name = "rpm")]
        public float Rpm { get; set; }   // 0 = fan stopped / OFF

        public CurvePoint() { }

        public CurvePoint(float tempC, float rpm)
        {
            TempC = tempC;
            Rpm = rpm;
        }
    }
}
