using System.Data.Entity;

namespace TaxNetXML.Models {
    public class UserContext : DbContext {
        public DbSet<User> Users { get; set; }
    }
}