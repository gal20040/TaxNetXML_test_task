using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml;
using TaxNetXML.Models;

//TODO: если Entity Framework нельзя использовать, тогда переделать метод Index - не использовать Entity Framework
//TODO: если Entity Framework нельзя использовать, тогда константы со строками вынести в отдельный настроечный файл
//TODO: разобраться с методом Dispose
//TODO: если Entity Framework можно использовать, тогда попробовать реализовать insert и update через фреймворк - см. метод ReadFromXml


namespace TaxNetXML.Controllers {
    public class HomeController : Controller {
        UserContext db = new UserContext();
        private const string pathToXML = "D:\\gal20040\\CSharp\\TaxNetXML\\TaxNetXML\\App_Data\\usersdb.xml";
        private const string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='|DataDirectory|\Userstore.mdf';Integrated Security=True";
        private const string sql = "SELECT * FROM Users";

        //public ViewResult Index() {
        //    // получаем из бд все объекты User
        //    IEnumerable<User> users = db.Users;
        //    // передаем все объекты в динамическое свойство Users в ViewBag
        //    ViewBag.Users = users;
        //    // возвращаем представление
        //    return View();
        //}

        //
        // Summary:
        //     Получает из БД список пользователей и отправляет его в представление.
        public async Task<ActionResult> Index() {
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

                //adapter.Fill(ds);

                //DataTable dt = ds.Tables[0];
                //// добавим новую строку
                //DataRow newRow = dt.NewRow();
                //newRow["Name"] = "Alice2";
                //newRow["Age"] = 240;
                //dt.Rows.Add(newRow);

                //создаем объект SqlCommandBuilder
                //SqlCommandBuilder commandBuilder = new SqlCommandBuilder(adapter);
                //adapter.DeleteCommand = commandBuilder.GetDeleteCommand(true);
                //adapter.UpdateCommand = commandBuilder.GetUpdateCommand(true);
                //adapter.InsertCommand = commandBuilder.GetInsertCommand(true);

                /*
DeleteCommand=DELETE FROM [Users]
              WHERE (([Id] = @Original_Id) AND ([Name] = @Original_Name) AND ([Age] = @Original_Age))
UpdateCommand=UPDATE [Users] SET [Name] = @Name, [Age] = @Age
              WHERE (([Id] = @Original_Id) AND ([Name] = @Original_Name) AND ([Age] = @Original_Age))
InsertCommand=INSERT INTO [Users] ([Name], [Age])
              VALUES (@Name, @Age)*/

                //ds.Clear();
                //ds.ReadXml(pathToXML);
                //s = ds.GetXml();
                //adapter.Update(ds);
                ////adapter.Update(dt); //альтернативный способ -обновление только одной таблицы

                //заново получаем данные из бд
                //очищаем полностью DataSet
                //ds.Clear();
                //// перезагружаем данные
                //adapter.Fill(ds);

                connection.Close();
            }
            return "Данные загружены в базу данных. " + queriesString;
        }
    }
}