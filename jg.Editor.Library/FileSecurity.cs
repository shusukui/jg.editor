using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Drawing;


namespace jg.Editor.Library
{

    public class FileSecurity
    {
        public delegate void OnDecrypting(double value);


        private static int BufferLength = 8 * 1024;


        public event OnDecrypting Decrypting = null;
        /// <summary>
        /// DES解密文件
        /// </summary>
        /// <param name="sInputFilename">需要解密的文件路径</param>
        /// <param name="sOutputFilename">解密后的文件路径</param>
        /// <param name="sKey">Key</param>
        public void DecryptFile(string sInputFilename, string sOutputFilename, Guid sKey)
        {
            try
            {
                RijndaelManaged RMCrypto = new RijndaelManaged();

                FileStream fs = new FileStream(sInputFilename, FileMode.Open);
                CryptoStream CryptStream = new CryptoStream(fs, RMCrypto.CreateDecryptor(sKey.ToByteArray(), sKey.ToByteArray()), CryptoStreamMode.Read);
                StreamReader SReader = new StreamReader(CryptStream);

                FileStream outputStream = new FileStream(sOutputFilename, FileMode.Create);
                int length;
                int step = 8196;
                double count = 0;
                byte[] buffer = new byte[step];

                while ((length = (CryptStream.Read(buffer, 0, step))) > 0)
                {
                    outputStream.Write(buffer, 0, length);
                    if (Decrypting != null) Decrypting((count += length) / fs.Length);
                }
                outputStream.Close();
                CryptStream.Close();
                fs.Close();
            }
            catch (Exception)
            {

            }
        }

        public static void StreamToFileInfo(string sInputFilename, string sOutputFilename)
        {
            byte[] buffer;
            buffer = GetStream(new FileInfo(sOutputFilename));
            if (!File.Exists(sInputFilename))
            {
                FileStream fs = new FileStream(sInputFilename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fs.Seek(fs.Length, SeekOrigin.Current);
                fs.Write(buffer, 0, buffer.Length);
                fs.Flush();
                fs.Close();

            }
        }

        /// <summary>
        /// DES解密文件图片
        /// </summary>
        /// <param name="sInputFilename">需要解密的文件路径</param>
        /// <param name="sKey">Key</param>
        /// <returns>返回图片信息</returns>
        public Bitmap DecryptFile(string sInputFilename, Guid sKey)
        {
            RijndaelManaged RMCrypto = new RijndaelManaged();
            FileStream fs = new FileStream(sInputFilename, FileMode.Open);
            CryptoStream CryptStream = new CryptoStream(fs, RMCrypto.CreateDecryptor(sKey.ToByteArray(), sKey.ToByteArray()), CryptoStreamMode.Read);
            StreamReader SReader = new StreamReader(CryptStream);

            MemoryStream memoryStream = new MemoryStream();
            int length;
            int step = 8196;
            double count = 0;
            byte[] buffer = new byte[step];

            while ((length = (CryptStream.Read(buffer, 0, step))) > 0)
            {
                memoryStream.Write(buffer, 0, length);
                if (Decrypting != null) Decrypting((count += length) / fs.Length);
            }
            Bitmap bitmap = new Bitmap(memoryStream);
            memoryStream.Close();
            CryptStream.Close();
            fs.Close();
            return bitmap;
        }

        public static string getSecurityKey(string key)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] s = md5.ComputeHash(UnicodeEncoding.UTF8.GetBytes(key));
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.Key = s;
            return Convert.ToBase64String(aes.Key);
        }
        //转换key 为byte数组
        public static byte[] toKey(string key)
        {
            byte[] aeskey = Convert.FromBase64String(key);
            return aeskey;
        }

        public static byte[] encrypt(byte[] data, string pwd)
        {
            byte[] key = toKey(getSecurityKey(pwd));
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = key;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(data, 0, data.Length);

            return resultArray;
        }
        public static FileInfo GetAssetInfo(string URL)
        {


            if (File.Exists(URL))
                return new FileInfo(URL);
            else

                return null;

        }

        /// <summary>
        /// 返回FILEINFObyte[]
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static byte[] GetStream(FileInfo fileInfo)
        {

            FileStream fs;
            byte[] buffer;
            try
            {
                fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);

                fs.Close();
                return buffer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static byte[] decrypt(byte[] data, string pwd)
        {
            byte[] key = toKey(getSecurityKey(pwd));
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = key;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] result = null;
            try
            {
                result = cTransform.TransformFinalBlock(data, 0, data.Length);
            }
            catch (Exception ex)
            {

                Console.Write(ex);
            }

            return result;
        }

        /// <summary>
        /// 返回指定目录下的所有文件信息
        /// </summary>
        /// <param name="strDirectory"></param>
        /// <returns></returns>
        public List<FileInfo> GetAllFilesInDirectory(string strDirectory, string NameParm)
        {
            List<FileInfo> listFiles = new List<FileInfo>(); //保存所有的文件信息  
            DirectoryInfo directory = new DirectoryInfo(strDirectory);


            FileInfo[] fileInfoArray = directory.GetFiles();
            if (fileInfoArray.Length > 0)
            {
                for (int i = 0; i < fileInfoArray.Length; i++)
                {
                    if (fileInfoArray[i].Name.Contains(NameParm))
                    {
                        listFiles.Add(fileInfoArray[i]);
                    }
                }

            }

            //foreach (DirectoryInfo _directoryInfo in directoryArray)
            //{
            //    DirectoryInfo directoryA = new DirectoryInfo(_directoryInfo.FullName);
            //    DirectoryInfo[] directoryArrayA = directoryA.GetDirectories();
            //    FileInfo[] fileInfoArrayA = directoryA.GetFiles();
            //    if (fileInfoArrayA.Length > 0) listFiles.AddRange(fileInfoArrayA);
            //    GetAllFilesInDirectory(_directoryInfo.FullName);//递归遍历  
            //}
            return listFiles;
        }

        // <summary>
        /// 从一个目录将其内容移动到另一目录  
        /// </summary>
        /// <param name="directorySource">源目录</param>
        /// <param name="directoryTarget">目标目录</param>
        public static void MoveFolderTo(string directorySource, string directoryTarget, string NameParm, string Path)
        {
            //检查是否存在目的目录  
            if (!Directory.Exists(directoryTarget))
            {
                Directory.CreateDirectory(directoryTarget);
            }
            //先来移动文件  
            DirectoryInfo directoryInfo = new DirectoryInfo(directorySource);
            FileInfo[] files = directoryInfo.GetFiles();
            //移动所有文件  
            foreach (FileInfo file in files)
            {
                if (file.Name.Contains(NameParm))
                {
                    //如果自身文件在运行，不能直接覆盖，需要重命名之后再移动  
                    if (File.Exists(System.IO.Path.Combine(directoryTarget, file.Name)))
                    {
                        if (File.Exists(System.IO.Path.Combine(directoryTarget, file.Name + Path)))
                        {
                            File.Delete(System.IO.Path.Combine(directoryTarget, file.Name + Path));
                        }
                        File.Move(System.IO.Path.Combine(directoryTarget, file.Name), System.IO.Path.Combine(directoryTarget, file.Name + Path));

                    }
                    file.MoveTo(System.IO.Path.Combine(directoryTarget, file.Name));
                }

            }
            ////最后移动目录  
            //DirectoryInfo[] directoryInfoArray = directoryInfo.GetDirectories();
            //foreach (DirectoryInfo dir in directoryInfoArray)
            //{
            //    MoveFolderTo(System.IO.Path.Combine(directorySource, dir.Name), System.IO.Path.Combine(directoryTarget, dir.Name));
            //}
        }
        /// <summary>
        /// 返回指定目录下的所有文件信息
        /// </summary>
        /// <param name="strDirectory"></param>
        /// <returns></returns>
        public List<FileInfo> GetAllFilesInDirectory(string strDirectory)
        {
            List<FileInfo> listFiles = new List<FileInfo>(); //保存所有的文件信息  
            DirectoryInfo directory = new DirectoryInfo(strDirectory);
            DirectoryInfo[] directoryArray = directory.GetDirectories();
            FileInfo[] fileInfoArray = directory.GetFiles();
            if (fileInfoArray.Length > 0) listFiles.AddRange(fileInfoArray);
            foreach (DirectoryInfo _directoryInfo in directoryArray)
            {
                DirectoryInfo directoryA = new DirectoryInfo(_directoryInfo.FullName);
                DirectoryInfo[] directoryArrayA = directoryA.GetDirectories();
                FileInfo[] fileInfoArrayA = directoryA.GetFiles();
                if (fileInfoArrayA.Length > 0) listFiles.AddRange(fileInfoArrayA);
                GetAllFilesInDirectory(_directoryInfo.FullName);//递归遍历  
            }
            return listFiles;
        }
        /// <summary>
        /// 从一个目录将其内容移动到另一目录  
        /// </summary>
        /// <param name="directorySource">源目录</param>
        /// <param name="directoryTarget">目标目录</param>
        private void MoveFolderTo(string directorySource, string directoryTarget)
        {

            if (Directory.Exists(directorySource))
            {
                //检查是否存在目的目录  
                if (!Directory.Exists(directoryTarget))
                {
                    Directory.CreateDirectory(directoryTarget);
                }
                //先来移动文件  
                DirectoryInfo directoryInfo = new DirectoryInfo(directorySource);

                FileInfo[] files = directoryInfo.GetFiles();
                //移动所有文件  
                foreach (FileInfo file in files)
                {
                    //如果自身文件在运行，不能直接覆盖，需要重命名之后再移动  
                    if (File.Exists(System.IO.Path.Combine(directoryTarget, file.Name)))
                    {
                        if (File.Exists(System.IO.Path.Combine(directoryTarget, file.Name + ".bak")))
                        {
                            File.Delete(System.IO.Path.Combine(directoryTarget, file.Name + ".bak"));
                        }
                        File.Move(System.IO.Path.Combine(directoryTarget, file.Name), System.IO.Path.Combine(directoryTarget, file.Name + ".bak"));

                    }
                    file.MoveTo(System.IO.Path.Combine(directoryTarget, file.Name));

                }
                //最后移动目录  
                DirectoryInfo[] directoryInfoArray = directoryInfo.GetDirectories();
                foreach (DirectoryInfo dir in directoryInfoArray)
                {
                    MoveFolderTo(System.IO.Path.Combine(directorySource, dir.Name), System.IO.Path.Combine(directoryTarget, dir.Name));
                }
            }
            else
            {

            }
        }

        /// <summary>
        /// 从一个目录将其内容复制到另一目录
        /// </summary>
        /// <param name="directorySource">源目录</param>
        /// <param name="directoryTarget">目标目录</param>
        public static void CopyFolderTo(string directorySource, string directoryTarget)
        {
            //检查是否存在目的目录  
            if (!Directory.Exists(directoryTarget))
            {
                Directory.CreateDirectory(directoryTarget);
            }
            //先来复制文件  
            DirectoryInfo directoryInfo = new DirectoryInfo(directorySource);
            FileInfo[] files = directoryInfo.GetFiles();
            //复制所有文件  
            foreach (FileInfo file in files)
            {

                if (!File.Exists(directoryTarget + "\\" + file.Name))
                {
                    file.CopyTo(System.IO.Path.Combine(directoryTarget, file.Name));
                }
            }
            //最后复制目录  
            DirectoryInfo[] directoryInfoArray = directoryInfo.GetDirectories();
            foreach (DirectoryInfo dir in directoryInfoArray)
            {
                if (!Directory.Exists(directoryTarget + "\\" + dir.Name))
                {
                    CopyFolderTo(System.IO.Path.Combine(directorySource, dir.Name), System.IO.Path.Combine(directoryTarget, dir.Name));
                }
            }
        }

        public static void encryptFile(string pwd, string sourceFile, string destFile)
        {
            FileStream destfs = File.Open(sourceFile, FileMode.Open);
            FileStream sourcefs = File.Open(destFile, FileMode.OpenOrCreate);

            byte[] destData = new byte[BufferLength];
            int length = 0;
            byte[] key = toKey(getSecurityKey(pwd));
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = key;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateEncryptor();

            CryptoStream cs = new CryptoStream(destfs, cTransform, CryptoStreamMode.Read);

            while ((length = cs.Read(destData, 0, BufferLength)) > 0)
            {
                sourcefs.Write(destData, 0, length);
            }
            cs.Close();
            destfs.Close();
            sourcefs.Close();
        }

        public static void decryptFile(string pwd, string destFile, string sourceFile)
        {
            try
            {
                FileStream destfs = File.Open(destFile, FileMode.OpenOrCreate);
                FileStream sourcefs = File.Open(sourceFile, FileMode.Open, FileAccess.Read);

                byte[] Buffer = new byte[BufferLength];
                int length = 0;
                byte[] key = toKey(getSecurityKey(pwd));
                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = key;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = rDel.CreateDecryptor();

                CryptoStream cs = new CryptoStream(sourcefs, cTransform, CryptoStreamMode.Read);

                while ((length = cs.Read(Buffer, 0, BufferLength)) > 0)
                {
                    destfs.Write(Buffer, 0, length);
                }
                cs.Flush();
                destfs.Flush();
                sourcefs.Flush();

                cs.Close();
                destfs.Close();
                sourcefs.Close();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void decryptHead(string pwd, string destFile, string sourceFile)
        {
            try
            {
                FileStream destfs = File.Open(destFile, FileMode.OpenOrCreate);
                FileStream sourcefs = File.Open(sourceFile, FileMode.Open);

                byte[] Buffer = new byte[BufferLength];
                int length = 0;
                byte[] key = toKey(getSecurityKey(pwd));
                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = key;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = rDel.CreateDecryptor();
                try
                {
                    using (CryptoStream cs = new CryptoStream(sourcefs, cTransform, CryptoStreamMode.Read))
                    {
                        int i = 0;
                        while ((length = cs.Read(Buffer, 0, BufferLength)) > 0)
                        {
                            destfs.Write(Buffer, 0, length);
                            i++;
                            if (i == 10) break;
                        }

                    }
                }
                catch(Exception ex)
                {
                }

                destfs.Flush();
                sourcefs.Flush();

                destfs.Close();
                sourcefs.Close();

            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message);
            }
        }

        #region
        ////转换key 为byte数组
        //public static string getSecurityKey(string key)
        //{
        //    MD5 md5 = new MD5CryptoServiceProvider();
        //    byte[] s = md5.ComputeHash(UnicodeEncoding.UTF8.GetBytes(key));
        //    AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
        //    aes.Key = s;
        //    return Convert.ToBase64String(aes.Key);
        //}
        ///// <summary>
        ///// 字节转换字符
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //public static byte[] toKey(string key)
        //{
        //    byte[] aeskey = Convert.FromBase64String(key);
        //    return aeskey;
        //}
        ///// <summary>
        ///// AES加密密码
        ///// </summary>
        ///// <param name="data"></param>
        ///// <param name="pwd"></param>
        ///// <returns></returns>
        //public static byte[] encrypt(byte[] data, string pwd)
        //{
        //    byte[] key = toKey(getSecurityKey(pwd));
        //    RijndaelManaged rDel = new RijndaelManaged();
        //    rDel.Key = key;
        //    rDel.Mode = CipherMode.ECB;
        //    rDel.Padding = PaddingMode.PKCS7;
        //    ICryptoTransform cTransform = rDel.CreateEncryptor();
        //    byte[] resultArray = cTransform.TransformFinalBlock(data, 0, data.Length);

        //    return resultArray;
        //}
        ///// <summary>
        ///// AES解密密码
        ///// </summary>
        ///// <param name="data"></param>
        ///// <param name="pwd"></param>
        ///// <returns></returns>
        //public static byte[] decrypt(byte[] data, string pwd)
        //{
        //    byte[] key = toKey(getSecurityKey(pwd));
        //    RijndaelManaged rDel = new RijndaelManaged();
        //    rDel.Key = key;
        //    rDel.Mode = CipherMode.ECB;
        //    rDel.Padding = PaddingMode.PKCS7;
        //    ICryptoTransform cTransform = rDel.CreateDecryptor();
        //    byte[] result = null;
        //    try
        //    {
        //        result = cTransform.TransformFinalBlock(data, 0, data.Length);
        //    }
        //    catch (Exception ex)
        //    {

        //        Console.Write(ex);
        //    }

        //    return result;
        //}
        ///// <summary>
        ///// AES加密文件
        ///// </summary>
        ///// <param name="pwd"></param>
        ///// <param name="sourceFile"></param>
        ///// <param name="destFile"></param>
        //public static void encryptFile(string pwd, string sourceFile, string destFile)
        //{
        //    FileStream destfs = File.Open(sourceFile, FileMode.Open);
        //    FileStream sourcefs = File.Open(destFile, FileMode.OpenOrCreate);

        //    int destLen = 1020;
        //    byte[] destData = new byte[destLen];


        //    int length = 0;
        //    byte[] sourceData;
        //    while ((length = destfs.Read(destData, 0, destLen)) > 0)
        //    {
        //        sourceData = encrypt(destData, pwd);
        //        sourcefs.Write(sourceData, 0, sourceData.Length);
        //    }
        //    destfs.Flush();
        //    sourcefs.Flush();
        //    destfs.Close();
        //    sourcefs.Close();
        //}
        ///// <summary>
        ///// AES解密文件
        ///// </summary>
        ///// <param name="pwd"></param>
        ///// <param name="destFile"></param>
        ///// <param name="sourceFile"></param>
        //public static void decryptFile(string pwd, string destFile, string sourceFile)
        //{
        //    FileStream destfs = File.Open(destFile, FileMode.Open);
        //    FileStream sourcefs = File.Open(sourceFile, FileMode.OpenOrCreate);

        //    int destLen = 1024;
        //    byte[] destData = new byte[destLen];


        //    int length = 0;
        //    byte[] sourceData;
        //    while ((length = destfs.Read(destData, 0, destLen)) > 0)
        //    {
        //        sourceData = decrypt(destData, pwd);
        //        sourcefs.Write(sourceData, 0, sourceData.Length);
        //    }
        //    destfs.Flush();
        //    sourcefs.Flush();
        //    destfs.Close();
        //    sourcefs.Close();
        //}
        #endregion
    }
}
