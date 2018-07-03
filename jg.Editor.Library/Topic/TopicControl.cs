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
namespace jg.Editor.Library.Topic
{
    [Serializable]
    public class TopicControl : Canvas
    {
        private TextBlock tb_title = new TextBlock();
        private List<TextBlock> tb_optionlist = new List<TextBlock>();
        private List<CheckBox> tb_checklist = new List<CheckBox>();
        public bool IsEdit { get; set; }
        public List<int> answerList = new List<int>();
        Button btnSubmit = null;
        private double _userscore = 0;
        public double UserScore
        {
            get { return _userscore; }
            set { _userscore = value; }
        }
        public static readonly DependencyProperty TopicInfoProperty = DependencyProperty.Register("TopicInfo", typeof(TopicInfo), typeof(TopicControl), new PropertyMetadata(null, OnTopicPropertyChanged));

        public TopicInfo TopicInfo
        {
            get { return (TopicInfo)GetValue(TopicInfoProperty); }
            set
            {
                value.PropertyChanged += v_PropertyChanged;
                if (value.TopicOptionList.Count == 0)
                    for (int i = 0; i < value.OptionCount; i++)
                        value.TopicOptionList.Add(new TopicOptionInfo() { Title = "选项" + i.ToString(), Id = i, Index = i });
                foreach (var v in value.TopicOptionList)
                    v.PropertyChanged += v_PropertyChanged;

                SetValue(TopicInfoProperty, value);
            }
        }

        void v_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "OptionCount":
                    if (TopicInfo.OptionCount < TopicInfo.TopicOptionList.Count)
                    {
                        for (int i = TopicInfo.TopicOptionList.Count - 1; i >= TopicInfo.OptionCount; i--)
                            TopicInfo.TopicOptionList.RemoveAt(i);
                    }
                    else
                    {
                        for (int i = TopicInfo.TopicOptionList.Count; i < TopicInfo.OptionCount; i++)
                        {
                            TopicInfo.TopicOptionList.Add(new TopicOptionInfo() { Title = "选项" + i.ToString(), Id = i, Index = i });
                        }
                    }
                    break;
            }
            //RefreshControl();
            //ReSize();
        }

        public TopicControl(bool isedit, List<int> _answerList = null)
        {
            if (_answerList != null)
            {
                answerList = _answerList;
            }
            IsEdit = isedit;
            InitializeComponent();
            if (TopicInfo == null)
                TopicInfo = new TopicInfo();

        }

        private void InitializeComponent()
        {
            this.MinWidth = 400; this.MinHeight = 300;
            this.SizeChanged += TopicControl_SizeChanged;
          
                btnSubmit = new Button() { Content = FindResource("FF000066") };
                btnSubmit.Background = new SolidColorBrush(Colors.LightGray);
                btnSubmit.Style = FindResource("btnSubmitStyle") as Style;
                btnSubmit.Height = 30;
                btnSubmit.Width = 50;
                btnSubmit.Click += new RoutedEventHandler(btnSubmit_Click);
                if (answerList.Count > 0)
                {
                    btnSubmit.Visibility = Visibility.Visible;
                }
                else
                {
                    btnSubmit.Visibility = Visibility.Collapsed;
                }
        }

        void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            bool IsRight = false;
           
            foreach (var v in TopicInfo.TopicOptionList)
            {
                v.RightVisibility = Visibility.Visible;
                
                if (v.Right)
                {

                    IsRight = false;
                    foreach (var vv in answerList)
                        if (v.Id == vv)
                        {
                            IsRight = true;
                            break;
                        }
                    if (!IsRight) break;
                }
            }

            if (IsRight)
            {
                _userscore = TopicInfo.Score;
                btnSubmit.Background = new SolidColorBrush(Colors.Green);
                btnSubmit.Content = FindResource("FF000069").ToString();
                MessageBox.Show(FindResource("FF000069").ToString()); //正确

            }
            else
            {
                _userscore = 0;
                btnSubmit.Background = new SolidColorBrush(Colors.Red);
                btnSubmit.Content = FindResource("FF00006A").ToString();
                MessageBox.Show(FindResource("FF00006A").ToString());//错误
            }

           
            btnSubmit.IsEnabled = false;
        }

        void TopicControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ReSize();
        }

        protected static void OnTopicPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (TopicControl)d;

            control.RefreshControl();
            control.DrawCollection();
        }

        private SolidColorBrush itemforeground = Brushes.Black;

        public SolidColorBrush ItemForeground
        {
            get { return itemforeground; }
            set
            {
                itemforeground = value;
                tb_title.Foreground = value;
                tb_optionlist.ForEach(delegate(TextBlock tb) { tb.Foreground = value; });
            }
        }

        public void RefreshControl()
        {
            TextBlock textBlock;
            CheckBox chkbox;
            tb_title.TextWrapping = TextWrapping.Wrap;
            tb_title.FontSize = 20;
            tb_title.FontFamily = new FontFamily("Microsoft YaHei");
            Binding binding, bindingText, bingdingVis;
            tb_optionlist.Clear();
            tb_checklist.Clear();

            bindingText = new Binding() { Source = TopicInfo, Path = new PropertyPath("Title"), Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
            tb_title.SetBinding(TextBlock.TextProperty, bindingText);
            int i = 1;
            foreach (var v in TopicInfo.TopicOptionList)
            {
                v.RightVisibility = Visibility.Collapsed;
                //if (v.Title == "") continue;
                if (i > TopicInfo.OptionCount) break;
                binding = new Binding();

                bingdingVis = new Binding();

                chkbox = new CheckBox();


                binding.Path = new PropertyPath("Right");
                binding.Mode = BindingMode.TwoWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                binding.Source = v;
                chkbox.SetBinding(CheckBox.IsCheckedProperty, binding);

                bool bingRight = answerList.Contains(v.Id);

                if (bingRight)
                {
                    bingdingVis.Path = new PropertyPath("RightVisibility");
                    bingdingVis.Mode = BindingMode.TwoWay;
                    bingdingVis.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    bingdingVis.Source = v;

                    //显示正确答案的bing
                    Label label = new Label();
                    label.Content = FindResource("FF000069").ToString();
                    label.Foreground = new SolidColorBrush(Colors.Green);
                    label.SetBinding(Label.VisibilityProperty, bingdingVis);
                    chkbox.Content = label;
                }
                bindingText = new Binding() { Source = v, Path = new PropertyPath("Title"), Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };

                textBlock = new TextBlock() { TextWrapping = TextWrapping.Wrap, FontSize = 18, FontFamily = new FontFamily("Microsoft YaHei") };
                textBlock.SetBinding(TextBlock.TextProperty, bindingText);
                textBlock.Foreground = ItemForeground;

                tb_optionlist.Add(textBlock);
                tb_checklist.Add(chkbox);
                i++;
            }
        }

        public void ReSize()
        {
            DrawCollection();
            int i = 0;
            Canvas.SetLeft(tb_title, 0);
            Canvas.SetTop(tb_title, 0);
            tb_title.Height = this.ActualHeight / 3;
            tb_title.Width = this.ActualWidth;
            if (this.ActualHeight == 0 && this.ActualWidth == 0) return;
            foreach (var v in tb_optionlist)
            {
                v.Height = (this.ActualHeight - this.ActualHeight / 3) / tb_optionlist.Count;
                v.Width = this.ActualWidth * .8 - 20;
                Canvas.SetLeft(v, 20);
                Canvas.SetTop(v, this.ActualHeight / 3 + v.Height * i);
                i++;
            }
            i = 0;
            foreach (var v in tb_checklist)
            {
                v.Height = (this.ActualHeight - this.ActualHeight / 3) / tb_optionlist.Count;
                v.Width = this.ActualWidth * .2;
                Canvas.SetLeft(v, this.ActualWidth * .8);
                Canvas.SetTop(v, this.ActualHeight / 3 + v.Height * i);
                i++;
            }
        }

        public void Clear()
        {
            if (this.TopicInfo != null)
                this.TopicInfo.Clear();
        }

        // 绘制界面
        void DrawCollection()
        {
            Children.Clear();

            Children.Add(tb_title);

            foreach (var v in tb_optionlist)
            {
                Children.Add(v);
            }
            foreach (var v in tb_checklist)
            {
                Children.Add(v);
            }
            Canvas.SetLeft(btnSubmit, this.ActualWidth - 50);
            Canvas.SetTop(btnSubmit, this.ActualHeight - 30);
            Children.Add(btnSubmit);
        }
    }
}