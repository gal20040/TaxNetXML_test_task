using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using TaxNetXML.Models;

namespace TaxNetXML.Controllers {
    public class BackupDBController : Controller {
        private FileContext db = new FileContext();

        // GET: BackupDB
        public async Task<ActionResult> Index() {
            return View(await db.Files.ToListAsync());
        }

        //
        // Summary:
        //     Определяет полный путь и имя выходного файла.
        //     Далее запускает методы: WriteToXml и GiveFileToUser.
        //
        // Returns:
        //     Ретранслирует выгруженный файл пользователю.
        public FileResult BackupDBToXML() { //FileResult
            //string returnMessage = "";
            string pathToOutputXML = "";
            //try {
            pathToOutputXML = string.Concat(Server.MapPath("~/Files/Output/db_backup"),
                                                //DateTime.Now.ToString(ConstantData.dateFormaForFileName), //todo скорее всего придётся отказаться. пока не знаю, как удалять такие файлы
                                                ".xml");
            //returnMessage = returnMessage + 
            WriteToXml(pathToOutputXML);
            //return returnMessage;


            return GiveFileToUser(pathToOutputXML);
            //} catch (Exception e) {
            //    returnMessage = returnMessage + pathToOutputXML + e.ToString();
            //    return returnMessage;
            //}
        }

        //
        // Summary:
        //     Получает из БД данные и записывает их в xml файл.
        //
        // Parameters:
        //   pathToOutputXML:
        //     Полный путь до файла в системе + его имя и расширение.
        private void WriteToXml(string pathToOutputXML) { //void
            //string returnMessage = "";
            SqlConnection connection = new SqlConnection(ConstantData.CONNECTION_STRING);

            //try {
            using (connection) {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(ConstantData.querySelectFromFiles, connection);

                DataSet dataSet = new DataSet("Files");
                DataTable dataTable = new DataTable("File");
                dataSet.Tables.Add(dataTable);
                adapter.Fill(dataSet.Tables["File"]);

                dataSet.WriteXml(pathToOutputXML);
            }
            //} catch (Exception e) {
            //    returnMessage = pathToOutputXML + e.ToString();
            //} finally {
            connection.Close();
            //}
            //return returnMessage;
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
        private FileResult GiveFileToUser(string pathToOutputXML) { //FileResult
            //string returnMessage = "";
            SqlConnection connection = new SqlConnection(ConstantData.CONNECTION_STRING);

            //try {
            using (connection) {
                string file_name = System.IO.Path.GetFileName(pathToOutputXML);

                connection.Close();
                return File(pathToOutputXML, ConstantData.file_type, file_name);
            }
            //} catch (Exception e) {
            //    //string returnMessage = e.ToString();
            //    connection.Close();
            //    return e.ToString();
            //}
            //finally {
            //    connection.Close();
            //}
        }

        //// GET: BackupDB/Details/5
        //public async Task<ActionResult> Details(int? id) {
        //    if (id == null) {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    File file = await db.Files.FindAsync(id);
        //    if (file == null) {
        //        return HttpNotFound();
        //    }
        //    return View(file);
        //}

        //// GET: BackupDB/Create
        //public ActionResult Create() {
        //    return View();
        //}

        //// POST: BackupDB/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create([Bind(Include = "Id,Name,FileVersion,DateTime")] File file) {
        //    if (ModelState.IsValid) {
        //        db.Files.Add(file);
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }

        //    return View(file);
        //}

        //// GET: BackupDB/Edit/5
        //public async Task<ActionResult> Edit(int? id) {
        //    if (id == null) {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    File file = await db.Files.FindAsync(id);
        //    if (file == null) {
        //        return HttpNotFound();
        //    }
        //    return View(file);
        //}

        //// POST: BackupDB/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include = "Id,Name,FileVersion,DateTime")] File file) {
        //    if (ModelState.IsValid) {
        //        db.Entry(file).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    return View(file);
        //}

        //// GET: BackupDB/Delete/5
        //public async Task<ActionResult> Delete(int? id) {
        //    if (id == null) {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    File file = await db.Files.FindAsync(id);
        //    if (file == null) {
        //        return HttpNotFound();
        //    }
        //    return View(file);
        //}

        //// POST: BackupDB/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(int id) {
        //    File file = await db.Files.FindAsync(id);
        //    db.Files.Remove(file);
        //    await db.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}