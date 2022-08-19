namespace Recorder
{
    public class TPolice
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string PoliceNum { get; set; } = null!;

        public string? Mobile { get; set; }

        public int? DeptId { get; set; }

        public int? Gender { get; set; }

        public int? Status { get; set; }

        public DateTime? CreateTime { get; set; }

        public int? CreateBy { get; set; }

        public int? DelFlag { get; set; }
    }
}
