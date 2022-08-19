using System.Text;

namespace Recorder
{
    public class EncryptionService
    {
        // 字符串
        private const string regular = "==";
        // 存根
        private const string stub = "hH!";

        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="expiration_time">到期时间</param>
        /// <returns></returns>
        public Task<string> Encryption(DateTime expiration_time)
        {
            DateTimeOffset offset = new(expiration_time);
            long time_stamp = offset.ToUnixTimeMilliseconds();
            string encryption = Convert.ToBase64String(Encoding.UTF8.GetBytes(time_stamp.ToString())).Replace(regular, stub);
            return Task.FromResult(encryption);
        }

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="license_stamp">编码字符串</param>
        /// <returns></returns>        
        public Task<string> Decrypt(string license_stamp)
        {
            string descrypt = Encoding.UTF8.GetString(Convert.FromBase64String(license_stamp.Replace(stub, regular)));
            return Task.FromResult(descrypt);
        }

        /// <summary>
        /// 许可到期较验
        /// </summary>
        /// <param name="license_stamp">编码字符串</param>
        /// <returns></returns>
        public async Task<bool> LicenseAsync(string license_stamp)
        {
            bool result = false;
            string time_stamp = await Decrypt(license_stamp);
            DateTime expirationTime = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(time_stamp)).ToLocalTime().DateTime;
            int day = (DateTime.Now - expirationTime).Days;
            if (day > 0)
            {
                result = true;
            }
            return result;
        }
    }
}
