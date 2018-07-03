
namespace jg.Editor.Library
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using System.Collections.Generic;

    public class MoveThumb : Thumb
    {
        private DesignerItem DesignerItem;
        
        private DesignerCanvas DesignerCanvas;

        List<double[]> oldValueList = new List<double[]>();
        public MoveThumb()
        {

            DragStarted += new DragStartedEventHandler(this.MoveThumb_DragStarted); // 拖放开始
            DragDelta += new DragDeltaEventHandler(this.MoveThumb_DragDelta); // 拖放细节
            DragCompleted += new DragCompletedEventHandler(MoveThumb_DragCompleted); // 拖放完成
        }

        void MoveThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {

            if (this.DesignerItem != null)
                foreach (DesignerItem item in this.DesignerCanvas.SelectedItems)
                {
                    double[] list = oldValueList.Find(model => model[0] == item.GetHashCode());
                    item.SetItemDragComplete(list[1], list[2], list[3], list[4]);
                }
        }

        private void MoveThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            double[] oldValue;
            this.DesignerItem = DataContext as DesignerItem;
            
            if (this.DesignerItem != null)
            {
                this.DesignerCanvas = VisualTreeHelper.GetParent(this.DesignerItem) as DesignerCanvas;
                oldValueList.Clear();
                foreach (DesignerItem item in this.DesignerCanvas.SelectedItems)
                {
                    oldValue = new double[5] { item.GetHashCode(), item.ActualWidth, item.ActualHeight, Canvas.GetLeft(item), Canvas.GetTop(item) };
                    oldValueList.Add(oldValue);
                }
            }
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            
            if (this.DesignerItem != null && this.DesignerCanvas != null && this.DesignerItem.IsSelected)
            {
                if (this.DesignerItem.IsLock == true) return;
                double minLeft = double.MaxValue;
                double minTop = double.MaxValue;

                double maxLeft = 0;
                double maxTop = 0;

                foreach (DesignerItem item in this.DesignerCanvas.SelectedItems)
                {
                    minLeft = Math.Min(Canvas.GetLeft(item), minLeft);
                    minTop = Math.Min(Canvas.GetTop(item), minTop);

                    maxLeft = Math.Max(Canvas.GetLeft(item) + item.ActualWidth, maxLeft);
                    maxTop = Math.Max(Canvas.GetTop(item) + item.ActualHeight, maxTop);
                }

                double deltaHorizontal = Math.Max(-minLeft, e.HorizontalChange);
                double deltaVertical = Math.Max(-minTop, e.VerticalChange);

                // if (maxLeft + deltaHorizontal < this.DesignerCanvas.MaxWidth) // 控制控件不移出容器
                    foreach (DesignerItem item in this.DesignerCanvas.SelectedItems)
                        Canvas.SetLeft(item, Canvas.GetLeft(item) + deltaHorizontal);

                // if (maxTop + deltaVertical < this.DesignerCanvas.MaxHeight) // 控制控件不移出容器
                    foreach (DesignerItem item in this.DesignerCanvas.SelectedItems)
                        Canvas.SetTop(item, Canvas.GetTop(item) + deltaVertical);

                // this.DesignerCanvas.InvalidateMeasure();
                e.Handled = true;

            }
        }
    }
}
