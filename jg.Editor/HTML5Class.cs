using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jg.Editor
{
    public class HTML5Class
    {

        public HTML5Class()
        {

        }
       
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="ActionHtmlfileParm">启动项</param>
        /// <param name="ActionHtmlDisParm">启动项文件夹</param>
        /// <param name="FileNameParm">文件名字</param>
        public HTML5Class(string ActionHtmlfileParm, string ActionHtmlDisParm, string FileNameParm)
        {
            ActionHtmlfile = ActionHtmlfileParm;
            ActionHtmlDis = ActionHtmlDisParm;
            FileName = FileNameParm;
        }
        private string _ActionHtmlfile;

        /// <summary>
        ///启动文件
        /// </summary>
        public string ActionHtmlfile
        {
            get { return _ActionHtmlfile; }
            set { _ActionHtmlfile = value; }
        }


        private string _ActionHtmlDis;

        /// <summary>
        /// 启动的文件夹
        /// </summary>
        public string ActionHtmlDis
        {
            get { return _ActionHtmlDis; }
            set { _ActionHtmlDis = value; }
        }


        private string _FileName;

        /// <summary>
        /// 文件名字
        /// </summary>
        public string FileName
        {
            get { return _FileName; }
            set { _FileName = value; }
        }



        private string _ImgFileName;

        /// <summary>
        /// 缩略图文件
        /// </summary>
        public string ImgFileName
        {
            get { return _ImgFileName; }
            set { _ImgFileName = value; }
        }

    }
}
