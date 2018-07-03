
namespace jg.Editor.Library.Property
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.ComponentModel;

    public class PropertyManage : StackPanel
    {
        public new delegate void OnPropertyChanged(ICommand command);
        public static event OnPropertyChanged PropertyChanged = null;
        public delegate AssResInfo AddAssResInfoTpage();
        public  event AddAssResInfoTpage eventAddAssResInfoTpage = null;
        Expander
            expanderStage = new Expander() { Tag = AssetProperty.Stage },
            expanderFont = new Expander() { Tag = AssetProperty.Font },
            expanderLocation = new Expander() { Tag = AssetProperty.Location },
            expanderTransform = new Expander() { Tag = AssetProperty.Transform },
            expanderbackColorCanvas = new Expander() { Tag = AssetProperty.BackColor },
            expanderforeColorCanvas = new Expander() { Tag = AssetProperty.ForeColor },
            expanderAction = new Expander() { Tag = AssetProperty.Action },
            expanderTextGrid = new Expander() { Tag = AssetProperty.TextGrid },
            expanderTopic = new Expander() { Tag = AssetProperty.Topic },
            expanderTopicDrag = new Expander() { Tag = AssetProperty.TopicDrag },
            expanderLine = new Expander() { Tag = AssetProperty.Line },
            expanderTpage=new Expander(){Tag=AssetProperty.TPageGroup}
            ;

        public ControlPropertyStage controlStage = new ControlPropertyStage();
        public ControlPropertyFont controlFont = new ControlPropertyFont();
        public ControlPropertyLocation controlLocation = new ControlPropertyLocation();
        public ControlPropertyTransform controlTransform = new ControlPropertyTransform();
        public ControlPropertyAction controlAction = new ControlPropertyAction();
        public ControlPropertyTextGrid controlTextGrid = new ControlPropertyTextGrid();
        public ControlPropertyTopic controlTopic = new ControlPropertyTopic();
        public ControlPropertyTopicDrag controlTopicDrag = new ControlPropertyTopicDrag();
        public ControlPropertyLine controlLine = new ControlPropertyLine();
        public ControlPropertyTPage controlTpage = new ControlPropertyTPage();
       public  Xceed.Wpf.Toolkit.ColorCanvas backColorCanvas = new Xceed.Wpf.Toolkit.ColorCanvas();
       public ControlPropertyground foreColorCanvas = new ControlPropertyground();

        public PropertyManage()
        {
            controlStage.PropertyBackgroundChanged += new RoutedPropertyChangedEventHandler<Color>(controlStage_PropertyBackgroundChanged);
            controlStage.PropertyBackimageChanged += new RoutedPropertyChangedEventHandler<string>(controlStage_PropertyBackimageChanged);
            controlStage.PropertyHeightChanged += new RoutedPropertyChangedEventHandler<double>(controlStage_PropertyHeightChanged);
            controlStage.PropertyWidthChanged += new RoutedPropertyChangedEventHandler<double>(controlStage_PropertyWidthChanged);

            controlFont.PropertyFontFamilyChanged += new RoutedPropertyChangedEventHandler<FontFamily>(controlFont_PropertyFontFamilyChanged);
            controlFont.PropertyFontSizeChanged += new RoutedPropertyChangedEventHandler<double>(controlFont_PropertyFontSizeChanged);
            controlFont.PropertyFontBoldChanged += new RoutedPropertyChangedEventHandler<bool>(controlFont_PropertyFontBoldChanged);
            controlFont.PropertyFontItalicChanged += new RoutedPropertyChangedEventHandler<bool>(controlFont_PropertyFontItalicChanged);

            controlLocation.PropertyHeightChanged += new RoutedPropertyChangedEventHandler<double>(controlLocation_PropertyHeightChanged);
            controlLocation.PropertyWidthChanged += new RoutedPropertyChangedEventHandler<double>(controlLocation_PropertyWidthChanged);
            controlLocation.PropertyXChanged += new RoutedPropertyChangedEventHandler<double>(controlLocation_PropertyXChanged);
            controlLocation.PropertyYChanged += new RoutedPropertyChangedEventHandler<double>(controlLocation_PropertyYChanged);
            controlLocation.PropertyItemNameChanged += new RoutedPropertyChangedEventHandler<string>(controlLocation_PropertyItemNameChanged);
            controlLocation.PropertyOpacityChanged += new RoutedPropertyChangedEventHandler<double>(controlLocation_PropertyOpacityChanged);

            controlTransform.PropertyRotateChanged += new RoutedPropertyChangedEventHandler<double>(controlTransform_PropertyRotateChanged);
            controlTransform.PropertyScaleXChanged += new RoutedPropertyChangedEventHandler<double>(controlTransform_PropertyScaleXChanged);
            controlTransform.PropertyScaleYChanged += new RoutedPropertyChangedEventHandler<double>(controlTransform_PropertyScaleYChanged);
            controlTransform.PropertySkewXChanged += new RoutedPropertyChangedEventHandler<double>(controlTransform_PropertySkewXChanged);
            controlTransform.PropertySkewYChanged += new RoutedPropertyChangedEventHandler<double>(controlTransform_PropertySkewYChanged);
            controlTransform.PropertyTranslateXChanged += new RoutedPropertyChangedEventHandler<double>(controlTransform_PropertyTranslateXChanged);
            controlTransform.PropertyTranslateYChanged += new RoutedPropertyChangedEventHandler<double>(controlTransform_PropertyTranslateYChanged);

            backColorCanvas.SelectedColorChanged += new RoutedPropertyChangedEventHandler<Color>(backColorCanvas_SelectedColorChanged);
            foreColorCanvas.selectCor.SelectedColorChanged += new RoutedPropertyChangedEventHandler<Color>(foreColorCanvas_SelectedColorChanged);

            controlTextGrid.PropertyGridColumnChanged += new RoutedPropertyChangedEventHandler<int>(controlTextGrid_PropertyGridColumnChanged);
            controlTextGrid.PropertyGridRowChanged += new RoutedPropertyChangedEventHandler<int>(controlTextGrid_PropertyGridRowChanged);
            controlTextGrid.PropertyGridBorderWidthChanged += new RoutedPropertyChangedEventHandler<int>(controlTextGrid_PropertyGridBorderWidthChanged);
            controlTpage.AddClick += controlTpage_AddClick;
           



            expanderStage.Content = controlStage;
            expanderFont.Content = controlFont;
            expanderLocation.Content = controlLocation;
            expanderTransform.Content = controlTransform;
            expanderbackColorCanvas.Content = backColorCanvas;
            expanderforeColorCanvas.Content = foreColorCanvas;
            expanderAction.Content = controlAction;
            expanderTextGrid.Content = controlTextGrid;
            expanderTopic.Content = controlTopic;
            expanderTopicDrag.Content = controlTopicDrag;
            expanderLine.Content = controlLine;
            expanderTpage.Content = controlTpage;
            expanderStage.Header = FindResource("FF000006").ToString();            
            expanderStage.IsExpanded = true;

            expanderFont.Header = FindResource("FF000007").ToString();
            expanderLocation.Header = FindResource("FF000008").ToString();
            expanderTransform.Header = FindResource("FF000009").ToString();
            expanderbackColorCanvas.Header = FindResource("FF00000A").ToString();
            expanderforeColorCanvas.Header = FindResource("FF00000B").ToString();
            expanderAction.Header = FindResource("FF00000C").ToString();
            expanderTextGrid.Header = FindResource("FF000034").ToString();
            expanderTopic.Header = FindResource("FF00004D").ToString();
            expanderTopicDrag.Header = FindResource("FF00005A").ToString();
            expanderLine.Header = FindResource("FF00007C").ToString();
            expanderTpage.Header = FindResource("FF000083").ToString();

            expanderFont.Margin = new Thickness(0, 1, 0, 0);
            expanderLocation.Margin = new Thickness(0, 1, 0, 0);
            expanderTransform.Margin = new Thickness(0, 1, 0, 0);
            expanderbackColorCanvas.Margin = new Thickness(0, 1, 0, 0);
            expanderforeColorCanvas.Margin = new Thickness(0, 1, 0, 0);
            expanderTextGrid.Margin = new Thickness(0, 1, 0, 0);
            expanderTopic.Margin = new Thickness(0, 1, 0, 0);
            expanderTopicDrag.Margin = new Thickness(0, 1, 0, 0);
            expanderStage.Margin = new Thickness(0, 1, 0, 0);
            expanderAction.Margin = new Thickness(0, 1, 0, 0);
            expanderLine.Margin = new Thickness(0, 1, 0, 0);
            expanderTpage.Margin = new Thickness(0, 1, 0, 0);

            expanderFont.Visibility = System.Windows.Visibility.Collapsed;
            expanderLocation.Visibility = System.Windows.Visibility.Collapsed;
            expanderTransform.Visibility = System.Windows.Visibility.Collapsed;
            expanderbackColorCanvas.Visibility = System.Windows.Visibility.Collapsed;
            expanderforeColorCanvas.Visibility = System.Windows.Visibility.Collapsed;
            expanderTextGrid.Visibility = System.Windows.Visibility.Collapsed;
            expanderTopic.Visibility = System.Windows.Visibility.Collapsed;
            expanderTopicDrag.Visibility = System.Windows.Visibility.Collapsed;
            expanderStage.Visibility = System.Windows.Visibility.Collapsed;
            expanderAction.Visibility = System.Windows.Visibility.Collapsed;
            expanderLine.Visibility = System.Windows.Visibility.Collapsed;
            expanderTpage.Visibility = System.Windows.Visibility.Collapsed;

            Children.Add(expanderStage);
            Children.Add(expanderFont);
            Children.Add(expanderLocation);
            Children.Add(expanderTransform);
            Children.Add(expanderforeColorCanvas);
            Children.Add(expanderbackColorCanvas);
            Children.Add(expanderAction);
            Children.Add(expanderTextGrid);
            Children.Add(expanderTopic);
            Children.Add(expanderTopicDrag);
            Children.Add(expanderLine);
            Children.Add(expanderTpage);
        }


        public void InitParMannger()
        {
            controlFont.txtFontSize.Text = "18";
            controlStage.txtWidth.Text = "1024";
            controlStage.txtHeight.Text = "768";
        }
        /// <summary>
        /// 返回资源
        /// </summary>
        /// <returns></returns>
        AssResInfo controlTpage_AddClick()
        {
          
            if(eventAddAssResInfoTpage!=null)
            {
                return eventAddAssResInfoTpage();
            }
            return null;
          
        }
        void controlTextGrid_PropertyGridBorderWidthChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            
        }
        void controlTextGrid_PropertyGridRowChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            //throw new NotImplementedException();
        }
        void controlTextGrid_PropertyGridColumnChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            //throw new NotImplementedException();
        }
        void controlLocation_PropertyOpacityChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DesignerItem item = sender as DesignerItem;
            PropertyOpacityCommand command;
            if (PropertyChanged != null && item != null)
            {
                command = new PropertyOpacityCommand(item);
                command.Execute(e.OldValue, e.NewValue);
                PropertyChanged(command);
            }
        }
        void controlLocation_PropertyItemNameChanged(object sender, RoutedPropertyChangedEventArgs<string> e)
        {
            
        }
        void backColorCanvas_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            foreach (var v in Source)
            {
                switch (((ToolboxItem)v.Content).Content.GetType().Name)
                {
                    case "ControlTextEditor":
                        ((jg.Editor.Library.Control.ControlTextEditor)((ToolboxItem)v.Content).Content).Background = new SolidColorBrush(e.NewValue);
                        ((jg.Editor.Library.Control.ControlTextEditor)((ToolboxItem)v.Content).Content).mainRTB.Document.Background = new SolidColorBrush(e.NewValue);
                        break;
                    default:
                        ((ToolboxItem)v.Content).ItemBackground = new SolidColorBrush(e.NewValue);
                        break;
                }
                
            }
        }
        void foreColorCanvas_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            foreach (var v in Source)
                ((ToolboxItem)v.Content).ItemForeground = new SolidColorBrush(e.NewValue);
        }
        void controlStage_PropertyWidthChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
        }
        void controlStage_PropertyHeightChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
        }
        void controlStage_PropertyBackimageChanged(object sender, RoutedPropertyChangedEventArgs<string> e)
        {
            
        }
        void controlStage_PropertyBackgroundChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            
        }
        void controlTransform_PropertyTranslateYChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DesignerItem item = sender as DesignerItem;
            PropertyTranslateYCommand command;
            if (PropertyChanged != null && item != null)
            {
                command = new PropertyTranslateYCommand(item);
                command.Execute(e.OldValue, e.NewValue);
                PropertyChanged(command);
            }
        }
        void controlTransform_PropertyTranslateXChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DesignerItem item = sender as DesignerItem;
            PropertyTranslateXCommand command;
            if (PropertyChanged != null && item != null)
            {
                command = new PropertyTranslateXCommand(item);
                command.Execute(e.OldValue, e.NewValue);
                PropertyChanged(command);
            }
        }
        void controlTransform_PropertySkewYChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DesignerItem item = sender as DesignerItem;
            PropertySkewYCommand command;
            if (PropertyChanged != null && item != null)
            {
                command = new PropertySkewYCommand(item);
                command.Execute(e.OldValue, e.NewValue);
                PropertyChanged(command);
            }
        }
        void controlTransform_PropertySkewXChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DesignerItem item = sender as DesignerItem;
            PropertySkewXCommand command;
            if (PropertyChanged != null && item != null)
            {
                command = new PropertySkewXCommand(item);
                command.Execute(e.OldValue, e.NewValue);
                PropertyChanged(command);
            }
        }
        void controlTransform_PropertyScaleYChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DesignerItem item = sender as DesignerItem;
            PropertyScaleYCommand command;
            if (PropertyChanged != null && item != null)
            {
                command = new PropertyScaleYCommand(item);
                command.Execute(e.OldValue, e.NewValue);
                PropertyChanged(command);
            }
        }
        void controlTransform_PropertyScaleXChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DesignerItem item = sender as DesignerItem;
            PropertyScaleXCommand command;
            if (PropertyChanged != null && item != null)
            {
                command = new PropertyScaleXCommand(item);
                command.Execute(e.OldValue, e.NewValue);
                PropertyChanged(command);
            }
        }
        void controlTransform_PropertyRotateChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DesignerItem item = sender as DesignerItem;
            PropertyRotateCommand command;
            if (PropertyChanged != null && item != null)
            {
                command = new PropertyRotateCommand(item);
                command.Execute(e.OldValue, e.NewValue);
                PropertyChanged(command);
            }
        }
        void controlLocation_PropertyHeightChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DesignerItem item = sender as DesignerItem;
            PropertyHeightCommand command;
            if (PropertyChanged != null && item != null)
            {
                command = new PropertyHeightCommand(item);
                command.Execute(e.OldValue, e.NewValue);
                PropertyChanged(command);
            }
        } 
        void controlLocation_PropertyWidthChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DesignerItem item = sender as DesignerItem;
            PropertyWidthCommand command;
            if (PropertyChanged != null && item != null)
            {
                command = new PropertyWidthCommand(item);
                command.Execute(e.OldValue, e.NewValue);
                PropertyChanged(command);
            }
        }
        void controlLocation_PropertyXChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DesignerItem item = sender as DesignerItem;
            PropertyXCommand command;
            if (PropertyChanged != null && item != null)
            {
                command = new PropertyXCommand(item);
                command.Execute(e.OldValue, e.NewValue);
                PropertyChanged(command);
            }
        }
        void controlLocation_PropertyYChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DesignerItem item = sender as DesignerItem;
            PropertyYCommand command;
            if (PropertyChanged != null && item != null)
            {
                command = new PropertyYCommand(item);
                command.Execute(e.OldValue, e.NewValue);
                PropertyChanged(command);
            }
        }
        void controlFont_PropertyFontItalicChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            DesignerItem item = sender as DesignerItem;
            PropertyFontItalicCommand command;
            if (PropertyChanged != null && item != null)
            {
                command = new PropertyFontItalicCommand(item);
                command.Execute(e.OldValue, e.NewValue);
                PropertyChanged(command);
            }
        }
        void controlFont_PropertyFontBoldChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            DesignerItem item = sender as DesignerItem;
            PropertyFontBoldCommand command;
            if (PropertyChanged != null && item != null)
            {
                command = new PropertyFontBoldCommand(item);
                command.Execute(e.OldValue, e.NewValue);
                PropertyChanged(command);
            }
        }
        void controlFont_PropertyFontSizeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DesignerItem item = sender as DesignerItem;
            PropertyFontSizeCommand command;
            if (PropertyChanged != null && item != null)
            {
                command = new PropertyFontSizeCommand(item);
                command.Execute(e.OldValue, e.NewValue);
                PropertyChanged(command);
            }
        }
        void controlFont_PropertyFontFamilyChanged(object sender, RoutedPropertyChangedEventArgs<FontFamily> e)
        {
            DesignerItem item = sender as DesignerItem;
            PropertyFontCommand command;
            if (PropertyChanged != null && item != null)
            {
                command = new PropertyFontCommand(item);
                command.Execute(e.OldValue, e.NewValue);
                PropertyChanged(command);
            }
        }        

        private List<DesignerItem> _source = new List<DesignerItem>();
        public List<DesignerItem> Source
        {
            get
            {
                return _source;
            }
            set
            {
                value.ForEach(delegate(DesignerItem item) { 
                    controlFont.Source = item; 
                    controlLocation.Source = item; 
                    controlTransform.Source = item;
                    controlAction.Source = item;
                    controlTextGrid.Source = item;
                    controlLine.Source = item;
                    controlTopic.Source = item;
                    controlTopicDrag.Source = item;
                    controlTpage.Source = item;
                    foreColorCanvas.Source = item;
                });
                if (value.Count > 0)
                {
                    SetPropertyList(((ToolboxItem)value[0].Content).AssetType);
                }
                _source = value;
            }
        }
        void SetPropertyList(AssetType assetType)
        {
            foreach (UIElement fe in Children)
            {
                fe.Visibility = System.Windows.Visibility.Collapsed;
            }
            foreach (var v in new AssetPropertyList())
            {
                if (v.Key == assetType)
                {
                    foreach (var vv in v.Value)
                    {
                        foreach (UIElement fe in Children)
                        {
                            Expander e = fe as Expander;
                            if (e == null) continue;
                            if (vv == (AssetProperty)e.Tag)
                            {
                                e.Visibility = System.Windows.Visibility.Visible;
                            }
                        }
                    }
                }
            }
        }        

        private DesignerCanvas _stage;
        public DesignerCanvas Stage
        {
            get { return _stage; }
            set
            {
                _stage = value; controlStage.Source = value;
                SetPropertyList(AssetType.Stage);
            }
        }
    }    
}