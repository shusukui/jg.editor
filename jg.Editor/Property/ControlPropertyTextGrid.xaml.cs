using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace jg.Editor.Property
{
    /// <summary>
    /// ControlPropertyActon.xaml 的交互逻辑
    /// </summary>
    public partial class ControlPropertyTextGrid : UserControl
    {
        public event RoutedPropertyChangedEventHandler<int> PropertyGridRowChanged = null;
        public event RoutedPropertyChangedEventHandler<int> PropertyGridColumnChanged = null;
        public event RoutedPropertyChangedEventHandler<int> PropertyGridBorderWidthChanged = null;

        Control.ControlTextGrid controlTextGrid = null;
        int oldColumn = 2, oldRow = 2,oldBorderWidth;
        public ControlPropertyTextGrid()
        {
            InitializeComponent();
        }

        private DesignerItem _source = null;
        public DesignerItem Source
        {
            get { return _source; }
            set
            {   
                _source = value;
                DesignerCanvas canvas = _source.Parent as DesignerCanvas;
                controlTextGrid =((ToolboxItem)value.Content).Content as Control.ControlTextGrid;
                if (controlTextGrid == null) return;
                itemRow = controlTextGrid.RowCount;
                itemColumn = controlTextGrid.ColumnCount;
                itemBorderWidth = controlTextGrid.BorderWidth;
            }
        }

        public int itemRow
        {
            get { if (controlTextGrid == null) return 2; else return controlTextGrid.RowCount; }
            set
            {
                if (controlTextGrid == null) return;
                txtRow.TextChanged -= txtRow_TextChanged;
                txtRow.Text = value.ToString();
                txtRow.TextChanged += txtRow_TextChanged;
            }
        }

        public int itemColumn
        {
            get { if (controlTextGrid == null) return 2; else return controlTextGrid.ColumnCount; }
            set
            {
                if (controlTextGrid == null) return;
                txtColumn.TextChanged -= txtColumn_TextChanged;
                txtColumn.Text = value.ToString();
                txtColumn.TextChanged += txtColumn_TextChanged;
            }
        }

        public int itemBorderWidth
        {
            get { if (controlTextGrid == null) return 2; else return controlTextGrid.BorderWidth; }
            set
            {
                if (controlTextGrid == null) return;
                txtBorderWidth.TextChanged += txtBorderWidth_TextChanged;
                txtBorderWidth.Text = value.ToString();
                txtBorderWidth.TextChanged += txtBorderWidth_TextChanged;
            }
        }

        void txtBorderWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            int borderwidth = 0;
            TextBox txt = sender as TextBox;
            if (txt == null) return;
            if (controlTextGrid == null) return;

            if (int.TryParse(txt.Text, out borderwidth))
            {
                controlTextGrid.BorderWidth = borderwidth;
                if (PropertyGridBorderWidthChanged == null) return;
                PropertyGridBorderWidthChanged(_source, new RoutedPropertyChangedEventArgs<int>(oldBorderWidth, borderwidth));
            }
        }

        void txtRow_TextChanged(object sender, TextChangedEventArgs e)
        {
            int row = 0;
            TextBox txt = sender as TextBox;
            if (txt == null) return;
            if (controlTextGrid == null) return;

            if (int.TryParse(txt.Text, out row))
            {
                controlTextGrid.RowCount = row;
                if (PropertyGridRowChanged == null) return;
                PropertyGridRowChanged(_source, new RoutedPropertyChangedEventArgs<int>(oldRow, row));
            }
        }
        
        void txtColumn_TextChanged(object sender, TextChangedEventArgs e)
        {
            int column = 0;
            TextBox txt = sender as TextBox;
            if (txt == null) return;
            if (controlTextGrid == null) return;

            if (int.TryParse(txt.Text, out column))
            {
                controlTextGrid.ColumnCount = column;
                if (PropertyGridColumnChanged == null) return;
                PropertyGridColumnChanged(_source, new RoutedPropertyChangedEventArgs<int>(oldColumn, column));
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt == null) return;
            switch (txt.Tag.ToString())
            {
                case "Row":
                    int.TryParse(txt.Text, out oldRow);
                    break;
                case "Column":
                    int.TryParse(txt.Text, out oldColumn);
                    break;
                case "BorderWidth":
                    int.TryParse(txt.Text, out oldBorderWidth);
                    break;
            }            
        }

    }
}
