using System.Windows;
using System.Windows.Controls;

namespace jg.Editor.Library
{
    public class ResizeChrome : System.Windows.Controls.Control
    {
        static ResizeChrome()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizeChrome), new FrameworkPropertyMetadata(typeof(ResizeChrome)));
        }
    }
}
