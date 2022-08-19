using Microsoft.EntityFrameworkCore;

namespace Recorder
{
    public class SmsDataService
    {
        private readonly EntityMySqlDBContext dbMySql = new();

        /// <summary>
        /// 查询酒检信息
        /// </summary>
        /// <returns></returns>
        public async Task<List<SmsData>> GetSmsItemAsync()
        {
            return await dbMySql.SmsDatas.ToListAsync();
        }
    }
}
