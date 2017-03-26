using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using TaxNetXML.Models;

//TODO если Entity Framework нельзя использовать, тогда переделать метод Index - не использовать Entity Framework
//TODO если Entity Framework нельзя использовать, тогда константы со строками вынести в отдельный настроечный файл
//TODO разобраться с методом Dispose
//TODO если Entity Framework можно использовать, тогда попробовать реализовать insert и update через фреймворк - см. метод ReadFromXml

namespace TaxNetXML.Controllers {
    public class HomeController : Controller {
        UserContext db = new UserContext();
        private string pathToXML = "D:\\gal20040\\CSharp\\TaxNetXML\\TaxNetXML\\App_Data\\usersdb.xml";
        //private static string pathToXML = Server.MapPath("~/App_Data/usersdb.xml");//"~\\App_Data\\usersdb.xml";
        private const string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='|DataDirectory|\Userstore.mdf';Integrated Security=True";
        private const string sql = "SELECT * FROM Users";

        //
        // Summary:
        //     Получает из БД список пользователей и отправляет его в представление.
        public async Task<ActionResult> Index() {
            string pathToXML = Server.MapPath("~/App_Data/usersdb.xml");
            IEnumerable<User> users = await db.Users.ToListAsync();
            ViewBag.Users = users;
            return View("Index");
        }

        //
        // Summary:
        //     Получает из БД список пользователей и записывает его в xml файл (см. переменную pathToXML).
        //
        // Returns:
        //     Возвращает сообщение об успешном сохранении данных в файл.
        public string WriteToXml() {
            using (SqlConnection connection = new SqlConnection(connectionString)) {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);

                DataSet dataSet = new DataSet("Users");
                DataTable dataTable = new DataTable("User");
                dataSet.Tables.Add(dataTable);
                adapter.Fill(dataSet.Tables["User"]);

                dataSet.WriteXml(pathToXML);
                connection.Close();
                return "Данные сохранены в файл";
            }
        }

        //
        // Summary:
        //     Считывает из xml файла (см. переменную pathToXML) данные по пользователям
        //     и в цикле перебирает их и составляет один общий SQL запрос на добавление новых пользователей.
        //
        // Returns:
        //     Возвращает сообщение об успешной загрузке данных в базу данных.
        public string ReadFromXml() {
            const string insertQueryTemplate = "INSERT INTO {0:d} (Name, Age) VALUES ('{1:d}', {2:d})";

            string queriesString = ""; //строка для нескольких SQL запросов
            string name = null;
            int age = 0;

            using (SqlConnection connection = new SqlConnection(connectionString)) {
                DataSet ds = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);
                SqlCommand sqlCommand;

                XmlReader xmlFile = XmlReader.Create(pathToXML, new XmlReaderSettings());
                ds.ReadXml(xmlFile);

                connection.Open();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++) {
                    name =                ds.Tables[0].Rows[i].ItemArray[0].ToString();
                    age = Convert.ToInt32(ds.Tables[0].Rows[i].ItemArray[1]);
                    queriesString = string.Concat(string.Format(insertQueryTemplate, "Users", name, age),
                                                  " ", queriesString);
                }

                sqlCommand = new SqlCommand(queriesString, connection);
                adapter.InsertCommand = sqlCommand;
                adapter.InsertCommand.ExecuteNonQuery();

                connection.Close();
                //TODO удалить обработанный файл
            }
            return "Данные загружены в базу данных. " + queriesString;
        }

        [HttpPost]
        public string Upload(HttpPostedFileBase upload) {
            if (upload != null) {
                // получаем имя файла
                string fileName = System.IO.Path.GetFileName(upload.FileName);
                // сохраняем файл в папку Files в проекте
                pathToXML = Server.MapPath("~/UploadedFiles/" + fileName);
                upload.SaveAs(pathToXML);
            }
            return ReadFromXml();
        }
    }
}