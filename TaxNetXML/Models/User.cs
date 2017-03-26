namespace TaxNetXML.Models
{
    public class User
    {
        //[Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int    Id   { get; set; } // ID пользователя
        public string Name { get; set; } // имя пользователя
        public int    Age  { get; set; } // возраст пользователя
    }
}