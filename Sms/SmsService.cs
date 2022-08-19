using System.Text;

namespace Recorder
{
    public class SmsService
    {
        /// <summary>
        /// Get请求外部API
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="httpClient"></param>
        /// <returns></returns>
        public async Task<string> DoGetRequestAsync(string uri)
        {
            try
            {
                using HttpClient client = new();
                client.BaseAddress = new Uri(uri);
                HttpResponseMessage response = await client.GetAsync(uri);
                var result = response.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return result;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// POST请求外部API
        /// </summary>
        /// <param name="uri">接口地址</param>
        /// <param name="parameter">接口参数</param>
        /// <param name="ContentType">请求头类型</param>
        /// <returns></returns>
        public async Task<string> DoPostRequest(string uri, string parameter, string ContentType)
        {
            try
            {
                using HttpClient client = new();
                // 请求方式
                client.DefaultRequestHeaders.Add("Method", "Post");
                // 请求头类型
                var content = new StringContent(parameter, Encoding.UTF8, ContentType);
                HttpResponseMessage response = await client.PostAsync(uri, content);
                var result = response.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return result;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
