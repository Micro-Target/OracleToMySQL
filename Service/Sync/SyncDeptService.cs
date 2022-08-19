using Microsoft.EntityFrameworkCore;

namespace Recorder
{
    public class SyncDeptService
    {
        private readonly EntityMySqlDBContext dbMySql = new();
        private readonly EntityOracleDBContext dbOracle = new();

        /// <summary>
        /// 查询部门信息
        /// </summary>
        /// <returns></returns>
        public async Task<List<PoliceStation>> GetPoliceStationItemAsync()
        {
            try
            {
                return await dbOracle.PoliceStations.AsNoTracking().Select(x => ItemToDTO(x)).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 查询部门信息
        /// </summary>
        /// <returns></returns>
        public async Task<List<SysDept>> GetSysDeptItemAsync()
        {
            try
            {
                return await dbMySql.Depts.AsNoTracking().ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 添加部门信息
        /// </summary>
        /// <param name="dept">部门信息</param>
        /// <returns></returns>
        public async Task<bool> AddSysDept(SysDept[] dept)
        {
            try
            {
                if (dept != null)
                {
                    dbMySql.Depts.AddRange(dept);
                    await dbMySql.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return true;
        }

        /// <summary>
        /// 部门信息格式转换处理
        /// </summary>
        /// <param name="station"></param>
        /// <returns></returns>
        private static PoliceStation ItemToDTO(PoliceStation station) =>
            new()
            {
                Policestano = station.Policestano,
                Policestation = !string.IsNullOrEmpty(station.Policestation) ? station.Policestation.Trim() : station.Policestation,
                Policestatype = !string.IsNullOrEmpty(station.Policestatype) ? station.Policestatype.Trim() : station.Policestatype,
                Policestaaff = station.Policestaaff == 0 ? -1 : station.Policestaaff,
                DisplayNo = station.DisplayNo,
                IsDutyFlag = station.IsDutyFlag,
            };
    }
}
