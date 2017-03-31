using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Web.Mvc;
using System.Xml;
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
        private async void WriteToXml(string pathToOutputXML) {
            //XmlNode newElem;
            XmlDocument doc = new XmlDocument();
            //XmlElement root = doc.DocumentElement;
            CultureInfo provider = CultureInfo.InvariantCulture;

            const string version = "1.0";
            const string encoding = "UTF-8";
            const string standalone = "yes";
            XmlNode docNode = doc.CreateXmlDeclaration(version, encoding, standalone);
            doc.AppendChild(docNode); //XML-декларация <?xml version='1.0' encoding="UTF-8" standalone="yes"?>

            XmlNode filesNode = doc.CreateElement("Files");
            doc.AppendChild(filesNode); //внешний тэг <Files></Files>

            IEnumerable<File> files = await db.Files.ToListAsync();
            foreach (File file in files) {
                XmlNode fileNode = doc.CreateElement("File");

                XmlAttribute fileVersionAttribute = doc.CreateAttribute("FileVersion");
                fileVersionAttribute.Value = file.FileVersion;
                fileNode.Attributes.Append(fileVersionAttribute);

                filesNode.AppendChild(fileNode);

                XmlNode nameNode = doc.CreateElement("Name");
                nameNode.AppendChild(doc.CreateTextNode(file.Name));
                fileNode.AppendChild(nameNode);

                XmlNode dateTimeNode = doc.CreateElement("DateTime");
                dateTimeNode.AppendChild(doc.CreateTextNode(file.DateTime.ToString(ConstantData.dateFormatWithDash,
                                                                                   provider
                                                                                  )
                                                           )
                                        );
                fileNode.AppendChild(dateTimeNode);
            }
            doc.Save(pathToOutputXML);
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