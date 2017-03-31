using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
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
        public async Task<string> Upload(HttpPostedFileBase upload) {
            if (upload != null) {
                string returnMessage = "";
                string pathToInputXML = Server.MapPath("~/Files/Input/Input.xml");

                try {
                    upload.SaveAs(pathToInputXML);
                    returnMessage = await ReadFromXml(pathToInputXML);
                } catch (IOException e) {
                    returnMessage = "Method Upload, Проблема при открытии файла: не существует или занят другим приложением. " + e.ToString();
                    HomeController._logger.Debug(returnMessage);
                } catch (Exception e) {
                    returnMessage = "Method Upload. " + e.ToString();
                    HomeController._logger.Debug(returnMessage);
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
        private async Task<string> ReadFromXml(string pathToInputXML) {
            string returnMessage = "";
            Models.File file = new Models.File();
            XmlReader xmlFile = XmlReader.Create(pathToInputXML, new XmlReaderSettings());
            object dateObject = null;

            try {
                CultureInfo provider = CultureInfo.InvariantCulture;

                XmlDocument doc = new XmlDocument();
                doc.Load(pathToInputXML);

                file.FileVersion = doc.SelectSingleNode("File/@FileVersion").InnerText;

                foreach (XmlNode node in doc.SelectNodes("//File")) {

                    dateObject = null;
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

                    HomeController.db.Files.Add(file);
                }
                await HomeController.db.SaveChangesAsync();

                returnMessage = "Данные загружены в базу данных.";
            } catch (FormatException e) {
                returnMessage = string.Concat("Method ReadFromXml, Проблема с форматом даты-времени. Дата, указанная в xml файле: ",
                                              Convert.ToString(dateObject), " ", e.ToString()
                                             );
                HomeController._logger.Debug(returnMessage);
            } catch (Exception e) {
                returnMessage = "Method ReadFromXml. " + e.ToString();
                HomeController._logger.Debug(returnMessage);
            } finally {
                xmlFile.Close();
                System.IO.File.Delete(pathToInputXML);
            }

            return returnMessage;
        }
    }
}