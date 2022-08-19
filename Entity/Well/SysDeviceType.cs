namespace Recorder
{
    public class SysDeviceType
    {
        public int Id { get; set; }

        public string? DeviceName { get; set; }

        public string? DeviceCategory { get; set; }

        public string DeviceCode { get; set; } = null!;

        public DateTime? CreateTime { get; set; }

        public DateTime? LastUpdateTime { get; set; }

        public int? LastUpdateBy { get; set; }

        public int? CreateBy { get; set; }
    }
}
