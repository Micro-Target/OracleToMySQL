using Microsoft.EntityFrameworkCore;

namespace Recorder
{
    public class SyncDriverService
    {
        private readonly EntityMySqlDBContext dbMySql = new();
        private readonly EntityOracleDBContext dbOracle = new();

        /// <summary>
        /// 查询驾驶人信息
        /// </summary>
        /// <returns></returns>
        public async Task<List<TDriverInfo>> GetDriverItemAsync()
        {
            try
            {
                return await dbOracle.TestingDatas.AsNoTracking().Where(p => !string.IsNullOrEmpty(p.SubjectName) && !string.IsNullOrEmpty(p.SubjectIdNo) && !string.IsNullOrEmpty(p.LicenseNumber)).Select(x => ItemToDTO(x)).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 查询驾驶人信息
        /// </summary>
        /// <returns></returns>
        public async Task<List<TDriverInfo>> GetDriverInfoItemAsync()
        {
            try
            {
                return await dbMySql.DriverInfos.AsNoTracking().ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 添加驾驶人信息
        /// </summary>
        /// <param name="drivers">警员信息</param>
        /// <returns></returns>
        public async Task<bool> AddTDriver(TDriverInfo[] drivers)
        {
            if (drivers != null)
            {
                dbMySql.DriverInfos.AddRange(drivers);
                await dbMySql.SaveChangesAsync();
            }
            return true;
        }

        /// <summary>
        /// 警员信息格式转换处理
        /// </summary>
        /// <param name="police">警员信息</param>
        /// <returns></returns>
        private static TDriverInfo ItemToDTO(TestingData data) =>
            new()
            {
                DriverIdentificationName = !string.IsNullOrEmpty(data.SubjectName) ? data.SubjectName.Trim() : data.SubjectName,
                DriverIdentificationNumber = !string.IsNullOrEmpty(data.SubjectIdNo) ? data.SubjectIdNo.Trim() : data.SubjectIdNo,
                LicensePlateNumber = !string.IsNullOrEmpty(data.LicenseNumber) ? data.LicenseNumber.Trim() : data.LicenseNumber,
                DrivingCar = !string.IsNullOrEmpty(data.FactLicenseType) ? data.FactLicenseType.Trim().ToUpper() : data.FactLicenseType,
                Dissent = 0,
                DriverIdentificationType = !string.IsNullOrEmpty(data.SubjectIdType) ? IdentificationTypeByParams(data.SubjectIdType.Trim()) : 1,
                Remark = "",
                CreateTime = data.TestTime,
                DelFlag = 0
            };


        /// <summary>
        /// 证件类型信息
        /// </summary>
        /// <param name="identtype"></param>
        /// <returns></returns>
        public static int IdentificationTypeByParams(string identtype)
        {
            int identification_type = identtype switch
            {
                "驾驶证" => 1,
                "身份证" => 2,
                "无证件" => 4,
                _ => 3,
            };
            return identification_type;
        }
    }
}
