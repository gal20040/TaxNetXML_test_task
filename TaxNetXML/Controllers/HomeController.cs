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
            IEnumerable<File> files = await db.Files.ToListAsync();
            ViewBag.Files = files;

            //todo сделать передачу заголовков таблицы
            //string[] columnHeaders = new string[] { "A", "B", "C" };
            //ViewBag.ColumnHeaders = columnHeaders;

            return View("Index");
        }
    }
}