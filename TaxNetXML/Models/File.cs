using System;

namespace TaxNetXML.Models {
//CREATE TABLE [dbo].[Files] (
//    [Id] INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
//    [Name] NVARCHAR(100) NOT NULL,
//    [FileVersion] NVARCHAR(50) NOT NULL,
//    [DateTime] DATETIME2(7) NOT NULL
//);

    //
    // Summary:
    //     Описывает структуру File из БД.
    //
    // Fields:
    //   Id:
    //     Номер записи
    //   Name:
    //     Наименование файла
    //   FileVersion:
    //     Версия файла
    //   DateTime:
    //     Дата изменения файла
    public class File {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FileVersion { get; set; }
        public DateTime DateTime { get; set; }
    }
}