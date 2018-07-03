

namespace jg.Editor.Library
{
    using System.Windows;
    using System.Windows.Controls;

    public class Toolbox : ItemsControl
    {
        private Size defaultItemSize = new Size(60, 60);
        public Size DefaultItemSize
        {
            get { return this.defaultItemSize; }
            set { this.defaultItemSize = value; }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ToolboxItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is ToolboxItem);
        }
    }
}
