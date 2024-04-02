using DarkLoop.Azure.Functions.Authorization;
using FunctionApp.Helpers;
using FunctionApp.Models.DAO;
using FunctionApp.Models.DTO.Items;
using FunctionApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FunctionApp.Triggers
{
    //[FunctionAuthorize]
    //[Authorize(Policy = "Admin")]
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


        //[Function("Identity")]
        //[OpenApiOperation(operationId: nameof(Identity), tags: ["Test Identity"],
        //    Summary = "Test Identity", Description = "Protect api from identity")]
        //[OpenApiSecurity("oidc_auth",
        //             SecuritySchemeType.OpenIdConnect,
        //             OpenIdConnectUrl = "https://demo.duendesoftware.com/.well-known/openid-configuration",
        //             OpenIdConnectScopes = "openid,profile")]
        //[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK,
        //    contentType: "application/json", bodyType: typeof(string), Summary = "Test Identity Response",
        //    Description = "Test Identity Response")]
        //public async Task<IActionResult> Identity([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "identity")] HttpRequest req)
        //{
        //    var headers = req.Headers.ToDictionary(p => p.Key, p => (string)p.Value);
        //    var handler = new JwtSecurityTokenHandler();
        //    var hasAuth = headers.TryGetValue("Authorization", out string auth);

        //    if (!hasAuth)
        //    {
        //        return new UnauthorizedResult();
        //    }

        //    var token = handler.ReadJwtToken(auth.Split(' ').Last());
        //    var claims = token.Claims;
        //    var content = new { headers = headers, claims = claims };

        //    return new NoContentResult();
        //}







        [Function("List")]
        [OpenApiOperation(operationId: nameof(List), tags: ["Item"],
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
        [OpenApiOperation(operationId: nameof(Get), tags: ["Item"],
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


        [FunctionAuthorize]
        [Authorize(Policy = "Admin")]
        [Function("Create")]
        [OpenApiOperation(operationId: nameof(Create), tags: ["Item"],
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
            if (req.HttpContext.User.Identity.IsAuthenticated)
            {

            }

            //check claim
            bool hasSub = req.HttpContext.User.Claims.TryGetClaim("sub", out string sub);



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


        [FunctionAuthorize]
        [Authorize(Policy = "Admin")]
        [Function("Update")]
        [OpenApiOperation(operationId: nameof(Update), tags: ["Item"],
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


        [FunctionAuthorize]
        [Authorize(Policy = "Admin")]
        [Function("Delete")]
        [OpenApiOperation(operationId: nameof(Delete), tags: ["Item"],
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