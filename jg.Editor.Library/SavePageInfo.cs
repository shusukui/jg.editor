using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using System.IO;

namespace jg.Editor.Library
{
    /// <summary>
    /// 保存目录信息
    /// </summary>
    [Serializable]
    public class SavePageInfo
    {
        int stageswitch;
        string background;
        bool autonext;
        bool isVisable;
        System.Windows.Visibility _WpfVisibility = System.Windows.Visibility.Visible;

        /// <summary>
        /// wpf的显示与隐藏
        /// </summary>
        [XmlAttribute("WpfVisibility")]
        public System.Windows.Visibility WpfVisibility
        {
            get { return _WpfVisibility; }
            set { _WpfVisibility = value; }
        }
        public SavePageInfo()
        {
            saveItemList = new List<SaveItemInfo>();
            PageId = new Guid();
        }

        [XmlAttribute("PageId")]
        public Guid PageId { get; set; }

        [XmlAttribute("Height")]
        public double Height { get; set; }

        [XmlAttribute("Width")]
        public double Width { get; set; }

        [XmlAttribute("Background")]
        public string Background { get { return background; } set { background = value; } }

        [XmlElement("saveItemList")]
        public List<SaveItemInfo> saveItemList { get; set; }

        [XmlAttribute("StageSwitch")]
        public int StageSwitch
        {
            get { return stageswitch; }
            set { stageswitch = value; }
        }

        [XmlAttribute("AutoNext")]
        public bool AutoNext
        {
            get { return autonext; }
            set { autonext = value; }
        }


        [XmlAttribute("IsVisable")]
        public bool IsVisable
        {
            get { return isVisable; }
            set
            {
                isVisable = value;
                if (isVisable)
                {
                    WpfVisibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    WpfVisibility = System.Windows.Visibility.Collapsed;
                }
            }
        }
    }
}
