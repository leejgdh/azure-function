using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace FunctionApp.Helpers
{
    /// <summary>
    /// Http Request의 Content를 쉽게 Deserlize 하기 위함.
    /// </summary>
    public static class FuncHttpHelper
    {
        public static async Task<TaskBase<T>> ParseBodyAsync<T>(this HttpRequest req)
        {
            var result = new TaskBase<T>(false);

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                var data = JsonConvert.DeserializeObject<T>(requestBody);

                result.IsSuccess = true;
                result.Result = data;

            }
            catch (Exception ex)
            {

                result.Exception = ex;

            }


            return result;
        }
    }
}
