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
                string returnMessage = "";
                string pathToInputXML = Server.MapPath("~/Files/Input/Input.xml");

                try {
                    upload.SaveAs(pathToInputXML);
                    returnMessage = ReadFromXml(pathToInputXML);
                } catch (IOException e) {
                    ConstantData._logger.Debug("Method Upload, Проблема при открытии файла: не существует или занят другим приложением. " + e.ToString());
                } catch (Exception e) {
                    ConstantData._logger.Debug("Method Upload. " + e.ToString());
                }

                return returnMessage;
            }

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
            XmlReader xmlFile = XmlReader.Create(pathToInputXML, new XmlReaderSettings());
            SqlConnection connection = new SqlConnection(ConstantData.CONNECTION_STRING);

            try {
                using (connection) {
                    SqlDataAdapter adapter = new SqlDataAdapter(ConstantData.querySelectFromFiles, connection);
                    SqlCommand sqlCommand;

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

                    sqlCommand = new SqlCommand(queriesString, connection);
                    adapter.InsertCommand = sqlCommand;
                    adapter.InsertCommand.ExecuteNonQuery();

                    returnMessage = "Данные загружены в базу данных.";
                }
            } catch (FormatException e) {
                ConstantData._logger.Debug("Method ReadFromXml, Проблема с форматом даты-времени. " + e.ToString());
            } catch (Exception e) {
                ConstantData._logger.Debug("Method ReadFromXml. " + e.ToString());
                returnMessage = e.ToString();
            } finally {
                connection.Close();
                xmlFile.Close();
                System.IO.File.Delete(pathToInputXML);
            }

            return returnMessage;
        }
    }
}
