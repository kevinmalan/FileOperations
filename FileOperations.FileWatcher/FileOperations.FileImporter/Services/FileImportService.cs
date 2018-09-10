using FileOperations.FileImporter.Models;
using System;
using System.Configuration;
using System.IO;
using System.Messaging;

namespace FileOperations.FileImporter.Services
{
    public class FileImportService
    {
        private readonly MessageQueue _messageQueue;

        public FileImportService()
        {
            const string queue = @".\Private$\FileQueue";

            _messageQueue = MessageQueue.Exists(queue) ? MessageQueue.Create(queue) : new MessageQueue(queue);
            _messageQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(FileMetadata) });
        }

        public void HandleFileImport()
        {
            // Reads first item in the Queue
            var message = _messageQueue.Receive(MessageQueueTransactionType.Single);
            FileMetadata fileMetadata = (FileMetadata)message?.Body;

            if (fileMetadata != null)
            {
                // TODO: Implement logic to handle the fileMetadata object.
            }

            MoveProcessedFile(fileMetadata?.FilePath);
        }

        private void MoveProcessedFile(string path)
        {
            string fileProcessedPath = ConfigurationManager.AppSettings["filesProcessedLocation"];
            string destinationFile = $"{fileProcessedPath}\\{Path.GetFileName(path)}";

            File.Move(path, destinationFile);
        }
    }
}