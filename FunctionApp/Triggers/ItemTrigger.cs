using FunctionApp.Helpers;
using FunctionApp.Models.DAO;
using FunctionApp.Models.DTO.Items;
using FunctionApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
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
        [OpenApiOperation(operationId: nameof(List), tags: new[] { "Get item list" },
            Summary = "Get item list", Description = "Get item list")]
        [OpenApiParameter(name:"name", In = Microsoft.OpenApi.Models.ParameterLocation.Query,Required = false, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK,
            contentType: "application/json", bodyType: typeof(List<Item>), Summary = "item list Response",
            Description = "item list Response")]
        public async Task<IActionResult> List([HttpTrigger(AuthorizationLevel.Function, "get", Route = "items")] HttpRequest req)
        {
            //QueryString ¾ò´Â¹ý
            string name = req.Query["name"];

            var items = _itemService.Query();

            if (!string.IsNullOrEmpty(name))
            {
                items = items.Where(e => e.Name.Contains(name));
            }

            var result = await items.ToListAsync();

            return new OkObjectResult(result);
        }


        [Function("Get")]
        [OpenApiOperation(operationId: nameof(Get), tags: new[] { "Get item" },
            Summary = "Get item", Description = "Get item")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK,
            contentType: "application/json", bodyType: typeof(Item), Summary = "item Response",
            Description = "item Response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound,
            contentType: "text", bodyType: typeof(string), Summary = "item Response",
            Description = "item Response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest,
            contentType: "text", bodyType: typeof(string), Summary = "item Response",
            Description = "item Response")]
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
        [OpenApiOperation(operationId: nameof(Create), tags: ["Create item"],
            Summary = "Create Item", Description = "Create Item")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(CreateItem._Request),
            Required = true, Description = "Create Item")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created,
            contentType: "application/json", bodyType: typeof(CreateItem._Response), Summary = "Create Item Response",
            Description = "Create Item Response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest,
            contentType: "text", bodyType: typeof(string), Summary = "item Response",
            Description = "item Response")]
        public async Task<IActionResult> Create([HttpTrigger(AuthorizationLevel.Function, "post", Route = "items")] HttpRequest req)
        {

            var parse = await req.ParseBodyAsync<CreateItem._Request>();

            if (parse.IsSuccess)
            {
                var insertRes = await _itemService.InsertAsync(parse.Result.ToEntity());

                if (insertRes.IsSuccess)
                {
                    var response = new CreateItem._Response(insertRes.Result);

                    return new CreatedResult($"/api/item/{response.Id}", response);
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
        [OpenApiOperation(operationId: nameof(Update), tags: ["Update item"],
            Summary = "Update Item", Description = "Update Item")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(UpdateItem._Request),
            Required = true, Description = "Update Item")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Accepted,
            contentType: "application/json", bodyType: typeof(UpdateItem._Response), Summary = "Update Item Response",
            Description = "Update Item Response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest,
            contentType: "text", bodyType: typeof(string), Summary = "item Response",
            Description = "item Response")]
        public async Task<IActionResult> Update([HttpTrigger(AuthorizationLevel.Function, "put", Route = "items/{itemId}")] HttpRequest req,
            Guid itemId)
        {

            var parse = await req.ParseBodyAsync<UpdateItem._Request>();

            if (parse.IsSuccess)
            {
                var updateRes = await _itemService.UpdateAsync(parse.Result.ToEntity());

                if (updateRes.IsSuccess)
                {
                    var response = new UpdateItem._Response(updateRes.Result);

                    return new AcceptedResult($"/api/item/{response.Id}", response);
                }
                else
                {
                    return new BadRequestObjectResult(updateRes.Message);
                }
            }
            else
            {
                return new BadRequestObjectResult(parse.Message);
            }
        }


        [Function("Delete")]
        [OpenApiOperation(operationId: nameof(Delete), tags: ["Delete item"],
            Summary = "Delete Item", Description = "Delete Item")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Item),
            Required = true, Description = "Delete Item")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NoContent,
            contentType: "application/json", bodyType: typeof(Item), Summary = "Delete Item Response",
            Description = "Delete Item Response")]
        public async Task<IActionResult> Delete([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "items/{itemId}")] HttpRequest req,
            Guid itemId)
        {

            var deleteRes = await _itemService.DeleteAsync(itemId);

            return new NoContentResult();
        }
    }
}