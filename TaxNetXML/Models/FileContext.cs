using System.Data.Entity;

namespace TaxNetXML.Models {

    //
    // Summary:
    //     Контекст данных для работы с моделью.
    public class FileContext : DbContext {
        public DbSet<File> Files { get; set; }
    }
}