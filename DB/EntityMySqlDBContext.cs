using Microsoft.EntityFrameworkCore;

namespace Recorder
{
    public partial class EntityMySqlDBContext : DbContext
    {
        public EntityMySqlDBContext()
        {
        }

        public EntityMySqlDBContext(DbContextOptions<EntityMySqlDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<SysDeviceType> DeviceTypes { get; set; } = null!;

        public virtual DbSet<SysDept> Depts { get; set; } = null!;

        public virtual DbSet<TPolice> Polices { get; set; } = null!;

        public virtual DbSet<TDevice> Devices { get; set; } = null!;

        public virtual DbSet<TDriverInfo> DriverInfos { get; set; } = null!;

        public virtual DbSet<TData> Datas { get; set; } = null!;

        public virtual DbSet<TMessage> Messages { get; set; } = null!;

        public virtual DbSet<SmsData> SmsDatas { get; set; } = null!;


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(AppSettingsHelper.ReadAppSettings("ConnectionStrings", "MySqlDataBase"), ServerVersion.Parse("8.0.30-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8_bin")
                .HasCharSet("utf8");

            modelBuilder.Entity<SysDeviceType>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PRIMARY");

                entity.ToTable("t_device_type");

                entity.HasComment("设备类型表");

                entity.Property(e => e.Id)
                   .HasColumnType("int(10)")
                   .HasColumnName("id");

                entity.Property(e => e.DeviceName)
                    .HasMaxLength(20)
                    .HasColumnName("device_name")
                    .HasComment("设备类型名称");

                entity.Property(e => e.DeviceCategory)
                    .HasMaxLength(10)
                    .HasColumnName("device_category")
                    .HasComment("设备种类");

                entity.Property(e => e.DeviceCode)
                    .HasMaxLength(2)
                    .HasColumnName("device_code")
                    .HasComment("设备类型代码");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasComment("创建日期");

                entity.Property(e => e.LastUpdateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("last_update_time")
                    .HasComment("修改日期");

                entity.Property(e => e.LastUpdateBy)
                    .HasColumnType("int(10)")
                    .HasColumnName("last_update_by")
                    .HasComment("修改人");

                entity.Property(e => e.CreateBy)
                    .HasColumnType("int(10)")
                    .HasColumnName("create_by")
                    .HasComment("创建人");
            });

            modelBuilder.Entity<SysDept>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PRIMARY");

                entity.ToTable("sys_dept");

                entity.HasComment("机构表");

                entity.Property(e => e.Id)
                   .HasColumnType("int(10)")
                   .HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name")
                    .HasComment("名称");

                entity.Property(e => e.ParentId)
                    .HasColumnType("int(10)")
                    .HasColumnName("parent_id")
                    .HasComment("上一级机构");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasComment("创建时间");

                entity.Property(e => e.DelFlag)
                    .HasColumnType("int(1)")
                    .HasColumnName("del_flag")
                    .HasComment("是否删除");

                entity.Property(e => e.IsDuty)
                    .HasColumnType("int(1)")
                    .HasColumnName("is_duty")
                    .HasComment("是否执勤单位");

                entity.Property(e => e.CreateBy)
                   .HasColumnType("int(10)")
                   .HasColumnName("create_by")
                   .HasComment("创建人");

                entity.Property(e => e.Disabled)
                   .HasColumnType("bit(1)")
                   .HasColumnName("disabled")
                   .HasComment("是否禁用");

                entity.Property(e => e.UserId)
                   .HasColumnType("int")
                   .HasColumnName("user_id")
                   .HasComment("用户ID");

                entity.Property(e => e.SynchId)
                   .HasMaxLength(300)
                   .HasColumnName("synch_id")
                   .HasComment("第三方酒检平台部门编号");
            });

            modelBuilder.Entity<TPolice>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PRIMARY");

                entity.ToTable("t_police");

                entity.HasComment("警员表");

                entity.Property(e => e.Id)
                   .HasColumnType("int(10)")
                   .HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name")
                    .HasComment("名称");

                entity.Property(e => e.PoliceNum)
                    .HasMaxLength(50)
                    .HasColumnName("police_num")
                    .HasComment("警号");

                entity.Property(e => e.Mobile)
                    .HasMaxLength(30)
                    .HasColumnName("mobile")
                    .HasComment("电话");

                entity.Property(e => e.DeptId)
                    .HasColumnType("int(10)")
                    .HasColumnName("dept_id")
                    .HasComment("所属机构");

                entity.Property(e => e.Gender)
                    .HasColumnType("int(1)")
                    .HasColumnName("gender")
                    .HasComment("性别");

                entity.Property(e => e.Status)
                    .HasColumnType("int(1)")
                    .HasColumnName("status")
                    .HasComment("状态");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasComment("创建时间");

                entity.Property(e => e.CreateBy)
                   .HasColumnType("int(10)")
                   .HasColumnName("create_by")
                   .HasComment("创建人");

                entity.Property(e => e.DelFlag)
                    .HasColumnType("int(1)")
                    .HasColumnName("del_flag")
                    .HasComment("是否删除");
            });

            modelBuilder.Entity<TDevice>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PRIMARY");

                entity.ToTable("t_device");

                entity.HasComment("设备表");

                entity.Property(e => e.Id)
                   .HasColumnType("int(10)")
                   .HasColumnName("id");

                entity.Property(e => e.DeviceSn)
                    .HasMaxLength(50)
                    .HasColumnName("device_sn")
                    .HasComment("设备序列号");

                entity.Property(e => e.DeviceTypeId)
                    .HasColumnType("int(10)")
                    .HasColumnName("device_type_id")
                    .HasComment("设备类型");

                entity.Property(e => e.DeptId)
                    .HasColumnType("int(10)")
                    .HasColumnName("dept_id")
                    .HasComment("所属机构");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("date")
                    .HasColumnName("create_date")
                    .HasComment("创建日期");

                entity.Property(e => e.CreateBy)
                    .HasColumnType("int(10)")
                    .HasColumnName("create_by")
                    .HasComment("创建人");

                entity.Property(e => e.LastUpdateDate)
                    .HasColumnType("date")
                    .HasColumnName("last_update_date")
                    .HasComment("检定完成日期");

                entity.Property(e => e.Status)
                    .HasColumnType("int(1)")
                    .HasColumnName("status")
                    .HasComment("状态");

                entity.Property(e => e.LastUpdateBy)
                    .HasColumnType("int(10)")
                    .HasColumnName("last_update_by")
                    .HasComment("更新人");

                entity.Property(e => e.LastUpdateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("last_update_time")
                    .HasComment("更新时间");

                entity.Property(e => e.DelFlag)
                    .HasColumnType("int(1)")
                    .HasColumnName("del_flag")
                    .HasComment("是否删除");

                entity.Property(e => e.PoliceId)
                    .HasColumnType("int(10)")
                    .HasColumnName("police_id")
                    .HasComment("警员id");
            });

            modelBuilder.Entity<TDriverInfo>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PRIMARY");

                entity.ToTable("t_driver_info");

                entity.HasComment("驾驶人信息表");

                entity.Property(e => e.Id)
                   .HasColumnType("int(10)")
                   .HasColumnName("id");

                entity.Property(e => e.DriverIdentificationName)
                    .HasMaxLength(100)
                    .HasColumnName("driver_identification_name")
                    .HasComment("驾驶人姓名");

                entity.Property(e => e.DriverIdentificationNumber)
                    .HasMaxLength(100)
                    .HasColumnName("driver_identification_number")
                    .HasComment("驾驶人证件号");

                entity.Property(e => e.LicensePlateNumber)
                    .HasMaxLength(100)
                    .HasColumnName("license_plate_number")
                    .HasComment("车牌号");

                entity.Property(e => e.DrivingCar)
                    .HasMaxLength(5)
                    .HasColumnName("driving_car")
                    .HasComment("实驾车型");

                entity.Property(e => e.Dissent)
                    .HasColumnType("int(1)")
                    .HasColumnName("dissent")
                    .HasComment("有无异议");

                entity.Property(e => e.DriverIdentificationType)
                    .HasColumnType("int(1)")
                    .HasColumnName("driver_identification_type")
                    .HasComment("证件类型");

                entity.Property(e => e.Remark)
                    .HasMaxLength(50)
                    .HasColumnName("remark")
                    .HasComment("备注");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasComment("创建时间");

                entity.Property(e => e.DelFlag)
                    .HasColumnType("int(1)")
                    .HasColumnName("del_flag")
                    .HasComment("是否删除");
            });

            modelBuilder.Entity<TData>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PRIMARY");

                entity.ToTable("t_data");

                entity.HasComment("酒检信息表");

                entity.Property(e => e.Id)
                   .HasColumnType("int(10)")
                   .HasColumnName("id");

                entity.Property(e => e.RecordNum)
                    .HasMaxLength(10)
                    .HasColumnName("record_num")
                    .HasComment("设备记录号");

                entity.Property(e => e.WineCheckValues)
                    .HasColumnType("double")
                    .HasColumnName("wine_check_values")
                    .HasComment("酒检值");

                entity.Property(e => e.Util)
                    .HasMaxLength(20)
                    .HasColumnName("util")
                    .HasComment("测量单位");

                entity.Property(e => e.MediaFile)
                    .HasMaxLength(200)
                    .HasColumnName("media_file")
                    .HasComment("媒体文件");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasComment("测试时间");

                entity.Property(e => e.UploadTime)
                    .HasColumnType("datetime")
                    .HasColumnName("upload_time")
                    .HasComment("上传时间");

                entity.Property(e => e.LeaderName)
                    .HasMaxLength(20)
                    .HasColumnName("leader_name")
                    .HasComment("带班领导");

                entity.Property(e => e.AlcoholValueState)
                   .HasColumnType("int(1)")
                   .HasColumnName("alcohol_value_state")
                   .HasComment("酒检值状态");

                entity.Property(e => e.DutyType)
                   .HasColumnType("int(1)")
                   .HasColumnName("duty_type")
                   .HasComment("执勤类型");

                entity.Property(e => e.DeviceStatus)
                    .HasColumnType("int(1)")
                    .HasColumnName("device_status")
                    .HasComment("设备状态");

                entity.Property(e => e.RecordMode)
                    .HasColumnType("int(1)")
                    .HasColumnName("record_mode")
                    .HasComment("记录方式");

                entity.Property(e => e.RecordStatus)
                    .HasColumnType("int(1)")
                    .HasColumnName("record_status")
                    .HasComment("记录状态");

                entity.Property(e => e.ImposeMeasuresCode)
                    .HasMaxLength(50)
                    .HasColumnName("Impose_measures_code")
                    .HasComment("强措编号");

                entity.Property(e => e.DriverInfoId)
                    .HasColumnType("int(10)")
                    .HasColumnName("driver_info_id")
                    .HasComment("驾驶人信息id");

                entity.Property(e => e.PoliceId)
                    .HasColumnType("int(10)")
                    .HasColumnName("police_id")
                    .HasComment("警员id");

                entity.Property(e => e.DeviceId)
                    .HasColumnType("int(10)")
                    .HasColumnName("device_id")
                    .HasComment("设备id");

                entity.Property(e => e.DelFlag)
                    .HasColumnType("int(1)")
                    .HasColumnName("del_flag")
                    .HasComment("是否删除");

                entity.Property(e => e.DeptId)
                    .HasColumnType("int(11)")
                    .HasColumnName("dept_id")
                    .HasComment("部门id");

                entity.Property(e => e.DeviceTypeId)
                    .HasColumnType("int(11)")
                    .HasColumnName("device_type_id")
                    .HasComment("设备类型id");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .HasColumnName("address")
                    .HasComment("设备类型id");

                entity.Property(e => e.TestMode)
                    .HasColumnType("int(1)")
                    .HasColumnName("test_mode")
                    .HasComment("测试模式");

                entity.Property(e => e.CopsName)
                    .HasMaxLength(20)
                    .HasColumnName("cops_name")
                    .HasComment("同组警员");

                entity.Property(e => e.UploadExternalStatus)
                    .HasColumnType("int(1)")
                    .HasColumnName("upload_external_status")
                    .HasComment("是否上传外挂平台");

                entity.Property(e => e.UploadExternalTime)
                    .HasColumnType("datetime")
                    .HasColumnName("upload_external_time")
                    .HasComment("上传到外挂平台时间");

                entity.Property(e => e.UploadExternalMsg)
                    .HasMaxLength(100)
                    .HasColumnName("upload_external_msg")
                    .HasComment("上传到外挂平台描述");
            });

            modelBuilder.Entity<TMessage>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PRIMARY");

                entity.ToTable("t_message");

                entity.HasComment("短信表");

                entity.Property(e => e.Id)
                   .HasColumnType("int(10)")
                   .HasColumnName("id");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasComment("创建时间");

                entity.Property(e => e.CreateBy)
                    .HasColumnType("int(10)")
                    .HasColumnName("create_by")
                    .HasComment("创建人");

                entity.Property(e => e.LastUpdateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("last_update_time")
                    .HasComment("更新时间");

                entity.Property(e => e.LastUpdateBy)
                    .HasColumnType("int(10)")
                    .HasColumnName("last_update_by")
                    .HasComment("更新人");

                entity.Property(e => e.Mobiles)
                    .HasMaxLength(200)
                    .HasColumnName("mobiles")
                    .HasComment("收信人");

                entity.Property(e => e.Content)
                    .HasColumnType("text")
                    .HasColumnName("content")
                    .HasComment("短信内容");

                entity.Property(e => e.SendTime)
                    .HasColumnType("datetime")
                    .HasColumnName("send_time")
                    .HasComment("发送时间");

                entity.Property(e => e.Flag)
                    .HasColumnType("int(1)")
                    .HasColumnName("flag")
                    .HasComment("发送标记");

                entity.Property(e => e.Remark)
                    .HasMaxLength(50)
                    .HasColumnName("remark")
                    .HasComment("备注");

                entity.Property(e => e.DelFlag)
                    .HasColumnType("int(1)")
                    .HasColumnName("del_flag")
                    .HasComment("删除标记");
            });

            modelBuilder.Entity<SmsData>(entity =>
            {
                entity.ToView("sms_data");

                entity.Property(e => e.Id)
                   .HasColumnType("int")
                   .HasColumnName("id");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time");

                entity.Property(e => e.WineCheckValues)
                    .HasColumnType("double")
                    .HasColumnName("wine_check_values");

                entity.Property(e => e.Util)
                    .HasColumnType("varchar")
                    .HasColumnName("util");

                entity.Property(e => e.DeptName)
                    .HasColumnType("varchar")
                    .HasColumnName("dept_name");

                entity.Property(e => e.PoliceNo)
                    .HasColumnType("varchar")
                    .HasColumnName("police_no");

                entity.Property(e => e.PoliceName)
                    .HasColumnType("varchar")
                    .HasColumnName("police_name");

                entity.Property(e => e.Mobiles)
                   .HasColumnType("varchar")
                   .HasColumnName("mobiles");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
