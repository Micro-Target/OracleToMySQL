namespace Recorder
{
    public class SysDept
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public int? ParentId { get; set; }

        public DateTime? CreateTime { get; set; }

        public int? DelFlag { get; set; }

        public int? IsDuty { get; set; }

        public int? CreateBy { get; set; }

        public int? Disabled { get; set; }

        public int? UserId { get; set; }

        public string? SynchId { get; set; }
    }
}
