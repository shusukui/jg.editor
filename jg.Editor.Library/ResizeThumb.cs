

namespace jg.Editor.Library
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Collections.Generic;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using System.Windows.Resources;
    using System.Windows.Input;

    public class ResizeThumb : Thumb
    {
        private DesignerItem DesignerItem;
        private DesignerCanvas DesignerCanvas;
        private double initialAngle;
        private RotateTransform rotateTransform;
        private Vector startVector;
        private Point centerPoint;
        private ContentControl designerItem;
        private Canvas canvas;
        List<double[]> oldValueList = new List<double[]>();

        public ResizeThumb()
            : base()
        {
            DragStarted += new DragStartedEventHandler(this.ResizeThumb_DragStarted);
            DragCompleted += new DragCompletedEventHandler(ResizeThumb_DragCompleted);
            Loaded += ResizeThumb_Loaded;

        }

        void ResizeThumb_Loaded(object sender, RoutedEventArgs e)
        {
            if (Name == "headAngle")
            {
                DragDelta += headAngleResizeThumb_DragDelta;

            }
            else
            {

                DragDelta += new DragDeltaEventHandler(this.ResizeThumb_DragDelta);

            }
        }
        void ResizeThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (this.DesignerItem != null)
            {
                foreach (DesignerItem item in this.DesignerCanvas.SelectedItems)
                {
                    double[] list = oldValueList.Find(model => model[0] == item.GetHashCode());
                    item.SetItemDragComplete(list[1], list[2], list[3], list[4]);
                }
            }
        }

        private void ResizeThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            double[] oldValue;
            this.DesignerItem = DataContext as DesignerItem;

            if (this.DesignerItem != null)
            {
                this.DesignerCanvas = VisualTreeHelper.GetParent(this.DesignerItem) as DesignerCanvas;

                if (this.DesignerCanvas != null)
                {

                    oldValueList.Clear();
                    foreach (DesignerItem item in this.DesignerCanvas.SelectedItems)
                    {

                        item.canvas = this.DesignerCanvas;
                        Point p = new Point();
                        //p = item.TranslatePoint(
                        //new Point(0,0),
                        //          this.DesignerCanvas);
                        // item.centerPoint = item.TranslatePoint(
                        //new Point(item.ActualWidth/2,
                        //          item.ActualHeight/ 2),
                        //          this.DesignerCanvas);



                        Point startPoint = Mouse.GetPosition(this.DesignerCanvas);
                        item.StartVector = Point.Subtract(startPoint, item.centerPoint);

                        item.RotateTransform = item.RenderTransform as RotateTransform;
                        if (item.RotateTransform == null)
                        {
                            item.RenderTransform = new RotateTransform(0);

                            item.InitialAngle = 0;
                        }
                        else
                        {
                            item.InitialAngle = item.RotateTransform.Angle;
                        }

                        oldValue = new double[5] { item.GetHashCode(), item.ActualWidth, item.ActualHeight, Canvas.GetLeft(item), Canvas.GetTop(item) };
                        oldValueList.Add(oldValue);
                    }
                }
            }
        }
        private void headAngleResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //if (this.DesignerItem != null && this.DesignerCanvas != null && this.DesignerItem.IsSelected)
            //{
            //    if (this.DesignerItem.IsLock == true) return;

            Thumb thumb = sender as Thumb;

            foreach (DesignerItem item in this.DesignerCanvas.SelectedItems)
            {

                #region MyRegion
              



                //Point p2 = new Point();

                //if (item.AngledX == 0 && item.AngledY == 0)
                //{
                //    double a = Canvas.GetLeft(item);
                //    double b = Canvas.GetTop(item);
                //    p2 = item.RenderTransform.Transform(new Point(a, b));

                //}




                //double minLeftItem = item.ActualWidth / 2;//控件的X  中心坐标
                //double minTopItem = item.ActualHeight / 2;//控件的Y 中心坐标


                //double minLeftThumb = this.Margin.Left + e.HorizontalChange; //thumbX 坐标
                //double minTopThumb = this.Margin.Top + e.VerticalChange; //thumbY 坐标
                //minLeftThumb = minLeftThumb + item.AngledX;
                //item.AngledX = minLeftThumb;
                //minTopThumb = minTopThumb + item.AngledY;
                //item.AngledY = minTopThumb;
                //double OldMinLeftThumb = this.Margin.Left;//在移动之前的X坐标
                //double OldminTopThumb = this.Margin.Top;//在移动之前的Y坐标
                //double AOffistLine = Math.Sqrt(Math.Pow(minLeftThumb - OldMinLeftThumb, 2) + Math.Pow(minTopThumb - OldminTopThumb, 2)); //偏移位置的大长度

                //double OldLine = Math.Sqrt(Math.Pow(minLeftItem - OldMinLeftThumb, 2) + Math.Pow(minTopItem - OldminTopThumb, 2));//在移动前的两点之间的距离

                //double NewLine = Math.Sqrt(Math.Pow(minLeftItem - minLeftThumb, 2) + Math.Pow(minTopItem - minTopThumb, 2));//在移动后的两点之间的距离

                //double AngleValue = 0;//最终旋转的度数

                //double denominatorValue = Math.Sqrt(Math.Pow(NewLine, 2) + Math.Pow(OldLine, 2) - Math.Pow(AOffistLine, 2));

                //double Elementvalue = 2 * NewLine * OldLine;
                //if (denominatorValue == 0)
                //{
                //    AngleValue = 90;
                //}
                //else if (denominatorValue < 0)
                //{
                //    AngleValue = 180;
                //}
                //else if (minLeftThumb == OldMinLeftThumb && minTopThumb == OldminTopThumb)
                //{
                //    AngleValue = 360;
                //}
                //else
                //{
                //    double cosA = denominatorValue / Elementvalue;
                //    double arcA = Math.Acos(cosA);
                //    AngleValue = arcA;
                //}
                //RotateTransform rotateTransform = new RotateTransform(AngleValue);

                //double CenterX = Canvas.GetLeft(item);
                //double CenterY = Canvas.GetTop(item);
                //rotateTransform.CenterX = CenterX + item.ActualWidth / 2;
                //rotateTransform.CenterY = CenterY + item.ActualHeight / 2;
                //item.RenderTransform = rotateTransform;
                #endregion

                Point currentPoint = Mouse.GetPosition(this.DesignerCanvas);
                Vector deltaVector = Point.Subtract(currentPoint, item.centerPoint);

                double angle = Vector.AngleBetween(item.StartVector, deltaVector);
                TransformGroup tg = ((ToolboxItem)item.Content).RenderTransform as TransformGroup;
                foreach (var v in tg.Children)
                {
                    if (v.GetType().Name == typeof(RotateTransform).Name)
                    {
                        // ((RotateTransform)v).CenterX = _source.ActualWidth / 2;
                        // ((RotateTransform)v).CenterY = _source.ActualHeight / 2;
                        ((RotateTransform)v).Angle = angle;
                      
                    }
                }
            }


        }


        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.DesignerItem != null && this.DesignerCanvas != null && this.DesignerItem.IsSelected)
            {
                if (this.DesignerItem.IsLock == true) return;
                double minLeft = double.MaxValue;
                double minTop = double.MaxValue;

                double minDeltaHorizontal = double.MaxValue;
                double minDeltaVertical = double.MaxValue;

                double maxDeltaHorizontal = 0;
                double maxDeltaVertical = 0;

                double dragDeltaVertical, dragDeltaHorizontal;
                Thumb thumb = sender as Thumb;

                foreach (DesignerItem item in this.DesignerCanvas.SelectedItems)
                {




                    minLeft = Math.Min(Canvas.GetLeft(item), minLeft);
                    minTop = Math.Min(Canvas.GetTop(item), minTop);

                    minDeltaVertical = Math.Min(minDeltaVertical, item.ActualHeight - item.MinHeight);
                    minDeltaHorizontal = Math.Min(minDeltaHorizontal, item.ActualWidth - item.MinWidth);

                    maxDeltaVertical = Math.Max(maxDeltaVertical, Canvas.GetTop(item) + item.ActualHeight);
                    maxDeltaHorizontal = Math.Max(maxDeltaHorizontal, Canvas.GetLeft(item) + item.ActualWidth);


                }

                foreach (DesignerItem item in this.DesignerCanvas.SelectedItems)
                {
                    switch (VerticalAlignment)
                    {
                        case VerticalAlignment.Bottom:
                            dragDeltaVertical = Math.Min(-e.VerticalChange, minDeltaVertical);
                            // if (maxDeltaVertical - dragDeltaVertical < this.DesignerCanvas.ActualHeight) // 控制控件不移出容器
                            item.Height = item.ActualHeight - dragDeltaVertical;
                            break;
                        case VerticalAlignment.Top:
                            dragDeltaVertical = Math.Min(Math.Max(-minTop, e.VerticalChange), minDeltaVertical);
                            Canvas.SetTop(item, Canvas.GetTop(item) + dragDeltaVertical);
                            item.Height = item.ActualHeight - dragDeltaVertical;
                            break;
                    }

                    switch (HorizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            dragDeltaHorizontal = Math.Min(Math.Max(-minLeft, e.HorizontalChange), minDeltaHorizontal);
                            Canvas.SetLeft(item, Canvas.GetLeft(item) + dragDeltaHorizontal);
                            item.Width = item.ActualWidth - dragDeltaHorizontal;
                            break;
                        case HorizontalAlignment.Right:
                            dragDeltaHorizontal = Math.Min(-e.HorizontalChange, minDeltaHorizontal);
                            // if (maxDeltaHorizontal - dragDeltaHorizontal < this.DesignerCanvas.ActualWidth) // 控制控件不移出容器
                            item.Width = item.ActualWidth - dragDeltaHorizontal;
                            break;
                    }
                }

                e.Handled = true;
            }
        }
    }
}
