using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jg.Editor.Library
{
    /// <summary>
    /// 文件压缩后的压缩包类
    /// </summary>
    public class GZipResult
    {
        /// <summary>
        /// 压缩包中包含的所有文件,包括子目录下的文件
        /// </summary>
        public GZipFileInfo[] Files = null;
        /// <summary>
        /// 要压缩的文件数
        /// </summary>
        public int FileCount = 0;
        public long TempFileSize = 0;
        public long ZipFileSize = 0;
        /// <summary>
        /// 压缩百分比
        /// </summary>
        public int CompressionPercent = 0;
        /// <summary>
        /// 临时文件
        /// </summary>
        public string TempFile = null;
        /// <summary>
        /// 压缩文件
        /// </summary>
        public string ZipFile = null;
        /// <summary>
        /// 是否删除临时文件
        /// </summary>
        public bool TempFileDeleted = false;
        public bool Errors = false;
    }
}
