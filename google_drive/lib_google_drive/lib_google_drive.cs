using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using MimeTypes;
using Google.Apis.Upload;

namespace lib_google_drive
{
    public class google_drive
    {
        // Thiết lập phạm vi truy xuất dữ liệu Scope = Drive để upload file
        string[] Scopes = { DriveService.Scope.Drive };
        string ApplicationName = "DuoclvToolUploadDrive";

        public string Uploadfile(string pathFile)
        {
            UserCredential credential;
            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // Thông tin về quyền truy xuất dữ liệu của người dùng được lưu ở thư mục token.json
                string credPath = "token.json";

                // Yêu cầu người dùng xác thực lần đầu và thông tin sẽ được lưu vào thư mục token.json
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,  // Quyền truy xuất dữ liệu của người dùng
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Tạo ra 1 dịch vụ Drive API - Create Drive API service với thông tin xác thực và ApplicationName
            var driveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // ID thư mục file, các bạn thay bằng ID của các bạn khi chạy
            var folderId = "1xoC_xjV9Rn7z3qrgPbCaJYPHwJTaPKns";
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                // Tên file sẽ lưu trên Google Drive
                Name = Path.GetFileName(pathFile),

                // Thư mục chưa file
                Parents = new List<string>
                {
                    folderId
                }
            };

            FilesResource.CreateMediaUpload request;
            using (var stream = new FileStream(pathFile, FileMode.Open))
            {
                string ext = (Path.GetFileName(pathFile).Contains(".")) ? Path.GetExtension(Path.GetFileName(pathFile)).ToLower() : "." + Path.GetFileName(pathFile);

                request = driveService.Files.Create(fileMetadata, stream, MimeTypeMap.GetMimeType(ext));

                // Cấu hình thông tin lấy về là ID
                request.Fields = "id";

                // thực hiện Upload
                request.Upload();
            }

            // Trả về thông tin file đã được upload lên Google Drive
            var file = request.ResponseBody;

            return file.Id;
        }

        static void Upload_ProgressChanged(IUploadProgress progress)
        {
            Console.WriteLine(progress.Status + " " + progress.BytesSent);
        }

        //static void Upload_ResponseReceived(File file)
        //{
        //    Console.WriteLine(file.Title + " was uploaded successfully");
        //}
    }
}
