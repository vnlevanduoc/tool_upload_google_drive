using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using HeyRed.Mime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace google_drive
{
    class Program
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/drive-dotnet-quickstart.json
        static string[] Scopes = { DriveService.Scope.DriveReadonly };
        static string ApplicationName = "DuoclvToolUploadDrive";

        static void Main(string[] args)
        {
            UserCredential credential;

            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Drive API service.
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            //Upload file
            UploadFile(service);


            // Define parameters of request.
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.PageSize = 10;
            //listRequest.Fields = "nextPageToken, files(id, name)";
            listRequest.Q = "parents in '0BzG-111t2JvxRXByR1F2eDJQV00', '1xoC_xjV9Rn7z3qrgPbCaJYPHwJTaPKns'";

            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;
            Console.WriteLine("Files:");
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    Console.WriteLine("{0} ({1})", file.Name, file.Id);
                }
            }
            else
            {
                Console.WriteLine("No files found.");
            }
            Console.Read();

        }

        private static void UploadFile(DriveService driveService)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = "GiaoTrinh.rar"
            };
            FilesResource.CreateMediaUpload request;
            using (var stream = new FileStream("C:\\Users\\duocl\\OneDrive\\Desktop\\GiaoTrinh.rar", FileMode.Open))
            {
                var mimeType = MimeTypesMap.GetMimeType(fileMetadata.Name);

                request = driveService.Files.Create(fileMetadata, stream, mimeType);
                request.Fields = "id";
                request.Upload();
            }
            var file = request.ResponseBody;
            Console.WriteLine("File ID: " + file.Id);
        }
    }
}
