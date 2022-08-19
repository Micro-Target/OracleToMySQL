using Microsoft.EntityFrameworkCore;

namespace Recorder
{
    public class SyncDeviceService
    {
        private readonly EntityMySqlDBContext dbMySql = new();
        private readonly EntityOracleDBContext dbOracle = new();

        private readonly static string[] H = { "稞麦H3", "稞麦H4", "稞麦H5" };
        private readonly static string[] S = { "谷雨S80", "沃土523Li", "格那315", "格那320" };
        private readonly static string[] L = { "大帝730Li", "730Li", "S300" };
        private readonly static string[] T = { "酒安5000T", "酒安8000T", "酒安9000T" };

        /// <summary>
        /// 同步设备信息
        /// </summary>
        /// <returns></returns>
        public async Task<List<DeviceList>> GetSyncDeviceAsync()
        {
            try
            {
                return await dbOracle.Devices.AsNoTracking().Select(x => ItemToDTO(x)).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 查询设备信息
        /// </summary>
        /// <returns></returns>
        public async Task<List<TDevice>> GetDeviceItemAsync()
        {
            try
            {
                return await dbMySql.Devices.AsNoTracking().ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 查询设备信息
        /// </summary>
        /// <param name="deviceSn">设备编号</param>
        /// <returns></returns>
        public async Task<TDevice?> GetDeviceItemByDeviceSn(string deviceSn)
        {
            return await dbMySql.Devices.FirstOrDefaultAsync(x => x.DeviceSn == deviceSn);
        }

        /// <summary>
        /// 添加设备信息
        /// </summary>
        /// <param name="devices">设备信息</param>
        /// <returns></returns>
        public async Task<bool> AddSysDevice(TDevice[] devices)
        {
            if (devices != null) 
            {
                dbMySql.Devices.AddRange(devices);
                await dbMySql.SaveChangesAsync();
            }
            return true;
        }

        /// <summary>
        /// 修改设备信息
        /// </summary>
        /// <param name="sysDevice">设备信息</param>
        /// <returns></returns>
        public async Task<bool> UpdateSysDeviceItem(TDevice sysDevice)
        {
            var device = await dbMySql.Devices.FindAsync(sysDevice.Id);
            if (device == null)
            {
                return false;
            }

            device.DeviceTypeId = sysDevice.DeviceTypeId;
            device.DeptId = sysDevice.DeptId;
            device.Status = sysDevice.Status;

            await dbMySql.SaveChangesAsync();
           
            return true;
        }

        /// <summary>
        /// 设备类型
        /// </summary>
        /// <param name="instrutype"></param>
        /// <returns></returns>
        public static string DeviceType(string instrutype)
        {
            string deviceType = string.Empty;

            if (H.Contains(instrutype))
            {
                deviceType = "00";
            }
            else if (S.Contains(instrutype))
            {
                deviceType = "32";
            }
            else if (L.Contains(instrutype))
            {
                deviceType = "73";
            }
            else if (T.Contains(instrutype))
            {
                deviceType = "80";
            }
            return deviceType;
        }

        /// <summary>
        /// 查询设备类型信息
        /// </summary>
        /// <param name="deviceType">设备类型</param>
        /// <returns></returns>
        public async Task<List<SysDeviceType>> GetDeviceTypeItemAsync()
        {
            return await dbMySql.DeviceTypes.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// 查询设备类型信息
        /// </summary>
        /// <param name="deviceType">设备类型</param>
        /// <returns></returns>
        public int GetDeviceTypeIdByDeviceType(string deviceType)
        {
            int deviceTypeId = 0;

            SysDeviceType? sysDeviceType = dbMySql.DeviceTypes.AsNoTracking().ToListAsync().Result.Find(x => x.Equals(deviceType));

            if (sysDeviceType != null)
            {
                deviceTypeId = sysDeviceType.Id;
            }

            return deviceTypeId;
        }

        /// <summary>
        /// 设备信息格式转换处理
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        private static DeviceList ItemToDTO(DeviceList device) =>
            new()
            {
                Instruno = !string.IsNullOrEmpty(device.Instruno) ? device.Instruno.Trim() : device.Instruno,
                Instrutype = !string.IsNullOrEmpty(device.Instrutype) ? device.Instrutype.Trim() : device.Instrutype,
                Instruproducer = !string.IsNullOrEmpty(device.Instruproducer) ? device.Instruproducer.Trim() : device.Instruproducer,
                Instrubuydate = device.Instrubuydate,
                Instruaff = !string.IsNullOrEmpty(device.Instruaff) ? device.Instruaff.Trim() : device.Instruaff,
                Instrustatus = !string.IsNullOrEmpty(device.Instrustatus) ? device.Instrustatus.Trim() : device.Instrustatus,
                PoliceNo = !string.IsNullOrEmpty(device.PoliceNo) ? device.PoliceNo.Trim() : device.PoliceNo,
                DeviceType = !string.IsNullOrEmpty(device.Instrutype) ? DeviceType(device.Instrutype.Trim()) : "99"
            };

    }
}
