using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileOperations.FileWatcher.Models
{
    public class FileMetadata
    {
        public string FilePath { get; set; }
        public DateTime PeriodFromDate { get; set; }
        public DateTime PeriodToDate { get; set; }
    }
}
