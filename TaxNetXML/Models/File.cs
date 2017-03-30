using System;

namespace TaxNetXML.Models {
//CREATE TABLE [dbo].[Files] (
//    [Id] INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
//    [FileName] NVARCHAR(100) NOT NULL,
//    [FileVersion] NVARCHAR(50) NOT NULL,
//    [ChangeDate] DATETIME2(7) NOT NULL
//);

    //
    // Summary:
    //     Описывает структуру File из БД.
    //
    // Fields:
    //   Id:
    //     Номер записи
    //   FileName:
    //     Наименование файла
    //   FileVersion:
    //     Версия файла
    //   ChangeDate:
    //     Дата изменения файла
    public class File {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileVersion { get; set; }
        public DateTime ChangeDate { get; set; }
    }
}