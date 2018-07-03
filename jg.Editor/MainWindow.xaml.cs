
namespace jg.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Collections.ObjectModel;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;

    using System.Xml.Serialization;
    using System.Windows.Markup;
    using System.Xml;
    using System.IO;
    using System.Diagnostics;
    using System.Reflection;
    using System.Windows.Media.Animation;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.ComponentModel;
    using jg.Editor.Library;
    using jg.Editor.Library.Topic;
    using jg.Editor.Library.Property;
    using System.Windows.Threading;
    using System.Net;
    using System.Net.Sockets;
    using jg.Security.Library;
    using System.Threading;

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        private List<DesignerCanvas> designerCanvasList = new List<DesignerCanvas>();
        public delegate void OnPackFileProcess(double value);
        public delegate void ActionGetResource(string ToolName);

        public OnPackFileProcess PackFileProcess;
        private List<string> DataFileList = new List<string>();
        private List<string> AssetFileList = new List<string>();
        double canvas_height = 0, canvas_width = 0;
        DataObject CurrentSelDataObject = null;

        public MainWindow()
        {
            InitializeComponent();

            this.SizeChanged += MainWindow_SizeChanged;
            PropertyManage.PropertyChanged += new PropertyManage.OnPropertyChanged(PropertyManage_PropertyChanged);
            controlTimeLine.SelectedFrameChanged += new ControlTimeLine.OnSelectedFrameChanged(controlTimeLine_SelectedFrameChanged);
            controlTimeLine.TimeChanging += new ControlTimeLine.OnTimeChanging(controlTimeLine_TimeChanging);

            DesignerCanvas.SelectedChanged += new DesignerCanvas.OnSelectedChanged(DesignerCanvas_SelectedChanged);
            DesignerCanvas.DesignerCanvasMouseOver += new DesignerCanvas.OnMouseOver(DesignerCanvas_DesignerCanvasMouseOver);
            //ToolboxItem.AddAsset += new ToolboxItem.OnAddAsset(ToolboxItem_AddAsset);


            setSliderAndSrollEvent();

        }
        /// <summary>
        /// 鼠标在画布的位置
        /// </summary>
        private Point MousePoint;
        void DesignerCanvas_DesignerCanvasMouseOver(MouseEventArgs e)
        {
            MousePoint = Mouse.GetPosition(e.Source as FrameworkElement);
        }

        void DesignerCanvas_SelectedChanged(object sender, Guid ItemId, bool IsSelected)
        {
            ObservableCollection<SaveItemInfo> ObservablecollectionDesignerItem = new ObservableCollection<SaveItemInfo>();
            try
            {

                IEnumerable<DesignerItem> designerItemIEnumerable;
                DesignerItem designerItem;
                if (IsSelected)
                {
                    if (sender is DesignerItem)
                    {
                        TreeViewItemInfo item = treeViewControl1.SelectedItem as TreeViewItemInfo;
                        if (item != null)
                        {
                            foreach (var v in designerCanvasList)
                            {


                                if (v.PageId == item.Id)
                                {

                                    designerItemIEnumerable = v.Children.OfType<DesignerItem>().Where(model => model.IsSelected);
                                    if (designerItemIEnumerable == null && designerItemIEnumerable.Count() == 0) continue;
                                    foreach (var o in designerItemIEnumerable)
                                    {
                                        ObservablecollectionDesignerItem.Add(GetSaveItemInfo(o));
                                    }
                                    break;
                                }
                            }
                        }
                        CurrentSelDataObject = new DataObject("JG_EDITOR_DESIGNERITEM", ObservablecollectionDesignerItem);
                    }
                    else if (sender is DesignerCanvas)
                    {
                        SelectedTree(Globals.treeviewSource, ((DesignerCanvas)sender).PageId);
                        ((DesignerCanvas)sender).DeselectAll();


                        foreach (UIElement v in ((DesignerCanvas)sender).Children)
                        {
                            int aaaaaaa = Panel.GetZIndex(v);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }
        }
        void SelectedTree(ObservableCollection<TreeViewItemInfo> treeViewItemList, Guid id)
        {
            foreach (var v in treeViewItemList)
            {
                if (v.Id == id)
                {
                    v.IsSelected = true;
                    return;
                }
                SelectedTree(v.Children, id);
            }
        }
        /// <summary>
        /// 跳转页面
        /// </summary>
        /// <param name="ToolBoxItem"></param>
        //void ToolboxItem_AddAsset(string ToolBoxItem)
        //{
        //    FileInfo AssetInfo = null;
        //    FileInfo ThumbnailsInfo = null;
        //    System.IO.DirectoryInfo itemsDis = null;
        //    WindowAssetSel windowAssetSel = null;
        //    HTML5Class model = null;
        //    if (propertyManage1.Stage == null) return;

        //    try
        //    {
        //        //根据素材类型，显示所选择的素材明细
        //        switch (ToolBoxItem)
        //        {
        //            case "Sound":
        //                windowAssetSel = new WindowAssetSel(new string[] { "mp3" });
        //                break;
        //            case "Movie":
        //                windowAssetSel = new WindowAssetSel(new string[] { "mp4", "wmv", "avi" });
        //                break;
        //            case "Animation":
        //                windowAssetSel = new WindowAssetSel(new string[] { "swf" });
        //                break;
        //            case "Image":
        //                windowAssetSel = new WindowAssetSel(new string[] { "jpg", "png", "bmp" });
        //                break;
        //            case "Model3D":
        //                windowAssetSel = new WindowAssetSel(new string[] { "unity3d" });
        //                break;
        //            case "Document":
        //                windowAssetSel = new WindowAssetSel(new string[] { "pdf", "doc", "docx", "xls", "xlsx", "ppt", "pptx" });
        //                break;
        //            case "HTML5":
        //                model = new HTML5Class();
        //                ImpHtml imp = new ImpHtml(model);
        //                imp.eventsetSteageBrower += imp_eventsetSteageBrower;
        //                if (imp.ShowDialog() == true)
        //                {
        //                    AssetInfo = new FileInfo(model.ActionHtmlfile);
        //                    ThumbnailsInfo = new FileInfo(model.ImgFileName);
        //                    itemsDis = new DirectoryInfo(model.ActionHtmlDis);
        //                    if (string.IsNullOrEmpty(AssetInfo.FullName) || string.IsNullOrEmpty(ThumbnailsInfo.FullName))
        //                        return;
        //                }
        //                break;
        //            case "TPageGroup":
        //                //model = new HTML5Class();
        //                //ImpHtml imp = new ImpHtml(model);
        //                //imp.eventsetSteageBrower += imp_eventsetSteageBrower;
        //                //if (imp.ShowDialog() == true)
        //                //{
        //                //    AssetInfo = new FileInfo(model.ActionHtmlfile);
        //                //    ThumbnailsInfo = new FileInfo(model.ImgFileName);
        //                //    itemsDis = new DirectoryInfo(model.ActionHtmlDis);
        //                //    if (string.IsNullOrEmpty(AssetInfo.FullName) || string.IsNullOrEmpty(ThumbnailsInfo.FullName))
        //                //        return;
        //                //}
        //                break;
        //        }

        //        if (windowAssetSel != null)
        //        {
        //            if (windowAssetSel.ShowDialog() == true)
        //            {
        //                AssetInfo = new FileInfo(windowAssetSel.Path);
        //                ThumbnailsInfo = new FileInfo(windowAssetSel.Thumbnails);
        //                if (string.IsNullOrEmpty(AssetInfo.FullName) || string.IsNullOrEmpty(ThumbnailsInfo.FullName))
        //                    return;
        //            }
        //            else
        //                return;
        //        }

        //        propertyManage1.Stage.AddItem(ToolBoxItem, AssetInfo == null ? "" : AssetInfo.FullName, ThumbnailsInfo == null ? "" : ThumbnailsInfo.FullName, itemsDis == null ? "" : itemsDis.FullName);
        //    }
        //    catch (Exception ex)
        //    {
        //        log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //        log.Error(ex.Message + "\r\n" + ex.StackTrace);

        //    }
        //}
        void controlTimeLine_TimeChanging(double Time, AnimationProperty animationPropertyList)
        {
            TransformGroup transformGroup;
            RotateTransform rotateTransform;
            ScaleTransform scaleTransform;
            SkewTransform skewTransform;
            TranslateTransform translateTransform;
            try
            {
                var v = animationPropertyList;

                if (v == null) return;
                if (v.tp1 == null || v.tp2 == null) return;

                foreach (var vv in v.tp1.propertyList)
                {
                    switch (vv.propertyEnum)
                    {
                        case PropertyEnum.ColorProperty:
                            break;
                        case PropertyEnum.DoubleProperty:
                            double v1, v2;
                            DesignerItem item = null;

                            var designerCanvas = (from model in designerCanvasList where model.PageId == controlTimeLine.PageId select model).First();
                            item = (from model in designerCanvas.Children.OfType<DesignerItem>() where model.ItemId == v.ItemId select model).First();

                            if (item == null) break;

                            AssetDoubleProperty dp1, dp2;
                            dp1 = vv as AssetDoubleProperty;
                            dp2 = v.tp2.propertyList.Find(model => model.Name == dp1.Name) as AssetDoubleProperty;
                            if (dp1 == null || dp2 == null) break;
                            v1 = dp1.Value;
                            v2 = dp2.Value;
                            if (v1 == v2) break;
                            DoubleAnimation doubleAnimation = new DoubleAnimation(v1, v2, v.timeSpan);

                            switch (vv.Name)
                            {
                                case "PropertyFontSizeCommand":// 字号动画
                                    Animation(item, DesignerItem.FontSizeProperty, doubleAnimation);
                                    SetDiect(item, DesignerItem.FontSizeProperty);
                                    break;
                                case "PropertyXCommand": // X轴动画
                                    Animation(item, Canvas.LeftProperty, doubleAnimation);
                                    SetDiect(item, Canvas.LeftProperty);
                                    break;
                                case "PropertyYCommand":// Y轴动画
                                    Animation(item, Canvas.TopProperty, doubleAnimation);
                                    SetDiect(item, Canvas.TopProperty);
                                    break;
                                case "PropertyWidthCommand":// 宽度动画
                                    Animation(item, DesignerItem.WidthProperty, doubleAnimation);
                                    SetDiect(item, DesignerItem.WidthProperty);
                                    break;
                                case "PropertyHeightCommand":// 高度动画
                                    Animation(item, DesignerItem.HeightProperty, doubleAnimation);
                                    SetDiect(item, DesignerItem.HeightProperty);
                                    break;
                                case "PropertyOpacityCommand": // 透明度
                                    Animation(item, Canvas.OpacityProperty, doubleAnimation);
                                    SetDiect(item, Canvas.OpacityProperty);
                                    break;
                                case "PropertyRotateCommand":// 旋转动画
                                    transformGroup = ((ToolboxItem)item.Content).RenderTransform as TransformGroup;
                                    if (null == transformGroup) return;
                                    rotateTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "RotateTransform") as RotateTransform;
                                    if (rotateTransform != null)
                                        Animation(rotateTransform, RotateTransform.AngleProperty, doubleAnimation);
                                    SetDiect(item, RotateTransform.AngleProperty);
                                    break;
                                case "PropertyScaleXCommand":// X轴缩放
                                    transformGroup = ((ToolboxItem)item.Content).RenderTransform as TransformGroup;
                                    if (null == transformGroup) return;
                                    scaleTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "ScaleTransform") as ScaleTransform;
                                    if (scaleTransform != null)
                                        Animation(scaleTransform, ScaleTransform.ScaleXProperty, doubleAnimation);
                                    SetDiect(item, ScaleTransform.ScaleXProperty);
                                    break;
                                case "PropertyScaleYCommand":// Y轴缩放
                                    transformGroup = ((ToolboxItem)item.Content).RenderTransform as TransformGroup;
                                    if (null == transformGroup) return;
                                    scaleTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "ScaleTransform") as ScaleTransform;
                                    if (scaleTransform != null)
                                        Animation(scaleTransform, ScaleTransform.ScaleYProperty, doubleAnimation);
                                    SetDiect(item, ScaleTransform.ScaleYProperty);
                                    break;
                                case "PropertySkewXCommand":// X轴2D变幻
                                    transformGroup = ((ToolboxItem)item.Content).RenderTransform as TransformGroup;
                                    if (null == transformGroup) return;
                                    skewTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "SkewTransform") as SkewTransform;
                                    if (skewTransform != null)
                                        Animation(skewTransform, SkewTransform.AngleXProperty, doubleAnimation);
                                    SetDiect(item, SkewTransform.AngleXProperty);
                                    break;
                                case "PropertySkewYCommand":// Y轴2D变幻
                                    transformGroup = ((ToolboxItem)item.Content).RenderTransform as TransformGroup;
                                    if (null == transformGroup) return;
                                    skewTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "SkewTransform") as SkewTransform;
                                    if (skewTransform != null)
                                        Animation(skewTransform, SkewTransform.AngleYProperty, doubleAnimation);
                                    SetDiect(item, SkewTransform.AngleYProperty);
                                    break;
                                case "PropertyTranslateXCommand":// X轴平移
                                    transformGroup = ((ToolboxItem)item.Content).RenderTransform as TransformGroup;
                                    if (null == transformGroup) return;
                                    translateTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "TranslateTransform") as TranslateTransform;
                                    if (translateTransform != null)
                                        Animation(translateTransform, TranslateTransform.XProperty, doubleAnimation);
                                    SetDiect(item, TranslateTransform.XProperty);
                                    break;
                                case "PropertyTranslateYCommand":// Y轴平移
                                    transformGroup = ((ToolboxItem)item.Content).RenderTransform as TransformGroup;
                                    if (null == transformGroup) return;
                                    translateTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "TranslateTransform") as TranslateTransform;
                                    if (translateTransform != null)
                                        Animation(translateTransform, TranslateTransform.YProperty, doubleAnimation);
                                    SetDiect(item, TranslateTransform.YProperty);
                                    break;
                            }
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
            }
        }

        private void SetDiect(UIElement element, DependencyProperty property)
        {

            if (controlTimeLine.delDictUi.Keys.Contains(element))
            {
                controlTimeLine.delDictUi[element].Add(property);
            }
            else
            {

                List<DependencyProperty> p = new List<DependencyProperty>();
                p.Add(property);
                controlTimeLine.delDictUi.Add(element, p);

            }
        }
        void Animation(UIElement element, DependencyProperty property, DoubleAnimation doubleAnimation)
        {
            doubleAnimation.FillBehavior = FillBehavior.HoldEnd;
            //doubleAnimation.Completed += (sender, e) =>
            //{
            //    element.BeginAnimation(property, null);
            //};
            element.BeginAnimation(property, doubleAnimation, HandoffBehavior.SnapshotAndReplace);
        }
        void Animation(Transform transform, DependencyProperty property, DoubleAnimation doubleAnimation)
        {
            doubleAnimation.FillBehavior = FillBehavior.HoldEnd;
            //doubleAnimation.Completed += (sender, e) =>
            //{
            //    transform.BeginAnimation(property, null);
            //};
            transform.BeginAnimation(property, doubleAnimation, HandoffBehavior.SnapshotAndReplace);
        }
        void PropertyManage_PropertyChanged(jg.Editor.Library.ICommand command)
        {
            Type type = command.GetType();
            PropertyInfo info = type.GetProperty("NewProperty");
            if (info == null) return;
            object value = info.GetValue(command, null);
            double doubleValue;
            AssetDoubleProperty doubleProperty;
            if (double.TryParse(value.ToString(), out doubleValue))
            {
                doubleProperty = new AssetDoubleProperty();
                doubleProperty.Name = type.Name;
                doubleProperty.Value = doubleValue;
                controlTimeLine.AddProperty(doubleProperty);
            }
        }
        void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RefreshStage();
        }
        void RefreshStage()
        {
            double maxHeight = 0, maxWidth = scrollViewer.ActualWidth - 20;

            foreach (var v in designerCanvasList)
            {
                maxWidth = Math.Max(maxWidth, v.ActualWidth);
                maxHeight = Math.Max(maxHeight, v.ActualHeight);
            }
            grid.Width = maxWidth * 1.5;

            foreach (var v in grid.RowDefinitions)
                v.Height = new GridLength(maxHeight * 1.5);

        }
        void Source_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            TreeViewItemInfo info = null;

            if (e.NewItems != null && e.NewItems.Count > 0)
                info = e.NewItems[0] as TreeViewItemInfo;
            if (e.OldItems != null && e.OldItems.Count > 0)
                info = e.OldItems[0] as TreeViewItemInfo;

            if (info == null) return;

            int Index = 0;
            try
            {
                CreatePage(Globals.treeviewSource, ref Index);
            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }
        }
        void treeViewControl1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItemInfo info = e.NewValue as TreeViewItemInfo;


            if (info == null) return;
            //树节点选择变更时，记录当前选中的节点，做复制操作时使用。
            List<Guid> idList = GetDesignerCanvasList(info);
            Clipboard_TreeViewInfo clipboard = new Clipboard_TreeViewInfo();
            clipboard.TreeViewItemInfo = GetCloneItem(info);
            foreach (var v in idList)
            {
                clipboard.SavePageList.Add(GetSavePageInfo(designerCanvasList.Find(model => model.PageId == v)));
            }
            CurrentSelDataObject = new DataObject("JG_EDITOR_TREEVIEWITEM", clipboard);


            //定位选中舞台
            DesignerCanvas canvas;

            canvas = designerCanvasList.FirstOrDefault(model => model.PageId == info.Id);

            if (canvas != null)
            {
                int row = Grid.GetRow(canvas);
                double height = 0;
                for (int i = 0; i < row; i++)
                {
                    height += grid.RowDefinitions[i].ActualHeight;
                }
                scrollViewer.ScrollToVerticalOffset(height);

                if (propertyManage1 != null) propertyManage1.Stage = canvas;
                //canvas.SelectedCanvas();
            }

        }
        //复制树节点到剪切板
        TreeViewItemInfo GetCloneItem(TreeViewItemInfo info)
        {
            TreeViewItemInfo treeViewItemInfo = new TreeViewItemInfo() { Id = Guid.NewGuid(), ParentId = info.ParentId, IsEdit = info.IsEdit, Title = info.Title };
            try
            {
                foreach (var v in info.Children)
                    treeViewItemInfo.Children.Add(GetCloneItem(v));
            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }
            return treeViewItemInfo;
        }
        List<Guid> GetDesignerCanvasList(TreeViewItemInfo info)
        {
            List<Guid> idList = new List<Guid>();
            idList.Add(info.Id);
            foreach (var v in info.Children)
                idList.AddRange(GetDesignerCanvasList(v));
            return idList;
        }
        void CreatePage(ObservableCollection<TreeViewItemInfo> list, ref int Index)
        {
            RowDefinition row;
            DesignerCanvas canvas;
            SavePageInfo savePageInfo;
            TextBlock tb = null;

            double maxHeight = 0, maxWidth = 0;

            foreach (var v in Globals.savePageList)
            {
                maxHeight = Math.Max(v.Height, maxHeight);
                maxWidth = Math.Max(v.Width, maxWidth);
            }

            maxHeight = Math.Max(maxHeight * 1.1, scrollViewer.ActualHeight - 20);
            maxWidth = Math.Max(maxWidth * 1.1, scrollViewer.ActualWidth - 20);
            grid.Width = maxWidth;

            foreach (var v in list)
            {
                canvas = designerCanvasList.FirstOrDefault(model => model.PageId == v.Id);

                if (canvas == null)
                {
                    canvas = new DesignerCanvas();
                    canvas.PageId = v.Id;
                    canvas.controlTimeLine = controlTimeLine;
                    savePageInfo = Globals.savePageList.FirstOrDefault(model => model.PageId == v.Id);
                    canvas.Height = propertyManage1.controlStage.Height;
                    canvas.Width = propertyManage1.controlStage.Width;

                    if (savePageInfo == null)
                    {
                        canvas.Background = new SolidColorBrush(propertyManage1.controlStage.Color);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(savePageInfo.Background))
                        {
                            canvas.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(savePageInfo.Background));
                            //canvas.Background = XamlReader.Load(XmlReader.Create(new StringReader(savePageInfo.Background))) as Brush;
                        }
                    }
                    canvas.AllowDrop = true;
                    canvas.Focusable = true;

                    canvas.Margin = new Thickness(30);
                    canvas.propertyManage = propertyManage1;
                    designerCanvasList.Add(canvas);

                    grid.Children.Add(canvas);
                }

                #region 添加标题
                //判断如果有TextBlock则去掉。
                foreach (var vv in canvas.Children)
                    if (vv.GetType().Name == "TextBlock")
                    {
                        canvas.Children.Remove((TextBlock)vv);
                        break;
                    }
                tb = new TextBlock() { HorizontalAlignment = System.Windows.HorizontalAlignment.Left, VerticalAlignment = System.Windows.VerticalAlignment.Top, Margin = new Thickness(0, -20, 0, 0) };
                tb.DataContext = v;
                tb.SetBinding(TextBlock.TextProperty, "Title");
                canvas.Children.Add(tb);
                #endregion

                for (int i = grid.RowDefinitions.Count; i < designerCanvasList.Count; i++)
                {
                    row = new RowDefinition();
                    row.Height = new GridLength(maxHeight);
                    grid.RowDefinitions.Add(row);
                }
                Grid.SetRow(canvas, Index);
                Index++;
                if (v.Children.Count > 0)
                    CreatePage(v.Children, ref Index);
            }

        }
        List<jg.Editor.Library.Control.ComboTree.TreeModel> FillTreeModel(List<TreeViewItemInfo> infoList)
        {
            List<jg.Editor.Library.Control.ComboTree.TreeModel> _treemodellist = new List<Library.Control.ComboTree.TreeModel>();
            jg.Editor.Library.Control.ComboTree.TreeModel _treemodel;

            foreach (var v in infoList)
            {
                _treemodel = TreeViewItemInfo2TreeModel(v);
                if (v.Children != null)
                    _treemodel.Children.AddRange(FillTreeModel(v.Children.ToList()));
                _treemodellist.Add(_treemodel);
            }
            return _treemodellist;

        }
        jg.Editor.Library.Control.ComboTree.TreeModel TreeViewItemInfo2TreeModel(TreeViewItemInfo info)
        {
            jg.Editor.Library.Control.ComboTree.TreeModel model = new Library.Control.ComboTree.TreeModel();
            model.SelectedValuePath = info.Id.ToString();
            model.DisplayValuePath = info.Title;
            return model;
        }
        /// <summary>
        /// 编辑器版本  0 代表企业  1 代表91   2 代表资源中心   
        /// </summary>
        //private readonly string VerEditor = System.Configuration.ConfigurationManager.AppSettings["VerEditor"];
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {
                propertyManage1.eventAddAssResInfoTpage += propertyManage1_eventAddAssResInfoTpage;

                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                //windowLogin windowlogin = new windowLogin();
                //if (VerEditor != "0")
                //{
                    //if (windowlogin.ShowDialog() != true)
                    //    this.Close();
                    //WinProBusy w = new WinProBusy(windowlogin.UserName, windowlogin.UserPassword);



                    //if (w.ShowDialog() == true)
                    //{

                    //    if (VerEditor == "1")
                    //    {
                    //        SetVerEditorBingAction(enumVerEditor.Local);
                    //    }
                    //    else
                    //    {
                    //        SetVerEditorBingAction(enumVerEditor.Resources);
                    //    }
                    //    if (string.IsNullOrEmpty(Globals.SavePath)) return;
                    //    Load(Globals.appStartupPath + "\\" + Globals.SavePath.Substring(Globals.SavePath.LastIndexOf("\\") + 1));

                    //}
                    //else
                    //{

                    //    this.Close();
                    //}


                //}
                //else
                //{
                    SetVerEditorBingAction(enumVerEditor.Local);
                    if (string.IsNullOrEmpty(Globals.SavePath)) return;
                    Load(Globals.appStartupPath + "\\" + Globals.SavePath.Substring(Globals.SavePath.LastIndexOf("\\") + 1));
                //}
            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }

        }

        #region 初始化版本


        /// <summary>
        /// 设置本地版本
        /// </summary>
        private void SetlocalVer()
        {

        }
        /// <summary>
        /// 设置资源中心版本
        /// </summary>
        private void SetResourceVer()
        {

        }
        /// <summary>
        /// 设置91版本
        /// </summary>
        private void Setdidi91Ver()
        {

        }
        #endregion
        AssResInfo propertyManage1_eventAddAssResInfoTpage()
        {
            return setShowTpageGroup();
        }

        public AssResInfo setShowTpageGroup()
        {

            AssResInfo arinfo =null;
            Window win = null;
            if (localTPageGroupMenu.IsChecked)
            {
                win = new LocalRes(global::jg.Editor.Properties.Resources.ExtensionServerImgFilter, true, out arinfo);
            }
            else
            {
                //win = new WindowAssetSel(new string[] { "jpg", "png", "bmp" }, out arinfo, true);
            }
            win.ShowDialog();

            return arinfo;
        }
        //void sc_ConnectFailed(object sender, JG.Library.Net.TcpCSFramework.NetEventArgs e)
        //{
        //    this.Dispatcher.Invoke(new Action(delegate { MessageBox.Show(this, "无法正常连接到授权认证服务器。"); this.Close(); }));
        //}
        //void sc_ReceivedDatagram(object sender, JG.Library.Net.TcpCSFramework.NetEventArgs e)
        //{
        //    switch (e.Client.Datagram.Substring(0, e.Client.Datagram.IndexOf(":")))
        //    {
        //        case "300"://接收密钥
        //            Globals.Key = e.Client.Datagram.Substring(e.Client.Datagram.IndexOf(":") + 1);
        //            break;
        //        case "400"://授权成功
        //            SecurityState = true;
        //            break;
        //        case "401"://服务器授权已满
        //            SecurityState = false;
        //            this.Dispatcher.Invoke(new Action(delegate { MessageBox.Show(this, "服务器授权已满，不能继续登录。"); }));
        //            break;
        //        case "402"://服务器授权已满
        //            SecurityState = false;
        //            this.Dispatcher.Invoke(new Action(delegate { MessageBox.Show(this, "无此授权。"); Environment.Exit(0); }));
        //            break;
        //        case "500"://被服务器主动踢掉
        //            SecurityState = false;
        //            //this.Dispatcher.Invoke(new Action(delegate { MessageBox.Show(this, "已与服务器断开连接，不能继续登录。"); }));
        //            break;
        //    }
        //}
        ////安全认证断开，退出程序。
        //void sc_DisConnectedServer(object sender, JG.Library.Net.TcpCSFramework.NetEventArgs e)
        //{
        //    this.Dispatcher.Invoke(new Action(delegate
        //    {
        //        MessageBox.Show(this, "已与服务器断开连接。");
        //        Environment.Exit(0);
        //    }));
        //}
        void GetSaveItemInfo(ref SaveItemInfo saveItemInfo, ToolboxItem item)
        {

            TransformGroup transformGroup;
            string s;

            saveItemInfo.Height = item.ActualHeight;
            saveItemInfo.AssetPathAndThumbnailsList = item.AssetPathAndThumbnailsList;
            saveItemInfo.Width = item.ActualWidth;
            saveItemInfo.AssetPath = item.AssetPath;
            saveItemInfo.Thumbnails = item.Thumbnails;
            saveItemInfo.ItemId = item.ItemId;
            saveItemInfo.LineHeight = item.LineHeight;
            saveItemInfo.ItemName = item.ItemName;
            saveItemInfo.ItemsDis = item.ItemsDis;
            transformGroup = item.RenderTransform as TransformGroup;


            if (null != transformGroup)
            {
                foreach (var transform in transformGroup.Children)
                {
                    switch (transform.GetType().Name)
                    {
                        case "RotateTransform":
                            RotateTransform rotateTransform = transform as RotateTransform;
                            if (rotateTransform != null) { saveItemInfo.Angle = rotateTransform.Angle; }
                            break;
                        case "ScaleTransform":
                            ScaleTransform scaleTransform = transform as ScaleTransform;
                            if (scaleTransform != null)
                            { saveItemInfo.ScaleX = scaleTransform.ScaleX; saveItemInfo.ScaleY = scaleTransform.ScaleY; }
                            break;
                        case "SkewTransform":
                            SkewTransform skewTransform = transform as SkewTransform;
                            if (skewTransform != null) { saveItemInfo.SkewX = skewTransform.AngleX; saveItemInfo.SkewY = skewTransform.AngleY; }
                            break;
                        case "TranslateTransform":
                            TranslateTransform translateTransform = transform as TranslateTransform;
                            if (translateTransform != null) { saveItemInfo.TranslateX = translateTransform.X; saveItemInfo.TranslateY = translateTransform.Y; }
                            break;
                    }
                }
            }

            saveItemInfo.FontFamily = item.FontFamily.ToString();
            saveItemInfo.FontSize = item.FontSize;
            saveItemInfo.IsLongText = item.IsLongText;
            saveItemInfo.LineHeight = item.LineHeight;
            saveItemInfo.IsShowDiv = item.IsShowDiv;
            saveItemInfo.IsDescPt = item.IsDescPt;
            saveItemInfo.Bold = item.FontWeight == FontWeights.Bold ? true : false;
            saveItemInfo.Italic = item.FontStyle == FontStyles.Italic ? true : false;
            saveItemInfo.Opacity = item.Opacity;
            saveItemInfo.Foreground = item.ItemForeground.ToString();
            switch (item.AssetType)
            {
                case AssetType.Text:
                    jg.Editor.Library.Control.ControlTextEditor editor = item.Content as jg.Editor.Library.Control.ControlTextEditor;

                    s = System.Windows.Markup.XamlWriter.Save(editor.Document);
                    saveItemInfo.Content = GetContent(jg.HTMLConverter.HtmlFromXamlConverter.ConvertXamlToHtml(s));
                    break;
                case AssetType.Topic:
                    TopicControl tc = item.Content as TopicControl;
                    saveItemInfo.Foreground = tc.ItemForeground.ToString();
                    s = System.Windows.Markup.XamlWriter.Save(tc.TopicInfo);
                    saveItemInfo.Content = GetContent(s);
                    saveItemInfo.Score = tc.TopicInfo.Score;
                    break;
                case AssetType.TopicDrag:
                    jg.Editor.Library.Control.ControlTopicDrag tdc = item.Content as jg.Editor.Library.Control.ControlTopicDrag;
                    saveItemInfo.Content = GetContent(tdc);
                    saveItemInfo.Score = tdc.Score;
                    break;
                case AssetType.TextGrid:
                    saveItemInfo.Content = GetContent(item.Content);
                    foreach (var v in ((ContentGrid)saveItemInfo.Content).List)
                    {
                        if (v.Children == null) continue;
                        if (!string.IsNullOrEmpty(v.Children.AssetPath))
                            v.Children.AssetPath = v.Children.AssetPath.Substring(v.Children.AssetPath.LastIndexOf("\\") + 1);
                        if (!string.IsNullOrEmpty(v.Children.Thumbnails))
                            v.Children.Thumbnails = v.Children.Thumbnails.Substring(v.Children.Thumbnails.LastIndexOf("\\") + 1);
                    }
                    break;

                case AssetType.Shape:
                    Shape element = item.Content as Shape;
                    saveItemInfo.Content = GetContent(element.Name);
                    break;
                case AssetType.Message:
                    saveItemInfo.Content = GetContent(item.Content);
                    break;
                case AssetType.Line:
                    saveItemInfo.Content = GetContent(item.Content);
                    break;
                case AssetType.TPageGroup:
                    jg.Editor.Library.Control.TPageControl pagecontrol = item.Content as jg.Editor.Library.Control.TPageControl;
                    pagecontrol.Tag = item;
                    saveItemInfo.Content = GetContent(pagecontrol);
                    saveItemInfo.AssetPathAndThumbnailsList = pagecontrol.Children;
                    break;
            }

            //颜色属性保存
            switch (item.Content.GetType().Name)
            {
                case "ControlTextEditor":
                    saveItemInfo.Background = ((jg.Editor.Library.Control.ControlTextEditor)item.Content).Background.ToString();
                    break;
                default:
                    saveItemInfo.Background = item.ItemBackground.ToString();
                    break;
            }
            saveItemInfo.assetType = item.AssetType;

        }
        SaveItemInfo GetSaveItemInfo(DesignerItem designerItem)
        {

            SaveItemInfo saveItemInfo = new SaveItemInfo();
            ToolboxItem item;

            item = designerItem.Content as ToolboxItem;

            if (item == null) return null;

            saveItemInfo.X = Canvas.GetLeft(designerItem);
            saveItemInfo.Y = Canvas.GetTop(designerItem);

            //事件动作
            if (designerItem.assetActionInfo != null)
                saveItemInfo.assetActionInfo = designerItem.assetActionInfo;

            //时间轴
            saveItemInfo.timeLineItemInfo = designerItem.timeLineItemInfo;
            saveItemInfo.ZIndex = Panel.GetZIndex(designerItem);

            GetSaveItemInfo(ref saveItemInfo, item);

            return saveItemInfo;
        }
        SavePageInfo GetSavePageInfo(DesignerCanvas designerCanvas)
        {
            SavePageInfo savePageInfo;
            SaveItemInfo saveItemInfo;
            DesignerItem designerItem;
            savePageInfo = new SavePageInfo();
            savePageInfo.PageId = designerCanvas.PageId;

            savePageInfo.Height = designerCanvas.ActualHeight;
            savePageInfo.Width = designerCanvas.ActualWidth;
            savePageInfo.Background = designerCanvas.Background.ToString();
            savePageInfo.StageSwitch = (int)designerCanvas.StageSwitch;
            savePageInfo.AutoNext = designerCanvas.AutoNext;
            savePageInfo.IsVisable = designerCanvas.IsVisable;

            SetwpfVis(Globals.treeviewSource, savePageInfo.PageId, savePageInfo.WpfVisibility);
            foreach (var vv in designerCanvas.Children)
            {
                designerItem = vv as DesignerItem;
                if (designerItem == null) continue;

                saveItemInfo = GetSaveItemInfo(designerItem);
                if (saveItemInfo.assetType == AssetType.TPageGroup)
                {

                    foreach (var v in saveItemInfo.AssetPathAndThumbnailsList)
                    {
                        SaveItemInfo model = CloneObject(saveItemInfo) as SaveItemInfo;

                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(v.AssetPath))
                            {
                                //string OldPath = "";
                                //OldPath = saveItemInfo.AssetPath;
                                //FileInfo f = FileSecurity.GetAssetInfo(OldPath);
                                //byte[] byteInfo = FileSecurity.GetStream(f);
                                //byte[] b = FileSecurity.decrypt(byteInfo, Globals.AssetDecryptKey);
                                model.AssetPath = v.AssetPath.Substring(v.AssetPath.LastIndexOf("\\") + 1);
                                if (!saveItemInfo.IsDescPt)
                                {
                                    if (!(model.AssetPath.IndexOf("decode") == 0))
                                    {
                                        string Extension = model.AssetPath.Substring(model.AssetPath.LastIndexOf(".") + 1);
                                        string AssetName = "decode" + model.AssetPath.Substring(0, model.AssetPath.LastIndexOf(".") + 1);
                                        string TempEncryptPath = AssetName + Extension;
                                        model.AssetPath = TempEncryptPath;
                                    }
                                }
                                else if ((model.AssetPath.IndexOf("decode") == 0))
                                {
                                    model.AssetPath = model.AssetPath.Substring("decode".Length);
                                    model.AssetPath = "encrypt" + model.AssetPath;

                                }
                                //else if (b == null && saveItemInfo.IsDescPt)
                                //{
                                //    model.AssetPath = "local" + model.AssetPath;
                                //}

                            }

                            if (!string.IsNullOrEmpty(v.Thumbnails))
                                model.Thumbnails = v.Thumbnails.Substring(v.Thumbnails.LastIndexOf("\\") + 1);
                            if (model == null) continue;

                            savePageInfo.saveItemList.Add(model);
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(saveItemInfo.AssetPath))
                    {
                        string OldPath = "";
                        OldPath = saveItemInfo.AssetPath;
                        FileInfo f = FileSecurity.GetAssetInfo(OldPath);
                        byte[] byteInfo = FileSecurity.GetStream(f);
                        byte[] b = FileSecurity.decrypt(byteInfo, Globals.AssetDecryptKey);

                        saveItemInfo.AssetPath = saveItemInfo.AssetPath.Substring(saveItemInfo.AssetPath.LastIndexOf("\\") + 1);
                        if (!saveItemInfo.IsDescPt && saveItemInfo.assetType != AssetType.HTML5)
                        {
                            if (!(saveItemInfo.AssetPath.IndexOf("decode") == 0))
                            {
                                string Extension = saveItemInfo.AssetPath.Substring(saveItemInfo.AssetPath.LastIndexOf(".") + 1);
                                string AssetName = "decode" + saveItemInfo.AssetPath.Substring(0, saveItemInfo.AssetPath.LastIndexOf(".") + 1);
                                string TempEncryptPath = AssetName + Extension;
                                saveItemInfo.AssetPath = TempEncryptPath;
                            }

                        }
                        else if ((saveItemInfo.AssetPath.IndexOf("decode") == 0) && saveItemInfo.assetType != AssetType.HTML5)
                        {
                            saveItemInfo.AssetPath = saveItemInfo.AssetPath.Substring("decode".Length);
                            saveItemInfo.AssetPath = "encrypt" + saveItemInfo.AssetPath;

                        }
                        else if (b == null && saveItemInfo.IsDescPt)
                        {
                            saveItemInfo.AssetPath = "local" + saveItemInfo.AssetPath;
                        }
                    }
                    if (!string.IsNullOrEmpty(saveItemInfo.Thumbnails))
                        saveItemInfo.Thumbnails = saveItemInfo.Thumbnails.Substring(saveItemInfo.Thumbnails.LastIndexOf("\\") + 1);
                    if (saveItemInfo.assetType == AssetType.HTML5)
                    {
                        if (!string.IsNullOrEmpty(saveItemInfo.ItemsDis))
                            saveItemInfo.ItemsDis = saveItemInfo.ItemsDis.Substring(saveItemInfo.ItemsDis.LastIndexOf("\\") + 1);
                    }
                    if (saveItemInfo == null) continue;
                    savePageInfo.saveItemList.Add(saveItemInfo);
                }
            }
            return savePageInfo;
        }


        /// <summary>
        /// 克隆一个对象
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private object CloneObject(object o)
        {
            object p = new object();
            try
            {
                Type t = o.GetType();
                PropertyInfo[] properties = t.GetProperties();
                p = t.InvokeMember("", System.Reflection.BindingFlags.CreateInstance, null, o, null);
                foreach (PropertyInfo pi in properties)
                {
                    if (pi.CanWrite)
                    {
                        object value = pi.GetValue(o, null);
                        pi.SetValue(p, value, null);
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.InnerException.Data.ToString()
               + "                   " + ex.InnerException.HelpLink.ToString()
              + "                    " + ex.InnerException.ToString()
            + "                    " + ex.InnerException.Message.ToString()
              + "                " + ex.InnerException.Source.ToString()
         + "                       " + ex.InnerException.StackTrace.ToString()
           + "                 " + ex.InnerException.TargetSite.ToString()
           + "                " + ex.InnerException.GetBaseException().ToString()
           + "                    " + ex.InnerException.GetBaseException().Message);
            }

            return p;
        }

        public void SetwpfVis(ObservableCollection<TreeViewItemInfo> info, Guid pageid, Visibility vis)
        {
            if (Globals.treeviewSource != null && Globals.treeviewSource.Count > 0)
            {

                foreach (var v in info)
                {
                    if (v.Id == pageid)
                    {
                        v.WpfVisibility = vis;

                        break;
                    }

                    if (v.Children != null && v.Children.Count > 0)
                    {
                        SetwpfVis(v.Children, pageid, vis);
                    }

                }

            }
        }
        XmlSerializer SerializerSavePageList(List<SavePageInfo> savePageList)
        {
            XmlSerializer xmlSerializer;
            xmlSerializer = new XmlSerializer(savePageList.GetType(),
                new Type[] {
                    typeof(SaveItemInfo),
                    typeof(ContentShape), 
                    typeof(ContentGrid),
                    typeof(ContentText), 
                    typeof(ContentTopicDrag),
                    typeof(ContentMessage),
                    typeof(ContentLine),
                    typeof(abstractContent),
                    typeof(ContentGridItem), 
                    typeof(AssetActionInfo),
                    typeof(TimeLineItemInfo),
                    typeof(TimePoint), 
                    typeof(ContentTpageGroup), 
                    typeof(abstractAssetProperty),
                    typeof(AssetDoubleProperty),
                    typeof(AssetColorProperty),

                });

            return xmlSerializer;
        }
        abstractContent GetContent(object ContentControl)
        {
            abstractContent content = null;
            jg.Editor.Library.Control.ControlTextGrid controlTextGrid;
            jg.Editor.Library.Control.ControlTopicDrag controlTopicDrag;
            jg.Editor.Library.Control.ControlMessage controlMessage;
            jg.Editor.Library.Control.ControlLine controlLine;
            jg.Editor.Library.Control.TPageControl pageControl;
            SaveItemInfo saveItemInfo;
            switch (ContentControl.GetType().Name)
            {
                case "ControlMessage":
                    controlMessage = ContentControl as jg.Editor.Library.Control.ControlMessage;
                    ContentMessage contentMessage = new ContentMessage();
                    contentMessage.Title = controlMessage.Title;
                    contentMessage.PointX = controlMessage.Location.X;
                    contentMessage.PointY = controlMessage.Location.Y;
                    content = (abstractContent)contentMessage;
                    break;
                case "ControlLine":
                    controlLine = ContentControl as jg.Editor.Library.Control.ControlLine;
                    ContentLine contentLine = new ContentLine();
                    contentLine.Point1X = controlLine.Point1.X;
                    contentLine.Point1Y = controlLine.Point1.Y;
                    content = (abstractContent)contentLine;
                    break;
                case "ControlTextGrid":
                    controlTextGrid = ContentControl as jg.Editor.Library.Control.ControlTextGrid;
                    ContentGrid contentGrid = new ContentGrid();
                    if (controlTextGrid == null) return null;
                    contentGrid.ColumnCount = controlTextGrid.ColumnCount;
                    contentGrid.RowCount = controlTextGrid.RowCount;
                    contentGrid.BorderWidth = controlTextGrid.BorderWidth;
                    foreach (var v in controlTextGrid.Children.OfType<TextBox>())
                        contentGrid.List.Add(new ContentGridItem() { Row = Grid.GetRow(v), Column = Grid.GetColumn(v), RowSpan = Grid.GetRowSpan(v), ColumnSpan = Grid.GetColumnSpan(v), Content = v.Text });

                    foreach (ToolboxItem v in controlTextGrid.Children.OfType<ToolboxItem>())
                    {
                        if (!string.IsNullOrEmpty(v.AssetPath) && File.Exists(v.AssetPath))
                            AssetFileList.Add(v.AssetPath.Substring(v.AssetPath.LastIndexOf("\\") + 1));
                        if (!string.IsNullOrEmpty(v.Thumbnails) && File.Exists(v.Thumbnails))
                            AssetFileList.Add(v.Thumbnails.Substring(v.Thumbnails.LastIndexOf("\\") + 1));

                        saveItemInfo = new SaveItemInfo();
                        GetSaveItemInfo(ref saveItemInfo, v);
                        contentGrid.List.Add(new ContentGridItem() { Row = Grid.GetRow(v), Column = Grid.GetColumn(v), RowSpan = Grid.GetRowSpan(v), ColumnSpan = Grid.GetColumnSpan(v), Children = saveItemInfo });
                    }

                    content = (abstractContent)contentGrid;

                    break;
                case "ControlTopicDrag":
                    controlTopicDrag = ContentControl as jg.Editor.Library.Control.ControlTopicDrag;
                    ContentTopicDrag contentTopicDrag = new ContentTopicDrag();
                    if (controlTopicDrag == null) return null;
                    contentTopicDrag.topicDragItemAnswerList = controlTopicDrag.topicDragItemAnswerList;
                    contentTopicDrag.topicDragItemList = controlTopicDrag.topicDragItemList;
                    contentTopicDrag.Background = controlTopicDrag.ItemBackground.ToString();
                    contentTopicDrag.Foreground = controlTopicDrag.ItemForeground.ToString();
                    contentTopicDrag.Score = controlTopicDrag.Score;
                    content = (abstractContent)contentTopicDrag;
                    break;
                case "TPageControl":

                    jg.Editor.Library.Control.TPageControl TPageControl = CloneObject(ContentControl) as jg.Editor.Library.Control.TPageControl;
                    ToolboxItem item = TPageControl.Tag as ToolboxItem;
                    ContentTpageGroup modelReturn = new ContentTpageGroup();
                    if (TpageGroupDict.Keys.Count > 0)
                    {
                        modelReturn.Children = TpageGroupDict[item.ItemId];
                        modelReturn.ImgCount = TpageGroupDict[item.ItemId].Count;
                        modelReturn.ShowHeight = TPageControl.Height;
                        modelReturn.ShowWidth = TPageControl.Width;
                    }
                    else
                    {
                        if (TPageControl.Children != null)
                        {
                            modelReturn.Children = TPageControl.Children;
                            modelReturn.ImgCount = TPageControl.Children.Count;
                            modelReturn.ShowHeight = TPageControl.Height;
                            modelReturn.ShowWidth = TPageControl.Width;
                        }
                    }
                    //jg.Editor.Library.Control.TPageControl page = new jg.Editor.Library.Control.TPageControl();
                    //page.Height = TPageControl.Height;
                    //page.Width = TPageControl.Width;
                    //page.canvasPageContent.Width = TPageControl.Width - 100;
                    //page.canvasPageContent.Height = TPageControl.Height - 2;
                    //page.Children = TpageGroupDict[item.ItemId];
                    //page.UpdateImgGroup();
                    content = (abstractContent)modelReturn;
                    break;
                case "String":
                    ContentText contentText = new ContentText();
                    contentText.Text = ContentControl.ToString();
                    content = (abstractContent)contentText;
                    break;
            }
            return content;
        }
  
        Dictionary<Guid, ObservableCollection<AssResInfo>> TpageGroupDict = new Dictionary<Guid, ObservableCollection<AssResInfo>>();
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                System.Windows.Forms.OpenFileDialog ofDialog = new System.Windows.Forms.OpenFileDialog();
                ofDialog.Filter = Properties.Resources.ExtensionFilter;
                if (ofDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    TpageGroupDict = new Dictionary<Guid, ObservableCollection<AssResInfo>>();
                    System.Threading.Tasks.Task t = new System.Threading.Tasks.Task((Action)delegate { ShowNewWin.Dispatcher.Invoke(new Action(delegate { ShowNewWin.Visibility = Visibility.Visible; })); });
                    t.Start();
                    System.Threading.Thread task = new System.Threading.Thread(new ThreadStart((Action)delegate
        {
            Globals.SavePath = ofDialog.FileName;
            Globals.TempFolder = Globals.appStartupPath + "\\" + Globals.SavePath.Substring(Globals.SavePath.LastIndexOf("\\") + 1);
            Globals.Release();
            tabControl1.Dispatcher.BeginInvoke(new Action(delegate { Load(Globals.TempFolder); }), DispatcherPriority.SystemIdle);

            #region 填充ControlPropertyAction控件的树。
            this.Dispatcher.BeginInvoke(new Action(delegate
            {
                List<jg.Editor.Library.Control.ComboTree.TreeModel> _treemodellist = FillTreeModel(Globals.treeviewSource.ToList());
                propertyManage1.controlAction.TreeModelList = _treemodellist;
            }));
            this.Dispatcher.Invoke(new Action(delegate { ShowNewWin.Visibility = Visibility.Collapsed; }), DispatcherPriority.SystemIdle);
            #endregion});

        }));

                    task.Start();


                }
            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }





        }

        void FillTree(ObservableCollection<TreeViewItemInfo> list)
        {
            treeViewControl1.SelectedItemChanged -= treeViewControl1_SelectedItemChanged;
            Binding binding = new Binding() { Source = list };
            treeViewControl1.SetBinding(TreeView.ItemsSourceProperty, binding);
            treeViewControl1.SelectedItemChanged += treeViewControl1_SelectedItemChanged;
        }
        void Load(string path)
        {
            XmlSerializer xmlSerializer;
            // 树目录
            xmlSerializer = new XmlSerializer(typeof(ObservableCollection<TreeViewItemInfo>));
            using (System.IO.FileStream fs = new System.IO.FileStream(path + "\\TreeSource.xml", System.IO.FileMode.OpenOrCreate))
            {
                Globals.treeviewSource = (ObservableCollection<TreeViewItemInfo>)xmlSerializer.Deserialize(fs);
                AddTreeViewItemAction(Globals.treeviewSource);
                FillTree(Globals.treeviewSource);
                fs.Flush();
                fs.Close();

            }
            // 舞台集合
            xmlSerializer = new XmlSerializer(typeof(List<SavePageInfo>), new Type[] { typeof(SaveItemInfo), 
                    typeof(ContentShape), 
                    typeof(ContentGrid),
                    typeof(ContentText),
                    typeof(ContentTpageGroup),
                    typeof(ContentMessage),
                    typeof(ContentLine),
                    typeof(ContentTopicDrag),
                    typeof(abstractContent),
                    typeof(ContentGridItem),
                    typeof(AssetActionInfo),
                    typeof(TimeLineItemInfo), 
                    typeof(TimePoint),
                    typeof(abstractAssetProperty),
                    typeof(AssetDoubleProperty), 
                    typeof(AssetColorProperty),      
            });


            using (System.IO.FileStream fs = new System.IO.FileStream(path + "\\Content.xml", System.IO.FileMode.Open))
            {
                try
                {
                    Globals.savePageList = (List<SavePageInfo>)xmlSerializer.Deserialize(fs);

                    DesignerCanvas canvas;

                    designerCanvasList.Clear();
                    grid.Children.Clear();
                    grid.RowDefinitions.Clear();

                    //修改素材路径到本地
                    foreach (var v in Globals.savePageList)
                    {
                        int itemCount = 1;
                        foreach (var item in v.saveItemList)
                        {
                            //表格控件需要单独处理
                            if (item.assetType == AssetType.TextGrid)
                            {
                                foreach (var vv in ((ContentGrid)item.Content).List)
                                {
                                    if (vv.Children == null) continue;
                                    if (!string.IsNullOrEmpty(vv.Children.AssetPath))
                                        vv.Children.AssetPath = Globals.TempFolder + "\\" + vv.Children.AssetPath;
                                    if (!string.IsNullOrEmpty(vv.Children.Thumbnails))
                                        vv.Children.Thumbnails = Globals.TempFolder + "\\" + vv.Children.Thumbnails;
                                }
                            }
                            else if (item.assetType == AssetType.TPageGroup)
                            {

                                bool IsContainsed = false;
                                ObservableCollection<AssResInfo> assresInfoobser = new ObservableCollection<AssResInfo>();
                                if (TpageGroupDict.Keys.Count > 0 && TpageGroupDict.Keys.Contains(item.ItemId))
                                {
                                    IsContainsed = true;
                                }

                                AssResInfo assresInfo = new AssResInfo();
                                assresInfo.ArId = item.ItemId;
                                assresInfo.AssetName = "图片" + itemCount;
                                itemCount++;
                                if (!string.IsNullOrEmpty(item.AssetPath))
                                {
                                    item.AssetPath = Globals.TempFolder + "\\" + item.AssetPath;
                                    assresInfo.AssetPath = item.AssetPath;
                                }
                                if (!string.IsNullOrEmpty(item.Thumbnails))
                                {
                                    item.Thumbnails = Globals.TempFolder + "\\" + item.Thumbnails;
                                    assresInfo.AssetPath = item.Thumbnails;
                                }

                                if (!IsContainsed)
                                {
                                    assresInfoobser.Add(assresInfo);
                                    TpageGroupDict.Add(item.ItemId, assresInfoobser);
                                }
                                else
                                {
                                    TpageGroupDict[item.ItemId].Add(assresInfo);
                                }

                                item.AssetPathAndThumbnailsList = TpageGroupDict[item.ItemId];
                            }

                            else
                            {
                                if (!string.IsNullOrEmpty(item.AssetPath))
                                    item.AssetPath = Globals.TempFolder + "\\" + item.AssetPath;
                                if (!string.IsNullOrEmpty(item.Thumbnails))
                                    item.Thumbnails = Globals.TempFolder + "\\" + item.Thumbnails;
                            }


                        }
                    }
                    if (Globals.savePageList.Count > 0)
                    {
                        propertyManage1.controlStage.Height = canvas_height = Globals.savePageList[0].Height;
                        propertyManage1.controlStage.Width = canvas_width = Globals.savePageList[0].Width;
                    }
                    //填充designerCanvasList
                    foreach (var v in Globals.savePageList)
                    {

                        canvas = new DesignerCanvas();
                        canvas.PageId = v.PageId;
                        canvas.IsVisable = v.IsVisable;
                        SetwpfVis(Globals.treeviewSource, v.PageId, v.WpfVisibility);
                        canvas.AutoNext = v.AutoNext;
                        canvas.controlTimeLine = controlTimeLine;
                        canvas.StageSwitch = (enumStageSwitch)v.StageSwitch;

                        canvas.Width = canvas_width;
                        canvas.Height = canvas_height;

                        if (!string.IsNullOrEmpty(v.Background))
                        {
                            if (v.Background.Substring(0, 1) == "#")
                                canvas.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(v.Background));
                            else
                                canvas.Background = XamlReader.Load(XmlReader.Create(new StringReader(v.Background))) as Brush;
                        }
                        canvas.AllowDrop = true;
                        canvas.Focusable = true;

                        canvas.Margin = new Thickness(30);
                        canvas.propertyManage = propertyManage1;
                        designerCanvasList.Add(canvas);

                        grid.Children.Add(canvas);
                        ToolboxItem.DirectoryTpageLoad = new Dictionary<Guid, Library.Control.TPageControl>();
                        ToolboxItem.AssResInfoObser = new Dictionary<Guid, ObservableCollection<AssResInfo>>();
                        DesignerCanvas.newDoesignerItemDictionary = new Dictionary<Guid, DesignerItem>();
                        foreach (var item in v.saveItemList.OrderBy(model => model.ZIndex))
                        {
                            canvas.AddItem(item, false);
                        }
                    }
                    fs.Flush();
                    fs.Close();
                }
                catch (Exception ex)
                {
                    log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                    log.Error(ex.Message + "\r\n" + ex.StackTrace);

                }
            }


            int Index = 0;
            CreatePage(Globals.treeviewSource, ref Index);

            List<Tuple<string, string>> assetFileList = new List<Tuple<string, string>>();

            foreach (var v in Globals.savePageList)
            {
                foreach (var vv in v.saveItemList)
                {

                    if (vv.assetType == AssetType.TextGrid)
                    {
                        foreach (var vvv in ((ContentGrid)vv.Content).List)
                        {
                            if (vvv.Children == null) continue;
                            if (string.IsNullOrEmpty(vvv.Children.AssetPath)) continue;
                            path = vvv.Children.AssetPath.Substring(vvv.Children.AssetPath.LastIndexOf("\\") + 1);
                            assetFileList.Add(new Tuple<string, string>(path.Substring(0, path.LastIndexOf(".")), path));

                        }
                    }

                    else
                    {
                        if (string.IsNullOrEmpty(vv.AssetPath)) continue;
                        path = vv.AssetPath.Substring(vv.AssetPath.LastIndexOf("\\") + 1);

                        assetFileList.Add(new Tuple<string, string>(path.Substring(0, path.LastIndexOf(".")), path));
                    }
                }
            }

            //后台下载电子书中缺少的素材.下载结束后线程自动结束.
            //System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Globals.DownLoadAsset));
            //thread.Start(assetFileList);
        }
        void AddTreeViewItemAction(ObservableCollection<TreeViewItemInfo> item)
        {
            item.CollectionChanged += Source_CollectionChanged;
            foreach (var v in item)
            {
                v.CollectionChanged += Source_CollectionChanged;
                if (v.Children.Count > 0) AddTreeViewItemAction(v.Children);
            }
        }
        private void Preview_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Publish();

                windowSave save = new windowSave() { DataFileList = DataFileList, AssetFileList = AssetFileList, PageCount = Globals.savePageList.Count };

                if (Globals.SavePath == "")
                {
                    if (save.ShowDialog() != true) return;
                }
                else
                {
                    save.Location(Globals.SavePath, DataFileList, AssetFileList);
                    save.Close();
                }                

                jg.PCPlayerLibrary.Entrance entrance = new PCPlayerLibrary.Entrance(Globals.SavePath);
                entrance.windowPreview = new PCPlayerLibrary.MainWindow();
                entrance.windowPreview.TittleName = entrance.CourseName;
                entrance.windowPreview.ShowDialog();

                double a = entrance.TotalScore;
                double b = entrance.Score;
            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }
        }
        private void btnRedo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DesignerCanvas.Redo();
            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }
        }
        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DesignerCanvas.Undo();
            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message + "\r\n" + ex.StackTrace);

            }
        }
        public void InitTreeModelList()
        {
            List<jg.Editor.Library.Control.ComboTree.TreeModel> _treemodellist = FillTreeModel(Globals.treeviewSource.ToList());
            propertyManage1.controlAction.TreeModelList = _treemodellist;
        }
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult MBR = new MessageBoxResult();
            if (Globals.treeviewSource.Count > 0)
            {
                MBR = MessageBox.Show("您是否要先保存当前操作", "编辑器", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            }

            if (MBR != null && MBR == MessageBoxResult.Yes)
            {
                btnPublish_Click(null, null);
                return;
            }
            else if (MBR != null && MBR == MessageBoxResult.Cancel)
            {
                return;
            }

            int Index = 0;
            Guid guid;

            try
            {
                Globals.TempFolder = "";
                Globals.SavePath = "";


                grid.RowDefinitions.Clear();
                Globals.savePageList.Clear();
                grid.Children.Clear();
                designerCanvasList.Clear();
                Globals.treeviewSource.Clear();

                Globals.CourseWareGuid = Guid.NewGuid();
                classinfo = new TreeViewItemInfo() { Id = guid = System.Guid.NewGuid(), ParentId = new Guid(), Title = "目录1", IsEdit = true, IsTbVisabled = Visibility.Collapsed, IsTxtVisabled = Visibility.Visible, Sort = 0 };
                Globals.treeviewSource.Add(classinfo);

                AddTreeViewItemAction(Globals.treeviewSource);
                FillTree(Globals.treeviewSource);
                CreatePage(Globals.treeviewSource, ref Index);
                SelectedTree(Globals.treeviewSource, guid);
            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                controlTimeLine.Play();
            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                controlTimeLine.Pause();
            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                controlTimeLine.Stop();
            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }
        }
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                TransformGroup transformGroup;
                RotateTransform rotateTransform;
                ScaleTransform scaleTransform;
                SkewTransform skewTransform;
                TranslateTransform translateTransform;
                List<abstractAssetProperty> propertyList = new List<abstractAssetProperty>();
                foreach (var v in designerCanvasList)
                {
                    var vv = v.Children.OfType<DesignerItem>().FirstOrDefault(model => model.IsSelected == true);
                    if (vv == null) continue;

                    #region 新增时间点记录当前属性

                    propertyList.Add(new AssetDoubleProperty() { Name = "PropertyFontSizeCommand", Value = vv.FontSize });
                    propertyList.Add(new AssetDoubleProperty() { Name = "PropertyXCommand", Value = Canvas.GetLeft(vv) });
                    propertyList.Add(new AssetDoubleProperty() { Name = "PropertyYCommand", Value = Canvas.GetTop(vv) });
                    propertyList.Add(new AssetDoubleProperty() { Name = "PropertyWidthCommand", Value = vv.ActualWidth });
                    propertyList.Add(new AssetDoubleProperty() { Name = "PropertyHeightCommand", Value = vv.ActualHeight });
                    propertyList.Add(new AssetDoubleProperty() { Name = "PropertyOpacityCommand", Value = vv.Opacity });

                    transformGroup = ((ToolboxItem)vv.Content).RenderTransform as TransformGroup;
                    if (null != transformGroup)
                    {
                        rotateTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "RotateTransform") as RotateTransform;
                        if (rotateTransform != null)
                            propertyList.Add(new AssetDoubleProperty() { Name = "PropertyRotateCommand", Value = rotateTransform.Angle });

                        scaleTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "ScaleTransform") as ScaleTransform;
                        if (scaleTransform != null)
                        {
                            propertyList.Add(new AssetDoubleProperty() { Name = "PropertyScaleXCommand", Value = scaleTransform.ScaleX });
                            propertyList.Add(new AssetDoubleProperty() { Name = "PropertyScaleYCommand", Value = scaleTransform.ScaleY });
                        }
                        skewTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "SkewTransform") as SkewTransform;
                        if (skewTransform != null)
                        {
                            propertyList.Add(new AssetDoubleProperty() { Name = "PropertySkewXCommand", Value = skewTransform.AngleX });
                            propertyList.Add(new AssetDoubleProperty() { Name = "PropertySkewYCommand", Value = skewTransform.AngleY });
                        }
                        translateTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "TranslateTransform") as TranslateTransform;
                        if (translateTransform != null)
                        {
                            propertyList.Add(new AssetDoubleProperty() { Name = "PropertyTranslateXCommand", Value = translateTransform.X });
                            propertyList.Add(new AssetDoubleProperty() { Name = "PropertyTranslateYCommand", Value = translateTransform.Y });
                        }
                    }

                    #endregion
                    break;
                }
                controlTimeLine.Add(propertyList);
            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }
        }
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {
                controlTimeLine.Remove();
            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }
        }
        //选中当前帧，修改控件对应属性
        void controlTimeLine_SelectedFrameChanged(Guid PageId, Guid ItemId, TimePoint timePoint)
        {
            TransformGroup transformGroup;
            RotateTransform rotateTransform;
            ScaleTransform scaleTransform;
            SkewTransform skewTransform;
            TranslateTransform translateTransform;

            DesignerCanvas canvas = designerCanvasList.FirstOrDefault(model => model.PageId == PageId);
            if (canvas == null) return;
            var item = canvas.Children.OfType<DesignerItem>().FirstOrDefault(model => model.ItemId == ItemId);
            if (item == null) return;
            foreach (AssetDoubleProperty property in timePoint.propertyList.FindAll(model => model.propertyEnum == PropertyEnum.DoubleProperty))
            {
                switch (property.Name)
                {
                    case "PropertyFontSizeCommand":
                        item.FontSize = property.Value;
                        break;
                    case "PropertyXCommand":
                        Canvas.SetLeft(item, property.Value);
                        break;
                    case "PropertyYCommand":
                        Canvas.SetTop(item, property.Value);
                        break;
                    case "PropertyWidthCommand":
                        item.Width = property.Value;
                        break;
                    case "PropertyHeightCommand":
                        item.Height = property.Value;
                        break;
                    case "PropertyOpacityCommand":
                        item.Opacity = property.Value;
                        break;
                    case "PropertyRotateCommand":
                        transformGroup = ((ToolboxItem)item.Content).RenderTransform as TransformGroup;
                        if (null == transformGroup) return;
                        rotateTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "RotateTransform") as RotateTransform;
                        if (rotateTransform != null)
                            rotateTransform.Angle = property.Value;
                        break;
                    case "PropertyScaleXCommand":
                        transformGroup = ((ToolboxItem)item.Content).RenderTransform as TransformGroup;
                        if (null == transformGroup) return;
                        scaleTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "ScaleTransform") as ScaleTransform;
                        if (scaleTransform != null)
                            scaleTransform.ScaleX = property.Value;
                        break;
                    case "PropertyScaleYCommand":
                        transformGroup = ((ToolboxItem)item.Content).RenderTransform as TransformGroup;
                        if (null == transformGroup) return;
                        scaleTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "ScaleTransform") as ScaleTransform;
                        if (scaleTransform != null)
                            scaleTransform.ScaleY = property.Value;
                        break;
                    case "PropertySkewXCommand":
                        transformGroup = ((ToolboxItem)item.Content).RenderTransform as TransformGroup;
                        if (null == transformGroup) return;
                        skewTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "SkewTransform") as SkewTransform;
                        if (skewTransform != null)
                            skewTransform.AngleX = property.Value;
                        break;
                    case "PropertySkewYCommand":
                        transformGroup = ((ToolboxItem)item.Content).RenderTransform as TransformGroup;
                        if (null == transformGroup) return;
                        skewTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "SkewTransform") as SkewTransform;
                        if (skewTransform != null)
                            skewTransform.AngleY = property.Value;
                        break;
                    case "PropertyTranslateXCommand":
                        transformGroup = ((ToolboxItem)item.Content).RenderTransform as TransformGroup;
                        if (null == transformGroup) return;
                        translateTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "TranslateTransform") as TranslateTransform;
                        if (translateTransform != null)
                            translateTransform.X = property.Value;
                        break;
                    case "PropertyTranslateYCommand":
                        transformGroup = ((ToolboxItem)item.Content).RenderTransform as TransformGroup;
                        if (null == transformGroup) return;
                        translateTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "TranslateTransform") as TranslateTransform;
                        if (translateTransform != null)
                            translateTransform.Y = property.Value;
                        break;

                }
            }
        }
        private void btnPageAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                ContextMenu menu = btn.ContextMenu;
                menu.PlacementTarget = btn;
                menu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                menu.IsOpen = true;
            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetTxtVisabledTrue(Globals.treeviewSource);
            TreeViewItemInfo info = treeViewControl1.SelectedItem as TreeViewItemInfo;
            TreeViewItemInfo addInfo;
            TreeViewItemInfo parentInfo = null;
            MenuItem menuItem = sender as MenuItem;
            if (menuItem == null) return;
            addInfo = new TreeViewItemInfo();

            switch (menuItem.Tag.ToString())
            {
                case "Main":
                    if (info != null)
                        parentInfo = GetParentInfo(info, Globals.treeviewSource);

                    if (parentInfo != null)
                        addInfo.ParentId = parentInfo.Id;
                    else
                        IsUpdateOrAdd = false;
                    addInfo.Id = Guid.NewGuid();
                    addInfo.IsEdit = true;
                    addInfo.IsTxtVisabled = Visibility.Visible;
                    addInfo.IsTbVisabled = Visibility.Collapsed;
                    addInfo.Title = "目录";

                    if (parentInfo == null)
                        Globals.treeviewSource.Add(addInfo);

                    else
                        parentInfo.Children.Add(addInfo);
                    classinfo = addInfo;
                    addInfo.CollectionChanged += Source_CollectionChanged;
                    break;
                case "Sub":
                    if (info == null) return;
                    IsUpdateOrAdd = false;
                    addInfo.ParentId = info.Id;
                    addInfo.IsTxtVisabled = Visibility.Visible;
                    addInfo.IsTbVisabled = Visibility.Collapsed;
                    addInfo.IsEdit = true;
                    addInfo.Title = "目录";

                    info.Children.Add(addInfo);
                    classinfo = addInfo;
                    addInfo.CollectionChanged += Source_CollectionChanged;
                    break;
                case "Multi":
                    break;
            }
            RefreshStage();
        }
        TreeViewItemInfo GetParentInfo(TreeViewItemInfo info, ObservableCollection<TreeViewItemInfo> list)
        {
            TreeViewItemInfo model = null;
            foreach (var v in list)
            {
                if (v.Id == info.ParentId && v.Id != v.ParentId)
                {
                    return v;
                }
                else
                {
                    model = GetParentInfo(info, v.Children);
                    if (model != null)
                    {
                        return model;
                    }
                }
            }
            return model;
        }
        private void btnCut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TreeViewItemInfo treeViewItemInfo;
                System.Windows.Clipboard.SetDataObject(CurrentSelDataObject);

                Clipboard_TreeViewInfo clipboard_TreeViewInfo = CurrentSelDataObject.GetData("JG_EDITOR_TREEVIEWITEM") as Clipboard_TreeViewInfo;
                ObservableCollection<SaveItemInfo> saveItemInfo = CurrentSelDataObject.GetData("JG_EDITOR_DESIGNERITEM") as ObservableCollection<SaveItemInfo>;
                if (clipboard_TreeViewInfo != null)
                {
                    if (treeViewControl1.SelectedItem == null) return;
                    if ((treeViewItemInfo = treeViewControl1.SelectedItem as TreeViewItemInfo) == null) return;
                    RemovePageItem(treeViewItemInfo.Id);
                }
                else if (saveItemInfo != null && saveItemInfo.Count > 0)
                {
                    if (propertyManage1.Stage == null) return;

                    foreach (var o in saveItemInfo)
                    {
                        var v = propertyManage1.Stage.Children.OfType<DesignerItem>().FirstOrDefault(model => model.ItemId == o.ItemId);
                        if (v != null)
                        {
                            propertyManage1.Stage.Children.Remove(v);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }
        }
        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Clipboard.SetDataObject(CurrentSelDataObject);
            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }
        }
        private void btnPageDel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (treeViewControl1.SelectedItem == null) return;
                var info = treeViewControl1.SelectedItem as TreeViewItemInfo;
                if (info == null) return;

                RemovePageItem(info.Id);
                InitTreeModelList();
            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// 删除目录及画面
        /// </summary>
        /// <param name="id">所要删除目录的id</param>
        void RemovePageItem(Guid id)
        {
            int Index = 0;

            pubRemoveTreeItem(id);

            RemoveChildrenAll(id, Globals.treeviewSource);
            RemoveTreeItem(id, Globals.treeviewSource);
            CreatePage(Globals.treeviewSource, ref Index);
        }

        /// <summary>
        /// 删除所有子目录的集合
        /// </summary>
        /// <param name="parintId">子目录父级ID</param>
        /// <param name="list">子集合</param>
        void RemoveChildrenAll(Guid parintId, ObservableCollection<TreeViewItemInfo> list)
        {

            foreach (var v in list)
            {
                if (v.ParentId == parintId)
                {
                    pubRemoveTreeItem(v.Id);
                    RemoveChildrenAll(v.Id, v.Children);
                }
                if (v.Children.Count > 0)
                {
                    RemoveChildrenAll(v.Id, v.Children);
                }

            }

        }
        /// <summary>
        /// 删除画布及其他
        /// </summary>
        /// <param name="id">所要删除目录的id</param>
        void pubRemoveTreeItem(Guid id)
        {

            var designerCanvas = designerCanvasList.FirstOrDefault(model => model.PageId == id);

            if (designerCanvas != null)
            {

                grid.Children.Remove(designerCanvas);
                designerCanvasList.Remove(designerCanvas);
                grid.RowDefinitions.RemoveAt(grid.RowDefinitions.Count - 1);

            }

            var savePageInfo = Globals.savePageList.FirstOrDefault(model => model.PageId == id);
            if (savePageInfo != null)
            {
                bool B = Globals.savePageList.Remove(savePageInfo);
            }

        }


        /// <summary>
        /// 从集合中删除树形目录
        /// </summary>
        /// <param name="id">所要删除目录ID</param>
        /// <param name="list">源集合</param>
        void RemoveTreeItem(Guid id, ObservableCollection<TreeViewItemInfo> list)
        {
            foreach (var v in list)
            {
                if (v.Id == id)
                {
                    list.Remove(v);
                    return;
                }
                RemoveTreeItem(id, v.Children);


            }
        }

        private void btnPaste_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TreeViewItemInfo treeViewItemInfo;
                int count = 1;
                IDataObject iData = System.Windows.Clipboard.GetDataObject();
                Clipboard_TreeViewInfo clipboard_TreeViewInfo = iData.GetData("JG_EDITOR_TREEVIEWITEM") as Clipboard_TreeViewInfo;
                ObservableCollection<SaveItemInfo> saveItemInfo = iData.GetData("JG_EDITOR_DESIGNERITEM") as ObservableCollection<SaveItemInfo>;
                ObservableCollection<SaveItemInfo> newsaveItemInfo;
                if (clipboard_TreeViewInfo != null)
                {
                    if (treeViewControl1.SelectedItem == null) return;
                    if ((treeViewItemInfo = treeViewControl1.SelectedItem as TreeViewItemInfo) == null) return;

                    clipboard_TreeViewInfo.TreeViewItemInfo.ParentId = treeViewItemInfo.Id;
                    treeViewItemInfo.Children.Add(clipboard_TreeViewInfo.TreeViewItemInfo);


                }
                else if (saveItemInfo != null)
                {
                    if (propertyManage1.Stage == null) return;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        bf.Serialize(ms, saveItemInfo);
                        ms.Seek(0, SeekOrigin.Begin);

                        newsaveItemInfo = bf.Deserialize(ms) as ObservableCollection<SaveItemInfo>;
                    }

                    //获取组件组中形成的最大矩形坐标
                    Point maxPoint = GetMaxPoint(newsaveItemInfo);
                    Point minPoint = GetMinPoint(newsaveItemInfo);
                    foreach (var o in newsaveItemInfo)
                    {
                        o.ItemId = Guid.NewGuid();

                        foreach (var v in propertyManage1.Stage.Children.OfType<DesignerItem>())
                        {
                            if (o.assetType == ((ToolboxItem)v.Content).AssetType)
                                count++;
                        }

                        o.ItemName = o.assetType.ToString() + count.ToString();
                        foreach (var v in o.timeLineItemInfo.TimePointList)
                        {
                            v.Id = Guid.NewGuid();
                        }

                        //获取组成的矩形的中心坐标
                        double sourceX = minPoint.X + (maxPoint.X - minPoint.X) / 2;//X
                        double sourceY = minPoint.Y + (maxPoint.Y - minPoint.Y) / 2;//Y

                        if ((sourceX >= o.X && sourceX <= o.X + o.Width) && (sourceY >= o.Y && sourceY <= o.Y + o.Height))//判断矩形中心坐标是否在此控件中 
                        {
                            //中心在控件组内的计算
                            double mouseX = MousePoint.X - Math.Abs(sourceX - o.X);
                            double mouseY = MousePoint.Y - Math.Abs(sourceY - o.Y);
                            o.X = mouseX < 0 ? 0 : mouseX;
                            o.Y = mouseY < 0 ? 0 : mouseY;
                        }
                        else
                        {
                            //中心在控件组外的计算
                            double mouseX = o.X + MousePoint.X - sourceX;
                            double mouseY = o.Y + MousePoint.Y - sourceY;
                            o.X = mouseX < 0 ? 0 : mouseX;
                            o.Y = mouseY < 0 ? 0 : mouseY;
                        }
                        o.ZIndex += 1;

                        propertyManage1.Stage.AddItem(o);
                    }
                }
            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }
        }



        /// <summary>
        /// 获取控件组最小的坐标
        /// </summary>
        /// <param name="list">选择的控件组集合</param>
        /// <returns>Point</returns>
        private Point GetMinPoint(ObservableCollection<SaveItemInfo> list)
        {
            double maxX = 0;
            double maxY = 0;
            Point pointDouble = new Point();


            foreach (var v in list)
            {
                //X
                if (maxX == 0)
                {
                    maxX = v.X;
                }
                else
                {
                    if (maxX > v.X)
                    {
                        maxX = v.X;
                    }
                }

                //Y
                if (maxY == 0)
                {
                    maxY = v.Y;
                }
                else
                {
                    if (maxY > v.Y)
                    {
                        maxY = v.Y;
                    }
                }
            }
            pointDouble.X = maxX;
            pointDouble.Y = maxY;
            return pointDouble;
        }
        /// <summary>
        /// 获取控件组最大的坐标
        /// </summary>
        /// <param name="list">选择的控件组集合</param>
        /// <returns>Point</returns>
        private Point GetMaxPoint(ObservableCollection<SaveItemInfo> list)
        {
            double maxX = 0;
            double maxY = 0;
            Point pointDouble = new Point();


            foreach (var v in list)
            {
                //X
                if (maxX == 0)
                {
                    maxX = v.X + v.Width;
                }
                else
                {
                    if (maxX < v.X + v.Width)
                    {
                        maxX = v.X + v.Width;
                    }
                }

                //Y
                if (maxY == 0)
                {
                    maxY = v.Y + v.Height;
                }
                else
                {
                    if (maxY < v.Y + v.Height)
                    {
                        maxY = v.Y + v.Height;
                    }
                }
            }
            pointDouble.X = maxX;
            pointDouble.Y = maxY;
            return pointDouble;

        }
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RoutedUICommand routedUICommand = e.Command as RoutedUICommand;
            if (routedUICommand == null) return;
            switch (routedUICommand.Text)
            {
                case "routedUICommandNew":
                    btnCreate_Click(btnCreate, new RoutedEventArgs());
                    break;
                case "routedUICommandLoad":
                    btnLoad_Click(btnLoad, new RoutedEventArgs());
                    break;
                case "routedUICommandSave":
                    //btnPublish_Click(btnPublish, new RoutedEventArgs());
                    Publish();

                    windowSave save = new windowSave() { DataFileList = DataFileList, AssetFileList = AssetFileList, PageCount = Globals.savePageList.Count };
                    if (Globals.SavePath == "")
                        if (save.ShowDialog() != true) return;
                        else
                        {
                            save.Location(Globals.SavePath, DataFileList, AssetFileList);
                            save.Close();
                        }


                    break;
                case "routedUICommandCut":
                    btnCut_Click(btnCut, new RoutedEventArgs());
                    break;
                case "routedUICommandCopy":
                    btnCopy_Click(btnCopy, new RoutedEventArgs());
                    break;
                case "routedUICommandPaste":
                    btnPaste_Click(btnPaste, new RoutedEventArgs());
                    break;
                case "routedUICommandUndo":
                    btnUndo_Click(btnUndo, new RoutedEventArgs());
                    break;
                case "routedUICommandRedo":
                    btnRedo_Click(btnRedo, new RoutedEventArgs());
                    break;
            }
        }
        private void btnPublish_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                Publish();
                windowSave save = new windowSave() { DataFileList = DataFileList, AssetFileList = AssetFileList, PageCount = Globals.savePageList.Count };
                save.ShowDialog();
            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }
        }
        void Publish()
        {
            DataFileList.Clear();
            AssetFileList.Clear();

            PackageData();
            PackageAsset();
        }
        //打包数据部分。
        void PackageData()
        {
            try
            {
                #region 保存树目录
                XmlSerializer xmlSerializer = new XmlSerializer(Globals.treeviewSource.GetType());

                if (System.IO.File.Exists(Globals.appStartupPath + "\\TreeSource.xml"))
                    System.IO.File.Delete(Globals.appStartupPath + "\\TreeSource.xml");

                System.IO.Stream stream = new System.IO.FileStream(Globals.appStartupPath + "\\TreeSource.xml", System.IO.FileMode.OpenOrCreate);
                xmlSerializer.Serialize(stream, Globals.treeviewSource);
                stream.Flush();
                stream.Close();

                #endregion

                #region 保存所有舞台信息
                Globals.savePageList.Clear();

                foreach (var v in designerCanvasList)
                {
                    Globals.savePageList.Add(GetSavePageInfo(v));
                }
                if (System.IO.File.Exists(Globals.appStartupPath + "\\Content.xml"))
                    System.IO.File.Delete(Globals.appStartupPath + "\\Content.xml");

                xmlSerializer = SerializerSavePageList(Globals.savePageList);

                using (System.IO.FileStream fs = new System.IO.FileStream(Globals.appStartupPath + "\\Content.xml", System.IO.FileMode.Create))
                {
                    xmlSerializer.Serialize(fs, Globals.savePageList);
                    fs.Flush();
                    fs.Close();
                }


                DataFileList.Add(Globals.appStartupPath + "\\TreeSource.xml");
                DataFileList.Add(Globals.appStartupPath + "\\Content.xml");
                #endregion

            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }
        }
        //打包素材部分
        void PackageAsset()
        {
            try
            {
                foreach (var v in Globals.savePageList)
                    foreach (var vv in v.saveItemList)
                    {
                        if (!string.IsNullOrEmpty(vv.Thumbnails) && !AssetFileList.Contains(vv.Thumbnails))
                            AssetFileList.Add(vv.Thumbnails);
                        if (!vv.IsDescPt && vv.assetType != AssetType.HTML5)
                        {
                            if (!string.IsNullOrEmpty(vv.AssetPath) && !AssetFileList.Contains(vv.AssetPath))
                            {
                                vv.AssetPath = vv.AssetPath.Substring("decode".Length);
                                string Extension = vv.AssetPath.Substring(vv.AssetPath.LastIndexOf(".") + 1);
                                string AssetName = "decode" + vv.AssetPath.Substring(0, vv.AssetPath.LastIndexOf(".") + 1);
                                string DescPt = vv.AssetPath;
                                string EncryptPath = AssetName + Extension;
                                string TempEncryptPath = AssetName + Extension;
                                if (System.IO.File.Exists(Globals.TempFolder + "\\" + vv.AssetPath))
                                {
                                    DescPt = Globals.TempFolder + "\\" + vv.AssetPath;
                                    TempEncryptPath = Globals.TempFolder + "\\" + EncryptPath;
                                }
                                else if (System.IO.File.Exists(Globals.appStartupPath + "\\" + Globals.assetFolder + "\\" + vv.AssetPath))
                                {
                                    DescPt = Globals.appStartupPath + "\\" + Globals.assetFolder + "\\" + vv.AssetPath;
                                    TempEncryptPath = Globals.appStartupPath + "\\" + Globals.assetFolder + "\\" + EncryptPath;
                                }
                                vv.AssetPath = EncryptPath;

                                if (File.Exists(TempEncryptPath))
                                {
                                    File.Delete(TempEncryptPath);
                                }


                                FileSecurity.StreamToFileInfo(TempEncryptPath, DescPt);
                                FileInfo f = FileSecurity.GetAssetInfo(TempEncryptPath);

                                byte[] byteInfo = FileSecurity.GetStream(f);
                                byte[] b = FileSecurity.decrypt(byteInfo, Globals.AssetDecryptKey);
                                if (b != null)
                                {

                                    if (File.Exists(TempEncryptPath))
                                    {
                                        File.Delete(TempEncryptPath);

                                        FileStream fs2 = new FileStream(TempEncryptPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                                        fs2.Seek(fs2.Length, SeekOrigin.Current);
                                        fs2.Write(b, 0, b.Length);

                                        fs2.Flush();
                                        fs2.Close();
                                    }
                                }
                                AssetFileList.Add(vv.AssetPath);
                            }

                        }
                        if (!string.IsNullOrEmpty(vv.AssetPath) && !AssetFileList.Contains(vv.AssetPath) && vv.IsDescPt && vv.assetType != AssetType.HTML5)
                        {
                            if (vv.AssetPath.IndexOf("encrypt") == 0)
                            {
                                string strencrypt = vv.AssetPath.Substring("encrypt".Length);
                                string strdecode = "decode" + strencrypt;

                                if (System.IO.File.Exists(Globals.TempFolder + "\\" + strdecode))
                                {
                                    strdecode = Globals.TempFolder + "\\" + strdecode;
                                    strencrypt = Globals.TempFolder + "\\" + vv.AssetPath;
                                }
                                else if (System.IO.File.Exists(Globals.appStartupPath + "\\" + Globals.assetFolder + "\\" + strdecode))
                                {
                                    strdecode = Globals.appStartupPath + "\\" + Globals.assetFolder + "\\" + strdecode;
                                    strencrypt = Globals.appStartupPath + "\\" + Globals.assetFolder + "\\" + vv.AssetPath;
                                }

                                if (File.Exists(strencrypt))
                                {
                                    File.Delete(strencrypt);
                                }


                                FileSecurity.StreamToFileInfo(strencrypt, strdecode);
                                FileInfo f = FileSecurity.GetAssetInfo(strencrypt);

                                byte[] byteInfo = FileSecurity.GetStream(f);
                                byte[] b = FileSecurity.encrypt(byteInfo, Globals.AssetDecryptKey);
                                if (b != null)
                                {

                                    if (File.Exists(strencrypt))
                                    {
                                        File.Delete(strencrypt);

                                        FileStream fs2 = new FileStream(strencrypt, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                                        fs2.Seek(fs2.Length, SeekOrigin.Current);
                                        fs2.Write(b, 0, b.Length);

                                        fs2.Flush();
                                        fs2.Close();
                                    }
                                }
                                AssetFileList.Add(vv.AssetPath);
                            }

                        }

                        if (!string.IsNullOrEmpty(vv.AssetPath) && !AssetFileList.Contains(vv.AssetPath) && vv.IsDescPt && vv.assetType != AssetType.HTML5)
                        {
                            if (vv.AssetPath.IndexOf("local") == 0)
                            {

                                string strencrypt = vv.AssetPath.Substring("local".Length);
                                string strdecode = strencrypt;

                                if (System.IO.File.Exists(Globals.TempFolder + "\\" + strdecode))
                                {
                                    strdecode = Globals.TempFolder + "\\" + strdecode;
                                    strencrypt = Globals.TempFolder + "\\" + vv.AssetPath;
                                }
                                else if (System.IO.File.Exists(Globals.appStartupPath + "\\" + Globals.assetFolder + "\\" + strdecode))
                                {
                                    strdecode = Globals.appStartupPath + "\\" + Globals.assetFolder + "\\" + strdecode;
                                    strencrypt = Globals.appStartupPath + "\\" + Globals.assetFolder + "\\" + vv.AssetPath;
                                }

                                if (File.Exists(strencrypt))
                                {
                                    File.Delete(strencrypt);
                                }


                                FileSecurity.StreamToFileInfo(strencrypt, strdecode);
                                FileInfo f = FileSecurity.GetAssetInfo(strencrypt);

                                byte[] byteInfo = FileSecurity.GetStream(f);
                                byte[] b = FileSecurity.encrypt(byteInfo, Globals.AssetDecryptKey);
                                if (b != null)
                                {

                                    if (File.Exists(strencrypt))
                                    {
                                        File.Delete(strencrypt);

                                        FileStream fs2 = new FileStream(strencrypt, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                                        fs2.Seek(fs2.Length, SeekOrigin.Current);
                                        fs2.Write(b, 0, b.Length);

                                        fs2.Flush();
                                        fs2.Close();
                                    }
                                }
                                AssetFileList.Add(vv.AssetPath);
                            }

                        }

                        if (!string.IsNullOrEmpty(vv.AssetPath) && !AssetFileList.Contains(vv.AssetPath) && vv.IsDescPt && vv.assetType != AssetType.HTML5)
                        {
                            AssetFileList.Add(vv.AssetPath);
                        }

                        if (vv.assetType == AssetType.HTML5)
                        {
                            //添加到资源的名字
                            string itemDis = vv.ItemsDis + ".zip";


                            //目录名字
                            string Diect = vv.ItemsDis;

                            //需要压缩的的目录的位置
                            string Assetfolder = "";

                            if (System.IO.Directory.Exists(Globals.TempFolder + "\\" + vv.ItemsDis))
                            {
                                Diect = Globals.TempFolder + "\\" + vv.ItemsDis + "\\";
                                Assetfolder = Globals.TempFolder + "\\";
                            }
                            else if (System.IO.Directory.Exists(Globals.appStartupPath + "\\" + Globals.assetFolder + "\\" + vv.ItemsDis))
                            {
                                Diect = Globals.appStartupPath + "\\" + Globals.assetFolder + "\\" + vv.ItemsDis + "\\";
                                Assetfolder = Globals.appStartupPath + "\\" + Globals.assetFolder + "\\";
                            }
                            GZipResult zip = GZip.Compress(Diect, Assetfolder, itemDis);
                            if (!string.IsNullOrEmpty(itemDis) && !AssetFileList.Contains(itemDis))
                                AssetFileList.Add(itemDis);
                        }

                    }
            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }
        }
        private void AlignButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;
                if (propertyManage1.Stage == null) return;
                switch (btn.Tag.ToString())
                {
                    case "Left":
                        DesignerCanvas.LayoutLeft(propertyManage1.Stage);
                        break;
                    case "Top":
                        DesignerCanvas.LayoutTop(propertyManage1.Stage);
                        break;
                    case "Right":
                        DesignerCanvas.LayoutRight(propertyManage1.Stage);
                        break;
                    case "Bottom":
                        DesignerCanvas.LayoutBottom(propertyManage1.Stage);
                        break;
                }
            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }
        }
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            switch (btn.Tag.ToString())
            {
                case "col1":
                    if (col1.Width == new GridLength(0, GridUnitType.Pixel))
                    {
                        btn.Content = "←";
                        col1.Width = new GridLength(200, GridUnitType.Pixel);
                    }
                    else
                    {
                        btn.Content = "→";
                        col1.Width = new GridLength(0, GridUnitType.Pixel);
                    }
                    break;
                case "col5":
                    if (col5.Width == new GridLength(0, GridUnitType.Pixel))
                    {
                        btn.Content = "→";
                        col5.Width = new GridLength(200, GridUnitType.Pixel);
                    }
                    else
                    {
                        btn.Content = "←";
                        col5.Width = new GridLength(0, GridUnitType.Pixel);
                    }
                    break;
                case "row4":
                    if (row4.Height == new GridLength(0, GridUnitType.Pixel))
                    {
                        btn.Content = "↓";
                        row4.Height = new GridLength(150, GridUnitType.Pixel);
                    }
                    else
                    {
                        btn.Content = "↑";
                        row4.Height = new GridLength(0, GridUnitType.Pixel);
                    }
                    break;
            }

            //↓→↑←
        }
        //private void EditableTabHeaderControl_EditEnd(bool IsSubmit, object tvItem)
        //{
        //    DispatcherTimer timer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 0, 0, 500) };
        //    timer.Tick += (sender, o) =>
        //        {
        //            #region 填充ControlPropertyAction控件的树。

        //            List<jg.Editor.Library.Control.ComboTree.TreeModel> _treemodellist = FillTreeModel(Globals.treeviewSource.ToList());
        //            propertyManage1.controlAction.TreeModelList = _treemodellist;

        //            #endregion
        //            timer.Stop();
        //        };

        //    timer.Start();
        //}

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            windowQuestion wq = new windowQuestion();
            wq.ShowDialog();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void btnHtml_Click(object sender, RoutedEventArgs e)
        {
            HTML5Class model = new HTML5Class();
            ImpHtml imp = new ImpHtml(model);

            //imp.eventsetSteageBrower += imp_eventsetSteageBrower;
            imp.ShowDialog();
        }

        #region 拖拽

        AdornerLayer mAdornerLayer = null;//拖拽内容的显示
        private int Count = 0;//用来作用拖拽数据的判定
        private bool IsUpdateOrAdd = false;
        TextBlock textBlock;//拖拽用的数据
        TreeViewItemInfo classinfo;//添加或拖拽用到的数据
        private void text_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox txtContent = (TextBox)sender;
            if (!IsUpdateOrAdd)
            {
                if (txtContent.Text.Trim() == "")
                {
                    MessageBox.Show("请输入目录名称！");
                }
                else
                {

                    if (txtContent.Tag.ToString() == new Guid().ToString())
                    {

                        classinfo.Title = txtContent.Text;
                        classinfo.IsTbVisabled = Visibility.Visible;
                        classinfo.IsTxtVisabled = Visibility.Hidden;
                        classinfo.ParentId = new Guid();
                        classinfo.Sort = Globals.treeviewSource.Count - 1;
                        Globals.treeviewSource[Globals.treeviewSource.Count - 1] = classinfo;

                    }
                    else
                    {
                        classinfo.Title = txtContent.Text;
                        classinfo.IsTbVisabled = Visibility.Visible;
                        classinfo.IsTxtVisabled = Visibility.Hidden;
                        AddTreeNode(classinfo.ParentId, Globals.treeviewSource, classinfo);

                    }
                }
            }
            else
            {
                classinfo.Title = txtContent.Text;
                classinfo.IsTbVisabled = Visibility.Visible;
                classinfo.IsTxtVisabled = Visibility.Hidden;
                SerachTemp(classinfo, Globals.treeviewSource);

            }
            InitTreeModelList();
        }
        //按下事件
        private void TextBlockEvent_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
                return;
            MouseDown(sender);
        }
        //抓取数据
        private void MouseDown(object sender)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
                return;
            textBlock = sender as TextBlock;
        }
        //BLOCK接收数据
        private void DropUpNode(object sender, DragEventArgs e)
        {
            TextBlock thistextBlock = sender as TextBlock;
            if (textBlock != null && textBlock != thistextBlock && e.Effects == DragDropEffects.Move)
            {

                DataItem dataItem = e.Data.GetData(typeof(DataItem)) as DataItem;
                TreeViewItemInfo sourceInfo = dataItem.Header as TreeViewItemInfo;
                TreeViewItemInfo targetInfo = thistextBlock.DataContext as TreeViewItemInfo;

                Point p = e.GetPosition(thistextBlock);
                if (sourceInfo.Id == targetInfo.ParentId)
                {
                    return;
                }

                TreeViewItemInfo treeViewModel = thistextBlock.DataContext as TreeViewItemInfo;

                if (p.Y < treeNodeValue && p.Y > 0)
                {
                    treeViewModel.isUpVis = Visibility.Visible;
                    treeViewModel.isDownVis = Visibility.Hidden;
                    if (sourceInfo.ParentId == sourceInfo.Id)
                    {
                        Globals.treeviewSource.Remove(sourceInfo);
                    }
                    else
                    {
                        RemoveTreeNode(sourceInfo, Globals.treeviewSource);
                    }

                    TreeViewItemInfoOrId treeViewitemmodel = GetSourcelist(targetInfo, Globals.treeviewSource);
                    Up(treeViewitemmodel, sourceInfo);
                    treeViewitemmodel.ResultList[treeViewitemmodel.Id] = sourceInfo;
                    return;
                }
                else if (p.Y > (thistextBlock.ActualHeight - treeNodeValue) && p.Y < thistextBlock.ActualHeight)
                {
                    treeViewModel.isUpVis = Visibility.Hidden;
                    treeViewModel.isDownVis = Visibility.Visible;
                    if (sourceInfo.ParentId == sourceInfo.Id)
                    {
                        Globals.treeviewSource.Remove(sourceInfo);
                    }
                    else
                    {
                        RemoveTreeNode(sourceInfo, Globals.treeviewSource);
                    }

                    TreeViewItemInfoOrId treeViewitemmodel = GetSourcelist(targetInfo, Globals.treeviewSource);
                    treeViewitemmodel.Id = treeViewitemmodel.Id + 1;
                    Down(treeViewitemmodel,sourceInfo);
                    treeViewitemmodel.ResultList[treeViewitemmodel.Id] = sourceInfo;
                    return;
                }


                if (sourceInfo.ParentId == targetInfo.Id)
                {
                    return;
                }
                if (IsMyChilren(targetInfo.Id, sourceInfo.Id, sourceInfo.Children))
                {
                    return;
                }

                if (sourceInfo.ParentId == sourceInfo.Id)
                {
                    Globals.treeviewSource.Remove(sourceInfo);
                }
                else
                {
                    RemoveTreeNode(sourceInfo, Globals.treeviewSource);
                }
                sourceInfo.ParentId = targetInfo.Id;

                AddTreeNode(targetInfo, Globals.treeviewSource, sourceInfo);

                treeViewModel.isUpVis = Visibility.Hidden;
                treeViewModel.isDownVis = Visibility.Hidden;
            }
        }


        public bool IsMyChilren(Guid tagetid, Guid sourid, ObservableCollection<TreeViewItemInfo> list)
        {
            bool b = false;

            foreach (var v in list)
            {
                if (v.ParentId == sourid && tagetid == v.Id)
                {
                    b = true;
                    break;
                }

                if (v.Children.Count > 0)
                {
                    b = IsMyChilren(tagetid, v.Id, v.Children);
                    if (b)
                    {
                        break;
                    }
                }
            }

            return b;

        }

        /// <summary>
        /// 上移
        /// </summary>
        /// <param name="treeViewitemmodel"></param>
        public void Up(TreeViewItemInfoOrId treeViewitemmodel, TreeViewItemInfo souceModel)
        {
            treeViewitemmodel.ResultList.Add(souceModel);
            TreeViewItemInfo model = null;
            TreeViewItemInfo modelAddOne = null;

            for (int i = treeViewitemmodel.Id; i < treeViewitemmodel.ResultList.Count; i++)
            {

                if (modelAddOne != null)
                {
                    model = modelAddOne;
                }
                else
                {

                    model = treeViewitemmodel.ResultList[i];
                }
                if ((i + 1) < treeViewitemmodel.ResultList.Count)
                {
                    modelAddOne = treeViewitemmodel.ResultList[i + 1];
                    treeViewitemmodel.ResultList[i + 1] = model;
                }

            }
        }


        /// <summary>
        /// 下移
        /// </summary>
        /// <param name="treeViewitemmodel"></param>
        public void Down(TreeViewItemInfoOrId treeViewitemmodel, TreeViewItemInfo souceModel)
        {


            treeViewitemmodel.ResultList.Add(souceModel);
            if (treeViewitemmodel.Id == treeViewitemmodel.ResultList.Count - 1)
            {
                return;
            }
            TreeViewItemInfo model = null;
            TreeViewItemInfo modelAddOne = null;

            for (int i = treeViewitemmodel.Id; i < treeViewitemmodel.ResultList.Count; i++)
            {

                if (modelAddOne != null)
                {
                    model = modelAddOne;
                }
                else
                {

                    model = treeViewitemmodel.ResultList[i];
                }
                if ((i + 1) < treeViewitemmodel.ResultList.Count)
                {
                    modelAddOne = treeViewitemmodel.ResultList[i + 1];
                    treeViewitemmodel.ResultList[i + 1] = model;
                }

            }

        }
        public class TreeViewItemInfoOrId
        {
            public int Id;
            public ObservableCollection<TreeViewItemInfo> ResultList = new ObservableCollection<TreeViewItemInfo>();
            public bool IsOk = false;
        }

        /// <summary>
        /// 递归获取源目录所在集合 
        /// </summary>
        /// <param name="SourceInfo">源目录</param>
        /// <param name="eachList">遍历的目录</param>
        /// <returns>TreeViewItemInfoOrId</returns>
        public TreeViewItemInfoOrId GetSourcelist(TreeViewItemInfo SourceInfo, ObservableCollection<TreeViewItemInfo> eachList)
        {
            TreeViewItemInfoOrId Result = new TreeViewItemInfoOrId();
            foreach (var item in eachList)
            {
                if (item.Id == SourceInfo.Id)
                {
                    Result.Id = eachList.IndexOf(item);
                    Result.ResultList = eachList;
                    Result.IsOk = true;
                    return Result;
                }
                if (item.Children != null && item.Children.Count > 0)
                {
                    Result = GetSourcelist(SourceInfo, item.Children);
                    if (Result.IsOk)
                    {
                        return Result;

                    }
                }

            }

            return Result;
        }
        //拖拽更新树节点（添加）
        private void AddTreeNode(TreeViewItemInfo parm, ObservableCollection<TreeViewItemInfo> list, TreeViewItemInfo AddNodeValue)
        {

            foreach (TreeViewItemInfo model in list)
            {
                if (model.Id == parm.Id)
                {
                    model.Children.Add(AddNodeValue);
                    break;
                }
                AddTreeNode(parm, model.Children, AddNodeValue);
            }

        }

        private void AddTreeNode(Guid parm, ObservableCollection<TreeViewItemInfo> list, TreeViewItemInfo AddNodeValue)
        {

            foreach (TreeViewItemInfo model in list)
            {
                if (model.Id == parm)
                {
                    AddNodeValue.Sort = model.Children.Count - 1;
                    model.Children[model.Children.Count - 1] = AddNodeValue;
                    break;
                }
                AddTreeNode(parm, model.Children, AddNodeValue);
            }

        }
        //拖拽更新树节点（删除）
        private void RemoveTreeNode(TreeViewItemInfo parm, ObservableCollection<TreeViewItemInfo> list)
        {
            foreach (TreeViewItemInfo model in list)
            {
                if (model.Id == parm.Id)
                {
                    list.Remove(parm);

                    break;
                }
                RemoveTreeNode(parm, model.Children);
            }
        }
        //拖进树容器中
        private void DropTreeView(object sender, DragEventArgs e)
        {
            TreeView thistreeView = sender as TreeView;
            if (textBlock != null && e.Effects == DragDropEffects.Move)
            {
                DataItem dataItem = e.Data.GetData(typeof(DataItem)) as DataItem;
                TreeViewItemInfo sourceInfo = dataItem.Header as TreeViewItemInfo;
            
                RemoveTreeNode(sourceInfo, Globals.treeviewSource);

                Globals.treeviewSource.Add(sourceInfo);
            }
        }
        //上下移动操作
        private void DropVisAVisNode(object sender, DragEventArgs e)
        {

        }
        /// <summary>
        /// 权重值
        /// </summary>
        private int treeNodeValue = 5;
        //鼠标滑动事件

        private void TextBlock_MouseMove(object sender, MouseEventArgs e)
        {

            if (Mouse.LeftButton != MouseButtonState.Pressed)
            {

                return;
            }

            if (textBlock == null)
            { return; }

            if ((sender as TextBlock) != textBlock) { return; }
            Border b = new Border();
            b.BorderBrush = new SolidColorBrush(Colors.Black);
            b.Background = new SolidColorBrush(Colors.BurlyWood);
            textBlock.Foreground = new SolidColorBrush(Colors.Black);

            DragDropAdorner adorner = new DragDropAdorner(textBlock, 12, 12); // Window class do not have AdornerLayer
            UIElement element = new UIElement();
            mAdornerLayer = AdornerLayer.GetAdornerLayer(treeViewControl1);
            mAdornerLayer.Add(adorner);
            DataItem item = new DataItem(textBlock.DataContext);
            DataObject dataObject = new DataObject(item.Clone());
            DragDrop.DoDragDrop(textBlock, dataObject, DragDropEffects.Move);
            mAdornerLayer.Remove(adorner);
        }
        //案按键改变时
        private void TextBlock_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (sender as TextBlock != null)
                mAdornerLayer.Update();
        }
        //鼠标按起时
        void TextBlock_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            textBlock = null;
        }
        //接受拖拽事件
        private void PubDropTreeAndBlockEvent(object sender, DragEventArgs e)
        {



            IsDropAction(sender, e);
        }
        //调用的拖拽放方法
        private void IsDropAction(object sender, DragEventArgs e)
        {
            TextBlock isTexkBlock = sender as TextBlock;
            if (Count == 0)
            {
                if (isTexkBlock != null)
                {


                    DropUpNode(sender, e); Count++;
                }
                else { DropTreeView(sender, e); }
            }
            else
            {
                Count = 0;
            }
        }


        //数据内容类
        class DataItem : ICloneable
        {
            public DataItem(object data)
            {
                mHeader = data;
            }

            public object Clone()
            {
                DataItem dataItem = new DataItem(mHeader);

                return dataItem;
            }

            public object Header
            {
                get { return mHeader; }
            }


            private object mHeader = null;

        }
        //集成对象类
        class SingleDataObject
        {
            public DataItem[] dataObject;
            public SingleDataObject(DataItem[] dataObjectparm)
            {
                dataObject = dataObjectparm;
            }
        }


        //鼠标右键
        private void RightMouseButtonClick()
        {

        }

        private void GridAdd_Click(object sender, RoutedEventArgs e)
        {
            SetTxtVisabledTrue(Globals.treeviewSource);
            MenuItem menuitem = sender as MenuItem;
            var selectedTVI = (TreeViewItemInfo)menuitem.Tag;
            if (selectedTVI != null)
            {
                IsUpdateOrAdd = false;
                classinfo = new TreeViewItemInfo();
                classinfo.IsTbVisabled = Visibility.Hidden;
                classinfo.IsTxtVisabled = Visibility.Visible;
                classinfo.Title = "目录";
                classinfo.IsEdit = true;
                classinfo.ParentId = selectedTVI.Id;
                classinfo.CollectionChanged += Source_CollectionChanged;
                selectedTVI.Children.Add(classinfo);
            }

            RefreshStage();
        }


        private bool SetTxtVisabledTrue(ObservableCollection<TreeViewItemInfo> list)
        {

            foreach (var item in list)
            {
                if (item.IsTxtVisabled == Visibility.Visible)
                {
                    TextBox txtContent = new TextBox();
                    txtContent.Text = item.Title;
                    txtContent.Tag = item.ParentId;
                    text_LostFocus(txtContent, null);
                    return true;
                }
                if (SetTxtVisabledTrue(item.Children))
                {
                    return true;
                }

            }
            return false;
        }
        private void GridDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (treeViewControl1.SelectedItem == null) return;
                var info = treeViewControl1.SelectedItem as TreeViewItemInfo;
                if (info == null) return;

                RemovePageItem(info.Id);
                InitTreeModelList();
            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }


        }

        private void GridUpdate_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuitem = sender as MenuItem;
            var selectedTVI = (TreeViewItemInfo)menuitem.Tag;
            selectedTVI.IsTbVisabled = Visibility.Hidden;
            selectedTVI.IsTxtVisabled = Visibility.Visible;

            classinfo = selectedTVI;
            classinfo.IsEdit = true;
            SerachTemp(classinfo, Globals.treeviewSource);
            IsUpdateOrAdd = true;
        }

        private void treeAdd_Click(object sender, RoutedEventArgs e)
        {
            SetTxtVisabledTrue(Globals.treeviewSource);
            classinfo = new TreeViewItemInfo();
            classinfo.IsTbVisabled = Visibility.Hidden;
            classinfo.IsTxtVisabled = Visibility.Visible;
            classinfo.IsEdit = true;
            classinfo.CollectionChanged += Source_CollectionChanged;
            Globals.treeviewSource.Add(classinfo);
        }

        private void SerachTemp(TreeViewItemInfo parm, ObservableCollection<TreeViewItemInfo> list)
        {
            foreach (TreeViewItemInfo model in list)
            {
                if (model.Id == parm.Id)
                {
                    list[list.IndexOf(model)] = parm;
                    break;

                }
                SerachTemp(parm, model.Children);
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                text_LostFocus(sender, null);
            }
        }
        private void TextBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            text_LostFocus(sender, null);
        }
        #endregion

        #region 另存为


        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            Globals.CourseWareGuid = Guid.NewGuid();
            try
            {
                Publish();
                windowSave save = new windowSave() { DataFileList = DataFileList, AssetFileList = AssetFileList, PageCount = Globals.savePageList.Count };
                save.ShowDialog();
            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        #endregion


        private void btnClone_Click(object sender, RoutedEventArgs e)
        {
            Globals.CourseWareGuid = Guid.NewGuid();
            try
            {
                Publish();
                windowSave save = new windowSave() { DataFileList = DataFileList, AssetFileList = AssetFileList, PageCount = Globals.savePageList.Count };
                save.ShowDialog();
            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }

        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            try
            {
                Publish();
                windowSave save = new windowSave() { DataFileList = DataFileList, AssetFileList = AssetFileList, PageCount = Globals.savePageList.Count };
                save.ShowDialog();
            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private void RibbonWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                Preview_Click(null, null);
            }
        }
        private void GridMainAdd_Click(object sender, RoutedEventArgs e)
        {
            SetTxtVisabledTrue(Globals.treeviewSource);
            TreeViewItemInfo info = treeViewControl1.SelectedItem as TreeViewItemInfo;
            TreeViewItemInfo addInfo;
            TreeViewItemInfo parentInfo = null;
            MenuItem menuItem = sender as MenuItem;
            if (menuItem == null) return;
            addInfo = new TreeViewItemInfo();
            if (info != null)
                parentInfo = GetParentInfo(info, Globals.treeviewSource);

            if (parentInfo != null)
                addInfo.ParentId = parentInfo.Id;
            else
                IsUpdateOrAdd = false;
            addInfo.Id = Guid.NewGuid();
            addInfo.IsEdit = true;
            addInfo.IsTxtVisabled = Visibility.Visible;
            addInfo.IsTbVisabled = Visibility.Collapsed;
            addInfo.Title = "目录";

            if (parentInfo == null)
                Globals.treeviewSource.Add(addInfo);
            else
                parentInfo.Children.Add(addInfo);
            classinfo = addInfo;
            addInfo.CollectionChanged += Source_CollectionChanged;
            RefreshStage();

        }
        private void RibbonWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(treeViewControl1);
            bool b = treeViewControl1.IsMouseOver;
            if (!b)
            {
                SetTxtVisabledTrue(Globals.treeviewSource);
            }
        }
        
        #region Slider 实现舞台缩放

        Point? lastCenterPositionOnTarget;
        Point? lastMousePositionOnTarget;
        Point? lastDragPoint;
        /// <summary>
        /// 初始化事件
        /// </summary>
        public void setSliderAndSrollEvent()
        {
            scrollViewer.ScrollChanged += OnScrollViewerScrollChanged;
            //scrollViewer.MouseLeftButtonUp += OnMouseLeftButtonUp;
            //scrollViewer.PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
            scrollViewer.PreviewMouseWheel += OnPreviewMouseWheel;

            //scrollViewer.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
            //scrollViewer.MouseMove += OnMouseMove;

            zoomSlider.ValueChanged += Slider_ValueChanged;
        }



        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            scaleTransform.ScaleX = e.NewValue / 100.0;
            scaleTransform.ScaleY = e.NewValue / 100.0;

            var centerOfViewport = new Point(scrollViewer.ViewportWidth / 2, scrollViewer.ViewportHeight / 2);
            lastCenterPositionOnTarget = scrollViewer.TranslatePoint(centerOfViewport, grid);

        }
        void OnMouseMove(object sender, MouseEventArgs e)
        {
            //if (lastDragPoint.HasValue)
            //{
            //    Point posNow = e.GetPosition(scrollViewer);

            //    double dX = posNow.X - lastDragPoint.Value.X;
            //    double dY = posNow.Y - lastDragPoint.Value.Y;

            //    lastDragPoint = posNow;

            //    scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - dX);
            //    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - dY);
            //}
        }

        void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //var mousePos = e.GetPosition(scrollViewer);
            //if (mousePos.X <= scrollViewer.ViewportWidth && mousePos.Y < scrollViewer.ViewportHeight) //make sure we still can use the scrollbars
            //{
            //    scrollViewer.Cursor = Cursors.SizeAll;
            //    lastDragPoint = mousePos;
            //    Mouse.Capture(scrollViewer);
            //}
        }

        void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            lastMousePositionOnTarget = Mouse.GetPosition(grid);
            KeyStates key = Keyboard.GetKeyStates(Key.LeftAlt);
            if (KeyStates.Down == key)
            {
                if (e.Delta > 0)
                {
                    zoomSlider.Value += 1;
                }
                if (e.Delta < 0)
                {
                    zoomSlider.Value -= 1;
                }

                e.Handled = true;
            }
        }

        void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //scrollViewer.Cursor = Cursors.Arrow;
            //scrollViewer.ReleaseMouseCapture();
            //lastDragPoint = null;
        }

        void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            scaleTransform.ScaleX = e.NewValue;
            scaleTransform.ScaleY = e.NewValue;

            var centerOfViewport = new Point(scrollViewer.ViewportWidth / 2, scrollViewer.ViewportHeight / 2);
            lastCenterPositionOnTarget = scrollViewer.TranslatePoint(centerOfViewport, grid);
        }

        void OnScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange != 0 || e.ExtentWidthChange != 0)
            {
                Point? targetBefore = null;
                Point? targetNow = null;

                if (!lastMousePositionOnTarget.HasValue)
                {
                    if (lastCenterPositionOnTarget.HasValue)
                    {
                        var centerOfViewport = new Point(scrollViewer.ViewportWidth / 2, scrollViewer.ViewportHeight / 2);
                        Point centerOfTargetNow = scrollViewer.TranslatePoint(centerOfViewport, grid);

                        targetBefore = lastCenterPositionOnTarget;
                        targetNow = centerOfTargetNow;
                    }
                }
                else
                {
                    targetBefore = lastMousePositionOnTarget;
                    targetNow = Mouse.GetPosition(grid);

                    lastMousePositionOnTarget = null;
                }

                if (targetBefore.HasValue)
                {
                    double dXInTargetPixels = targetNow.Value.X - targetBefore.Value.X;
                    double dYInTargetPixels = targetNow.Value.Y - targetBefore.Value.Y;

                    double multiplicatorX = e.ExtentWidth / grid.Width;
                    double multiplicatorY = e.ExtentHeight / grid.Height;

                    double newOffsetX = scrollViewer.HorizontalOffset - dXInTargetPixels * multiplicatorX;
                    double newOffsetY = scrollViewer.VerticalOffset - dYInTargetPixels * multiplicatorY;

                    if (double.IsNaN(newOffsetX) || double.IsNaN(newOffsetY))
                    {
                        return;
                    }

                    scrollViewer.ScrollToHorizontalOffset(newOffsetX);
                    scrollViewer.ScrollToVerticalOffset(newOffsetY);
                }
            }
        }
        #endregion

        #region 本地化

        /// <summary>
        /// 本地资源打开
        /// </summary>
        /// <param name="ToolName">资源key</param>
        private void LocalShowWin(string key)
        {
            FileInfo AssetInfo = null;
            FileInfo ThumbnailsInfo = null;
            System.IO.DirectoryInfo itemsDis = null;
            LocalRes localRes = null;
            HTML5Class model = null;
            if (propertyManage1.Stage == null) return;

            try
            {
                System.Windows.Forms.OpenFileDialog ofDialog = new System.Windows.Forms.OpenFileDialog();
                //根据素材类型，显示所选择的素材明细
                switch (key)
                {
                    case "Sound":
                        ofDialog.Filter = Properties.Resources.ExtensionServerSoundFilter;
                        break;
                    case "Movie":
                        ofDialog.Filter = Properties.Resources.ExtensionServerMovieFilter;
                        break;
                    case "Image":
                        ofDialog.Filter = Properties.Resources.ExtensionServerImgFilter;
                        break;
                    case "Document":
                        ofDialog.Filter = Properties.Resources.ExtensionServerDocumentFilter;
                        break;
                }


                if (ofDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    AssetInfo = new FileInfo(ofDialog.FileName);

                    switch (key)
                    {
                        case "Sound":
                            ThumbnailsInfo = new FileInfo(Globals.appStartupPath + "\\Image\\Thumbnails_Sound.png");
                            break;
                        case "Movie":
                            ThumbnailsInfo = new FileInfo(Globals.appStartupPath + "\\Image\\Thumbnails_Movie.png");
                            break;
                        case "Image":
                            ThumbnailsInfo = new FileInfo(ofDialog.FileName);
                            break;
                        case "Document":
                            ThumbnailsInfo = new FileInfo(Globals.appStartupPath + "\\Image\\Thumbnails_Document.png");
                            break;
                    }
                    
                    if (string.IsNullOrEmpty(AssetInfo.FullName) || string.IsNullOrEmpty(ThumbnailsInfo.FullName))
                        return;
                }

                propertyManage1.Stage.AddItem(key, AssetInfo == null ? "" : AssetInfo.FullName, ThumbnailsInfo == null ? "" : ThumbnailsInfo.FullName, itemsDis == null ? "" : itemsDis.FullName, false);
            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message + "\r\n" + ex.StackTrace);

            }
        }

        /// <summary>
        /// 初始化绑定本地还是资源的数据
        /// </summary>
        /// <param name="model">本地还是资源中心的数据 enum</param>
        private void SetVerEditorBingAction(enumVerEditor model)
        {
            DictionaryResource = new Dictionary<string, ActionGetResource>();
            SetControlVisFuntion(model);
        }

        /// <summary>
        /// 初始化绑定本地还是资源的数据
        /// </summary>
        /// <param name="model">本地还是资源中心的数据 enum</param>
        private void SetVerEditorBingAction()
        {
            DictionaryResource = new Dictionary<string, ActionGetResource>();
            SetControlVisFuntion();
        }


        //private void setVisAction(enumVerEditor model)
        //{
        //    switch (model)
        //    {
        //        case enumVerEditor.Local:

        //            break;
        //        case enumVerEditor.Resources:

        //            break;
        //        default:
        //            break;
        //    }
        //}
        /// <summary>
        /// 设置按钮事件
        /// </summary>
        /// <param name="enummodel">本地还是资源中心的数据  enum</param>
        private void SetControlVisFuntion(enumVerEditor enummodel)
        {
            IEnumerable<Fluent.SplitButton> IEnumerableSplitButton = ResourcesRibbonGroupBox.Items.OfType<Fluent.SplitButton>();
            foreach (var v in IEnumerableSplitButton)
            {
                IEnumerable<Fluent.MenuItem> IEnumerableMenuItem = v.Items.OfType<Fluent.MenuItem>();
                string key = Fluent.KeyTip.GetKeys(v);
                Fluent.MenuItem model = IEnumerableMenuItem.FirstOrDefault(p => (Fluent.KeyTip.GetKeys(p) == key && p.Tag.ToString() == enummodel.ToString()));

                model.IsSplited = true;
                model.IsChecked = true;
                model.Visibility = Visibility.Visible;
                model.Click += model_Click;

                SetActionDictionaryResource(SetActionDictionaryResource(enummodel.ToString(), key), key);

                v.Click += v_Click;
            }

        }


        /// <summary>
        /// 当要显示全部的时候
        /// </summary>
        private void SetControlVisFuntion()
        {
            IEnumerable<Fluent.SplitButton> IEnumerableSplitButton = ResourcesRibbonGroupBox.Items.OfType<Fluent.SplitButton>();
            foreach (var v in IEnumerableSplitButton)
            {


                IEnumerable<Fluent.MenuItem> IEnumerableMenuItem = v.Items.OfType<Fluent.MenuItem>();


                string key = Fluent.KeyTip.GetKeys(v);


                IEnumerable<Fluent.MenuItem> modelList = IEnumerableMenuItem.Where(p => Fluent.KeyTip.GetKeys(p) == key);

                foreach (var Item in modelList)
                {
                    Item.Visibility = Visibility.Visible;
                    if (Item.IsSplited)
                    {
                        Item.IsChecked = true;
                        Item.Click += model_Click;
                        SetActionDictionaryResource(SetActionDictionaryResource(Item.Tag.ToString(), key), key);
                        v.Click += v_Click;
                    }
                }
            }

        }
        /// <summary>
        /// 设置事件库
        /// </summary>
        /// <param name="actionModel">事件</param>
        /// <param name="Key">主键</param>

        void SetActionDictionaryResource(ActionGetResource actionModel, string Key)
        {
            bool IsContains = false;
            if (DictionaryResource.Keys.Count > 0)
            {
                if (DictionaryResource.Keys.Contains(Key))
                {
                    IsContains = true;
                    DictionaryResource[Key] = actionModel;
                }
            }
            if (!IsContains)
            {
                DictionaryResource.Add(Key, actionModel);
            }

        }
        void v_Click(object sender, RoutedEventArgs e)
        {
            Fluent.SplitButton p = sender as Fluent.SplitButton;

            string key = Fluent.KeyTip.GetKeys(p);

            if (DictionaryResource.Keys.Count > 0)
            {
                DictionaryResource[key](key);
            }
        }


        /// <summary>
        /// 事件库
        /// </summary>
        Dictionary<string, ActionGetResource> DictionaryResource = new Dictionary<string, ActionGetResource>();
        void model_Click(object sender, RoutedEventArgs e)
        {


            Fluent.MenuItem p = sender as Fluent.MenuItem;
            if (p.Tag.ToString() == "Local")
            {
                string key = Fluent.KeyTip.GetKeys(p);
                SetActionDictionaryResource(SetActionDictionaryResource(p.Tag.ToString(), key), key);
                LocalShowWin(key);
            }
        }

        /// <summary>
        /// 加载事件
        /// </summary>
        /// <param name="p"> 区分本地事件 还是资源中心</param>
        /// <param name="Key">主键</param>
        /// <returns>ActionGetResource</returns>
        ActionGetResource SetActionDictionaryResource(string p, string Key)
        {
            ActionGetResource model = null;

            if (p == "Local")
            {
                model = new ActionGetResource(LocalShowWin);

            }

           
            return model;
        }

        private void SplitButton_Click(object sender, RoutedEventArgs e)
        {
            Fluent.SplitButton split = sender as Fluent.SplitButton;
            if (split != null)
            {
                //ToolboxItem_AddAsset(split.Tag.ToString());
            }


        }

        #endregion
    }

    public enum enumVerEditor
    {
        /// <summary>
        /// 本地
        /// </summary>
        Local,
        /// <summary>
        /// 资源中心
        /// </summary>
        Resources
    }
}