using System.ComponentModel.DataAnnotations.Schema;

namespace Recorder
{
    public class PoliceStation
    {
        public int Policestano { get; set; }

        public string? Policestation { get; set; }

        public string? Policestatype { get; set; }

        public int? Policestaaff { get; set; }

        public int? DisplayNo { get; set; }

        public string? IsDutyFlag { get; set; }

        [NotMapped]
        public int PoliceStaLevel { get; set; }
    }
}
