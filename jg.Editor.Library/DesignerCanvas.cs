namespace jg.Editor.Library
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Xml;
    using jg.Editor.Library.Topic;
    using jg.Editor.Library.Property;
    using jg.Editor.Library.Control;
    using System.ComponentModel;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Xml.Serialization;
    using System.Windows.Shapes;
    using System.Collections.ObjectModel;

    public class DesignerCanvas : Canvas
    {
        public delegate void OnSelectedChanged(object sender, Guid ItemId, bool IsSelected);
        public static OnSelectedChanged SelectedChanged = null;

        public delegate void OnMouseOver(MouseEventArgs e);
        public static OnMouseOver DesignerCanvasMouseOver = null;
        private Point? dragStartPoint = null;

        public Guid PageId { get; set; }

        //private ControlTimeLine controltimeline = null;

        //public ControlTimeLine ControlTimeLine
        //{
        //    get { return controltimeline; }
        //    set
        //    {
        //        controltimeline = value;
        //        controltimeline.SelectedItemChanged += controltimeline_SelectedItemChanged;
        //        controltimeline.SetZIndex += new Editor.ControlTimeLine.OnSetZIndex(controltimeline_SetZIndex);
        //    }
        //}
        private ControlTimeLine controltimeline = null;
        public ControlTimeLine controlTimeLine
        {
            get { return controltimeline; }
            set
            {
                controltimeline = value;
                controltimeline.SelectedItemChanged += controltimeline_SelectedItemChanged;
                controltimeline.SetZIndex += controltimeline_SetZIndex;
            }
        }

        void controltimeline_SetZIndex(Guid Id, int ZIndex)
        {
            UIElement element = Children.OfType<DesignerItem>().FirstOrDefault(model => model.ItemId == Id);

            foreach (var v in Children.OfType<DesignerItem>())
            {
                object oo = v.ItemId;
            }

            if (element != null)
                Panel.SetZIndex(element, ZIndex);
        }

        private enumStageSwitch stageswitch = enumStageSwitch.SwitchFF00003D;
        public enumStageSwitch StageSwitch
        {
            get { return stageswitch; }
            set { stageswitch = value; }
        }

        private bool autonext = false;

        /// <summary>
        /// 自动跳转到下一页
        /// </summary>
        public bool AutoNext
        {
            get { return autonext; }
            set { autonext = value; }
        }


        private bool isVisable = true;
        /// <summary>
        /// 是否隐藏此页
        /// </summary>
        public bool IsVisable
        {
            get { return isVisable; }
            set { isVisable = value; }
        }




        void controltimeline_SelectedItemChanged(object sender, Guid ItemId)
        {
            ControlTimeLine item = sender as ControlTimeLine;
            if (item == null) return;
            if (item.PageId != this.PageId) return;

            DeselectAll();

            var selectedItems = from info in this.Children.OfType<DesignerItem>()
                                where info.ItemId == ItemId
                                select info;

            if (selectedItems != null)
                foreach (var v in selectedItems)
                    v.IsSelected = true;

        }

        public PropertyManage propertyManage = new PropertyManage();

        public static RemoteControl rc = new RemoteControl();

        public IEnumerable<DesignerItem> SelectedItems
        {
            get
            {
                var selectedItems = from item in this.Children.OfType<DesignerItem>()
                                    where item.IsSelected == true
                                    select item;
                return selectedItems;
            }
        }

        public void DeselectAll()
        {
            foreach (DesignerItem item in this.SelectedItems)
            {
                item.IsSelected = false;
            }
        }

        public static void Redo()
        {
            rc.RedoCommand();
        }

        public static void Undo()
        {
            rc.UndoCommand();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            PropertyManage.PropertyChanged += new PropertyManage.OnPropertyChanged(this.propertyManage_PropertyChanged);
        }

        public void SelectedCanvas()
        {
            this.DeselectAll();
            if (propertyManage != null) propertyManage.Stage = this;
            base.Focus();

            RefreshTimeControl();

            if (SelectedChanged != null) SelectedChanged(this, this.PageId, true);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Source == this)
            {
                this.dragStartPoint = new Point?(e.GetPosition(this));
                //this.DeselectAll();
                //if (propertyManage != null) propertyManage.Stage = this;
                //base.Focus();

                //RefreshTimeControl();
                SelectedCanvas();
                e.Handled = true;
            }
            //if (SelectedChanged != null) SelectedChanged(this, this.PageId, true);
        }

        void RefreshTimeControl()
        {
            Guid SelItem = Guid.NewGuid();
            ObservableCollection<TimeLineItemInfo> list = new ObservableCollection<TimeLineItemInfo>();
            if (controlTimeLine == null) return;

            DesignerItem designerItem;
            controlTimeLine.PageId = this.PageId;

            foreach (var v in this.Children.OfType<DesignerItem>().OrderByDescending(model => Panel.GetZIndex(model)))
            {
                designerItem = v as DesignerItem;
                if (designerItem == null) continue;
                if (designerItem.timeLineItemInfo == null)
                    switch (((ToolboxItem)designerItem.Content).AssetType)
                    {
                        case AssetType.Movie:
                        case AssetType.Sound:
                            designerItem.timeLineItemInfo = new TimeLineItemInfo("", 0, designerItem.ItemId, ((ToolboxItem)designerItem.Content).AssetType, ((ToolboxItem)designerItem.Content).TimeLength);
                            break;
                        default:
                            designerItem.timeLineItemInfo = new TimeLineItemInfo();
                            break;
                    }


                designerItem.timeLineItemInfo.Id = v.ItemId;
                designerItem.timeLineItemInfo.assetType = ((ToolboxItem)v.Content).AssetType;
                designerItem.timeLineItemInfo.Name = v.ItemName;


                list.Add(designerItem.timeLineItemInfo);
                if (designerItem.IsSelected == true) SelItem = designerItem.ItemId;
            }
            controlTimeLine.SetPage(this.PageId, list, SelItem);
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                for (int i = base.Children.Count - 1; i >= 0; i--)
                {
                    DesignerItem item = base.Children[i] as DesignerItem;
                    if (item == null) continue;
                    ControlRemove controlRemove = new ControlRemove(item);
                    rc.RedoCommand(controlRemove);
                    if ((item != null) && item.IsSelected)
                    {
                        base.Children.RemoveAt(i);
                    }
                }
            }
            base.OnPreviewKeyUp(e);
        }

        private void newItem_ItemDragComplete(object sender, double width, double height, double left, double top, double oldwidth, double oldheight, double oldleft, double oldtop)
        {
            DesignerItem item = sender as DesignerItem;
            if (item != null)
            {
                ControlMoveOrResize control = new ControlMoveOrResize(item);
                rc.RedoCommand(control);
                control.Execute(width, height, left, top, oldwidth, oldheight, oldleft, oldtop);
            }
            if (controlTimeLine == null) return;
            AssetDoubleProperty doubleProperty;
            doubleProperty = new AssetDoubleProperty() { Name = "PropertyWidthCommand", Value = width };
            controlTimeLine.AddProperty(doubleProperty);

            doubleProperty = new AssetDoubleProperty() { Name = "PropertyHeightCommand", Value = height };
            controlTimeLine.AddProperty(doubleProperty);

            doubleProperty = new AssetDoubleProperty() { Name = "PropertyXCommand", Value = left };
            controlTimeLine.AddProperty(doubleProperty);

            doubleProperty = new AssetDoubleProperty() { Name = "PropertyYCommand", Value = top };
            controlTimeLine.AddProperty(doubleProperty);


        }

        private void newItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.propertyManage != null)
            {
                this.propertyManage.Source = this.SelectedItems.ToList<DesignerItem>();
                RefreshTimeControl();
            }
        }

        public static Dictionary<Guid, DesignerItem> newDoesignerItemDictionary = new Dictionary<Guid, DesignerItem>();
        public void AddItem(SaveItemInfo info)
        {

            try
            {
                TransformGroup transformGroup;
                DesignerItem newItem = null;
                ToolboxItem toolboxItem;
                bool IsYes = false;

                if (newDoesignerItemDictionary.Keys.Count > 0)
                {
                    foreach (var v in newDoesignerItemDictionary.Keys)
                    {
                        if (info.ItemId == v)
                        {
                            IsYes = true;
                            newItem = newDoesignerItemDictionary[v];
                            ToolboxItem.GetToolBoxItem(info);
                            break;
                        }
                    }
                }
                if (!IsYes)
                {
                    newItem = new DesignerItem();
                    newDoesignerItemDictionary.Add(info.ItemId, newItem);
                    toolboxItem = ToolboxItem.GetToolBoxItem(info);
                    Panel.SetZIndex(newItem, info.ZIndex);
                    newItem.ItemDragComplete += this.newItem_ItemDragComplete;
                    newItem.SelectedChanged += newItem_SelectedChanged;
                    newItem.Width = info.Width;
                    newItem.Height = info.Height;
                    newItem.FontFamily = new FontFamily(info.FontFamily);
                    newItem.FontSize = info.FontSize;
                   
                    newItem.assetActionInfo = info.assetActionInfo;
                    newItem.timeLineItemInfo = info.timeLineItemInfo;
                    newItem.Content = toolboxItem;
                    newItem.LineHeight = info.LineHeight;
                    Canvas.SetLeft(newItem, info.X);
                    Canvas.SetTop(newItem, info.Y);


                    transformGroup = ((ToolboxItem)newItem.Content).RenderTransform as TransformGroup;
                    foreach (var v in transformGroup.Children)
                    {
                        switch (v.GetType().Name)
                        {
                            case "RotateTransform":
                                ((RotateTransform)v).Angle = info.Angle;
                                break;
                            case "ScaleTransform":
                                ((ScaleTransform)v).ScaleX = info.ScaleX == 0 ? 1 : info.ScaleX;
                                ((ScaleTransform)v).ScaleY = info.ScaleY == 0 ? 1 : info.ScaleY;
                                break;
                            case "SkewTransform":
                                ((SkewTransform)v).AngleX = info.SkewX;
                                ((SkewTransform)v).AngleY = info.SkewY;
                                break;
                            case "TranslateTransform":
                                ((TranslateTransform)v).X = info.TranslateX;
                                ((TranslateTransform)v).Y = info.TranslateY;
                                break;
                        }
                    }
                }
                //newItem = new DesignerItem();



                if (!base.Children.Contains(newItem))
                {
                    base.Children.Add(newItem);

                    newItem.PropertyChanged += new PropertyChangedEventHandler(this.newItem_PropertyChanged);
                    ControlCreate controlCreate = new ControlCreate(newItem);
                    rc.RedoCommand(controlCreate);
                    //this.DeselectAll();
                    newItem.IsSelected = true;
                }


            }
            catch (Exception ex)
            {


            }



        }

        public void AddItem(SaveItemInfo info,bool falseVale)
        {

            try
            {
                TransformGroup transformGroup;
                DesignerItem newItem = null;
                ToolboxItem toolboxItem;
                bool IsYes = false;

                if (newDoesignerItemDictionary.Keys.Count > 0)
                {
                    foreach (var v in newDoesignerItemDictionary.Keys)
                    {
                        if (info.ItemId == v)
                        {
                            IsYes = true;
                            newItem = newDoesignerItemDictionary[v];
                            ToolboxItem.GetToolBoxItem(info);
                            break;
                        }
                    }
                }
                if (!IsYes)
                {
                    newItem = new DesignerItem();
                    newDoesignerItemDictionary.Add(info.ItemId, newItem);
                    toolboxItem = ToolboxItem.GetToolBoxItem(info);
                    Panel.SetZIndex(newItem, info.ZIndex);
                    newItem.ItemDragComplete += this.newItem_ItemDragComplete;
                    newItem.SelectedChanged += newItem_SelectedChanged;
                    newItem.Width = info.Width;
                    newItem.Height = info.Height;
                    newItem.FontFamily = new FontFamily(info.FontFamily);
                    newItem.FontSize = info.FontSize;
                 
                    newItem.assetActionInfo = info.assetActionInfo;
                    newItem.timeLineItemInfo = info.timeLineItemInfo;
                    newItem.Content = toolboxItem;
                    newItem.LineHeight = info.LineHeight;

                    Canvas.SetLeft(newItem, info.X);
                    Canvas.SetTop(newItem, info.Y);


                    transformGroup = ((ToolboxItem)newItem.Content).RenderTransform as TransformGroup;
                    foreach (var v in transformGroup.Children)
                    {
                        switch (v.GetType().Name)
                        {
                            case "RotateTransform":
                                ((RotateTransform)v).Angle = info.Angle;
                                break;
                            case "ScaleTransform":
                                ((ScaleTransform)v).ScaleX = info.ScaleX == 0 ? 1 : info.ScaleX;
                                ((ScaleTransform)v).ScaleY = info.ScaleY == 0 ? 1 : info.ScaleY;
                                break;
                            case "SkewTransform":
                                ((SkewTransform)v).AngleX = info.SkewX;
                                ((SkewTransform)v).AngleY = info.SkewY;
                                break;
                            case "TranslateTransform":
                                ((TranslateTransform)v).X = info.TranslateX;
                                ((TranslateTransform)v).Y = info.TranslateY;
                                break;
                        }
                    }
                }
                //newItem = new DesignerItem();



                if (!base.Children.Contains(newItem))
                {
                    base.Children.Add(newItem);

                    newItem.PropertyChanged += new PropertyChangedEventHandler(this.newItem_PropertyChanged);
                    ControlCreate controlCreate = new ControlCreate(newItem);
                    rc.RedoCommand(controlCreate);
                    //this.DeselectAll();
                    newItem.IsSelected = falseVale;
                }


            }
            catch (Exception ex)
            {


            }



        }
        /// <summary>
        /// 添加的时候触发
        /// </summary>
        /// <param name="xamlString"></param>
        /// <param name="AssetPath"></param>
        /// <param name="Thumbnails"></param>
        /// <param name="itemsDis"></param>
        public void AddItem(string xamlString, string AssetPath, string Thumbnails, string itemsDis)
        {
            try
            {
                string TempPath;
                DesignerItem newItem = null;
                ToolboxItem item = null;
                newItem = new DesignerItem();
                Image image = null;
                BitmapImage bipImg = null;
                if (!String.IsNullOrEmpty(xamlString))
                {
                    #region
                    item = new ToolboxItem();

                    item.AssetPath = AssetPath;
                    item.Thumbnails = Thumbnails;
                    item.ItemsDis = itemsDis;
                    switch (xamlString)
                    {
                        case "Text":
                            item.AssetType = AssetType.Text;
                            ControlTextEditor controlTextEditor = new ControlTextEditor();
                            controlTextEditor.mainRTB.Tag = item;
                            controlTextEditor.mainRTB.TextChanged += mainRTB_TextChanged;
                            item.Content = controlTextEditor;
                            newItem.FontSize = 18;

                            break;
                        case "Topic":
                            item.AssetType = AssetType.Topic;
                            Topic.TopicControl topicControl = new TopicControl(true);

                            TopicInfo topicInfo = new TopicInfo() { TopicType = TopicType.Judge, Title = "题干", Score = 10 };
                            topicInfo.OptionCount = 2;
                            for (int i = 0; i < topicInfo.OptionCount; i++)
                            {
                                topicInfo.TopicOptionList.Add(new TopicOptionInfo() { Id = i, Index = i, Title = "题目" + (i + 1).ToString(), Right = false });
                            }
                            topicInfo.TopicOptionList[0].Right = true;
                            topicControl.TopicInfo = topicInfo;
                            topicControl.Background = Brushes.White;
                            topicControl.ItemForeground = Brushes.Black;

                            item.Content = topicControl;
                            break;
                        case "TopicDrag":
                            item.AssetType = AssetType.TopicDrag;
                            List<TopicDragItemAnswerInfo> topicDragItemAnswerList = new List<TopicDragItemAnswerInfo>();
                            List<TopicDragItemInfo> topicDragItemList = new List<TopicDragItemInfo>();

                            topicDragItemList.Add(new TopicDragItemInfo() { Id = Guid.NewGuid(), Text = "项目一" });
                            topicDragItemList.Add(new TopicDragItemInfo() { Id = Guid.NewGuid(), Text = "项目二" });
                            topicDragItemList.Add(new TopicDragItemInfo() { Id = Guid.NewGuid(), Text = "项目三" });
                            topicDragItemList.Add(new TopicDragItemInfo() { Id = Guid.NewGuid(), Text = "项目四" });
                            topicDragItemList.Add(new TopicDragItemInfo() { Id = Guid.NewGuid(), Text = "项目五" });
                            topicDragItemList.Add(new TopicDragItemInfo() { Id = Guid.NewGuid(), Text = "项目六" });
                            topicDragItemList.Add(new TopicDragItemInfo() { Id = Guid.NewGuid(), Text = "项目七" });
                            topicDragItemList.Add(new TopicDragItemInfo() { Id = Guid.NewGuid(), Text = "项目八" });

                            topicDragItemAnswerList.Add(new TopicDragItemAnswerInfo() { Id = topicDragItemList[0].Id, AnswerId = Guid.NewGuid(), AnswerPoint = new Point(100, 100), QuestionId = Guid.NewGuid(), QuestionPoint = new Point(500, 100) });
                            topicDragItemAnswerList.Add(new TopicDragItemAnswerInfo() { Id = topicDragItemList[2].Id, AnswerId = Guid.NewGuid(), AnswerPoint = new Point(100, 200), QuestionId = Guid.NewGuid(), QuestionPoint = new Point(500, 200) });
                            topicDragItemAnswerList.Add(new TopicDragItemAnswerInfo() { Id = topicDragItemList[5].Id, AnswerId = Guid.NewGuid(), AnswerPoint = new Point(100, 300), QuestionId = Guid.NewGuid(), QuestionPoint = new Point(500, 300) });
                            topicDragItemAnswerList.Add(new TopicDragItemAnswerInfo() { Id = topicDragItemList[3].Id, AnswerId = Guid.NewGuid(), AnswerPoint = new Point(100, 400), QuestionId = Guid.NewGuid(), QuestionPoint = new Point(500, 400) });

                            ControlTopicDrag controlTopicDarg = new ControlTopicDrag(topicDragItemAnswerList, topicDragItemList, true);

                            item.Content = controlTopicDarg;
                            break;
                        case "Movie":
                            image = new Image();
                            TempPath = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Temp" + item.AssetPath.Substring(item.AssetPath.LastIndexOf("\\"));
                            // FileSecurity.decryptHead(Globals.AssetDecryptKey, TempPath, item.AssetPath);

                            bipImg = new BitmapImage();
                            bipImg = new BitmapImage(new Uri(item.Thumbnails, UriKind.Absolute));
                            image = new Image() { Source = bipImg };
                            image.Stretch = Stretch.Fill;
                            item.TimeLength = Common.GetMediaTimeLength(TempPath);
                            item.AssetType = AssetType.Movie;
                            item.Content = image;
                            break;
                        case "Image":
                            image = new Image();
                            bipImg = new BitmapImage();
                            bipImg = new BitmapImage(new Uri(item.Thumbnails, UriKind.Absolute));
                            image = new Image() { Source = bipImg };
                            image.Stretch = Stretch.Fill;

                            //item.Height = bipImg.PixelHeight;
                            //item.Width = bipImg.PixelWidth;
                            item.AssetType = AssetType.Image;
                            item.Content = image;

                            break;
                        case "TextGrid":
                            item.AssetType = AssetType.TextGrid;
                            ControlTextGrid controlTextGrid = new ControlTextGrid();
                            controlTextGrid.RowCount = 2;
                            controlTextGrid.ColumnCount = 2;
                            item.Content = controlTextGrid;

                            controlTextGrid.TextBackground = Brushes.White;
                            controlTextGrid.TextForeground = Brushes.Black;
                            item.ItemBackground = Brushes.White;
                            item.ItemForeground = Brushes.Black;
                            break;
                        case "Sound":
                            image = new Image();
                            //FileSecurity.decryptFile("2-1655469",TempPath, info.AssetPath);
                            TempPath = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Temp" + item.AssetPath.Substring(item.AssetPath.LastIndexOf("\\"));
                            // FileSecurity.decryptHead(Globals.AssetDecryptKey, TempPath, item.AssetPath);
                            bipImg = new BitmapImage();
                            bipImg = new BitmapImage(new Uri(item.Thumbnails, UriKind.Absolute));
                            image = new Image() { Source = bipImg };
                            image.Stretch = Stretch.Fill;
                            item.TimeLength = Common.GetMediaTimeLength(TempPath);
                            item.AssetType = AssetType.Sound;
                            item.Content = image;
                            break;
                        case "Document":
                            image = new Image();
                            bipImg = new BitmapImage();
                            bipImg = new BitmapImage(new Uri(item.Thumbnails, UriKind.Absolute));
                            image = new Image() { Source = bipImg };
                            image.Stretch = Stretch.Fill;
                            item.AssetType = AssetType.Document;
                            item.Content = image;
                            break;
                        case "Message":
                            item.AssetType = AssetType.Message;
                            ControlMessage controlMessage = new ControlMessage(true);
                            item.Content = controlMessage;
                            item.ItemBackground = Brushes.White;
                            break;
                        case "Line":
                            item.AssetType = AssetType.Line;
                            ControlLine controlLine = new ControlLine(true);
                            item.Content = controlLine;
                            item.ItemBackground = Brushes.Black;
                            break;
                        case "HTML5":
                            image = new Image();
                            bipImg = new BitmapImage();
                            bipImg = new BitmapImage(new Uri(item.Thumbnails, UriKind.Absolute));
                            image = new Image() { Source = bipImg };

                            item.AssetType = AssetType.HTML5;
                            image.Stretch = Stretch.Fill;
                            item.Content = image;
                            break;
                        case "TPageGroup":
                            item.AssetType = AssetType.TPageGroup;
                            TPageControl tpc = new TPageControl();
                            item.Content = tpc;

                            break;
                        default:
                            item = XamlReader.Load(XmlReader.Create(new StringReader(xamlString))) as ToolboxItem;
                            if (item.AssetType == AssetType.Shape)
                            {
                                System.Windows.Shapes.Path path = item.Content as System.Windows.Shapes.Path;
                                path.Stretch = Stretch.Fill;
                            }
                            break;
                    }
                    #endregion
                }
                if (item != null)
                {
                    if (String.IsNullOrEmpty(xamlString))
                    {
                        Panel panel = item.Parent as Panel;
                        if (panel != null)
                        {
                            panel.Children.Remove(item);
                        }
                        else
                        {
                            return;
                        }
                    }


                    item.ItemId = Guid.NewGuid();

                    newItem.Content = item;
                    newItem.ItemDragComplete += new DesignerItem.OnItemDragComplete(this.newItem_ItemDragComplete);
                    newItem.SelectedChanged += new DesignerItem.OnSelectedChanged(newItem_SelectedChanged);
                    Point position = new Point(0, 0);
                    if (item.MinHeight != 0 && item.MinWidth != 0)
                    {
                        newItem.Width = item.MinWidth * 2;
                        newItem.Height = item.MinHeight * 2;
                    }
                    else
                    {
                        switch (xamlString)
                        {
                            case "Topic":
                                newItem.Width = 400;
                                newItem.Height = 300;
                                break;
                            case "TopicDrag":
                                newItem.Width = 500;
                                newItem.Height = 400;
                                break;
                            default:
                                newItem.Width = item.Width;
                                newItem.Height = item.Height;
                                break;
                        }
                        if (bipImg != null)
                        {
                            newItem.Width = bipImg.PixelWidth;
                            newItem.Height = bipImg.PixelHeight;
                        }
                    }

                    DesignerCanvas.SetLeft(newItem, 0);
                    DesignerCanvas.SetTop(newItem, 0);
                    newItem.ItemName = item.AssetType.ToString() + base.Children.Count.ToString();
                    base.Children.Add(newItem);
                    newItem.PropertyChanged += new PropertyChangedEventHandler(this.newItem_PropertyChanged);
                    Panel.SetZIndex(newItem, this.Children.Count);
                    ControlCreate controlCreate = new ControlCreate(newItem);
                    rc.RedoCommand(controlCreate);
                    this.DeselectAll();
                    newItem.IsSelected = true;
                }
                RefreshTimeControl();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// 添加的时候触发
        /// </summary>
        /// <param name="xamlString"></param>
        /// <param name="AssetPath"></param>
        /// <param name="Thumbnails"></param>
        /// <param name="itemsDis"></param>
        /// <param name="Isentrypt"></param>    
        public void AddItem(string xamlString, string AssetPath, string Thumbnails, string itemsDis,bool Isentrypt)
        {
            try
            {
                //string TempPath;
                DesignerItem newItem = null;
                ToolboxItem item = null;
                newItem = new DesignerItem();
                Image image = null;
                BitmapImage bipImg = null;
                if (!String.IsNullOrEmpty(xamlString))
                {
                    #region
                    item = new ToolboxItem();

                    item.AssetPath = AssetPath;
                    item.Thumbnails = Thumbnails;
                    item.ItemsDis = itemsDis;
                    switch (xamlString)
                    {
                        case "Text":
                            item.AssetType = AssetType.Text;
                            ControlTextEditor controlTextEditor = new ControlTextEditor();
                            controlTextEditor.mainRTB.Tag = item;
                            controlTextEditor.mainRTB.TextChanged += mainRTB_TextChanged;
                            item.Content = controlTextEditor;
                            newItem.FontSize = 18;

                            break;
                        case "Topic":
                            item.AssetType = AssetType.Topic;
                            Topic.TopicControl topicControl = new TopicControl(true);

                            TopicInfo topicInfo = new TopicInfo() { TopicType = TopicType.Judge, Title = "题干", Score = 10 };
                            topicInfo.OptionCount = 2;
                            for (int i = 0; i < topicInfo.OptionCount; i++)
                            {
                                topicInfo.TopicOptionList.Add(new TopicOptionInfo() { Id = i, Index = i, Title = "题目" + (i + 1).ToString(), Right = false });
                            }
                            topicInfo.TopicOptionList[0].Right = true;
                            topicControl.TopicInfo = topicInfo;
                            topicControl.Background = Brushes.White;
                            topicControl.ItemForeground = Brushes.Black;

                            item.Content = topicControl;
                            break;
                        case "TopicDrag":
                            item.AssetType = AssetType.TopicDrag;
                            List<TopicDragItemAnswerInfo> topicDragItemAnswerList = new List<TopicDragItemAnswerInfo>();
                            List<TopicDragItemInfo> topicDragItemList = new List<TopicDragItemInfo>();

                            topicDragItemList.Add(new TopicDragItemInfo() { Id = Guid.NewGuid(), Text = "项目一" });
                            topicDragItemList.Add(new TopicDragItemInfo() { Id = Guid.NewGuid(), Text = "项目二" });
                            topicDragItemList.Add(new TopicDragItemInfo() { Id = Guid.NewGuid(), Text = "项目三" });
                            topicDragItemList.Add(new TopicDragItemInfo() { Id = Guid.NewGuid(), Text = "项目四" });
                            topicDragItemList.Add(new TopicDragItemInfo() { Id = Guid.NewGuid(), Text = "项目五" });
                            topicDragItemList.Add(new TopicDragItemInfo() { Id = Guid.NewGuid(), Text = "项目六" });
                            topicDragItemList.Add(new TopicDragItemInfo() { Id = Guid.NewGuid(), Text = "项目七" });
                            topicDragItemList.Add(new TopicDragItemInfo() { Id = Guid.NewGuid(), Text = "项目八" });

                            topicDragItemAnswerList.Add(new TopicDragItemAnswerInfo() { Id = topicDragItemList[0].Id, AnswerId = Guid.NewGuid(), AnswerPoint = new Point(100, 100), QuestionId = Guid.NewGuid(), QuestionPoint = new Point(500, 100) });
                            topicDragItemAnswerList.Add(new TopicDragItemAnswerInfo() { Id = topicDragItemList[2].Id, AnswerId = Guid.NewGuid(), AnswerPoint = new Point(100, 200), QuestionId = Guid.NewGuid(), QuestionPoint = new Point(500, 200) });
                            topicDragItemAnswerList.Add(new TopicDragItemAnswerInfo() { Id = topicDragItemList[5].Id, AnswerId = Guid.NewGuid(), AnswerPoint = new Point(100, 300), QuestionId = Guid.NewGuid(), QuestionPoint = new Point(500, 300) });
                            topicDragItemAnswerList.Add(new TopicDragItemAnswerInfo() { Id = topicDragItemList[3].Id, AnswerId = Guid.NewGuid(), AnswerPoint = new Point(100, 400), QuestionId = Guid.NewGuid(), QuestionPoint = new Point(500, 400) });

                            ControlTopicDrag controlTopicDarg = new ControlTopicDrag(topicDragItemAnswerList, topicDragItemList, true);

                            item.Content = controlTopicDarg;
                            break;
                        case "Movie":
                            image = new Image();
                            //TempPath = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Temp" + item.AssetPath.Substring(item.AssetPath.LastIndexOf("\\"));
                            //FileSecurity.decryptHead(Globals.AssetDecryptKey, TempPath, item.AssetPath);

                            bipImg = new BitmapImage();
                            bipImg = new BitmapImage(new Uri(item.Thumbnails, UriKind.Absolute));
                            image = new Image() { Source = bipImg };
                            image.Stretch = Stretch.Fill;
                            item.TimeLength = Common.GetMediaTimeLength(item.AssetPath);
                            item.AssetType = AssetType.Movie;
                            item.Content = image;
                            break;
                       
                        case "Image":
                            image = new Image();
                            bipImg = new BitmapImage();
                            bipImg = new BitmapImage(new Uri(item.Thumbnails, UriKind.Absolute));
                            image = new Image() { Source = bipImg };
                            image.Stretch = Stretch.Fill;

                            //item.Height = bipImg.PixelHeight;
                            //item.Width = bipImg.PixelWidth;
                            item.AssetType = AssetType.Image;
                            item.Content = image;

                            break;
                        case "TextGrid":
                            item.AssetType = AssetType.TextGrid;
                            ControlTextGrid controlTextGrid = new ControlTextGrid();
                            controlTextGrid.RowCount = 2;
                            controlTextGrid.ColumnCount = 2;
                            item.Content = controlTextGrid;

                            controlTextGrid.TextBackground = Brushes.White;
                            controlTextGrid.TextForeground = Brushes.Black;
                            item.ItemBackground = Brushes.White;
                            item.ItemForeground = Brushes.Black;
                            break;
                        case "Sound":
                            image = new Image();
                            //FileSecurity.decryptFile("2-1655469",TempPath, info.AssetPath);
                            //TempPath = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Temp" + item.AssetPath.Substring(item.AssetPath.LastIndexOf("\\"));
                            //FileSecurity.decryptHead(Globals.AssetDecryptKey, TempPath, item.AssetPath);
                            bipImg = new BitmapImage();
                            bipImg = new BitmapImage(new Uri(item.Thumbnails, UriKind.Absolute));
                            image = new Image() { Source = bipImg };
                            image.Stretch = Stretch.Fill;
                            item.TimeLength = Common.GetMediaTimeLength(item.AssetPath);
                            item.AssetType = AssetType.Sound;
                            item.Content = image;
                            break;
                        case "Document":
                            image = new Image();
                            bipImg = new BitmapImage();
                            bipImg = new BitmapImage(new Uri(item.Thumbnails, UriKind.Absolute));
                            image = new Image() { Source = bipImg };
                            image.Stretch = Stretch.Fill;
                            item.AssetType = AssetType.Document;
                            item.Content = image;
                            break;
                        case "Message":
                            item.AssetType = AssetType.Message;
                            ControlMessage controlMessage = new ControlMessage(true);
                            item.Content = controlMessage;
                            item.ItemBackground = Brushes.White;
                            break;
                        case "Line":
                            item.AssetType = AssetType.Line;
                            ControlLine controlLine = new ControlLine(true);
                            item.Content = controlLine;
                            item.ItemBackground = Brushes.Black;
                            break;
                        case "HTML5":
                            image = new Image();
                            bipImg = new BitmapImage();
                            bipImg = new BitmapImage(new Uri(item.Thumbnails, UriKind.Absolute));
                            image = new Image() { Source = bipImg };

                            item.AssetType = AssetType.HTML5;
                            image.Stretch = Stretch.Fill;
                            item.Content = image;
                            break;
                        case "TPageGroup":
                            item.AssetType = AssetType.TPageGroup;
                            TPageControl tpc = new TPageControl();
                            item.Content = tpc;

                            break;
                        default:
                            item = XamlReader.Load(XmlReader.Create(new StringReader(xamlString))) as ToolboxItem;
                            if (item.AssetType == AssetType.Shape)
                            {
                                System.Windows.Shapes.Path path = item.Content as System.Windows.Shapes.Path;
                                path.Stretch = Stretch.Fill;
                            }
                            break;
                    }
                    #endregion
                }
                if (item != null)
                {
                    if (String.IsNullOrEmpty(xamlString))
                    {
                        Panel panel = item.Parent as Panel;
                        if (panel != null)
                        {
                            panel.Children.Remove(item);
                        }
                        else
                        {
                            return;
                        }
                    }


                    item.ItemId = Guid.NewGuid();
                    item.IsDescPt = false;
                    newItem.Content = item;
                    newItem.ItemDragComplete += new DesignerItem.OnItemDragComplete(this.newItem_ItemDragComplete);
                    newItem.SelectedChanged += new DesignerItem.OnSelectedChanged(newItem_SelectedChanged);
                    Point position = new Point(0, 0);
                    if (item.MinHeight != 0 && item.MinWidth != 0)
                    {
                        newItem.Width = item.MinWidth * 2;
                        newItem.Height = item.MinHeight * 2;
                    }
                    else
                    {
                        switch (xamlString)
                        {
                            case "Topic":
                                newItem.Width = 400;
                                newItem.Height = 300;
                                break;
                            case "TopicDrag":
                                newItem.Width = 500;
                                newItem.Height = 400;
                                break;
                            default:
                                newItem.Width = item.Width;
                                newItem.Height = item.Height;
                                break;
                        }
                        if (bipImg != null)
                        {
                            newItem.Width = bipImg.PixelWidth;
                            newItem.Height = bipImg.PixelHeight;
                        }
                    }

                    DesignerCanvas.SetLeft(newItem, 0);
                    DesignerCanvas.SetTop(newItem, 0);
                    newItem.ItemName = item.AssetType.ToString() + base.Children.Count.ToString();
                    base.Children.Add(newItem);
                    newItem.PropertyChanged += new PropertyChangedEventHandler(this.newItem_PropertyChanged);
                    Panel.SetZIndex(newItem, this.Children.Count);
                    ControlCreate controlCreate = new ControlCreate(newItem);
                    rc.RedoCommand(controlCreate);
                    this.DeselectAll();
                    newItem.IsSelected = true;
                }
                RefreshTimeControl();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        /// <summary>
        /// 文本改变的时候
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mainRTB_TextChanged(object sender, TextChangedEventArgs e)
        {

            RichTextBox rtb = sender as RichTextBox;
            Rect rectStart = rtb.Document.ContentStart.GetCharacterRect(LogicalDirection.Forward);
            Rect rectEnd = rtb.Document.ContentEnd.GetCharacterRect(LogicalDirection.Forward);
            var height = rectEnd.Bottom - rectStart.Top;
            var remainH = rectEnd.Height / 2.0;
            var myHeight = Math.Min(MaxHeight, Math.Max(MinHeight, height + remainH));
            if (myHeight > rtb.ActualHeight)
            {
                if (rtb.Tag != null)
                {
                    ToolboxItem item = rtb.Tag as ToolboxItem;
                    item.IsLongText = true;
                }
            }
        }

        void newItem_SelectedChanged(object sender, Guid ItemId, bool IsSelected)
        {
            if (SelectedChanged != null)
            {
                SelectedChanged(sender, ItemId, IsSelected);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton != MouseButtonState.Pressed)
            {
                this.dragStartPoint = null;
            }

            if (this.dragStartPoint.HasValue)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    RubberbandAdorner adorner = new RubberbandAdorner(this, this.dragStartPoint);
                    if (adorner != null)
                    {
                        adornerLayer.Add(adorner);
                    }
                }

                e.Handled = true;
            }

            if (DesignerCanvasMouseOver != null)
            {
                DesignerCanvasMouseOver(e);
            }

        }
        private void propertyManage_PropertyChanged(ICommand command)
        {
            rc.RedoCommand(command);
        }
        public static void LayoutLeft(DesignerCanvas canvas)
        {
            double minLeft = double.MaxValue;
            foreach (var v in canvas.SelectedItems)
                minLeft = Math.Min(minLeft, DesignerCanvas.GetLeft(v));
            foreach (var v in canvas.SelectedItems)
                DesignerCanvas.SetLeft(v, minLeft);
        }
        public static void LayoutTop(DesignerCanvas canvas)
        {
            double maxTop = double.MaxValue;
            foreach (var v in canvas.SelectedItems)
                maxTop = Math.Min(maxTop, DesignerCanvas.GetTop(v));
            foreach (var v in canvas.SelectedItems)
                DesignerCanvas.SetTop(v, maxTop);
        }
        public static void LayoutRight(DesignerCanvas canvas)
        {
            double maxRight = 0;
            foreach (var v in canvas.SelectedItems)
                maxRight = Math.Max(maxRight, DesignerCanvas.GetLeft(v) + v.ActualWidth);
            foreach (var v in canvas.SelectedItems)
                DesignerCanvas.SetLeft(v, maxRight - v.ActualWidth);
        }
        public static void LayoutBottom(DesignerCanvas canvas)
        {
            double maxBottom = 0;
            foreach (var v in canvas.SelectedItems)
                maxBottom = Math.Max(maxBottom, DesignerCanvas.GetTop(v) + v.ActualHeight);
            foreach (var v in canvas.SelectedItems)
                DesignerCanvas.SetTop(v, maxBottom - v.ActualHeight);
        }
    }
}