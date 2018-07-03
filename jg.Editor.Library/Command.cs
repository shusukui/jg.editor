

namespace jg.Editor.Library
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public interface ICommand
    {
        void Execute();
        void Undo();
    }
    // 位移或尺寸变化
    public class ControlMoveOrResize : ICommand
    {
        double _oldwidth = 0, _oldheight = 0, _oldleft = 0, _oldtop = 0;
        double _newwidth = 0, _newheight = 0, _newleft = 0, _newtop = 0;
        DesignerItem _item;
        public ControlMoveOrResize(DesignerItem item)
        {
            _item = item;
        }

        public void Execute()
        {
            _item.Width = _newwidth;
            _item.Height = _newheight;
            Canvas.SetLeft(_item, _newleft);
            Canvas.SetTop(_item, _newtop);
        }

        public void Execute(double width, double height, double left, double top, double oldwidth, double oldheight, double oldleft, double oldtop)
        {
            _item.Width = width;
            _item.Height = height;
            Canvas.SetLeft(_item, left);
            Canvas.SetTop(_item, top);

            _oldwidth = oldwidth;
            _oldheight = oldheight;
            _oldleft = oldleft;
            _oldtop = oldtop;

            _newwidth = width;
            _newheight = height;
            _newleft = left;
            _newtop = top;
        }

        public void Undo()
        {
            Canvas.SetLeft(_item, _oldleft);
            Canvas.SetTop(_item, _oldtop);
            _item.Height = _oldheight;
            _item.Width = _oldwidth;
        }
    }

    // 创建控件
    public class ControlCreate : ICommand
    {
        DesignerItem _item;
        DesignerCanvas _contentCanvas;

        public ControlCreate(DesignerItem item)
        {
            _item = item;
            _contentCanvas = _item.Parent as DesignerCanvas;
        }

        public void Execute()
        {
            if (_contentCanvas != null)
            {
                _contentCanvas.Children.Add(_item);
                _item.IsSelected = true;
            }
        }

        public void Undo()
        {
            if (_contentCanvas != null)
                _contentCanvas.Children.Remove(_item);
        }
    }

    // 移除控件
    public class ControlRemove : ICommand
    {
        DesignerItem _contentcontrol;
        DesignerCanvas _contentCanvas;

        public ControlRemove(DesignerItem contentControl)
        {
            _contentcontrol = contentControl;
            _contentCanvas = _contentcontrol.Parent as DesignerCanvas;
        }

        public void Execute()
        {
            if (_contentCanvas != null)
                _contentCanvas.Children.Remove(_contentcontrol);
        }

        public void Undo()
        {
            if (_contentCanvas != null)
            {
                _contentCanvas.Children.Add(_contentcontrol);
                _contentcontrol.IsSelected = true;
            }
        }
    }

    // 字体变化
    public class PropertyFontCommand : ICommand
    {
        DesignerItem _item;
        FontFamily _oldProperty;
        FontFamily _newProperty;

        public PropertyFontCommand(DesignerItem item)
        {
            _item = item;
        }

        public void Execute()
        {
            _item.FontFamily = _newProperty;
        }

        public void Execute(FontFamily oldProperty, FontFamily newProperty)
        {
            _oldProperty = oldProperty;
            _newProperty = newProperty;
        }

        public void Undo()
        {
            _item.FontFamily = _oldProperty;
        }
    }

    // 字号变化
    public class PropertyFontSizeCommand : ICommand
    {
        DesignerItem _item;
        double _oldProperty;
        double _newProperty;

        public double OldProperty { get { return _oldProperty; } }
        public double NewProperty { get { return _newProperty; } }

        public PropertyFontSizeCommand(DesignerItem item)
        {
            _item = item;
        }
        public void Execute()
        {
            _item.FontSize = _newProperty;
        }
        public void Execute(double oldProperty, double newProperty)
        {
            _oldProperty = oldProperty;
            _newProperty = newProperty;
        }
        public void Undo()
        {
            _item.FontSize = _oldProperty;
        }
    }

    // 文本加粗
    public class PropertyFontBoldCommand : ICommand
    {
        DesignerItem _item;
        bool _oldProperty;
        bool _newProperty;
        public PropertyFontBoldCommand(DesignerItem item)
        {
            _item = item;
        }
        public void Execute()
        {
            if (_newProperty)
                _item.FontWeight = FontWeights.Bold;
            else
                _item.FontWeight = FontWeights.Normal;
        }
        public void Execute(bool oldProperty, bool newProperty)
        {
            _oldProperty = oldProperty;
            _newProperty = newProperty;
        }
        public void Undo()
        {
            if (_oldProperty)
                _item.FontWeight = FontWeights.Bold;
            else
                _item.FontWeight = FontWeights.Normal;
        }
    }

    // 文本斜体
    public class PropertyFontItalicCommand : ICommand
    {
        DesignerItem _item;
        bool _oldProperty;
        bool _newProperty;
        public PropertyFontItalicCommand(DesignerItem item)
        {
            _item = item;
        }
        public void Execute()
        {
            if (_newProperty)
                _item.FontStyle = FontStyles.Italic;
            else
                _item.FontStyle = FontStyles.Normal;
        }
        public void Execute(bool oldProperty, bool newProperty)
        {
            _oldProperty = oldProperty;
            _newProperty = newProperty;
        }
        public void Undo()
        {
            if (_oldProperty)
                _item.FontStyle = FontStyles.Italic;
            else
                _item.FontStyle = FontStyles.Normal;
        }
    }

    // 高度变化
    public class PropertyHeightCommand : ICommand
    {
        DesignerItem _item;
        double _oldProperty;
        double _newProperty;

        public double OldProperty { get { return _oldProperty; } }
        public double NewProperty { get { return _newProperty; } }

        public PropertyHeightCommand(DesignerItem item)
        {
            _item = item;
        }
        public void Execute()
        {
            _item.Height = _newProperty;
        }
        public void Execute(double oldProperty, double newProperty)
        {
            _oldProperty = oldProperty;
            _newProperty = newProperty;
        }
        public void Undo()
        {
            _item.Height = _oldProperty;
        }
    }

    // 宽度变化
    public class PropertyWidthCommand : ICommand
    {
        DesignerItem _item;
        double _oldProperty;
        double _newProperty;

        public double OldProperty { get { return _oldProperty; } }
        public double NewProperty { get { return _newProperty; } }

        public PropertyWidthCommand(DesignerItem item)
        {
            _item = item;
        }
        public void Execute()
        {
            _item.Width = _newProperty;
        }
        public void Execute(double oldProperty, double newProperty)
        {
            _oldProperty = oldProperty;
            _newProperty = newProperty;
        }
        public void Undo()
        {
            _item.Width = _oldProperty;
        }
    }

    // 左边距变化
    public class PropertyXCommand : ICommand
    {
        DesignerItem _item;
        double _oldProperty;
        double _newProperty;

        public double OldProperty { get { return _oldProperty; } }
        public double NewProperty { get { return _newProperty; } }

        public PropertyXCommand(DesignerItem item)
        {
            _item = item;
        }
        public void Execute()
        {
            DesignerCanvas.SetLeft(_item, _newProperty);
        }
        public void Execute(double oldProperty, double newProperty)
        {
            _oldProperty = oldProperty;
            _newProperty = newProperty;
        }
        public void Undo()
        {
            DesignerCanvas.SetLeft(_item, _oldProperty);
        }
    }

    // 上边距变化
    public class PropertyYCommand : ICommand
    {
        DesignerItem _item;
        double _oldProperty;
        double _newProperty;

        public double OldProperty { get { return _oldProperty; } }
        public double NewProperty { get { return _newProperty; } }

        public PropertyYCommand(DesignerItem item)
        {
            _item = item;
        }
        public void Execute()
        {
            DesignerCanvas.SetTop(_item, _newProperty);
        }
        public void Execute(double oldProperty, double newProperty)
        {
            _oldProperty = oldProperty;
            _newProperty = newProperty;
        }
        public void Undo()
        {
            DesignerCanvas.SetTop(_item, _oldProperty);
        }
    }

    //Y轴平移
    public class PropertyOpacityCommand : ICommand
    {
        DesignerItem _item;
        double _oldProperty;
        double _newProperty;

        public double OldProperty { get { return _oldProperty; } }
        public double NewProperty { get { return _newProperty; } }

        public PropertyOpacityCommand(DesignerItem item)
        {
            _item = item;
        }
        public void Execute()
        {
            _item.Opacity = _newProperty;
        }
        public void Execute(double oldProperty, double newProperty)
        {
            _oldProperty = oldProperty;
            _newProperty = newProperty;
        }
        public void Undo()
        {
            _item.Opacity = _oldProperty;
        }
    }
    
    //旋转角度
    public class PropertyRotateCommand : ICommand
    {
        DesignerItem _item;
        double _oldProperty;
        double _newProperty;

        public double OldProperty { get { return _oldProperty; } }
        public double NewProperty { get { return _newProperty; } }

        public PropertyRotateCommand(DesignerItem item)
        {
            _item = item;
        }
        public void Execute()
        {
            TransformGroup transformGroup = ((ToolboxItem)_item.Content).RenderTransform as TransformGroup;
            if (null == transformGroup) return;
            RotateTransform rotateTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "RotateTransform") as RotateTransform;
            if (rotateTransform != null) { rotateTransform.Angle = _newProperty; }
        }
        public void Execute(double oldProperty, double newProperty)
        {
            _oldProperty = oldProperty;
            _newProperty = newProperty;
        }
        public void Undo()
        {
            TransformGroup transformGroup = ((ToolboxItem)_item.Content).RenderTransform as TransformGroup;
            if (null == transformGroup) return;
            RotateTransform rotateTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "RotateTransform") as RotateTransform;
            if (rotateTransform != null) { rotateTransform.Angle = _oldProperty; }
        }
    }

    //X轴缩放
    public class PropertyScaleXCommand : ICommand
    {
        DesignerItem _item;
        double _oldProperty;
        double _newProperty;

        public double OldProperty { get { return _oldProperty; } }
        public double NewProperty { get { return _newProperty; } }

        public PropertyScaleXCommand(DesignerItem item)
        {
            _item = item;
        }
        public void Execute()
        {
            TransformGroup transformGroup = ((ToolboxItem)_item.Content).RenderTransform as TransformGroup;
            if (null == transformGroup) return;
            ScaleTransform scaleTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "ScaleTransform") as ScaleTransform;
            if (scaleTransform != null) { scaleTransform.ScaleX = _newProperty; }
        }
        public void Execute(double oldProperty, double newProperty)
        {
            _oldProperty = oldProperty;
            _newProperty = newProperty;
        }
        public void Undo()
        {
            TransformGroup transformGroup = ((ToolboxItem)_item.Content).RenderTransform as TransformGroup;
            if (null == transformGroup) return;
            ScaleTransform scaleTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "ScaleTransform") as ScaleTransform;
            if (scaleTransform != null) { scaleTransform.ScaleX = _oldProperty; }
        }
    }

    //X轴缩放
    public class PropertyScaleYCommand : ICommand
    {
        DesignerItem _item;
        double _oldProperty;
        double _newProperty;

        public double OldProperty { get { return _oldProperty; } }
        public double NewProperty { get { return _newProperty; } }

        public PropertyScaleYCommand(DesignerItem item)
        {
            _item = item;
        }
        public void Execute()
        {
            TransformGroup transformGroup = ((ToolboxItem)_item.Content).RenderTransform as TransformGroup;
            if (null == transformGroup) return;
            ScaleTransform scaleTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "ScaleTransform") as ScaleTransform;
            if (scaleTransform != null) { scaleTransform.ScaleY = _newProperty; }
        }
        public void Execute(double oldProperty, double newProperty)
        {
            _oldProperty = oldProperty;
            _newProperty = newProperty;
        }
        public void Undo()
        {
            TransformGroup transformGroup = ((ToolboxItem)_item.Content).RenderTransform as TransformGroup;
            if (null == transformGroup) return;
            ScaleTransform scaleTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "ScaleTransform") as ScaleTransform;
            if (scaleTransform != null) { scaleTransform.ScaleY = _oldProperty; }
        }
    }

    //X轴2D扭曲
    public class PropertySkewXCommand : ICommand
    {
        DesignerItem _item;
        double _oldProperty;
        double _newProperty;

        public double OldProperty { get { return _oldProperty; } }
        public double NewProperty { get { return _newProperty; } }

        public PropertySkewXCommand(DesignerItem item)
        {
            _item = item;
        }
        public void Execute()
        {
            TransformGroup transformGroup = ((ToolboxItem)_item.Content).RenderTransform as TransformGroup;
            if (null == transformGroup) return;
            SkewTransform skewTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "SkewTransform") as SkewTransform;
            if (skewTransform != null) { skewTransform.AngleX = _newProperty; }
        }
        public void Execute(double oldProperty, double newProperty)
        {
            _oldProperty = oldProperty;
            _newProperty = newProperty;
        }
        public void Undo()
        {
            TransformGroup transformGroup = ((ToolboxItem)_item.Content).RenderTransform as TransformGroup;
            if (null == transformGroup) return;
            SkewTransform skewTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "SkewTransform") as SkewTransform;
            if (skewTransform != null) { skewTransform.AngleX = _oldProperty; }
        }
    }

    //Y轴2D扭曲
    public class PropertySkewYCommand : ICommand
    {
        DesignerItem _item;
        double _oldProperty;
        double _newProperty;

        public double OldProperty { get { return _oldProperty; } }
        public double NewProperty { get { return _newProperty; } }

        public PropertySkewYCommand(DesignerItem item)
        {
            _item = item;
        }
        public void Execute()
        {
            TransformGroup transformGroup = ((ToolboxItem)_item.Content).RenderTransform as TransformGroup;
            if (null == transformGroup) return;
            SkewTransform skewTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "SkewTransform") as SkewTransform;
            if (skewTransform != null) { skewTransform.AngleY = _newProperty; }
        }
        public void Execute(double oldProperty, double newProperty)
        {
            _oldProperty = oldProperty;
            _newProperty = newProperty;
        }
        public void Undo()
        {
            TransformGroup transformGroup = ((ToolboxItem)_item.Content).RenderTransform as TransformGroup;
            if (null == transformGroup) return;
            SkewTransform skewTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "SkewTransform") as SkewTransform;
            if (skewTransform != null) { skewTransform.AngleY = _oldProperty; }
        }
    }

    //X轴平移
    public class PropertyTranslateXCommand : ICommand
    {
        DesignerItem _item;
        double _oldProperty;
        double _newProperty;

        public double OldProperty { get { return _oldProperty; } }
        public double NewProperty { get { return _newProperty; } }

        public PropertyTranslateXCommand(DesignerItem item)
        {
            _item = item;
        }
        public void Execute()
        {
            TransformGroup transformGroup = ((ToolboxItem)_item.Content).RenderTransform as TransformGroup;
            if (null == transformGroup) return;
            TranslateTransform translateTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "TranslateTransform") as TranslateTransform;
            if (translateTransform != null) { translateTransform.X = _newProperty; }
        }
        public void Execute(double oldProperty, double newProperty)
        {
            _oldProperty = oldProperty;
            _newProperty = newProperty;
        }
        public void Undo()
        {
            TransformGroup transformGroup = ((ToolboxItem)_item.Content).RenderTransform as TransformGroup;
            if (null == transformGroup) return;
            TranslateTransform translateTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "TranslateTransform") as TranslateTransform;
            if (translateTransform != null) { translateTransform.X = _oldProperty; }
        }
    }

    //Y轴平移
    public class PropertyTranslateYCommand : ICommand
    {
        DesignerItem _item;
        double _oldProperty;
        double _newProperty;

        public double OldProperty { get { return _oldProperty; } }
        public double NewProperty { get { return _newProperty; } }

        public PropertyTranslateYCommand(DesignerItem item)
        {
            _item = item;
        }
        public void Execute()
        {
            TransformGroup transformGroup = ((ToolboxItem)_item.Content).RenderTransform as TransformGroup;
            if (null == transformGroup) return;
            TranslateTransform translateTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "TranslateTransform") as TranslateTransform;
            if (translateTransform != null) { translateTransform.Y = _newProperty; }
        }
        public void Execute(double oldProperty, double newProperty)
        {
            _oldProperty = oldProperty;
            _newProperty = newProperty;
        }
        public void Undo()
        {
            TransformGroup transformGroup = ((ToolboxItem)_item.Content).RenderTransform as TransformGroup;
            if (null == transformGroup) return;
            TranslateTransform translateTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "TranslateTransform") as TranslateTransform;
            if (translateTransform != null) { translateTransform.Y = _oldProperty; }
        }
    }

    // 命令集合
    public class RemoteControl
    {
        int index = 0;
        List<ICommand> icommandList = new List<ICommand>();

        public RemoteControl()
        {
           // Canvas = canvas;
        }

        public void RedoCommand()
        {
            if (index == icommandList.Count - 1) return;
            if (index < icommandList.Count - 1) index++;
            icommandList[index].Execute();
        }

        public void RedoCommand(ICommand command)
        {
            if (index < 0)
                icommandList.Clear();
            else if (index < icommandList.Count - 1) // 撤销到此位置，再执行操作，把当前位置后的操作全部删除。
                icommandList.RemoveRange(index, icommandList.Count - index);

            icommandList.Add(command);
            index = icommandList.Count - 1;
        }

        public void UndoCommand()
        {
            if (index < 0) return;
            icommandList[index].Undo();
            index--;
        }
    }
}