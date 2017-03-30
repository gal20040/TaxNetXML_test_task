namespace TaxNetXML.Models {
    public static class ConstantData {
        public static readonly string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='|DataDirectory|\FileStore.mdf';Integrated Security=True";
        public static readonly string querySelectFromFiles = "SELECT * FROM Files";
        public static readonly string insertQueryTemplate = "INSERT INTO {0:d} (FileName, FileVersion, ChangeDate) VALUES ('{1:d}', '{2:d}', '{3:d}')";
        public static readonly string dateFormat = "yyyy-MM-ddTHH:mm:ss.ffzzz";
    }
}