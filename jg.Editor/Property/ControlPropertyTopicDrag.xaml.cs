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
    /// ControlPropertyTopic.xaml 的交互逻辑
    /// </summary>
    public partial class ControlPropertyTopicDrag : UserControl
    {
        Control.ControlTopicDrag controlTopicDrag;
        List<Control.TopicDragItemInfo> topicDragItemList = new List<Control.TopicDragItemInfo>();
        List<Control.TopicDragItemAnswerInfo> topicDragItemAnswerList = new List<Control.TopicDragItemAnswerInfo>();

        public ControlPropertyTopicDrag()
        {
            InitializeComponent();
        }

        private DesignerItem _source = null;
        public DesignerItem Source
        {
            get { return _source; }

            set
            {
                int count1, count2;
                if (_source != null && _source.ItemId == value.ItemId) return;
                _source = value;
                controlTopicDrag = ((ToolboxItem)value.Content).Content as Control.ControlTopicDrag;
                if (controlTopicDrag == null) return;
                count1 = controlTopicDrag.topicDragItemAnswerList.Count;
                count2 = controlTopicDrag.topicDragItemList.Count - controlTopicDrag.topicDragItemAnswerList.Count;

                txtItemCount1.TextChanged -= txt_TextChanged;
                txtItemCount1.Text = count1.ToString();
                txtItemCount1.TextChanged += txt_TextChanged;

                txtItemCount2.TextChanged -= txt_TextChanged;
                txtItemCount2.Text = count2.ToString();
                txtItemCount2.TextChanged += txt_TextChanged;

                DrawGrid(count1,count2);
                BindingUI();
            }
        }

        void BindingUI()
        {
            Binding binding;
            if (controlTopicDrag == null) return;

            for (int i = 0; i < controlTopicDrag.topicDragItemList.Count; i++)
            {
                var v = gridItem.Children.OfType<TextBox>().FirstOrDefault(model=> (int)model.Tag == i);
                if (v == null) continue;
                binding = new Binding();
                binding.Source = controlTopicDrag.topicDragItemList[i];
                binding.Path = new PropertyPath("Text");
                binding.Mode = BindingMode.TwoWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
                v.SetBinding(TextBox.TextProperty, binding);
            }
        }

        private void txt_TextChanged(object sender, TextChangedEventArgs e)
        {           
            int count1 = 0, count2 = 0;
            if (int.TryParse(txtItemCount1.Text, out count1) == true && int.TryParse(txtItemCount2.Text,out count2))
            {
                DrawGrid(count1 , count2);
            }
            controlTopicDrag.TopicDragItemAnswerCount = count1;
            controlTopicDrag.TopicDragItemCount =count1+ count2;
            BindingUI();
        }

        void DrawGrid(int count1, int count2)
        {
            gridItem.RowDefinitions.Clear();
            gridItem.Children.Clear();
            RowDefinition rowDefinition;
            Binding binding;
            foreach (var v in GetTextBox(count1 + count2))
            {
                binding = new Binding();
                rowDefinition = new RowDefinition();
                binding.Source = v;
                binding.Path = new PropertyPath("ActualHeight");
                rowDefinition.SetBinding(RowDefinition.HeightProperty, binding);
                gridItem.RowDefinitions.Add(rowDefinition);
                gridItem.Children.Add(v);
            }
            foreach (var v in GetTextBlock(count1, count2))
            {
                
                gridItem.Children.Add(v);
            }

        }

        IEnumerable<TextBlock> GetTextBlock(int count1,int count2)
        {
            List<TextBlock> list = new List<TextBlock>();
            TextBlock textblock;
            for (int i = 0; i < count1; i++)
            {
                textblock = new TextBlock() { Text = "项目" + (i + 1).ToString() };
                Grid.SetRow(textblock, i);
                Grid.SetColumn(textblock, 0);
                list.Add(textblock);
            }
            for (int i = 0; i < count2; i++)
            {
                textblock = new TextBlock() { Text = "干扰项" + (i + 1).ToString() };
                Grid.SetRow(textblock, i + count1);
                Grid.SetColumn(textblock, 0);
                list.Add(textblock);
            }


            return list;
        }

        IEnumerable<TextBox> GetTextBox(int count)
        {
            List<TextBox> list = new List<TextBox>();
            TextBox textBox;
            for (int i = 0; i < count; i++)
            {
                textBox = new TextBox() { Height = 25, Tag = i };
                Grid.SetRow(textBox, i);
                Grid.SetColumn(textBox, 1);
                list.Add(textBox);
            }
            return list;
        }
    }
}