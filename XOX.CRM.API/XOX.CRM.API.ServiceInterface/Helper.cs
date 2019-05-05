using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.API.ServiceInterface
{
    public class WebClientEx : WebClient
    {
        public int Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            request.Timeout = Timeout;
            return request;
        }
    }

    public static class Helper
    {
        public static string FileUpload(string Base64, string FileName)
        {
            if (Base64 != "" && FileName != "")
            {
                byte[] imgByte = Convert.FromBase64String(Base64);
                MemoryStream ms = new MemoryStream(imgByte);
                string datetime = DateTime.Now.ToString("yyyyMMddhhmmss");
                string fileName = datetime + "-" + FileName;
                fileName = fileName.Replace(" ", "-");
                string path = System.Web.Hosting.HostingEnvironment.MapPath("~/bin");
                string be = ConfigurationManager.AppSettings["upload-dir-before"];
                string af = ConfigurationManager.AppSettings["upload-dir-after"];
                path = path.Replace(be, af) + @"\" + fileName;
                using (FileStream fs = File.Create(path))
                {
                    ms.WriteTo(fs);
                }

                return fileName;
            }
            else
            {
                return null;
            }
        }

        public static string FileUploadURL(string URL, string FileName)
        {
            if (URL != "" && FileName != "")
            {
                FileName = FileName.Replace(" ", "-");
                string path = System.Web.Hosting.HostingEnvironment.MapPath("~/bin");
                string be = ConfigurationManager.AppSettings["upload-dir-before"];
                string af = ConfigurationManager.AppSettings["upload-dir-after"];
                path = path.Replace(be, af) + @"\" + FileName;

                string datetime = DateTime.Now.ToString("yyyyMMddhhmmss");
                string newpath = Path.GetDirectoryName(path) + @"\" + datetime + "-" + Path.GetFileName(path);

                if (!Directory.Exists(Path.GetDirectoryName(newpath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(newpath));

                try
                {
                    WebClientEx webClient = new WebClientEx();
                    webClient.Timeout = 900000;
                    webClient.DownloadFile(URL, newpath);
                }
                catch
                {
                    try
                    {
                        var AltURL = URL.Replace("https://crp.xoxpostpaid.com.my/Content/uploads/", "https://crp.xoxpostpaid.com.my/Account/GetImg?id=");
                        WebClientEx webClient = new WebClientEx();
                        webClient.Timeout = 900000;

                        if (ByteArrayToFile(newpath, webClient.DownloadData(AltURL)) == false)
                        {
                            return null;
                        }
                    }
                    catch
                    {
                        return null;
                    }
                }

                return FileName.Replace(Path.GetFileName(path), Path.GetFileName(newpath));
            }
            else
            {
                return null;
            }
        }
        public static bool ByteArrayToFile(string _FileName, byte[] _ByteArray)
        {
            try
            {
                // Open file for reading
                System.IO.FileStream _FileStream =
                   new System.IO.FileStream(_FileName, System.IO.FileMode.Create,
                                            System.IO.FileAccess.Write);
                // Writes a block of bytes to this stream using data from
                // a byte array.
                _FileStream.Write(_ByteArray, 0, _ByteArray.Length);

                // close file stream
                _FileStream.Close();

                return true;
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}",
                                  _Exception.ToString());
            }

            // error occured, return false
            return false;
        }

        public static string Encrypt(string key, string toEncrypt)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            // Get the key from config file
            //System.Windows.Forms.MessageBox.Show(key);
            //If hashing use get hashcode regards to your key
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            //Always release the resources and flush data
            // of the Cryptographic service provide. Best Practice

            hashmd5.Clear();


            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)

            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format
            return ByteToString(resultArray);//Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        public static string ByteToString(byte[] buff)
        {
            string sbinary = "";

            for (int i = 0; i < buff.Length; i++)
            {
                sbinary += buff[i].ToString("X2"); // hex format
            }
            return (sbinary);
        }

    }
}
