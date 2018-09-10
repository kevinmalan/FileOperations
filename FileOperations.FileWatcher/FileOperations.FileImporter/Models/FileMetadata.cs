using System;

namespace FileOperations.FileImporter.Models
{
    public class FileMetadata
    {
        public string FilePath { get; set; }
        public DateTime PeriodFromDate { get; set; }
        public DateTime PeriodToDate { get; set; }
    }
}