using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AsusFanControlGUI.Models
{
    [DataContract]
    public class FanCurveProfile
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "isBuiltIn")]
        public bool IsBuiltIn { get; set; }

        [DataMember(Name = "cpuCurve")]
        public List<CurvePoint> CpuCurve { get; set; }

        [DataMember(Name = "gpuCurve")]
        public List<CurvePoint> GpuCurve { get; set; }

        public FanCurveProfile()
        {
            CpuCurve = new List<CurvePoint>();
            GpuCurve = new List<CurvePoint>();
        }

        /// <summary>Returns a deep copy of this profile (for Custom derivation).</summary>
        public FanCurveProfile Clone(string newName = null)
        {
            var clone = new FanCurveProfile
            {
                Name = newName ?? Name,
                IsBuiltIn = false,
                CpuCurve = new List<CurvePoint>(),
                GpuCurve = new List<CurvePoint>()
            };
            foreach (var pt in CpuCurve) clone.CpuCurve.Add(new CurvePoint(pt.TempC, pt.Rpm));
            foreach (var pt in GpuCurve) clone.GpuCurve.Add(new CurvePoint(pt.TempC, pt.Rpm));
            return clone;
        }
    }
}
