using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using N14DCAT006.Models;

namespace N14DCAT006.Controllers
{
    public class BaiHatController : Controller
    {

        N14DCAT006DbContext data = new N14DCAT006DbContext();
        // GET: BaiHat
        public ActionResult Index(int id)
        {
            BaiHat baiHat = data.BaiHats.SingleOrDefault(n => n.MaBaiHat == id);
            ViewBag.thumuc = baiHat.MaBaiHat.ToString() + ".wav";
            User user = (User) Session["TaiKhoan"];
            if (user == null)
            {
                Download("", baiHat, false);
            }
            else
            {
                var download_user = data.DownloadBaiHats.Where(n => (n.MaBaiHat == id) && (n.MaUser == user.MaUser)).SingleOrDefault();
                if (download_user == null)
                {
                    download_user=new DownloadBaiHat();
                    download_user.MaBaiHat = id;
                    download_user.MaUser = user.MaUser;
                    string s = user.MaUser + "-" + user.TaiKhoanUser + "-" + baiHat.MaBaiHat + "-" + baiHat.TenBaiHat;
                    Download(s, baiHat, true);
                    string name_upload = baiHat.MaBaiHat.ToString() + "-" + user.MaUser.ToString() + ".wav";
                    string id_drive = Upload(name_upload, baiHat);
                    download_user.link = id_drive;
                    data.DownloadBaiHats.Add(download_user);
                }
                else
                {
                    
                }
            }
            return View(baiHat);
        }
        public string Upload(string name, BaiHat baiHat)
        {
            string fileId = baiHat.link;
            string ApplicationName = "Drive API .NET Quickstart";
            string[] scopes = new string[] { DriveService.Scope.Drive,
                             DriveService.Scope.DriveFile};
            var clientId = "843374652518-0dlrtbe20h90du34vj6k30tnt0p5tism.apps.googleusercontent.com";
            var clientSecret = "2JsXDylX8KqcwebMYnKW_aXU";
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            },
                                                        scopes,
                                                        Environment.UserName,
                                                        CancellationToken.None,
                                                        new FileDataStore("MyAppsToken")).Result;
            DriveService service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            service.HttpClient.Timeout = TimeSpan.FromMinutes(100);

            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = name
            };
            FilesResource.CreateMediaUpload request2;
            using (var stream = new System.IO.FileStream(Server.MapPath("~/Music/"+baiHat.MaBaiHat+".wav"),
                                    System.IO.FileMode.Open))
            {
                request2 = service.Files.Create(
                    fileMetadata, stream, "audio/wav");
                request2.Fields = "id";
                request2.Upload();
            }
            var file2 = request2.ResponseBody;
            return file2.Id;
        }
        public void Download(string message,BaiHat baiHat, bool isUser)
        {
            string fileId = baiHat.link;
            string ApplicationName = "Drive API .NET Quickstart";
            string[] scopes = new string[] { DriveService.Scope.Drive,
                             DriveService.Scope.DriveFile};
            var clientId = "843374652518-0dlrtbe20h90du34vj6k30tnt0p5tism.apps.googleusercontent.com";
            var clientSecret = "2JsXDylX8KqcwebMYnKW_aXU";
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            },
                                                        scopes,
                                                        Environment.UserName,
                                                        CancellationToken.None,
                                                        new FileDataStore("MyAppsToken")).Result;
            DriveService service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            service.HttpClient.Timeout = TimeSpan.FromMinutes(100);

            var request = service.Files.Get(fileId);
            var stream1 = new System.IO.MemoryStream();
            request.MediaDownloader.ProgressChanged +=
                (IDownloadProgress progress) =>
                {
                    switch (progress.Status)
                    {
                        case DownloadStatus.Downloading:
                            {
                                Console.WriteLine(progress.BytesDownloaded);
                                break;
                            }
                        case DownloadStatus.Completed:
                            {
                                Console.WriteLine("Download complete.");
                                break;
                            }
                        case DownloadStatus.Failed:
                            {
                                Console.WriteLine("Download failed.");
                                break;
                            }
                    }
                };
            request.Download(stream1);
            if (isUser == true)
            {
                WaveStream wave = new WaveStream(stream1);
                wave.Flush();
                MemoryStream tam = Hide(message, wave);
                using (FileStream f = new FileStream(Server.MapPath("~/Music/"+baiHat.MaBaiHat+".wav"), FileMode.Create, FileAccess.Write))
                {
                    tam.WriteTo(f);
                    f.Flush();
                }
            }
            else
            {
                using (FileStream f = new FileStream(Server.MapPath("~/Music/" + baiHat.MaBaiHat + ".wav"), FileMode.Create, FileAccess.Write))
                {
                    stream1.WriteTo(f);
                }
            }
        }
        private static string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }
        static MemoryStream destinationStream = new MemoryStream();
        private static MemoryStream Hide(string s, WaveStream sourceStream)
        {
            int messageLength = s.Length;
            byte[] length = BitConverter.GetBytes(messageLength);

            byte[] messageByte = Encoding.UTF8.GetBytes(s);
            byte[] merge = new byte[length.Length + messageByte.Length];
            Buffer.BlockCopy(length, 0, merge, 0, length.Length);
            Buffer.BlockCopy(messageByte, 0, merge, length.Length, messageByte.Length);
            Stream messageStream = new MemoryStream(merge);
            int bytesPerSample = 2;
            byte[] waveBuffer = new byte[bytesPerSample];
            byte message, bit, waveByte;
            int messageBuffer; 
            while ((messageBuffer = messageStream.ReadByte()) >= 0)
            {
                message = (byte)messageBuffer;
                
                for (int bitIndex = 0; bitIndex < 8; bitIndex++)
                {
                    sourceStream.Read(waveBuffer, 0, waveBuffer.Length);
                    waveByte = waveBuffer[bytesPerSample - 1];
                    
                    bit = (byte)(((message & (byte)(1 << bitIndex)) > 0) ? 1 : 0);
                    
                    if ((bit == 1) && ((waveByte % 2) == 0))
                    {
                        waveByte += 1;
                    }
                    else if ((bit == 0) && ((waveByte % 2) == 1))
                    {
                        waveByte -= 1;
                    }

                    waveBuffer[bytesPerSample - 1] = waveByte;
                    
                    destinationStream.Write(waveBuffer, 0, bytesPerSample);
                }
            }
            waveBuffer = new byte[sourceStream.Length - sourceStream.Position];
            sourceStream.Read(waveBuffer, 0, waveBuffer.Length);
            destinationStream.Write(waveBuffer, 0, waveBuffer.Length);
            destinationStream = WaveStream.CreateStream(destinationStream, sourceStream.Format);
            return destinationStream;
        }
        static MemoryStream messageStream = new MemoryStream();
        private static string Extract(WaveStream sourceStream)
        {


            int bytesPerSample = 2;
            byte[] waveBuffer = new byte[bytesPerSample];
            byte message, bit, waveByte;
            int messageLength = 0; 
            while ((messageLength == 0 || messageStream.Length < messageLength))
            {
                message = 0;
                for (int bitIndex = 0; bitIndex < 8; bitIndex++)
                {
                    sourceStream.Read(waveBuffer, 0, waveBuffer.Length);
                    waveByte = waveBuffer[bytesPerSample - 1];
                    
                    bit = (byte)(((waveByte % 2) == 0) ? 0 : 1);
                    
                    message += (byte)(bit << bitIndex);
                }
                
                messageStream.WriteByte(message);

                if (messageLength == 0 && messageStream.Length == 4)
                {
                    messageStream.Seek(0, SeekOrigin.Begin);
                    messageLength = new BinaryReader(messageStream).ReadInt32();
                    messageStream.Seek(0, SeekOrigin.Begin);
                    messageStream.SetLength(0);
                }
            }
            var array = messageStream.ToArray();
            string kq = Encoding.UTF8.GetString(array, 0, array.Length);
            return kq;
        }
        private static byte GetKeyValue(Stream keyStream)
        {
            int keyValue;
            if ((keyValue = keyStream.ReadByte()) < 0)
            {
                keyStream.Seek(0, SeekOrigin.Begin);
                keyValue = keyStream.ReadByte();
                if (keyValue == 0) { keyValue = 1; }
            }
            return (byte)keyValue;
        }
        public static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }
        [HttpGet]
        public ActionResult Check()
        {
            ViewBag.Thongbao = "";
            return View();
        }
        [HttpPost]
        public ActionResult Check(FormCollection collection, HttpPostedFileBase fileupload)
        {
            var filename = Path.GetFileName(fileupload.FileName);
            var path = Path.Combine(Server.MapPath("~/Check"), filename);
            if (System.IO.File.Exists(path))
            {
                ViewBag.Thongbao = "File đã tồn tại";
            }
            else
            {
                fileupload.SaveAs(path);
                using (Stream openStream = System.IO.File.OpenRead(path))
                {
                    WaveStream wave2 = new WaveStream(openStream);
                    string s4 = Extract(wave2);
                    ViewBag.Thongbao = s4;
                }
            }
            return View(this);

        }
    }
}