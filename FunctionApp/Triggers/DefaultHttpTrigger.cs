using FunctionApp.Models.DAO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionApp.Triggers
{
    public class DefaultHttpTrigger
    {
        private readonly ILogger<DefaultHttpTrigger> _logger;

        public DefaultHttpTrigger(ILogger<DefaultHttpTrigger> logger)
        {
            _logger = logger;
        }


        List<Item> Items = new List<Item>()
        {
            new Item(1,"1번 아이템"),
            new Item(2,"2번 아이템"),
        };


        [Function("List")]
        public IActionResult List([HttpTrigger(AuthorizationLevel.Function, "get", Route = "items")] HttpRequest req)
        {
            //QueryString 얻는법
            string name = req.Query["name"];


            return new OkObjectResult(Items);
        }


        [Function("Get")]
        public IActionResult Get([HttpTrigger(AuthorizationLevel.Function, "get", Route = "items/{itemId}")] HttpRequest req,
            int itemId)
        {

            var item = Items.FirstOrDefault(e => e.Id == itemId);

            return new OkObjectResult(item);
        }



        [Function("Create")]
        public async Task<IActionResult> Create([HttpTrigger(AuthorizationLevel.Function, "post", Route = "items")] HttpRequest req,
            Item item)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                var data = JsonConvert.DeserializeObject<Item>(requestBody);
            }
            catch (Exception ex)
            {


            }

            return new OkObjectResult(item);
        }


        [Function("Update")]
        public async Task<IActionResult> Update([HttpTrigger(AuthorizationLevel.Function, "put", Route = "items/{itemId}")] HttpRequest req,
            int itemId, Item item)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                var data = JsonConvert.DeserializeObject<Item>(requestBody);
            }
            catch (Exception ex)
            {


            }

            return new OkObjectResult(item);
        }


        [Function("Delete")]
        public IActionResult Delete([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "items/{itemId}")] HttpRequest req,
            int itemId)
        {

            //Delete 
            var item = Items.FirstOrDefault(e => e.Id == itemId);

            return new OkObjectResult(item);
        }
    }
}
