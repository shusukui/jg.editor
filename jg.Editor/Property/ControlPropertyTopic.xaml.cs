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
    public partial class ControlPropertyTopic : UserControl
    {
        Topic.TopicInfo topicInfo;
        Topic.TopicControl topicControl = null;
        public ControlPropertyTopic()
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
                if ((topicControl = ((ToolboxItem)_source.Content).Content as Topic.TopicControl) == null) return;

                topicInfo = topicControl.TopicInfo;
               
                if (topicInfo != null)
                    BindingUI();
            }
        }

        void BindingUI()
        {
            TextBox txt;
            CheckBox chk;
            int i = 0;
            txtTitle.DataContext = topicInfo;
            txtNumber.DataContext = topicInfo;
            cmbList.SelectedIndex = (int)topicInfo.TopicType;

            foreach (var v in topicInfo.TopicOptionList)
            {
                txt = this.FindName("txt" + i.ToString()) as TextBox;
                chk = this.FindName("checkBox" + i.ToString()) as CheckBox;

                if (txt == null || chk == null) continue;
                txt.DataContext = v;
                chk.DataContext = v;
                i++;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void cmbList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            if (cmb == null) return;
            if (Source == null) return;
            switch (cmb.SelectedIndex)
            {
                case 0:
                    txtNumber.Text = "2";
                    txtNumber.IsReadOnly = true;
                    topicControl.TopicInfo.TopicType = Topic.TopicType.Judge;
                    break;
                case 1:
                    topicControl.TopicInfo.TopicType = Topic.TopicType.Single;
                    break;
                case 2:
                    topicControl.TopicInfo.TopicType = Topic.TopicType.Multiple;
                    break;
            }
            if (cmb.SelectedIndex == 0)
            {

            }
            else
            {
                txtNumber.IsReadOnly = false;
            }
        }
    }
}
