using Microsoft.AspNetCore.Mvc;

namespace Recorder
{
    /// <summary>
    /// 许可控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LicenseController : Controller
    {
        private readonly EncryptionService _encryption = new();

        /// <summary>
        /// 获取许可串码
        /// </summary>
        /// <param name="expiration_time">到期时间</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> Get(string expiration_time)
        {
            string key;
            try
            {
                DateTime time = DateTime.Parse(expiration_time);
                key = await _encryption.Encryption(time);
            }
            catch (Exception e)
            {
                key = e.Message;
            }
            return key;
        }
    }
}
