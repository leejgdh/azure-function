namespace FunctionApp.Models.DAO
{
    public class Item
    {
        public Item()
        {
                
        }

        public Item(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }    
    }
}
