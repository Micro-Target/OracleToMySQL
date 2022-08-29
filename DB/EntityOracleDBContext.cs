using Microsoft.EntityFrameworkCore;

namespace Recorder
{
    public partial class EntityOracleDBContext : DbContext
    {
        private readonly string synchronize = AppSettingsHelper.ReadAppSettings("OpenLock", "Synchronize");
        private readonly string wireless = AppSettingsHelper.ReadAppSettings("OpenLock", "Wireless");

        public EntityOracleDBContext()
        {
        }

        public EntityOracleDBContext(DbContextOptions<EntityOracleDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<PoliceStation> PoliceStations { get; set; } = null!;

        public virtual DbSet<TestPolice> TestPolices { get; set; } = null!;

        public virtual DbSet<DeviceList> Devices { get; set; } = null!;

        public virtual DbSet<TestingData> TestingDatas { get; set; } = null!;



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseOracle(AppSettingsHelper.ReadAppSettings("ConnectionStrings", "OracleDataBase"), b => b.UseOracleSQLCompatibility("11"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (synchronize.Equals("0") || wireless.Equals("0"))
            {
                modelBuilder.Entity<PoliceStation>(entity =>
                {
                    entity.ToTable("POLICESTATION");

                    entity.HasKey(e => e.Policestano);

                    entity.Property(e => e.Policestano)
                        .IsRequired()
                        .HasColumnName("POLICESTANO");

                    entity.Property(e => e.Policestation)
                        .HasColumnName("POLICESTATION");

                    entity.Property(e => e.Policestatype)
                        .HasColumnName("POLICESTATYPE");

                    entity.Property(e => e.Policestaaff)
                        .HasColumnName("POLICESTAAFF");

                    entity.Property(e => e.DisplayNo)
                        .HasColumnName("DISPLAY_NO");

                    entity.Property(e => e.IsDutyFlag)
                        .HasColumnName("IS_DUTY_FLAG");

                    entity.HasQueryFilter(e => e.IsDutyFlag != "N");
                });

                modelBuilder.Entity<TestPolice>(entity =>
                {
                    entity.ToTable("TESTPOLICE");

                    entity.HasKey(e => e.TestPoliceId);

                    entity.Property(e => e.TestPoliceId)
                        .IsRequired()
                        .HasColumnName("TESTPOLICEID");

                    entity.Property(e => e.TestPoliceNo)
                        .HasColumnName("TESTPOLICENO");

                    entity.Property(e => e.TestPoliceName)
                        .HasColumnName("TESTPOLICENAME");

                    entity.Property(e => e.PoliceStaNo)
                        .HasColumnName("POLICESTANO");

                    entity.Property(e => e.Phone)
                        .HasColumnName("PHONE");

                    entity.HasQueryFilter(e => e.TestPoliceNo.Trim().Length == 6);
                });

                modelBuilder.Entity<DeviceList>(entity =>
                {
                    entity.ToTable("DEVICELIST");

                    entity.HasKey(e => e.Instruno);

                    entity.Property(e => e.Instruno)
                        .IsRequired()
                        .HasColumnName("INSTRUNO");

                    entity.Property(e => e.Instrutype)
                        .HasColumnName("INSTRUTYPE");

                    entity.Property(e => e.Instruproducer)
                        .HasColumnName("INSTRUPRODUCER");

                    entity.Property(e => e.Instrubuydate)
                        .HasColumnName("INSTRUBUYDATE");

                    entity.Property(e => e.Instruaff)
                        .HasColumnName("INSTRUAFF");

                    entity.Property(e => e.Instrustatus)
                        .HasColumnName("INSTRUSTATUS");

                    entity.Property(e => e.PoliceNo)
                        .HasColumnName("POLICE_NO");

                    entity.Property(e => e.StatusId)
                        .HasColumnName("STATUS_ID");

                    entity.Property(e => e.CheckId)
                        .HasColumnName("CHECK_ID");

                    entity.Property(e => e.ServiceId)
                        .HasColumnName("SERVICE_ID");

                    entity.Property(e => e.DeviceType)
                        .HasColumnName("DEVICE_TYPE");

                    entity.Property(e => e.CheckReport)
                        .HasColumnName("CHECK_REPORT");

                    entity.Property(e => e.CardSim)
                        .HasColumnName("CARD_SIM");

                    entity.Property(e => e.Autonomy)
                        .HasColumnName("AUTONOMY");
                });

                modelBuilder.Entity<TestingData>(entity =>
                {
                    entity.ToTable("TESTINGDATA");

                    entity.HasKey(e => e.TestserialNo);

                    entity.Property(e => e.TestserialNo)
                        .IsRequired()
                        .HasColumnName("TESTSERIALNO");

                    entity.Property(e => e.DeviceNo)
                          .HasColumnName("DEVICENO");

                    entity.Property(e => e.TestDate)
                        .HasColumnName("TESTDATE");

                    entity.Property(e => e.TestTime)
                        .HasColumnName("TESTTIME");

                    entity.Property(e => e.TestType)
                        .HasColumnName("TESTTYPE");

                    entity.Property(e => e.TestClass)
                        .HasColumnName("TESTCLASS");

                    entity.Property(e => e.TestResult)
                        .HasColumnName("TESTRESULT");

                    entity.Property(e => e.TestUnit)
                        .HasColumnName("TESTUNIT");

                    entity.Property(e => e.TestUser)
                        .HasColumnName("TRANSUSER");

                    entity.Property(e => e.TransDate)
                        .HasColumnName("TRANSDATE");

                    entity.Property(e => e.SubjectName)
                        .HasColumnName("SUBJECTNAME");

                    entity.Property(e => e.SubjectIdType)
                        .HasColumnName("SUBJECTIDTYPE");

                    entity.Property(e => e.SubjectIdNo)
                        .HasColumnName("SUBJECTIDNO");

                    entity.Property(e => e.TestPolice)
                        .HasColumnName("TESTPOLICE");

                    entity.Property(e => e.DeviceTestNo)
                        .HasColumnName("DEVICETESTNO");

                    entity.Property(e => e.TestPoliceSta)
                        .HasColumnName("TESTPOLICESTA");

                    entity.Property(e => e.FactLicenseType)
                       .HasColumnName("FACTLICENSETYPE");

                    entity.Property(e => e.LicenseNumber)
                       .HasColumnName("LICENSENUMBER");

                    entity.Property(e => e.ObjectionFlag)
                       .HasColumnName("OBJECTIONFLAG");

                    entity.Property(e => e.DisposalType)
                       .HasColumnName("DISPOSALTYPE");

                    entity.Property(e => e.TestPoliceSecond)
                       .HasColumnName("TESTPOLICE_SECOND");

                    entity.Property(e => e.TestPlace)
                       .HasColumnName("TEST_PLACE");

                    entity.Property(e => e.LeaderPolice)
                       .HasColumnName("LEADER_POLICE");

                    entity.Property(e => e.DataType)
                       .HasColumnName("DATA_TYPE");

                    entity.Property(e => e.WirelessUploadFlag)
                       .HasColumnName("WIRELESS_UPLOAD_FLAG");

                    entity.Property(e => e.TestPoliceNo)
                       .HasColumnName("TESTPOLICE_NO");

                    entity.Property(e => e.TId)
                        .HasColumnName("T_ID");
                        
                    entity.Property(e => e.DutyType)
                       .HasColumnName("DUTY_TYPE");

                    entity.Property(e => e.DutyDaduiId)
                       .HasColumnName("DUTY_DADUI_ID");

                    entity.Property(e => e.DutyZhongDuiId)
                       .HasColumnName("DUTY_ZHONGDUI_ID");

                    entity.HasQueryFilter(e => e.DataType == "Normal" && e.TestPoliceSta != null && e.TransDate >= DateTime.Now.AddMonths(-1) && e.TransDate < DateTime.Now.AddMinutes(5));
                });   
            }
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
