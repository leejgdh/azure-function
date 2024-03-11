using FunctionApp.Helpers;
using FunctionApp.Models.DAO;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.Services
{
    public interface IItemService
    {
        Task<TaskBase<Item>> DeleteAsync(Guid Id);
        Task<TaskBase<Item>> GetAsync(Guid id);
        Task<TaskBase<Item>> InsertAsync(Item item);
        IQueryable<Item> Query();
        Task<TaskBase<Item>> UpdateAsync(Item item);
    }

    public class ItemService : IItemService
    {
        MyDbContext _context;

        public ItemService(
            MyDbContext context
            )
        {
            _context = context;
        }



        public IQueryable<Item> Query()
        {

            var query = _context.Items;

            return query;
        }


        public async Task<TaskBase<Item>> GetAsync(Guid id)
        {

            var result = new TaskBase<Item>(false);


            var entity = await _context.Items.FirstOrDefaultAsync(e => e.Id == id);

            if (entity != null)
            {

                result.IsSuccess = true;
                result.Result = entity;

                return result;
            }
            else
            {
                result.Message = "Item not found";

                return result;
            }
        }

        public async Task<TaskBase<Item>> InsertAsync(Item item)
        {
            var result = new TaskBase<Item>(false);

            _context.Items.Add( item );

            await _context.SaveChangesAsync();


            result.IsSuccess = true;
            result.Result = item;

            return result;
        }


        public async Task<TaskBase<Item>> UpdateAsync(Item item)
        {
            var result = new TaskBase<Item>(false);

            var entityRes = await GetAsync(item.Id);

            if (entityRes.IsSuccess)
            {

                entityRes.Result.Name = item.Name;

                await _context.SaveChangesAsync();

                result.IsSuccess = true;
                result.Result = item;

                return result;

            }
            else
            {
                return entityRes;
            }


        }



        public async Task<TaskBase<Item>> DeleteAsync(Guid Id)
        {

            var result = new TaskBase<Item>(true);

            var entityRes = await GetAsync(Id);

            if (entityRes.IsSuccess)
            {

                _context.Items.Remove(entityRes.Result);
                await _context.SaveChangesAsync();

            }

            return result;
        }

    }
}
