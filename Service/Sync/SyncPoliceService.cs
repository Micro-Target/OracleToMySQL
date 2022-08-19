using Microsoft.EntityFrameworkCore;

namespace Recorder
{
    public class SyncPoliceService
    {
        private readonly EntityMySqlDBContext dbMySql = new();
        private readonly EntityOracleDBContext dbOracle = new();

        /// <summary>
        /// 查询警员信息
        /// </summary>
        /// <returns></returns>
        public async Task<List<TestPolice>> GetTestPoliceItemAsync()
        {
            try
            {
                return await dbOracle.TestPolices.AsNoTracking().Select(x => ItemToDTO(x)).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 查询警员信息
        /// </summary>
        /// <returns></returns>
        public async Task<List<TPolice>> GetTPolicetItemAsync()
        {
            try
            {
                return await dbMySql.Polices.AsNoTracking().ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 添加警员信息
        /// </summary>
        /// <param name="police">警员信息</param>
        /// <returns></returns>
        public async Task<bool> AddTPolice(TPolice[] police)
        {
            try
            {
                if (police != null)
                {
                    dbMySql.Polices.AddRange(police);
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
        /// 警员信息格式转换处理
        /// </summary>
        /// <param name="police">警员信息</param>
        /// <returns></returns>
        private static TestPolice ItemToDTO(TestPolice police) =>
            new()
            {
                TestPoliceId = police.TestPoliceId,
                TestPoliceNo = !string.IsNullOrEmpty(police.TestPoliceNo) ? police.TestPoliceNo.Trim() : police.TestPoliceNo,
                TestPoliceName = !string.IsNullOrEmpty(police.TestPoliceName) ? police.TestPoliceName.Trim() : police.TestPoliceName,
                PoliceStaNo = police.PoliceStaNo,
                Phone = !string.IsNullOrEmpty(police.Phone) ? police.Phone.Trim() != "null" ? police.Phone.Trim() : "" : police.Phone
            };
    }
}
