namespace TaxNetXML.Models {
    public static class ConstantData {
        public static readonly string CONNECTION_STRING = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='|DataDirectory|\FileStore.mdf';Integrated Security=True";
        public static readonly string querySelectFromFiles = "SELECT * FROM Files";
        public static readonly string insertQueryTemplate = "INSERT INTO {0:d} (Name, FileVersion, DateTime) VALUES ('{1:d}', '{2:d}', '{3:d}')";
        public static readonly string dateFormatWithDash = "yyyy-MM-ddTHH:mm:ss.ffzzz";
        public static readonly string dateFormaForFileName = "yyyy-MM-ddTHH-mm-sszzz";
        public static readonly string file_type = "application/xml";
    }
}