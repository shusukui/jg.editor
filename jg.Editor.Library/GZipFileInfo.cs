using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jg.Editor.Library
{
    /// <summary>
    /// 要压缩的文件信息
    /// </summary>
    public class GZipFileInfo
    {
        /// <summary>
        /// 文件索引
        /// </summary>
        public int Index = 0;
        /// <summary>
        /// 文件相对路径，'/'
        /// </summary>
        public string RelativePath = null;
        public DateTime ModifiedDate;
        /// <summary>
        /// 文件内容长度
        /// </summary>
        public int Length = 0;
        public bool AddedToTempFile = false;
        public bool RestoreRequested = false;
        public bool Restored = false;
        /// <summary>
        /// 文件绝对路径,'\'
        /// </summary>
        public string LocalPath = null;
        public string Folder = null;

        public bool ParseFileInfo(string fileInfo)
        {
            bool success = false;
            try
            {
                if (!string.IsNullOrEmpty(fileInfo))
                {
                    // get the file information
                    string[] info = fileInfo.Split(',');
                    if (info != null && info.Length == 4)
                    {
                        this.Index = Convert.ToInt32(info[0]);
                        this.RelativePath = info[1].Replace("/", "\\");
                        this.ModifiedDate = Convert.ToDateTime(info[2]);
                        this.Length = Convert.ToInt32(info[3]);
                        success = true;
                    }
                }
            }
            catch
            {
                success = false;
            }
            return success;
        }
    }
}
