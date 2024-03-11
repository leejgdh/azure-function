using FunctionApp.Helpers;
using FunctionApp.Models.DAO;
using FunctionApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FunctionApp.Triggers
{
    public class ItemTrigger
    {
        private readonly ILogger<ItemTrigger> _logger;
        private readonly IItemService _itemService;


        public ItemTrigger(
            IItemService itemService,
            ILogger<ItemTrigger> logger)
        {
            _itemService = itemService;
            _logger = logger;
        }

        [Function("List")]
        public async Task<IActionResult> List([HttpTrigger(AuthorizationLevel.Function, "get", Route = "items")] HttpRequest req)
        {
            //QueryString ¾ò´Â¹ý
            string name = req.Query["name"];

            var items = _itemService.Query();

            var result = await items.ToListAsync();

            return new OkObjectResult(result);
        }


        [Function("Get")]
        public async Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Function, "get", Route = "items/{itemId}")] HttpRequest req,
            Guid itemId)
        {

            var result = await _itemService.GetAsync(itemId);

            if (result.IsSuccess)
            {

                return new OkObjectResult(result.Result);
            }
            else
            {
                return new BadRequestObjectResult(result.Message);
            }
        }



        [Function("Create")]
        public async Task<IActionResult> Create([HttpTrigger(AuthorizationLevel.Function, "post", Route = "items")] HttpRequest req)
        {

            var parse = await req.ParseBodyAsync<Item>();

            if (parse.IsSuccess)
            {
                var insertRes = await _itemService.InsertAsync(parse.Result);

                if (insertRes.IsSuccess)
                {

                    return new OkObjectResult(insertRes.Result);
                }
                else
                {
                    return new BadRequestObjectResult(insertRes.Message);
                }
            }
            else
            {
                return new BadRequestObjectResult(parse.Message);
            }

        }


        [Function("Update")]
        public async Task<IActionResult> Update([HttpTrigger(AuthorizationLevel.Function, "put", Route = "items/{itemId}")] HttpRequest req,
            Guid itemId)
        {

            var parse = await req.ParseBodyAsync<Item>();

            if (parse.IsSuccess)
            {
                var insertRes = await _itemService.UpdateAsync(parse.Result);

                if (insertRes.IsSuccess)
                {

                    return new OkObjectResult(insertRes.Result);
                }
                else
                {
                    return new BadRequestObjectResult(insertRes.Message);
                }
            }
            else
            {
                return new BadRequestObjectResult(parse.Message);
            }
        }


        [Function("Delete")]
        public async Task<IActionResult> Delete([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "items/{itemId}")] HttpRequest req,
            Guid itemId)
        {

            var deleteRes = await _itemService.DeleteAsync(itemId);

            return new NoContentResult();
        }
    }
}