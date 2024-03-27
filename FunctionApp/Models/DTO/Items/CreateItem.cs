using FunctionApp.Models.DAO;

namespace FunctionApp.Models.DTO.Items
{
    public class CreateItem
    {
        public class _Request
        {

            public string Name { get; set; }


            public Item ToEntity()
            {
                var entity = new Item()
                {
                    Name = Name,
                };

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
