
namespace jg.Editor.Library
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Xml;
    using System.IO;

    /// <summary>
    /// 保存素材信息
    /// </summary>
    [Serializable]
    public class SaveItemInfo
    {
        double height = 0, width = 0, x = 0, y = 0, opacity = 0;
        double angle = 0, scalex = 1, scaley = 1, skewx = 0, skewy = 0, translatex = 0, translatey = 0, _score = 0;
        int zindex = 0;
        string itemname = "", assetpath = "", thumbnails = "", itemsDis = "";
        AssetActionInfo assetactioninfo;
        TimeLineItemInfo timelineiteminfo;
        abstractContent content;
        Guid _itemid;

        #region DesignerItem

        [XmlAttribute("ItemName")]
        public string ItemName
        {
            get { return itemname; }
            set { itemname = value; }
        }

        [XmlAttribute("ItemId")]
        public Guid ItemId
        {
            get { return _itemid; }
            set { _itemid = value; }
        }

        [XmlAttribute("Height")]
        public double Height
        {
            get { return height; }
            set { height = value; }
        }

        [XmlAttribute("Width")]
        public double Width
        {
            get { return width; }
            set { width = value; }
        }

        [XmlAttribute("X")]
        public double X
        {
            get { return x; }
            set { x = value; }
        }

        [XmlAttribute("Y")]
        public double Y
        {
            get { return y; }
            set { y = value; }
        }

        [XmlAttribute("ZIndex")]
        public int ZIndex
        {
            get { return zindex; }
            set { zindex = value; }
        }

        [XmlAttribute("Angle")]
        public double Angle
        {
            get { return angle; }
            set { angle = value; }
        }

        [XmlAttribute("ScaleX")]
        public double ScaleX
        {
            get { return scalex; }
            set { scalex = value; }
        }

        [XmlAttribute("ScaleY")]
        public double ScaleY
        {
            get { return scaley; }
            set { scaley = value; }
        }

        [XmlAttribute("SkewX")]
        public double SkewX
        {
            get { return skewx; }
            set { skewx = value; }
        }

        [XmlAttribute("SkewY")]
        public double SkewY
        {
            get { return skewy; }
            set { skewy = value; }
        }

        [XmlAttribute("TranslateX")]
        public double TranslateX
        {
            get { return translatex; }
            set { translatex = value; }
        }

        [XmlAttribute("TranslateY")]
        public double TranslateY
        {
            get { return translatey; }
            set { translatey = value; }
        }
        #endregion

        #region 字体文本
        [XmlAttribute("FontFamily")]
        public string FontFamily { get; set; }

        [XmlAttribute("FontSize")]
        public double FontSize { get; set; }

        [XmlAttribute("Bold")]
        public bool Bold { get; set; }

        [XmlAttribute("Italic")]
        public bool Italic { get; set; }
        [XmlAttribute("IsLongText")]
        public bool IsLongText { get; set; }

        [XmlAttribute("LineHeight")]
        public double LineHeight { get; set; }
        #endregion
        [XmlAttribute("IsShowDiv")]
        public bool IsShowDiv { get; set; }

        [XmlAttribute("IsDescPt")]
        public bool IsDescPt { get; set; }
        #region Asset

        public abstractContent Content
        {
            get { return content; }
            set { content = value; }
        }

        [XmlAttribute("assetType")]
        public AssetType assetType { get; set; }

        [XmlAttribute("AssetPath")]
        public string AssetPath { get { return assetpath; } set { assetpath = value; } }

        [XmlAttribute("Thumbnails")]
        public string Thumbnails
        {
            get { return thumbnails; }
            set { thumbnails = value; }
        }

        [XmlAttribute("ItemsDis")]
        public string ItemsDis
        {
            get { return itemsDis; }
            set { itemsDis = value; }
        }

        [XmlIgnore] //此为不序列化
        public System.Collections.ObjectModel.ObservableCollection<AssResInfo> AssetPathAndThumbnailsList { get; set; }
        /// <summary>
        /// 事件动作
        /// </summary>

        public AssetActionInfo assetActionInfo
        {
            get { return assetactioninfo; }
            set { assetactioninfo = value; }
        }

        public TimeLineItemInfo timeLineItemInfo
        {
            get { return timelineiteminfo; }
            set { timelineiteminfo = value; }
        }

        #endregion


        /// <summary>
        /// 背景色
        /// </summary>
        [XmlAttribute("Background")]
        public string Background { get; set; }
        /// <summary>
        /// 前景色
        /// </summary>
        [XmlAttribute("Foreground")]
        public string Foreground { get; set; }

        /// <summary>
        /// 透明度
        /// </summary>
        [XmlAttribute("Opacity")]
        public double Opacity
        {
            get { return opacity; }
            set { opacity = value; }
        }

        /// <summary>
        /// 分数
        /// </summary>
        [XmlAttribute("Score")]
        public double Score
        {
            get { return _score; }
            set { _score = value; }
        }
    }


}