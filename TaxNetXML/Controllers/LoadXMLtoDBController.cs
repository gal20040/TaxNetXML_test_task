using System;
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
            string queriesString = ""; //общий SQL запрос для нескольких строк
            string returnMessage = "";
            Models.File file = new Models.File();
            //Данные загружены в базу данных. INSERT INTO Files (Name, FileVersion, DateTime) VALUES ('testNameFile', '', '30.03.2017') s1=30 марта 2017 г. s2=13:00:00 s3
            //2017-03-30T13:00:00.00+03:00
            XmlReader xmlFile = XmlReader.Create(pathToInputXML, new XmlReaderSettings());
            SqlConnection connection = new SqlConnection(ConstantData.CONNECTION_STRING);

            try {
                using (connection) {
                    //const string insertQueryTemplate = "INSERT INTO {0:d} (Name, Age) VALUES ('{1:d}', '{2:d}', '{3:d}')";
                    //string pathToInputXML = Server.MapPath("~/Files/Input/Input.xml"); //todo переделать на уникальное имя файла

                    //DataSet ds = new DataSet();
                    SqlDataAdapter adapter = new SqlDataAdapter(ConstantData.querySelectFromFiles, connection);
                    SqlCommand sqlCommand;

                    //ds.ReadXml(xmlFile);

                    connection.Open();
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    object dateObject;

                    XmlDocument doc = new XmlDocument();
                    doc.Load(pathToInputXML);

                    file.FileVersion = doc.SelectSingleNode("File/@FileVersion").InnerText;

                    foreach (XmlNode node in doc.SelectNodes("//File")) {
                        file.Name = node.SelectSingleNode("Name").InnerText;

                        //если в тэге DateTime пусто, то берём текущие дату и время.
                        dateObject = node.SelectSingleNode("DateTime").InnerText;
                        if (dateObject == null)
                            file.DateTime = DateTime.Now;
                        else
                            file.DateTime = DateTime.ParseExact(
                                                              Convert.ToString(dateObject),
                                                              ConstantData.dateFormatWithDash, provider
                                                             );

                        //добавляем всю инфу по текущему node в шаблон запроса
                        //и добавляем готовый запрос в общий список
                        //todo сейчас для каждой строки свой запрос - сделать общий запрос.
                        queriesString = string.Concat(string.Format(ConstantData.insertQueryTemplate,
                                                                    "Files", file.Name, file.FileVersion,
                                                                    file.DateTime.ToString(ConstantData.dateFormatWithDash)
                                                                   ),
                                                      " ", queriesString
                                                     );
                    }

                    //for (int i = 0; i < ds.Tables[0].Rows.Count; i++) {
                    //    file.FileVersion = Convert.ToString(ds.Tables[0].Rows[i].ItemArray[0]);

                    //    file.Name = Convert.ToString(ds.Tables[0].Rows[i].ItemArray[0]);

                    //    dateObject = ds.Tables[0].Rows[i].ItemArray[1];
                    //    if (dateObject == null)
                    //        file.DateTime = DateTime.Now;
                    //    else
                    //        file.DateTime = DateTime.ParseExact(
                    //                                          Convert.ToString(dateObject),
                    //                                          ConstantData.dateFormatWithDash, provider
                    //                                         );

                    //    queriesString = string.Concat(string.Format(ConstantData.insertQueryTemplate,
                    //                                                "Files", file.Name, file.FileVersion,
                    //                                                file.DateTime.ToString(ConstantData.dateFormatWithDash)
                    //                                               ),
                    //                                  " ", queriesString
                    //                                 );
                    //}

                    sqlCommand = new SqlCommand(queriesString, connection);
                    adapter.InsertCommand = sqlCommand;
                    adapter.InsertCommand.ExecuteNonQuery();

                    returnMessage = "Данные загружены в базу данных.";
                }
            } catch (FormatException e) {
                returnMessage = "Проблема с форматом даты-времени.\r\n" + e.ToString();
            } catch (Exception e) {
                returnMessage = e.ToString();
            } finally {
                connection.Close();
                xmlFile.Close();
                System.IO.File.Delete(pathToInputXML);
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
