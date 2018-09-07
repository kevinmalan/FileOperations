using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using FileOperations.FileWatcher.Models;

namespace FileOperations.FileWatcher.Services
{
    public class FileListenService
    {
        private readonly IList<FileSystemWatcher> _watchers;
        private readonly MessageQueue _messageQueue;

        public FileListenService()
        {
            const string queue = @".\Private$\FileQueue";
            _messageQueue = !MessageQueue.Exists(queue) ? MessageQueue.Create(queue) : new MessageQueue(queue);

            InitFileWatchers(ref _watchers);
        }

        private void InitFileWatchers(ref IList<FileSystemWatcher> watchers)
        {
            string filesPath = ConfigurationManager.AppSettings["fileLocation"];
            DirectoryInfo directoryInfo = new DirectoryInfo(filesPath);

            watchers = directoryInfo.EnumerateDirectories()
                .Select(dir => $@"{filesPath}\{dir}")
                .Select(fullPath => new FileSystemWatcher
                {
                    Path = fullPath,
                    NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName
                }).ToList();
        }

        public void StartService()
        {
            foreach (var watcher in _watchers)
            {
                watcher.Created += Watcher_Created;
                watcher.Renamed += Watcher_Created;
                watcher.EnableRaisingEvents = true;
            }
        }

        public void StopService()
        {
        }

        public void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            string filePath = e.FullPath;
            string dateString = filePath.Substring(0, filePath.Length - e.Name.Length).Replace("\\", "");
            string periodFromDateText = dateString.Substring(dateString.Length - 16, 8);
            string periodToDateText = dateString.Substring(dateString.Length - 8);
            DateTime periodFromDate = DateTime.ParseExact(periodFromDateText, "yyyyMMdd", CultureInfo.InvariantCulture);
            DateTime periodToDate = DateTime.ParseExact(periodToDateText, "yyyyMMdd", CultureInfo.InvariantCulture);

            FileMetadata fileMetadata = new FileMetadata
            {
                FilePath = filePath,
                PeriodFromDate = periodFromDate,
                PeriodToDate = periodToDate
            };

            SendMessageToQueue(fileMetadata);
        }

        private void SendMessageToQueue(FileMetadata fileMetadata)
        {
            _messageQueue.Formatter = new XmlMessageFormatter(new Type[] {typeof(FileMetadata)});
            _messageQueue.Send(fileMetadata);
        }

    }
}
