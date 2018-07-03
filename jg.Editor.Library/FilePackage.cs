using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Threading;

namespace jg.Editor.Library
{
    /// <summary>
    /// 文件读写类
    /// </summary>
    public class FilePackage
    {
        public delegate void OnProcess(double value);
        public event OnProcess Process = null;
        private Guid Key = new Guid();
        /// <summary>
        /// 将一个文件释放到一个文件夹中
        /// </summary>
        /// <param name="CoursePath">目标文件</param>
        /// <param name="FolderPath">目标文件夹</param>
        public Guid Release(string CoursePath, string FolderPath)
        {
            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Release));
            AutoResetEvent[] events = new AutoResetEvent[] { new AutoResetEvent(false) };
            Tuple<string, string, AutoResetEvent> tuple = new Tuple<string, string, AutoResetEvent>(CoursePath, FolderPath, events[0]);
            thread.Start(tuple);

            WaitHandle.WaitAll(events);//等待子线程退出
            return Key;
        }

        /// <summary>
        /// 释放文件
        /// </summary>
        /// <param name="sender"></param>
        private void Release(object sender)
        {
            string CoursePath, FolderPath;
            var p = (Tuple<string,string, AutoResetEvent>)sender;
            CoursePath = p.Item1;
            FolderPath = p.Item2;
            //解压文件
            FileStream fs = new FileStream(CoursePath, FileMode.Open);

            if (!Directory.Exists(FolderPath)) Directory.CreateDirectory(FolderPath);

            if (File.Exists(FolderPath + "\\index.xml"))
                File.Delete(FolderPath + "\\index.xml");
            FileStream fsHeader = new FileStream(FolderPath + "\\index.xml", FileMode.OpenOrCreate);
            FileStream fsBody;

            double maxLength = 0;
            double currentLength = 0;
            int Length;
            long fileLength;
            XmlDocument xmlDoc = new XmlDocument();

            //释放文件头
            maxLength = fs.Length;
            byte[] buffer = new byte[65536];
            fs.Read(buffer, 0, buffer.Length);

            fsHeader.Write(buffer, 0, buffer.Length);
            fsHeader.Close();

            xmlDoc.Load(FolderPath + "\\index.xml");
            long seek = buffer.Length;

            //得到文件 key
            XmlNodeList nodeKey = xmlDoc.GetElementsByTagName("Key");

            if (nodeKey.Count > 0)
            {
                if (!Guid.TryParse(nodeKey[0].Attributes["value"].Value, out Key))
                {
                    throw new Exception("加载文件失败，请检查文件格式是否正确。");
                }
            }
            else
                throw new Exception("加载文件失败，请检查文件格式是否正确。");

            //释放文件主体。
            try
            {
                foreach (XmlNode xmlNode in xmlDoc.GetElementsByTagName("File"))
                {
                    try
                    {
                        if (File.Exists(FolderPath + "\\" + xmlNode.Attributes["value"].Value))
                            File.Delete(FolderPath + "\\" + xmlNode.Attributes["value"].Value);
                    }
                    catch
                    {
                        seek += int.Parse(xmlNode.Attributes["length"].Value);
                        continue;
                    }
                    fsBody = new FileStream(FolderPath + "\\" + xmlNode.Attributes["value"].Value, FileMode.OpenOrCreate);
                    fileLength = int.Parse(xmlNode.Attributes["length"].Value);
                    fs.Seek(seek, SeekOrigin.Begin);
                    while ((Length = fs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fsBody.Write(buffer, 0, Length < fileLength ? Length : (int)fileLength);
                        currentLength += Length < fileLength ? Length : (int)fileLength;
                        fileLength -= Length;
                        if (fileLength <= 0) break;
                        if (Process != null) Process(currentLength / maxLength);
                    }
                    fsBody.Close();

                    seek += int.Parse(xmlNode.Attributes["length"].Value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("加载文件失败，请检查文件格式是否正确。");
            }
            if (Process != null) Process(1.00);
            p.Item3.Set();
        }

        /// <summary>
        /// 将文件夹写入到一个文件。
        /// </summary>
        /// <param name="CoursePath">目标文件夹</param>
        /// <param name="FolderPath">目标文件</param>
        public void Publish(string CoursePath, string FolderPath)
        {
            //将列表写入到一个文件。
            List<FileInfo> fileList = new List<FileInfo>();
            FileStream fs = new FileStream(CoursePath, FileMode.OpenOrCreate, FileAccess.Write);
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode mainNode;
            XmlNode xmlNode;
            XmlAttribute xmlAttribute;
            mainNode = xmlDoc.CreateElement("List");
            FileInfo fileInfo;
            double maxLength = 0;
            double currentLength = 0;
            try
            {
                
                foreach (string file in Directory.GetFiles(FolderPath))
                {
                    fileInfo = new FileInfo(file);
                    fileList.Add(fileInfo);

                    xmlNode = xmlDoc.CreateElement("File");

                    xmlAttribute = xmlDoc.CreateAttribute("value");
                    xmlAttribute.Value = fileInfo.Name;
                    xmlNode.Attributes.Append(xmlAttribute);

                    xmlAttribute = xmlDoc.CreateAttribute("length");
                    xmlAttribute.Value = fileInfo.Length.ToString();
                    xmlNode.Attributes.Append(xmlAttribute);
                    maxLength += fileInfo.Length;
                    mainNode.AppendChild(xmlNode);
                }
                xmlDoc.AppendChild(mainNode);
                xmlDoc.Save("index.xml");


                byte[] HeadBuffer = new byte[65536];
                int Length;
                byte[] BodyBuffer = new byte[65536];
                FileStream fsBody;
                FileStream fsHeader = new FileStream("index.xml", FileMode.Open);
                fsHeader.Read(HeadBuffer, 0, HeadBuffer.Length);
                fsHeader.Close();
                fs.Write(HeadBuffer, 0, HeadBuffer.Length);
                foreach (FileInfo info in fileList)
                {
                    fsBody = new FileStream(info.FullName, FileMode.Open);
                    while ((Length = fsBody.Read(BodyBuffer, 0, BodyBuffer.Length)) > 0)
                    {
                        fs.Write(BodyBuffer, 0, Length);
                        currentLength += Length;
                        if (Process != null) Process(currentLength / maxLength);
                    }
                    fsBody.Close();
                }
                if (Process != null) Process(1.00);
                fs.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 将文件列表写成一个文件
        /// </summary>
        /// <param name="fileList">文件列表</param>
        /// <param name="path">生成的文件</param>
        public void Publish(Guid Key, List<string> fileList, string path)
        {
            double maxLength;
            SaveFile(path, fileList, GetHeader(Key, fileList, out maxLength), maxLength);
        }

        /// <summary>
        /// 根据文件列表fileList，生成头文件。并根据path保存
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="fileList"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public string GetHeader(Guid Key,List<string> fileList,out double maxLength)
        {
            //将列表写入到一个文件。
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode mainNode;
            XmlNode xmlNode;
            XmlAttribute xmlAttribute;
            mainNode = xmlDoc.CreateElement("List");
            FileInfo fileInfo;
            double _maxLength = 0;
            
            
            xmlNode = xmlDoc.CreateElement("Key");
            xmlAttribute = xmlDoc.CreateAttribute("value");
            xmlAttribute.Value = Key.ToString();
            xmlNode.Attributes.Append(xmlAttribute);
            mainNode.AppendChild(xmlNode);

            foreach (string file in fileList)
            {                
                if (!File.Exists(file)) continue;
                fileInfo = new FileInfo(file);

                xmlNode = xmlDoc.CreateElement("File");

                xmlAttribute = xmlDoc.CreateAttribute("value");
                xmlAttribute.Value = fileInfo.Name;
                xmlNode.Attributes.Append(xmlAttribute);

                xmlAttribute = xmlDoc.CreateAttribute("length");
                xmlAttribute.Value = fileInfo.Length.ToString();
                xmlNode.Attributes.Append(xmlAttribute);
                _maxLength += fileInfo.Length;
                mainNode.AppendChild(xmlNode);
            }
            xmlDoc.AppendChild(mainNode);
            Guid guid = System.Guid.NewGuid();
            xmlDoc.Save(guid.ToString() + ".xml");
            maxLength = _maxLength;
            return guid.ToString() + ".xml";
        }

        /// <summary>
        /// 根据头文件，文件列表，写成一个文件。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileList"></param>
        /// <param name="Header"></param>
        /// <param name="maxLength"></param>
        public void SaveFile(string path, List<string> fileList,string Header,double maxLength)
        {
            if (File.Exists(path)) File.Delete(path);
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);

            double currentLength = 0;
            FileInfo info;
            byte[] HeadBuffer = new byte[65536];
            int Length;
            byte[] BodyBuffer = new byte[65536];
            FileStream fsBody;

            
            FileStream fsHeader = new FileStream(Header, FileMode.Open);
            fsHeader.Read(HeadBuffer, 0, HeadBuffer.Length);
            fsHeader.Close();
            //写入头信息
            fs.Write(HeadBuffer, 0, HeadBuffer.Length);

            //写入文件主体。
            foreach (string file in fileList)
            {
                if (!File.Exists(file)) continue;
                info = new FileInfo(file);
                fsBody = new FileStream(info.FullName, FileMode.Open, FileAccess.Read);
                while ((Length = fsBody.Read(BodyBuffer, 0, BodyBuffer.Length)) > 0)
                {
                    fs.Write(BodyBuffer, 0, Length);
                    currentLength += Length;
                    if (Process != null) Process(currentLength / maxLength);
                }
                fsBody.Close();
            }
            if (Process != null) Process(1.00);
            fs.Close();
            
            //删除临时文件
            foreach(var v in fileList)
                if(v.Substring(v.LastIndexOf('.')) == ".xml")
                    System.IO.File.Delete(v);
            System.IO.File.Delete(Header);

        }
    }
}