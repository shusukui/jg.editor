namespace jg.Editor.Library
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media.Imaging;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using jg.Editor.Library.Topic;
    using System.Xml;
    using System.IO;
    using jg.Editor.Library.Control;
    using System.Windows.Documents;
    using System.Windows.Forms.Integration;
    using System.Windows.Media.Animation;
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Collections.ObjectModel;

    public class ToolboxItem : ContentControl, IAssetInfo, INotifyPropertyChanged
    {
        public delegate void OnAddAsset(string ToolBoxItem);

        public static event OnAddAsset AddAsset = null;

        public event ShowMaxBox.ShowAsset showAsset = null;
        private bool IsDown = false;

        public static string xulrunner = "xulrunner\\";


        private bool isedit = true;
        public bool IsEdit { get { return isedit; } set { isedit = value; } }

        private bool isShowDiv;

        /// <summary>
        /// 是否弹出层
        /// </summary>
        public bool IsShowDiv
        {
            get { return isShowDiv; }
            set { isShowDiv = value; }
        }
        private bool _IsDescPt = true;

        /// <summary>
        /// 是否加密
        /// </summary>
        public bool IsDescPt
        {
            get { return _IsDescPt; }
            set { _IsDescPt = value; }
        }

        private double _LineHeight;
        /// <summary>
        /// 行高
        /// </summary>
        public double LineHeight
        {
            get { return _LineHeight; }
            set { _LineHeight = value; }
        }

        public static readonly DependencyProperty ItemBackgroundProperty = DependencyProperty.Register("ItemBackground", typeof(SolidColorBrush), typeof(ToolboxItem), new FrameworkPropertyMetadata(Brushes.White, new PropertyChangedCallback(ItemBackgroundProperty_Changed)));
        public static readonly DependencyProperty ItemForegroundProperty = DependencyProperty.Register("ItemForeground", typeof(SolidColorBrush), typeof(ToolboxItem), new FrameworkPropertyMetadata(Brushes.White, new PropertyChangedCallback(ItemForegroundProperty_Changed)));

        public SolidColorBrush ItemBackground
        {
            get { return (SolidColorBrush)GetValue(ItemBackgroundProperty); }
            set { SetValue(ItemBackgroundProperty, value); }
        }

        public SolidColorBrush ItemForeground
        {
            get { return (SolidColorBrush)GetValue(ItemForegroundProperty); }
            set
            {
                SetValue(ItemForegroundProperty, value);
                OnPropertyChanged("ItemForeground");

            }
        }

        private static void ItemBackgroundProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ToolboxItem item = d as ToolboxItem;
            if (null == item) return;

            switch (item.AssetType)
            {
                case AssetType.Topic:
                    Topic.TopicControl topicControl = item.Content as Topic.TopicControl;
                    if (topicControl == null) break;
                    topicControl.Background = (SolidColorBrush)e.NewValue;
                    break;
                case AssetType.TopicDrag:
                    Control.ControlTopicDrag controlTopicDrag = item.Content as Control.ControlTopicDrag;
                    if (controlTopicDrag == null) break;
                    controlTopicDrag.ItemBackground = (SolidColorBrush)e.NewValue;
                    break;
                case AssetType.Text:
                    ControlTextEditor rtb = item.Content as ControlTextEditor;
                    if (rtb != null)
                    {
                        rtb.Background = (SolidColorBrush)e.NewValue;
                        rtb.mainRTB.Document.Background = (SolidColorBrush)e.NewValue;
                    }
                    break;
                case AssetType.TextGrid:

                    jg.Editor.Library.Control.ControlTextGrid controlTextGrid = item.Content as jg.Editor.Library.Control.ControlTextGrid;
                    if (controlTextGrid != null)
                        controlTextGrid.TextBackground = (SolidColorBrush)e.NewValue;
                    break;
                case AssetType.Message:
                    jg.Editor.Library.Control.ControlMessage message = item.Content as jg.Editor.Library.Control.ControlMessage;
                    if (message != null)
                        message.Background = (SolidColorBrush)e.NewValue;
                    break;
                case AssetType.Line:
                    jg.Editor.Library.Control.ControlLine controlLine = item.Content as jg.Editor.Library.Control.ControlLine;
                    if (controlLine != null)
                        controlLine.Background = (SolidColorBrush)e.NewValue;
                    break;
            }
        }

        private static void ItemForegroundProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ToolboxItem item = d as ToolboxItem;
            if (null == item) return;

            switch (item.AssetType)
            {
                case AssetType.Topic:
                    Topic.TopicControl topicControl = item.Content as Topic.TopicControl;
                    if (topicControl == null) break;
                    topicControl.ItemForeground = (SolidColorBrush)e.NewValue;
                    break;
                case AssetType.TopicDrag:
                    Control.ControlTopicDrag controlTopicDrag = item.Content as Control.ControlTopicDrag;
                    if (controlTopicDrag == null) break;
                    controlTopicDrag.ItemForeground = (SolidColorBrush)e.NewValue;
                    break;
                case AssetType.TextGrid:
                    jg.Editor.Library.Control.ControlTextGrid controlTextGrid = item.Content as jg.Editor.Library.Control.ControlTextGrid;
                    if (controlTextGrid != null) controlTextGrid.TextForeground = (SolidColorBrush)e.NewValue;
                    break;
                case AssetType.Text:
                    jg.Editor.Library.Control.ControlTextEditor text = item.Content as jg.Editor.Library.Control.ControlTextEditor;
                    if (text != null)
                    {
                        text.mainRTB.Selection.ApplyPropertyValue(Run.ForegroundProperty, (SolidColorBrush)e.NewValue);
                    }
                    break;
                case AssetType.Message:
                    jg.Editor.Library.Control.ControlMessage message = item.Content as jg.Editor.Library.Control.ControlMessage;
                    if (message != null)
                        message.Foreground = (SolidColorBrush)e.NewValue;
                    break;
            }
        }

        protected Point? dragStartPoint = null;

        private double _timeLength = 50;
        public double TimeLength
        {
            get { return _timeLength; }
            set { _timeLength = value; }
        }
        private bool _IsLongText;
        public bool IsLongText
        {
            get { return _IsLongText; }
            set { _IsLongText = value; }
        }

        static ToolboxItem()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolboxItem), new FrameworkPropertyMetadata(typeof(ToolboxItem)));
        }

        public ToolboxItem()
        {
            TransformGroup tg = new TransformGroup();
            RotateTransform rotateTransform = new RotateTransform();
            ScaleTransform scaleTransform = new ScaleTransform();
            SkewTransform skewTransform = new SkewTransform();
            TranslateTransform translateTransform = new TranslateTransform();
            this.RenderTransformOrigin = new Point(0.5, 0.5);

            //ItemId = System.Guid.NewGuid();
            //this.Name = "A" + ItemId.ToString().Substring(0, 6);

            tg.Children.Add(rotateTransform);
            tg.Children.Add(scaleTransform);
            tg.Children.Add(skewTransform);
            tg.Children.Add(translateTransform);
            this.RenderTransform = tg;

        }




        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            if (IsEdit == false) return;
            this.dragStartPoint = new Point?(e.GetPosition(this));
            IsDown = true;
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            string xamlString;
            base.OnPreviewMouseUp(e);
            if (IsDown == false) return;
            IsDown = false;
            if (!(this.Parent is Toolbox) && !(this.Parent is WrapPanel)) return;


            switch (this.AssetType)
            {
                case jg.Editor.Library.AssetType.Text:
                    xamlString = "Text";
                    break;
                case jg.Editor.Library.AssetType.Topic:
                    xamlString = "Topic";
                    break;
                case jg.Editor.Library.AssetType.TopicDrag:
                    xamlString = "TopicDrag";
                    break;
                case jg.Editor.Library.AssetType.Image:
                    xamlString = "Image";
                    break;
                case jg.Editor.Library.AssetType.Movie:
                    xamlString = "Movie";
                    break;
                case jg.Editor.Library.AssetType.TextGrid:
                    xamlString = "TextGrid";
                    break;
                case jg.Editor.Library.AssetType.Sound:
                    xamlString = "Sound";
                    break;
                case Editor.Library.AssetType.Document:
                    xamlString = "Document";
                    break;
                case Editor.Library.AssetType.Message:
                    xamlString = "Message";
                    break;
                case Editor.Library.AssetType.Line:
                    xamlString = "Line";
                    break;
                case Editor.Library.AssetType.HTML5:
                    xamlString = "HTML5";
                    break;
                case Editor.Library.AssetType.TPageGroup:
                    xamlString = "TPageGroup";
                    break;
                default:
                    xamlString = XamlWriter.Save(this);
                    break;
            }
            if (AddAsset != null) AddAsset(xamlString);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {

            base.OnMouseMove(e);
            if (IsEdit == false) return;
            DataObject dataObject;
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                this.dragStartPoint = null;
                return;
            }

            DesignerCanvas canvas = Common.FindVisualParent<DesignerCanvas>(this) as DesignerCanvas;
            if (canvas != null)
            {
                object o = this.InputHitTest(e.GetPosition(this));

                if (this.InputHitTest(e.GetPosition(this)) is Run || this.InputHitTest(e.GetPosition(this)) is FlowDocument || this.InputHitTest(e.GetPosition(this)) is Paragraph)
                    return;

                if (Common.FindVisualParent<jg.Editor.Library.Control.ControlTopicDrag>((DependencyObject)this.InputHitTest(e.GetPosition(this))) != null) return;

                dataObject = new DataObject();
                dataObject.SetData("TOOLBOXITEM", this);

                if (this.Content is Control.ControlMessage) return;
                if (this.Content is Control.ControlLine) return;
                if (!(this.Content is Control.ControlTextGrid))
                    DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Copy);
            }
        }

        #region IAssetInfo

        /// <summary>
        /// 素材Id
        /// </summary>
        public Guid ItemId { get; set; }
        /// <summary>
        /// 素材名称
        /// </summary>
        public string ItemName { get; set; }

        public System.Collections.ObjectModel.ObservableCollection<AssResInfo> AssetPathAndThumbnailsList { get; set; }

        public AssetType AssetType { get; set; }

        public string ItemsDis { get; set; }
        public string Thumbnails { get; set; }

        public bool Create()
        {
            return true;
        }

        public string AssetPath { get; set; }

        #endregion
        public static Dictionary<Guid, System.Collections.ObjectModel.ObservableCollection<AssResInfo>> AssResInfoObser = new Dictionary<Guid, System.Collections.ObjectModel.ObservableCollection<AssResInfo>>();
        public static Dictionary<Guid, TPageControl> DirectoryTpageLoad = new Dictionary<Guid, TPageControl>();
        public static ToolboxItem GetToolBoxItem(SaveItemInfo info)
         {
            Image image;
            ToolboxItem item = null;
            item = new ToolboxItem();
            item.AssetType = info.assetType;
            item.ItemName = info.ItemName;
            item.ItemId = info.ItemId;
            switch (info.assetType)
            {            
                case AssetType.Movie:
                case AssetType.Sound:
                case AssetType.HTML5:
                case AssetType.Document:
                    image = new Image() { Source = new BitmapImage(new Uri(info.Thumbnails, UriKind.Absolute)) };
                    image.Stretch = Stretch.Fill;
                    item.Content = image;
                    break;
                case AssetType.Topic:
                    TopicControl tc = new TopicControl(true);
                    tc.TopicInfo = XamlReader.Load(XmlReader.Create((TextReader)new StringReader(((ContentText)info.Content).Text))) as TopicInfo;
                    item.Content = tc;
                    break;
                case AssetType.TopicDrag:
                    ContentTopicDrag contentTopicDrag = info.Content as ContentTopicDrag;
                    if (contentTopicDrag == null) break;

                    ControlTopicDrag controlTopicDarg = new ControlTopicDrag(contentTopicDrag.topicDragItemAnswerList, contentTopicDrag.topicDragItemList,
                        new SolidColorBrush((Color)ColorConverter.ConvertFromString(contentTopicDrag.Background)),
                        new SolidColorBrush((Color)ColorConverter.ConvertFromString(contentTopicDrag.Foreground)), true);
                    controlTopicDarg.Score = info.Score;
                    item.Content = controlTopicDarg;
                    break;
                case AssetType.Text:
                    ControlTextEditor content = new ControlTextEditor();
                    if (info.LineHeight != 0)
                    {
                        content.mainRTB.Document.LineHeight = info.LineHeight;
                    }

                    content.mainRTB.Document = XamlReader.Load(XmlReader.Create((TextReader)new StringReader(jg.HTMLConverter.HtmlToXamlConverter.ConvertHtmlToXaml(((ContentText)info.Content).Text, true)))) as FlowDocument;
                    //content.mainRTB.Document = XamlReader.Load(XmlReader.Create((TextReader)new StringReader(((ContentText)info.Content).Text))) as FlowDocument;
                    if (info.LineHeight != 0)
                    {
                        content.mainRTB.Document.LineHeight = info.LineHeight;
                    }
                    content.mainRTB.Document.FontSize = info.FontSize;
                    content.mainRTB.Document.FontFamily = new FontFamily(info.FontFamily);
                    item.LineHeight = info.LineHeight;
                    item.FontFamily = new FontFamily(info.FontFamily);
                    item.FontSize = info.FontSize;
                    item.Content = content;
                    break;
                case AssetType.TextGrid:
                    ControlTextGrid controlTextGrid = new ControlTextGrid();
                    controlTextGrid.Source = (ContentGrid)info.Content;
                    item.Content = controlTextGrid;
                    break;
                case AssetType.Shape:
                    System.Windows.Shapes.Path path = new System.Windows.Shapes.Path() { Name = ((ContentText)info.Content).Text };
                    path.Style = item.FindResource(((ContentText)info.Content).Text) as Style;
                    path.Stretch = Stretch.Fill;
                    item.Content = path;
                    break;
                case AssetType.Image:
                    image = new Image() { Source = new BitmapImage(new Uri(info.Thumbnails, UriKind.Absolute)) };
                    image.Stretch = Stretch.Fill;
                    item.Content = image;
                    break;
                case AssetType.Message:
                    ControlMessage controlMessage = new ControlMessage(true);
                    ContentMessage contentMessage = info.Content as ContentMessage;
                    if (contentMessage == null) break;
                    controlMessage.Title = contentMessage.Title;
                    controlMessage.Location = new Point(contentMessage.PointX, contentMessage.PointY);
                    item.Content = controlMessage;
                    break;
                case AssetType.Line:
                    ControlLine controlLine = new ControlLine(true);
                    ContentLine contentLine = info.Content as ContentLine;
                    if (contentLine == null) break;
                    controlLine.Point1 = new Point(contentLine.Point1X, contentLine.Point1Y);
                    item.Content = controlLine;
                    break;
                case AssetType.TPageGroup:
                    bool dirInfo = false;
                    if (AssResInfoObser.Keys.Count > 0)
                    {
                        foreach (var v in AssResInfoObser.Keys)
                        {
                            if (v == item.ItemId)
                            {
                                dirInfo = true;
                                AssResInfo resInfo = new AssResInfo();
                                resInfo.ArId = info.ItemId;
                                resInfo.AssetName = "图片" + (DirectoryTpageLoad[v].Children.Count + 1);
                                resInfo.AssetPath = info.AssetPath;
                                resInfo.Thumbnails = info.Thumbnails;
                                AssResInfoObser[v].Add(resInfo);
                                DirectoryTpageLoad[v].Children = AssResInfoObser[v];
                                DirectoryTpageLoad[v].UpdateImgGroup();
                                break;
                            }
                        }
                    }
                    if (!dirInfo)
                    {
                        TPageControl page = new TPageControl();
                        page.Height = info.Height;
                        page.Width = info.Width;
                        page.canvasPageContent.Width = info.Width - 100;
                        page.canvasPageContent.Height = info.Height - 2;
                        AssResInfo resInfo = new AssResInfo();
                        resInfo.ArId = info.ItemId;
                        resInfo.AssetName = "图片1";
                        resInfo.AssetPath = info.AssetPath;
                        resInfo.Thumbnails = info.Thumbnails;
                        System.Collections.ObjectModel.ObservableCollection<AssResInfo> observableAssResInfo = new System.Collections.ObjectModel.ObservableCollection<AssResInfo>();
                        observableAssResInfo.Add(resInfo);
                        page.Children = observableAssResInfo;
                        page.UpdateImgGroup();
                        AssResInfoObser.Add(item.ItemId, observableAssResInfo);
                        DirectoryTpageLoad.Add(item.ItemId, page);
                        item.Content = DirectoryTpageLoad[item.ItemId];

                    }
                    break;
            }
            item.IsLongText = info.IsLongText;
            item.IsShowDiv = info.IsShowDiv;
            item.IsDescPt = info.IsDescPt;

            item.Opacity = info.Opacity;
            if (info.Background != null)
                item.ItemBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(info.Background));
            if (info.Foreground != null)
                item.ItemForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(info.Foreground));

            item.AssetPath = info.AssetPath;
            item.Thumbnails = info.Thumbnails;

            return item;
        }


        public static Dictionary<string, System.Collections.ObjectModel.ObservableCollection<string>> DirectoryAssResInfo = new Dictionary<string, System.Collections.ObjectModel.ObservableCollection<string>>();
        public static Dictionary<string, TPageControl> DirectoryTpage = new Dictionary<string, TPageControl>();

        public static void SetShowDiv(SaveItemInfo info, ToolboxItem item, UIElement element)
        {
            if (info.IsShowDiv)
            {
                if (info.assetType == AssetType.TPageGroup)
                {
                    UIElement ule = (UIElement)CloneObject(element);
                    ShowMaxBox smb2 = new ShowMaxBox();
                    smb2.stringlist = DirectoryAssResInfo[item.ItemId.ToString()];
                    smb2.item = ule;
                    item.Content = smb2;
                }
                else
                {
                    ShowMaxBox smb = new ShowMaxBox();
                    smb.item = element;
                    item.Content = smb;
                }
            }
            else
            {
                item.Content = element;
            }
        }
        /// <summary>
        /// 克隆一个对象
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private static object CloneObject(object o)
        {
            Type t = o.GetType();
            PropertyInfo[] properties = t.GetProperties();
            Object p = t.InvokeMember("", System.Reflection.BindingFlags.CreateInstance, null, o, null);
            foreach (PropertyInfo pi in properties)
            {
                if (pi.CanWrite)
                {
                    object value = pi.GetValue(o, null);

                    pi.SetValue(p, value, null);

                }
            }
            return p;
        }
        /// <summary>
        /// 播放器调用方法
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>


        public static ToolboxItem GetToolBoxItemPreview(SaveItemInfo info)
        {
            TransformGroup transformGroup;
            WindowsFormsHost winformHost;


            string TempPath = "";
            Image image;
            ToolboxItem item = null;
            item = new ToolboxItem();
            item.AssetType = info.assetType;
            item.ItemId = info.ItemId;
            item.ItemName = info.ItemName;

            if (!string.IsNullOrEmpty(info.AssetPath))
            {
                TempPath = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Temp" + info.AssetPath.Substring(info.AssetPath.LastIndexOf("\\"));
                //MessageBox.Show("getitem,"+TempPath);
                try
                {

                    if (System.IO.File.Exists(TempPath))
                    {
                        System.IO.File.Delete(TempPath);
                    }

                    if (!info.IsDescPt || info.assetType == AssetType.HTML5)
                    {
                        FileSecurity.StreamToFileInfo(TempPath, info.AssetPath);
                        if (info.assetType == AssetType.HTML5)
                        {
                            string ItemdisFile = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Temp" + "\\" + info.ItemsDis.Substring(info.ItemsDis.LastIndexOf("\\"));
                            if (Directory.Exists(ItemdisFile))
                            {
                                DirectoryInfo di = new DirectoryInfo(ItemdisFile);
                                di.Delete(true);
                            }

                            Directory.CreateDirectory(ItemdisFile);
                            string TempFile = ItemdisFile + "\\";

                            string DecFile = info.ItemsDis.Substring(0, info.ItemsDis.LastIndexOf("\\") + 1);


                            GZip.Decompress(DecFile, TempFile, info.ItemsDis.Substring(info.ItemsDis.LastIndexOf("\\") + 1) + ".zip");

                        }
                    }

                    else
                    {
                        FileSecurity.decryptFile(Globals.AssetDecryptKey, TempPath, info.AssetPath);
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                    //log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                    //log.Error(ex.Message + "\r\n" + ex.StackTrace);
                }


                Globals.tempFileList.Add(TempPath);
                //info.AssetPath = TempPath;
            }
            switch (info.assetType)
            {
                case AssetType.Movie:

                    Control.ControlMediaElement mediaElement = new Control.ControlMediaElement();
                    mediaElement.Source = new Uri(TempPath, UriKind.Absolute);
                    mediaElement.mediaElement.LoadedBehavior = MediaState.Manual;
                    mediaElement.mediaElement.UnloadedBehavior = MediaState.Stop;
                    
                    //mediaElement.Stop();
                    item.Content = mediaElement;

                    break;
             
                case AssetType.Sound:
                    MediaPlayer mediaPlayer = new MediaPlayer();
                    mediaPlayer.Open(new Uri(TempPath, UriKind.Absolute));
                    mediaPlayer.Stop();
                    item.Content = mediaPlayer;
                    item.Visibility = Visibility.Hidden;
                    break;
                case AssetType.Topic:
                    System.Collections.Generic.List<int> answerList = new System.Collections.Generic.List<int>();

                    TopicInfo topicInfo = XamlReader.Load(XmlReader.Create((TextReader)new StringReader(((ContentText)info.Content).Text))) as TopicInfo;
                    foreach (var v in topicInfo.TopicOptionList)//记录正确答案。
                        if (v.Right) answerList.Add(v.Id);
                    TopicControl tc = new TopicControl(false, answerList);
                    tc.TopicInfo = topicInfo;
                    tc.Clear();
                    //tc.TopicInfo.TopicOptionList[0].Right = true;
                    //foreach (var v in tc.TopicInfo.TopicOptionList)
                    //    v.IsSelected = false;

                    SetShowDiv(info, item, tc);
                    break;
                case AssetType.TopicDrag:
                    ContentTopicDrag contentTopicDrag = info.Content as ContentTopicDrag;
                    if (contentTopicDrag == null) break;
                    ControlTopicDrag controlTopicDarg = new ControlTopicDrag(contentTopicDrag.topicDragItemAnswerList, contentTopicDrag.topicDragItemList,
                        new SolidColorBrush((Color)ColorConverter.ConvertFromString(contentTopicDrag.Background)),
                        new SolidColorBrush((Color)ColorConverter.ConvertFromString(contentTopicDrag.Foreground)), false);
                    controlTopicDarg.Score = info.Score;

                    SetShowDiv(info, item, controlTopicDarg);
                    break;
                case AssetType.Text:
                    System.Windows.Controls.RichTextBox richtextbox = new System.Windows.Controls.RichTextBox();
                    richtextbox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                    richtextbox.BorderThickness = new Thickness(0);
                    richtextbox.IsReadOnly = true;

                    richtextbox.Document = XamlReader.Load(XmlReader.Create((TextReader)new StringReader(jg.HTMLConverter.HtmlToXamlConverter.ConvertHtmlToXaml(((ContentText)info.Content).Text, true)))) as FlowDocument;
                    richtextbox.Background = richtextbox.Document.Background;
                    if (info.LineHeight != 0)
                    {
                        richtextbox.Document.LineHeight = info.LineHeight;
                    }
                    richtextbox.Document.FontSize = info.FontSize;
                    richtextbox.Document.FontFamily = new FontFamily(info.FontFamily);
                    item.LineHeight = info.LineHeight;

                    SetShowDiv(info, item, richtextbox);
                    break;
                case AssetType.TextGrid:
                    ControlTextGrid controlTextGrid = new ControlTextGrid();
                    controlTextGrid.IsEdit = false;
                    controlTextGrid.Source = (ContentGrid)info.Content;

                    SetShowDiv(info, item, controlTextGrid);
                    item.ItemBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(info.Background));
                    item.ItemForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(info.Foreground));

                    break;
                case AssetType.Shape:
                    System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
                    path.Style = path.FindResource(((ContentText)info.Content).Text) as Style;
                    path.Stretch = Stretch.Fill;

                    SetShowDiv(info, item, path);
                    break;
                case AssetType.Image:

                    using (FileStream fs = new FileStream(TempPath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, (int)fs.Length);
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = new MemoryStream(buffer);
                        bitmapImage.EndInit();
                        image = new Image() { Source = bitmapImage };
                        image.Stretch = Stretch.Fill;

                        SetShowDiv(info, item, image);

                    }
                    break;
                case AssetType.Document:
                    WebBrowser webBrowser = new WebBrowser();
                    webBrowser.Source = new Uri(TempPath, UriKind.Absolute);
                    SetShowDiv(info, item, webBrowser);
                    break;
                case AssetType.Message:
                    ControlMessage controlMessage = new ControlMessage(false);
                    ContentMessage contentMessage = info.Content as ContentMessage;
                    if (contentMessage == null) break;
                    controlMessage.Title = contentMessage.Title;
                    controlMessage.Location = new Point(contentMessage.PointX, contentMessage.PointY);
                    SetShowDiv(info, item, controlMessage);
                    break;
                case AssetType.Line:
                    ControlLine controlLine = new ControlLine(false);
                    ContentLine contentLine = info.Content as ContentLine;
                    if (contentLine == null) break;
                    controlLine.Point1 = new Point(contentLine.Point1X, contentLine.Point1Y);
                    SetShowDiv(info, item, controlLine);
                    break;
                case AssetType.HTML5:
                    WebBrowerGecko wbgk = new WebBrowerGecko();
                    System.Windows.Forms.Integration.WindowsFormsHost w = new System.Windows.Forms.Integration.WindowsFormsHost();
                    w.Child = wbgk;
                    wbgk.Navigate(TempPath);
                    w.SetValue(Panel.ZIndexProperty, 10);
                    w.Width = info.Width;
                    w.Height = info.Height;
                    w.Background = new SolidColorBrush(Colors.Red);
                    w.Tag = TempPath;
                    item.Content = w;

                    break;
                case AssetType.TPageGroup:
                    bool dirInfo = false;
                    if (DirectoryAssResInfo.Keys.Count > 0)
                    {
                        foreach (var v in DirectoryAssResInfo.Keys)
                        {
                            if (v == item.ItemId.ToString())
                            {
                                dirInfo = true;
                                DirectoryAssResInfo[v].Add(TempPath);
                                UpdateImgGroup(DirectoryAssResInfo[v], DirectoryTpage[v]);
                                break;
                            }
                        }
                    }
                    if (!dirInfo)
                    {
                        TPageControl page = new TPageControl();
                        page.Height = info.Height;
                        page.Width = info.Width;
                        page.canvasPageContent.Width = info.Width - 100;
                        page.canvasPageContent.Height = info.Height - 2;
                        System.Collections.ObjectModel.ObservableCollection<string> observableAssResInfo = new System.Collections.ObjectModel.ObservableCollection<string>();
                        observableAssResInfo.Add(TempPath);
                        UpdateImgGroup(observableAssResInfo, page);
                        DirectoryAssResInfo.Add(item.ItemId.ToString(), observableAssResInfo);
                        DirectoryTpage.Add(item.ItemId.ToString(), page);
                        item.Content = DirectoryTpage[item.ItemId.ToString()];

                    }
                    TPageControl tpage = new TPageControl();
                    tpage = DirectoryTpage[item.ItemId.ToString()];
                    SetShowDiv(info, item, tpage);
                    break;
            }



            //item.Opacity = info.Opacity;
            item.AssetPath = info.AssetPath;
            item.Thumbnails = info.Thumbnails;

            if (info.Background != null)
                item.ItemBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(info.Background));
            if (info.Foreground != null)
                item.ItemForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(info.Foreground));

            item.IsEdit = false;
            item.ItemId = info.ItemId;
            item.Width = info.Width;
            item.Height = info.Height;

            Canvas.SetLeft(item, info.X);
            Canvas.SetTop(item, info.Y);

            System.Windows.Controls.Panel.SetZIndex(item, info.ZIndex);

            transformGroup = item.RenderTransform as TransformGroup;
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

            return item;
        }

        static void geckowebBrowser_HandleCreated(object sender, EventArgs e)
        {
            Skybound.Gecko.GeckoWebBrowser geckowebBrowser = sender as Skybound.Gecko.GeckoWebBrowser;
            geckowebBrowser.Navigate(geckowebBrowser.Tag.ToString());
        }

        /// <summary>
        /// 更新图片组
        /// </summary>

        private static void UpdateImgGroup(System.Collections.ObjectModel.ObservableCollection<string> observable, TPageControl PageControl)
        {
            System.Collections.ObjectModel.ObservableCollection<Panel> Panels = new System.Collections.ObjectModel.ObservableCollection<Panel>();

            for (int i = 0; i < observable.Count; i++)
            {
                WrapPanel rectangle = new WrapPanel();
                rectangle.Width = PageControl.canvasPageContent.Width;
                rectangle.Height = PageControl.canvasPageContent.Height;
                BitmapImage bitimage = new BitmapImage();
                bitimage.BeginInit();
                bitimage.UriSource = new Uri(observable[i], UriKind.Absolute);
                bitimage.EndInit();

                ImageBrush brush = new ImageBrush();
                brush.ImageSource = bitimage;
                rectangle.Background = brush;
                Panels.Add(rectangle);
            }
            PageControl.AddPage(Panels, 1);
        }
        /// <summary>
        /// 播放器调用方法
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static ToolboxItem GetGridChildrenPreview(SaveItemInfo info)
        {
            WindowsFormsHost winformHost;

            string TempPath = "";
            Image image;
            ToolboxItem item = null;
            item = new ToolboxItem();
            item.AssetType = info.assetType;
            item.ItemId = info.ItemId;
            item.ItemName = info.ItemName;

            if (!string.IsNullOrEmpty(info.AssetPath))
            {
                TempPath = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Temp" + info.AssetPath.Substring(info.AssetPath.LastIndexOf("\\"));
                //MessageBox.Show("getitem,"+TempPath);
                try
                {


                    if (System.IO.File.Exists(TempPath))
                    {
                        System.IO.File.Delete(TempPath);
                    }

                    if (!info.IsDescPt || info.assetType == AssetType.HTML5)
                    {
                        FileSecurity.StreamToFileInfo(TempPath, info.AssetPath);
                        if (info.assetType == AssetType.HTML5)
                        {
                            string ItemdisFile = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Temp" + "\\" + info.ItemsDis.Substring(info.ItemsDis.LastIndexOf("\\"));
                            if (Directory.Exists(ItemdisFile))
                            {
                                DirectoryInfo di = new DirectoryInfo(ItemdisFile);
                                di.Delete(true);
                            }

                            Directory.CreateDirectory(ItemdisFile);
                            string TempFile = ItemdisFile + "\\";

                            string DecFile = info.ItemsDis.Substring(0, info.ItemsDis.LastIndexOf("\\") + 1);


                            GZip.Decompress(DecFile, TempFile, info.ItemsDis.Substring(info.ItemsDis.LastIndexOf("\\") + 1) + ".zip");

                        }
                    }

                    else
                    {
                        FileSecurity.decryptFile(Globals.AssetDecryptKey, TempPath, info.AssetPath);
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                    //log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                    //log.Error(ex.Message + "\r\n" + ex.StackTrace);
                }

            }

            switch (info.assetType)
            {
                case AssetType.Movie:
                    Control.ControlMediaElement mediaElement = new Control.ControlMediaElement();
                    mediaElement.Source = new Uri(TempPath, UriKind.Absolute);
                    mediaElement.mediaElement.LoadedBehavior = MediaState.Manual;
                    mediaElement.mediaElement.UnloadedBehavior = MediaState.Stop;
                    //mediaElement.Stop();
                    item.Content = mediaElement;
                    break;              
                case AssetType.Sound:
                    MediaPlayer mediaPlayer = new MediaPlayer();
                    mediaPlayer.Open(new Uri(TempPath, UriKind.Absolute));
                    mediaPlayer.Stop();
                    item.Content = mediaPlayer;
                    item.Visibility = Visibility.Hidden;
                    break;
                case AssetType.Topic:
                    System.Collections.Generic.List<int> answerList = new System.Collections.Generic.List<int>();

                    TopicInfo topicInfo = XamlReader.Load(XmlReader.Create((TextReader)new StringReader(((ContentText)info.Content).Text))) as TopicInfo;
                    foreach (var v in topicInfo.TopicOptionList)//记录正确答案。
                        if (v.Right) answerList.Add(v.Id);
                    TopicControl tc = new TopicControl(false, answerList);
                    tc.TopicInfo = topicInfo;
                    tc.Clear();
                    //tc.TopicInfo.TopicOptionList[0].Right = true;
                    //foreach (var v in tc.TopicInfo.TopicOptionList)
                    //    v.IsSelected = false;
                    SetShowDiv(info, item, tc);
                    break;
                case AssetType.TopicDrag:
                    ContentTopicDrag contentTopicDrag = info.Content as ContentTopicDrag;
                    if (contentTopicDrag == null) break;
                    ControlTopicDrag controlTopicDarg = new ControlTopicDrag(contentTopicDrag.topicDragItemAnswerList, contentTopicDrag.topicDragItemList,
                        new SolidColorBrush((Color)ColorConverter.ConvertFromString(contentTopicDrag.Background)),
                        new SolidColorBrush((Color)ColorConverter.ConvertFromString(contentTopicDrag.Foreground)), false);
                    controlTopicDarg.Score = info.Score;

                    SetShowDiv(info, item, controlTopicDarg);
                    break;
                case AssetType.Text:
                    System.Windows.Controls.RichTextBox richtextbox = new System.Windows.Controls.RichTextBox();
                    richtextbox.BorderThickness = new Thickness(0);
                    richtextbox.IsReadOnly = true;
                    if (info.LineHeight != 0)
                    {
                        richtextbox.Document.LineHeight = info.LineHeight;
                    }
                    richtextbox.Document = XamlReader.Load(XmlReader.Create((TextReader)new StringReader(((ContentText)info.Content).Text))) as FlowDocument;
                    richtextbox.Background = richtextbox.Document.Background;
                    if (info.LineHeight != 0)
                    {
                        richtextbox.Document.LineHeight = info.LineHeight;
                    }
                    richtextbox.Document.FontSize = info.FontSize;
                    richtextbox.Document.FontFamily = new FontFamily(info.FontFamily);
                    item.LineHeight = info.LineHeight;
                    SetShowDiv(info, item, richtextbox);
                    break;
                case AssetType.TextGrid:
                    ControlTextGrid controlTextGrid = new ControlTextGrid();
                    controlTextGrid.IsEdit = false;
                    controlTextGrid.Source = (ContentGrid)info.Content;
                    SetShowDiv(info, item, controlTextGrid);
                    break;
                case AssetType.Shape:
                    System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
                    path.Style = path.FindResource(((ContentText)info.Content).Text) as Style;
                    SetShowDiv(info, item, path);
                    break;
                case AssetType.Image:
                    using (FileStream fs = new FileStream(TempPath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, (int)fs.Length);
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = new MemoryStream(buffer);
                        bitmapImage.EndInit();

                        image = new Image() { Source = bitmapImage };
                        image.Stretch = Stretch.Fill;
                        SetShowDiv(info, item, image);
                    }

                    break;
                case AssetType.Document:
                    WebBrowser webBrowser = new WebBrowser();
                    webBrowser.Source = new Uri(TempPath, UriKind.Absolute);
                    SetShowDiv(info, item, webBrowser);
                    break;
                case AssetType.Message:
                    ControlMessage controlMessage = new ControlMessage(false);
                    ContentMessage contentMessage = info.Content as ContentMessage;
                    if (contentMessage == null) break;
                    controlMessage.Title = contentMessage.Title;
                    controlMessage.Location = new Point(contentMessage.PointX, contentMessage.PointY);
                    SetShowDiv(info, item, controlMessage);
                    break;
                case AssetType.Line:
                    ControlLine controlLine = new ControlLine(false);
                    ContentLine contentLine = info.Content as ContentLine;
                    if (contentLine == null) break;
                    controlLine.Point1 = new Point(contentLine.Point1X, contentLine.Point1Y);
                    SetShowDiv(info, item, controlLine);
                    break;
                case AssetType.HTML5:
                    WebBrowerGecko wbgk = new WebBrowerGecko();
                    System.Windows.Forms.Integration.WindowsFormsHost w = new System.Windows.Forms.Integration.WindowsFormsHost();
                    w.Child = wbgk;
                    wbgk.Navigate(TempPath);
                    w.SetValue(Panel.ZIndexProperty, 10);
                    w.Width = info.Width;
                    w.Height = info.Height;
                    w.Background = new SolidColorBrush(Colors.Red);
                    w.Tag = TempPath;
                    item.Content = w;
                    break;
                case AssetType.TPageGroup:

                    bool dirInfo = false;
                    if (DirectoryAssResInfo.Keys.Count > 0)
                    {
                        foreach (var v in DirectoryAssResInfo.Keys)
                        {
                            if (v == item.ItemId.ToString())
                            {
                                dirInfo = true;
                                DirectoryAssResInfo[v].Add(TempPath);
                                UpdateImgGroup(DirectoryAssResInfo[v], DirectoryTpage[v]);
                                break;
                            }
                        }
                    }

                    if (!dirInfo)
                    {
                        TPageControl page = new TPageControl();
                        page.Height = info.Height;
                        page.Width = info.Width;
                        page.canvasPageContent.Width = info.Width - 100;
                        page.canvasPageContent.Height = info.Height - 2;
                        System.Collections.ObjectModel.ObservableCollection<string> observableAssResInfo = new System.Collections.ObjectModel.ObservableCollection<string>();
                        observableAssResInfo.Add(TempPath);
                        UpdateImgGroup(observableAssResInfo, page);
                        DirectoryAssResInfo.Add(item.ItemId.ToString(), observableAssResInfo);
                        DirectoryTpage.Add(item.ItemId.ToString(), page);
                    }



                    TPageControl tpage = new TPageControl();
                    tpage = DirectoryTpage[item.ItemId.ToString()];
                    SetShowDiv(info, item, tpage);
                    break;

            }

            item.IsEdit = false;
            item.AssetPath = info.AssetPath;
            item.Thumbnails = info.Thumbnails;
            return item;
        }



        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #region INotifyPropertyChanged Members
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion



    }
}