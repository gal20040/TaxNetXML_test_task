using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Mvc;
using TaxNetXML.Models;

//todo сделать автосоздание бд. если её нет
//TODO переделать подпись в _Layout.cshtml
//TODO разобраться с методом Dispose
//TODO если Entity Framework можно использовать, тогда попробовать реализовать insert и update через фреймворк - см. метод ReadFromXml

namespace TaxNetXML.Controllers {
    public class HomeController : Controller {
        private FileContext db = new FileContext();

        //TODO переделать на относительный путь
        //private string pathToXML; // = "D:\\gal20040\\CSharp\\TaxNetXML\\TaxNetXML\\App_Data\\filesdb.xml";
        //private static string pathToXML = Server.MapPath("~/App_Data/filesdb.xml");//"~\\App_Data\\filesdb.xml";

        //private const string CONNECTION_STRING = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='|DataDirectory|\FileStore.mdf';Integrated Security=True";
        //private const string sql = "SELECT * FROM Files";

        //
        // Summary:
        //     Получает из БД список данных и отправляет его в представление пользователю.
        public async Task<ActionResult> Index() {
            //string pathToXML = Server.MapPath("~/App_Data/filesdb.xml");
            IEnumerable<File> files = await db.Files.ToListAsync();
            ViewBag.Files = files;

            //todo сделать передачу заголовков таблицы
            //string[] columnHeaders = new string[] { "A", "B", "C" };
            //ViewBag.ColumnHeaders = columnHeaders;

            return View("Index");
        }

        ////
        //// Summary:
        ////     Определяет полный путь и имя выходного файла.
        ////     Далее запускает методы: WriteToXml и GiveFileToUser.
        ////
        //// Returns:
        ////     Ретранслирует выгруженный файл пользователю.
        //public FileResult BackupDBToXML() { //FileResult
        //    //string returnMessage = "";
        //    string pathToOutputXML = "";
        //    //try {
        //    pathToOutputXML = string.Concat(Server.MapPath("~/Files/Output/db_backup"),
        //                                        //DateTime.Now.ToString(ConstantData.dateFormaForFileName), //todo скорее всего придётся отказаться. пока не знаю, как удалять такие файлы
        //                                        ".xml");
        //        //returnMessage = returnMessage + 
        //        WriteToXml(pathToOutputXML);
        //        //return returnMessage;


        //        return GiveFileToUser(pathToOutputXML);
        //    //} catch (Exception e) {
        //    //    returnMessage = returnMessage + pathToOutputXML + e.ToString();
        //    //    return returnMessage;
        //    //}
        //}

        ////
        //// Summary:
        ////     Получает из БД данные и записывает их в xml файл.
        ////
        //// Parameters:
        ////   pathToOutputXML:
        ////     Полный путь до файла в системе + его имя и расширение.
        //private void WriteToXml(string pathToOutputXML) { //void
        //    //string returnMessage = "";
        //    SqlConnection connection = new SqlConnection(ConstantData.CONNECTION_STRING);

        //    //try {
        //        using (connection) {
        //            connection.Open();
        //            SqlDataAdapter adapter = new SqlDataAdapter(ConstantData.querySelectFromFiles, connection);

        //            DataSet dataSet = new DataSet("Files");
        //            DataTable dataTable = new DataTable("File");
        //            dataSet.Tables.Add(dataTable);
        //            adapter.Fill(dataSet.Tables["File"]);

        //            dataSet.WriteXml(pathToOutputXML);
        //        }
        //    //} catch (Exception e) {
        //    //    returnMessage = pathToOutputXML + e.ToString();
        //    //} finally {
        //    connection.Close();
        //    //}
        //    //return returnMessage;
        //}

        ////
        //// Summary:
        ////     Выгружает файл пользователю.
        ////
        //// Parameters:
        ////   pathToOutputXML:
        ////     Полный путь до файла в системе + его имя и расширение.
        ////
        //// Returns:
        ////     Выгружает файл пользователю.
        //private FileResult GiveFileToUser(string pathToOutputXML) { //FileResult
        //    //string returnMessage = "";
        //    SqlConnection connection = new SqlConnection(ConstantData.CONNECTION_STRING);

        //    //try {
        //        using (connection) {
        //            string file_name = System.IO.Path.GetFileName(pathToOutputXML);

        //            connection.Close();
        //            return File(pathToOutputXML, ConstantData.file_type, file_name);
        //        }
        //    //} catch (Exception e) {
        //    //    //string returnMessage = e.ToString();
        //    //    connection.Close();
        //    //    return e.ToString();
        //    //}
        //    //finally {
        //    //    connection.Close();
        //    //}
        //}
    }
}