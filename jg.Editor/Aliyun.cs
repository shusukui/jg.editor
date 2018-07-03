
namespace Aliyun
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Aliyun.OpenServices.OpenStorageService;
    using Aliyun.OpenServices.OpenTableService;
    using System.IO;
    using System.Threading;
using Aliyun.OpenServices;

    class OSS
    {

        public delegate void OnDownLoadProcessing(double value);
        public delegate void OnUpLoadProcessing(double value);
        public delegate void OnUpLoadSuccessfully(bool value,string message);
        public event OnDownLoadProcessing DownLoadProcessing = null;
        public event OnUpLoadProcessing UpLoadProcessing = null;
        public event OnUpLoadSuccessfully UpLoadSuccessfully = null;

        public delegate void CompleteLoad();
        public event CompleteLoad CompleteUpDownLoad = null;

        
        OssClient client = null;
        
        ClientConfiguration config = new ClientConfiguration();
                

        Thread downloadThread = null;
        Thread uploadThread = null;
        Thread uploadProcess = null;


        private int length = 0; //块大小 

        public OSS(string accessId,string accessKey,int _length)
        {
            config.ConnectionTimeout = -1;
            length = _length;
            client = new OssClient(new Uri("http://oss.aliyuncs.com"), accessId, accessKey, config);

            
        }

        public void UpLoadFile(string bucket, string fileKey, string sourcePath)
        {
            FileStream fs = null;
            UpLoadDataInfo uploadDataInfo = new UpLoadDataInfo();
            try
            {
                fs = new FileStream(sourcePath, FileMode.Open, FileAccess.Read);
            }
            catch (IOException ioEx) //文件正在使用中，不能打开
            {
                return;
            }

            uploadDataInfo.bucket = bucket;
            uploadDataInfo.fileKey = fileKey;
            uploadDataInfo.fs = fs;
            
            uploadThread = new Thread(new ParameterizedThreadStart(_UpLoadFile));
            uploadProcess = new Thread(new ParameterizedThreadStart(_UpLoadProcess));

            uploadThread.Start(uploadDataInfo);
            uploadProcess.Start(fs);

               
        }

        public void _UpLoadProcess(object value)
        {
            bool IsSuccessfully = true;
            string message = "";
            if (value is FileStream)
            {
                FileStream fs = (FileStream)value;
                
                while (true)
                {
                    try
                    {
                        if (UpLoadProcessing != null)
                            UpLoadProcessing(Convert.ToDouble(fs.Position) / Convert.ToDouble(fs.Length));
                    }
                        
                    catch (Exception ex)
                    {
                        //message = ex.Message;
                        //IsSuccessfully = false;
                        break;
                    }
                }
                if (UpLoadSuccessfully != null)
                    UpLoadSuccessfully(IsSuccessfully, message);
            }
        }

        public void _UpLoadFile(object value)
        {
            ObjectMetadata om = new ObjectMetadata();
            UpLoadDataInfo uploadDataInfo = null;
            if (value is UpLoadDataInfo)
            {
                uploadDataInfo = (UpLoadDataInfo)value;
            }
            else return;
            
            client.DeleteObject(uploadDataInfo.bucket, uploadDataInfo.fileKey);
            client.PutObject(uploadDataInfo.bucket, uploadDataInfo.fileKey, uploadDataInfo.fs, om);
            
        }

        public void DownLoadFile(string bucket, string fileKey, string targetPath)
        {
            string[] value = new string[] { bucket, fileKey, targetPath };

            downloadThread = new Thread(new ParameterizedThreadStart(_DownLoadFile));
            downloadThread.Start(value);
        }

        public void _DownLoadFile(object value)
        {
            OssObject ossobject = null;
            GetObjectRequest request = null;
            ObjectMetadata metadata;
            FileStream fs = null;
            int readline = 0;       //当前读取的字节长度
            long countlength = 0;    //已经读取的字节
            byte[] buffer = new byte[length];      //文件块

            string bucket = "", fileKey = "", targetPath = "";

            if (value is string[])
            {
                bucket = ((string[])value)[0];
                fileKey = ((string[])value)[1];
                targetPath = ((string[])value)[2];
            }
            else
                return;



            if (File.Exists(targetPath))
            {
                try
                {
                    File.Delete(targetPath);
                }
                catch (IOException ioEx) //文件正在使用中，不能删除。
                {
                    return;
                }
            }
            fs = new FileStream(targetPath, FileMode.Create);
            try
            {
                request = new Aliyun.OpenServices.OpenStorageService.GetObjectRequest(bucket, fileKey);
                ossobject = client.GetObject(request);//获取文件流


                //获取需要下载文件的信息。
                metadata = client.GetObjectMetadata(bucket, fileKey);

                while ((readline = ossobject.Content.Read(buffer, 0, length)) > 0)
                {
                    fs.Write(buffer, 0, readline);
                    countlength += readline;
                    if (DownLoadProcessing != null)
                        DownLoadProcessing(Convert.ToDouble(countlength) / Convert.ToDouble(metadata.ContentLength));
                }

            }
            catch (Aliyun.OpenServices.ServiceException serviceEx) //下载过程中出现错误。
            {

            }
            finally
            {
                ossobject.Dispose();
                fs.Close();
            }
        }

        public string GetUri(string bucket, string fileKey)
        {
            return client.GeneratePresignedUri(bucket, fileKey, System.DateTime.Now.AddMinutes(1)).ToString();
        }

        public bool GetObj(string bucket, string fileKey)
        {
            try
            {
                client.GetObject(bucket, fileKey);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public OssObject GetFile(string bucket, string filekey)
        {
            return client.GetObject(bucket, filekey);
        }

        public bool Copy(string filekey)
        {

            try
            {
                CopyObjectRequest cp = new CopyObjectRequest("jingge2014", filekey, "jingge2015", filekey);
                client.CopyObject(cp);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CreateBuckey()
        {
            client.CreateBucket("jingge2016");
        }
    }
    public class UpLoadDataInfo
    {
        public string bucket{get;set;}
        public string fileKey { get; set; }
        public FileStream fs { get; set; }
    }
}
