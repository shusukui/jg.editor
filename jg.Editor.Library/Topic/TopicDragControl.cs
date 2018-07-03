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
namespace jg.Editor.Topic
{
    [Serializable]
    public class TopicDragControl : Canvas
    {
        private TextBlock Title = new TextBlock();
        private ObservableCollection<TopicDragItem> LeftText = new ObservableCollection<TopicDragItem>();
        private ObservableCollection<TopicDragItem> RightText = new ObservableCollection<TopicDragItem>();

        private TopicDragItem SelItem = null;

        public static readonly DependencyProperty TopicDragInfoProperty = DependencyProperty.Register("TopicDragInfo", typeof(TopicDragInfo), typeof(TopicDragControl), new PropertyMetadata(null, OnTopicDragPropertyChanged));

        public TopicDragInfo TopicDragInfo { get { return (TopicDragInfo)GetValue(TopicDragInfoProperty); } set { SetValue(TopicDragInfoProperty, value); } }

        Point pushPoint = new Point(0, 0);
        Point currentPoint = new Point(0, 0);

        public TopicDragControl()
        {
            InitializeComponent();
            if (TopicDragInfo == null)
            {


                Topic.TopicDragInfo info = new Topic.TopicDragInfo();
                Topic.TopicDragItem option1 = new Topic.TopicDragItem() { Id = 1, Title = "AAA", AnswerId = 5 };
                Topic.TopicDragItem option2 = new Topic.TopicDragItem() { Id = 2, Title = "BBB", AnswerId = 6 };
                Topic.TopicDragItem option3 = new Topic.TopicDragItem() { Id = 3, Title = "CCC", AnswerId = 7 };
                Topic.TopicDragItem option4 = new Topic.TopicDragItem() { Id = 4, Title = "DDD", AnswerId = 8 };


                Topic.TopicDragItem option5 = new Topic.TopicDragItem() { Id = 5, Title = "EEE", AnswerId = 1 };
                Topic.TopicDragItem option6 = new Topic.TopicDragItem() { Id = 6, Title = "FFF", AnswerId = 2 };
                Topic.TopicDragItem option7 = new Topic.TopicDragItem() { Id = 7, Title = "GGG", AnswerId = 3 };
                Topic.TopicDragItem option8 = new Topic.TopicDragItem() { Id = 8, Title = "HHH", AnswerId = 4 };

                info.LeftOption.Add(option1);
                info.LeftOption.Add(option2);
                info.LeftOption.Add(option3);
                info.LeftOption.Add(option4);
                // info.LeftOption.AddRange(new ObservableCollection<Topic.TopicDragItem> { option1, option2, option3, option4 });

                info.RightOption.Add(option5);
                info.RightOption.Add(option6);
                info.RightOption.Add(option7);
                info.RightOption.Add(option8);
                // info.RightOption.AddRange(new ObservableCollection<Topic.TopicDragItem> { option5, option6, option7, option8 });
                TopicDragInfo = info;

            }
        }

        private void InitializeComponent()
        {
            this.MinWidth = 150; this.MinHeight = 150;

            // 设计时显示内容
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                if (this.TopicDragInfo == null)
                {
                    Topic.TopicDragInfo info = new Topic.TopicDragInfo();
                    Topic.TopicDragItem option1 = new Topic.TopicDragItem() { Id = 1, Title = "AAA", AnswerId = 5 };
                    Topic.TopicDragItem option2 = new Topic.TopicDragItem() { Id = 2, Title = "BBB", AnswerId = 6 };
                    Topic.TopicDragItem option3 = new Topic.TopicDragItem() { Id = 3, Title = "CCC", AnswerId = 7 };
                    Topic.TopicDragItem option4 = new Topic.TopicDragItem() { Id = 4, Title = "DDD", AnswerId = 8 };


                    Topic.TopicDragItem option5 = new Topic.TopicDragItem() { Id = 5, Title = "EEE", AnswerId = 1 };
                    Topic.TopicDragItem option6 = new Topic.TopicDragItem() { Id = 6, Title = "FFF", AnswerId = 2 };
                    Topic.TopicDragItem option7 = new Topic.TopicDragItem() { Id = 7, Title = "GGG", AnswerId = 3 };
                    Topic.TopicDragItem option8 = new Topic.TopicDragItem() { Id = 8, Title = "HHH", AnswerId = 4 };


                    info.LeftOption.Add(option1);
                    info.LeftOption.Add(option2);
                    info.LeftOption.Add(option3);
                    info.LeftOption.Add(option4);
                    // info.LeftOption.AddRange(new ObservableCollection<Topic.TopicDragItem> { option1, option2, option3, option4 });

                    info.RightOption.Add(option5);
                    info.RightOption.Add(option6);
                    info.RightOption.Add(option7);
                    info.RightOption.Add(option8);
                    // info.RightOption.AddRange(new ObservableCollection<Topic.TopicDragItem> { option5, option6, option7, option8 });
                    TopicDragInfo = info;
                }
            }
            this.Background = Brushes.LightCoral;
            this.SizeChanged +=TopicDragControl_SizeChanged;
        }

        void TopicDragControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ReSize();
        }

        protected static void OnTopicDragPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (TopicDragControl)d;
            control.RefreshControl();
            control.DrawCollection();
        }

        private void RefreshControl()
        {
            if (TopicDragInfo == null) return;

            Title.Text = TopicDragInfo.Title;

            LeftText.Clear();
            RightText.Clear();

            foreach (var v in TopicDragInfo.LeftOption)
            {
                LeftText.Add(v);
                v.PreviewMouseLeftButtonDown += tb_PreviewMouseLeftButtonDown;
                v.PreviewMouseLeftButtonUp += tb_PreviewMouseLeftButtonUp;
                v.PreviewMouseMove += tb_PreviewMouseMove;
            }
            foreach (var v in TopicDragInfo.RightOption)
                RightText.Add(v);


            // LeftText.ForEach(delegate(TopicDragItem tb)
            // { 
            //    tb.PreviewMouseLeftButtonDown += tb_PreviewMouseLeftButtonDown; 
            //    tb.PreviewMouseLeftButtonUp += tb_PreviewMouseLeftButtonUp;
            //    tb.PreviewMouseMove += tb_PreviewMouseMove; });
        }

        void ReSize()
        {
            DrawCollection();
            for (int i = 0; i < LeftText.Count; i++)
            {
                SetLeft(LeftText[i], this.ActualWidth * .1);
                SetTop(LeftText[i], this.ActualHeight / LeftText.Count * i);
                if (LeftText[i].UserAnswer != -1)
                    LeftText[i].LinePoint = new Point(GetLeft(LeftText[i]) + LeftText[i].ActualWidth, GetTop(LeftText[i]) + LeftText[i].ActualHeight / 2);
            }
            for (int i = 0; i < RightText.Count; i++)
            {
                SetLeft(RightText[i], this.ActualWidth * .7);
                SetTop(RightText[i], this.ActualHeight / RightText.Count * i);
                RightText[i].LinePoint = new Point(GetLeft(RightText[i]), GetTop(RightText[i]) + RightText[i].ActualHeight / 2);
            }
            DrawLine();
        }

        void DrawLine()
        {
            Path path;
            TopicDragItem rightItem;
            for (int i = Children.Count - 1; i >= 0; i--)
                if (Children[i] is Path) Children.RemoveAt(i);

            foreach (var v in LeftText)
            {

                // rightItem = RightText.Find(model => model.Id == v.UserAnswer);

                rightItem = RightText.FirstOrDefault(model => model.Id == v.UserAnswer);
                // rightItem = RightText.First(model => model.Id == v.UserAnswer);
                if (rightItem == null && v.LinePoint != new Point(0, 0))
                {
                    Geometry g = new LineGeometry(v.LinePoint, currentPoint);
                    path = new Path();
                    path.Stroke = Brushes.Black;
                    path.Data = g;
                    Children.Add(path);
                }
                else if (rightItem != null) 
                {
                    Geometry g = new LineGeometry(v.LinePoint, rightItem.LinePoint);
                    path = new Path();
                    path.Stroke = Brushes.Black;
                    path.Data = g;
                    Children.Add(path);
                }
            }
        }


        void DrawCollection()
        {
            Children.Clear();
            foreach (var v in LeftText)
                Children.Add(v);
            // LeftText.ForEach(delegate(TopicDragItem tb) { Children.Add(tb); });
            foreach (var v in RightText)
                Children.Add(v);
            // RightText.ForEach(delegate(TopicDragItem tb) { Children.Add(tb); });
        }

        bool FindItem(TopicDragItem p)// 比较器（这里的p为Plans中的元素，即p.PlanID与给定的PlanID比较）
        {
            if (SelItem.Id == p.Id) 
                return true; 
            else 
                return false;
        }

        void tb_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SelItem = sender as TopicDragItem;
            if (SelItem == null) return;
            SelItem.UserAnswer = -1;
            SelItem.LinePoint = new Point(e.GetPosition(null).X - e.GetPosition(SelItem).X + SelItem.ActualWidth, e.GetPosition(null).Y - e.GetPosition(SelItem).Y + SelItem.ActualHeight / 2);
        }

        void tb_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            SelItem = sender as TopicDragItem;
            if (SelItem == null) return;

            // int index = LeftText.FindIndex(new Predicate<TopicDragItem>(FindItem));

            if (e.GetPosition(null).X > currentPoint.X)
                currentPoint.X = e.GetPosition(null).X - 1;
            else
                currentPoint.X = e.GetPosition(null).X + 1;

            if (e.GetPosition(null).Y > currentPoint.Y)
                currentPoint.Y = e.GetPosition(null).Y - 1;
            else
                currentPoint.Y = e.GetPosition(null).Y + 1;

            if (SelItem.CaptureMouse())
                DrawLine();

        }

        void tb_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SelItem = sender as TopicDragItem;
            if (SelItem == null) return;
            SelItem.ReleaseMouseCapture();
            // int index = LeftText.FindIndex(new Predicate<TopicDragItem>(FindItem));
            object o = InputHitTest(e.GetPosition(this));
            TopicDragItem tb2 = GetMouseSelItem((DependencyObject)InputHitTest(e.GetPosition(this))) as TopicDragItem;
            if (tb2 != null)
            {
                SelItem.UserAnswer = tb2.Id;
                tb2.LinePoint = currentPoint;
            }
            else
            {
                SelItem.LinePoint = new Point(0, 0);
                currentPoint = new Point(0, 0);
                SelItem.UserAnswer = -1;
            }
            DrawLine();
        }

        TopicDragItem GetMouseSelItem(DependencyObject obj)
        {
            TopicDragItem item;
            if (obj == null) return null;
            if ((item = VisualTreeHelper.GetParent(obj) as TopicDragItem) != null)
                return item;
            else
                return GetMouseSelItem(VisualTreeHelper.GetParent(obj));
        }
    }


}
