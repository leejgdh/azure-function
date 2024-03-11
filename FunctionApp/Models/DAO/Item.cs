namespace FunctionApp.Models.DAO
{
    public class Item
    {
        public Item()
        {
                
        }

        public Item(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }

        public string Name { get; set; }    
    }
}
