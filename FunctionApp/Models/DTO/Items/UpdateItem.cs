using FunctionApp.Models.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.Models.DTO.Items
{
    public class UpdateItem
    {
        public class _Request
        {
            public Guid Id { get; set; }

            public string Name { get; set; }


            public Item ToEntity()
            {
                var entity = new Item(Id, Name);

                return entity;
            }


        }

        public class _Response 
        {
            public _Response(Item entity)
            {
                Id = entity.Id;
                Name = entity.Name;
            }

            public Guid Id { get; set; }

            public string Name { get; set; }

        }
    }
}
