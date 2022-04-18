using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = ConfigurationManager.AppSettings["imagesPath"];
            
            Console.WriteLine("********************");
            Console.WriteLine("Starting the camera");
            Console.WriteLine("********************");

            Thread.Sleep(5000); //will sleep for 5 sec

            Console.WriteLine("Capturing the live video stream, and breaking it down into frames");
            Console.WriteLine("Press ESC to stop");

            do
            {
                while (!Console.KeyAvailable)
                {
                    var frame = GetRandomFile(path);
                    Console.WriteLine("Frame: " + frame);
                    UploadtoBlob(frame);
                    Thread.Sleep(3000); //will sleep for 3 sec
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }


        static string GetRandomFile(string path)
        {
            string file = null;
           
            if (!string.IsNullOrEmpty(path))
            {
                var rand = new Random();                
                var files = Directory.GetFiles(path, "*.png");
                file = files[rand.Next(files.Length)].ToString();
            }
            return file;
        }


        static void UploadtoBlob(string path)
        {
            string blobConnectionString = ConfigurationManager.AppSettings["blobConnectionString"];

            // Intialize BobClient 
            BlobClient blobClient = new Azure.Storage.Blobs.BlobClient(
                connectionString: blobConnectionString,
                blobContainerName: "deviceoutput",
                blobName: Path.GetFileName(path));

            try
            {
                blobClient.Upload(path);
            }
            catch (Azure.RequestFailedException ex)
            {
                //Ignore if file already exists
                if(!ex.ErrorCode.Equals("BlobAlreadyExists"))
                {
                    throw;
                }                
            }            
        }
    }
}
