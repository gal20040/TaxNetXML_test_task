using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using TaxNetXML.Models;

namespace TaxNetXML.Controllers {
    public class LoadXMLtoDBController : Controller {
        // GET: LoadXMLtoDB
        public ActionResult Index() {
            return View();
        }

        //
        // Summary:
        //     Получает от пользователя файл, сохраняет его и передаёт в метод загрузки данных в БД.
        //
        // Parameters:
        //   upload:
        //     Переданный с клиента на сервер файл.
        //
        // Returns:
        //     Если файл не передан, то возвращает соответствующее сообщение.
        //     Иначе ретранслирует сообщение об успешной загрузке данных в БД.
        [HttpPost]
        public string Upload(HttpPostedFileBase upload) {
            if (upload != null) {
                string returnMessage;
                string pathToInputXML = Server.MapPath("~/Files/Input/Input.xml"); //todo переделать на уникальное имя файла

                try {
                    upload.SaveAs(pathToInputXML);
                    returnMessage = ReadFromXml(pathToInputXML);
                } catch (IOException e) {
                    returnMessage = "Проблема при открытии файла: не существует или занят другим приложением.\r\n"
                                    + e.ToString();
                } catch (Exception e) {
                    returnMessage = e.ToString();
                }

                return returnMessage;
            }

            //todo переделать на сообщение на странице.
            return "Файл не был передан. Загрузка в базу данных не выполнена.";
        }

        //
        // Summary:
        //     Считывает из xml файла данные, в цикле перебирает их и составляет,
        //     а затем запускает один общий SQL запрос на добавление новых данных в БД.
        //
        // Parameters:
        //   pathToInputXML:
        //     Полный путь до файла в системе + его имя и расширение.
        //
        // Returns:
        //     Возвращает сообщение об успешной загрузке данных в БД.
        private string ReadFromXml(string pathToInputXML) {
            string queriesString = ""; //строка для нескольких SQL запросов
            string returnMessage = "";
            Models.File file = new Models.File();
            //Данные загружены в базу данных. INSERT INTO Files (FileName, FileVersion, ChangeDate) VALUES ('testNameFile', '', '30.03.2017') s1=30 марта 2017 г. s2=13:00:00 s3
            //2017-03-30T13:00:00.00+03:00
            XmlReader xmlFile = XmlReader.Create(pathToInputXML, new XmlReaderSettings());
            SqlConnection connection = new SqlConnection(ConstantData.connectionString);

            try {
                using (connection) {
                    //const string insertQueryTemplate = "INSERT INTO {0:d} (Name, Age) VALUES ('{1:d}', '{2:d}', '{3:d}')";
                    //string pathToInputXML = Server.MapPath("~/Files/Input/Input.xml"); //todo переделать на уникальное имя файла

                    DataSet ds = new DataSet();
                    SqlDataAdapter adapter = new SqlDataAdapter(ConstantData.querySelectFromFiles, connection);
                    SqlCommand sqlCommand;

                    ds.ReadXml(xmlFile);

                    connection.Open();
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    object dateObject;

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++) {
                        file.FileName = Convert.ToString(ds.Tables[0].Rows[i].ItemArray[0]);

                        dateObject = ds.Tables[0].Rows[i].ItemArray[1];
                        if (dateObject == null)
                            file.ChangeDate = DateTime.Now;
                        else
                            file.ChangeDate = DateTime.ParseExact(
                                                              Convert.ToString(dateObject),
                                                              ConstantData.dateFormat, provider
                                                             );

                        file.FileVersion = ""; //TODO вытащить версию из атрибута

                        queriesString = string.Concat(string.Format(ConstantData.insertQueryTemplate,
                                                                    "Files", file.FileName, file.FileVersion,
                                                                    file.ChangeDate.ToString(ConstantData.dateFormat)
                                                                   ),
                                                      " ", queriesString
                                                     );
                    }

                    //INSERT INTO Files(FileName, FileVersion, ChangeDate) VALUES('testNameFile', '', '2017-03-30T13:00:00.00+03:00')
                    sqlCommand = new SqlCommand(queriesString, connection);
                    adapter.InsertCommand = sqlCommand;
                    adapter.InsertCommand.ExecuteNonQuery();


                    //TODO удалить обработанный файл
                    returnMessage = "Данные загружены в базу данных. " + queriesString;
                }
            } catch (Exception e) {
                returnMessage = e.ToString();
            } finally {
                connection.Close();
                //xmlFile.Close();
                //System.IO.File.Delete(pathToInputXML);
            }

            //todo переделать на сообщение на странице.
            return returnMessage;
        }

        //// GET: LoadXMLtoDB/Details/5
        //public ActionResult Details(int id) {
        //    return View();
        //}

        //// GET: LoadXMLtoDB/Create
        //public ActionResult Create() {
        //    return View();
        //}

        //// POST: LoadXMLtoDB/Create
        //[HttpPost]
        //public ActionResult Create(FormCollection collection) {
        //    try {
        //        // TODO: Add insert logic here

        //        return RedirectToAction("Index");
        //    } catch {
        //        return View();
        //    }
        //}

        //// GET: LoadXMLtoDB/Edit/5
        //public ActionResult Edit(int id) {
        //    return View();
        //}

        //// POST: LoadXMLtoDB/Edit/5
        //[HttpPost]
        //public ActionResult Edit(int id, FormCollection collection) {
        //    try {
        //        // TODO: Add update logic here

        //        return RedirectToAction("Index");
        //    } catch {
        //        return View();
        //    }
        //}

        //// GET: LoadXMLtoDB/Delete/5
        //public ActionResult Delete(int id) {
        //    return View();
        //}

        //// POST: LoadXMLtoDB/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection) {
        //    try {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    } catch {
        //        return View();
        //    }
        //}
    }
}
