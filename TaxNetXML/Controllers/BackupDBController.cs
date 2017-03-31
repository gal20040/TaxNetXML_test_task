using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using TaxNetXML.Models;

namespace TaxNetXML.Controllers {
    public class BackupDBController : Controller {
        private FileContext db = new FileContext();

        //
        // Summary:
        //     Определяет полный путь и имя выходного файла.
        //     Далее запускает методы: WriteToXml и GiveFileToUser.
        //
        // Returns:
        //     Ретранслирует выгруженный файл пользователю.
        public FileResult BackupDBToXML() { //FileResult
            string pathToOutputXML = "";
            pathToOutputXML = Server.MapPath("~/Files/Output/db_backup.xml");
            WriteToXml(pathToOutputXML);

            return GiveFileToUser(pathToOutputXML);
        }

        //
        // Summary:
        //     Получает из БД данные и записывает их в xml файл.
        //
        // Parameters:
        //   pathToOutputXML:
        //     Полный путь до файла в системе + его имя и расширение.
        private void WriteToXml(string pathToOutputXML) { //void
            using (SqlConnection connection = new SqlConnection(ConstantData.CONNECTION_STRING)) {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(ConstantData.querySelectFromFiles, connection);

                DataSet dataSet = new DataSet("Files");
                DataTable dataTable = new DataTable("File");
                dataSet.Tables.Add(dataTable);
                adapter.Fill(dataSet.Tables["File"]);

                dataSet.WriteXml(pathToOutputXML);
                connection.Close();
            }
        }

        //
        // Summary:
        //     Выгружает файл пользователю.
        //
        // Parameters:
        //   pathToOutputXML:
        //     Полный путь до файла в системе + его имя и расширение.
        //
        // Returns:
        //     Выгружает файл пользователю.
        private FileResult GiveFileToUser(string pathToOutputXML) {
            string file_name = System.IO.Path.GetFileName(pathToOutputXML);

            return File(pathToOutputXML, ConstantData.file_type, file_name);
        }
    }
}