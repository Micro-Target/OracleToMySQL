using Microsoft.EntityFrameworkCore;

namespace Recorder
{
    public class SyncMessageService
    {
        private readonly EntityMySqlDBContext dbMySql = new();

        /// <summary>
        /// 查询短信信息
        /// </summary>
        /// <returns></returns>
        public async Task<List<TMessage>> GetMessageItemAsync()
        {
            try
            {
                return await dbMySql.Messages.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 查询短信信息
        /// </summary>
        /// <returns></returns>
        public async Task<List<TMessage>> GetMessageItemAsync(int flag)
        {
            try
            {
                return await dbMySql.Messages.Where(x => x.Flag.Equals(flag)).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 查询短信信息
        /// </summary>
        /// <returns></returns>
        public async Task<TMessage?> GetMessageAsync()
        {
            try
            {
                return await dbMySql.Messages.Where(x => x.Flag.Equals(0)).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 添加短信信息
        /// </summary>
        /// <param name="messages">短信信息</param>
        /// <returns></returns>
        public async Task<bool> AddMessage(TMessage[] messages)
        {
            try
            {
                if (messages != null)
                {
                    dbMySql.Messages.AddRange(messages);
                    await dbMySql.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 添加短信信息
        /// </summary>
        /// <param name="messages">短信信息</param>
        /// <returns></returns>
        public async Task<bool> UpdateMessage(TMessage[] messages)
        {
            try
            {
                if (messages != null)
                {
                    dbMySql.Messages.UpdateRange(messages);
                    await dbMySql.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
