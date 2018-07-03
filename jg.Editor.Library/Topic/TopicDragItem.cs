using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace jg.Editor.Topic
{
    [Serializable]
    public class TopicDragItem : ContentControl
    {
        public TopicDragItem()
        { }

        private int id = -1;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private string title = "";
        public string Title
        {
            get { return title; }
            set { title = value; DataContext = value; }
        }

        private int answerid = -1;
        /// <summary>
        /// 实际答案
        /// </summary>
        public int AnswerId
        {
            get { return answerid; }
            set { answerid = value; }
        }

        private int useranswer = -1;
        /// <summary>
        /// 用户答案
        /// </summary>
        public int UserAnswer
        {
            get { return useranswer; }
            set { useranswer = value; }
        }

        private Point linepoint = new Point(0, 0);
        /// <summary>
        /// 连线开始位置（左侧选项）
        /// </summary>

        public Point LinePoint { get { return linepoint; } set { linepoint = value; } }

    }
}