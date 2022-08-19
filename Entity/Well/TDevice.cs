namespace Recorder
{
    public class TDevice
    {
        public int Id { get; set; }

        public string DeviceSn { get; set; } = null!;

        public int? DeviceTypeId { get; set; }

        public int? DeptId { get; set; }

        public DateTime? CreateDate { get; set; }

        public int? CreateBy { get; set; }

        public DateTime? LastUpdateDate { get; set; }

        public int? Status { get; set; }

        public int? LastUpdateBy { get; set; }

        public DateTime? LastUpdateTime { get; set; }

        public int? DelFlag { get; set; }

        public int? PoliceId { get; set; }
    }
}
