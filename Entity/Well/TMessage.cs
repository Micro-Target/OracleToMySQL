namespace Recorder
{
    public class TMessage
    {
        public int Id { get; set; }

        public DateTime? CreateTime { get; set; }

        public int? CreateBy { get; set; }
        
        public DateTime? LastUpdateTime { get; set; }

        public int? LastUpdateBy { get; set; }

        public string? Mobiles { get; set; }

        public string? Content { get; set; }

        public DateTime? SendTime { get; set; }

        public int? Flag { get; set; }

        public string? Remark { get; set; }

        public int? DelFlag { get; set; }
    }
}
