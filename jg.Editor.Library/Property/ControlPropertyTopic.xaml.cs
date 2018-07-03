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

namespace jg.Editor.Library.Property
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
                if (_source != null && _source.GetHashCode() == value.GetHashCode()) return;
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
            for (i = 0; i <=5; i++)
            {
                txt = this.FindName("txt" + i.ToString()) as TextBox;
                chk = this.FindName("checkBox" + i.ToString()) as CheckBox;
                txt.TextChanged -= txt_TextChanged;
                txt.IsEnabled = false;
                chk.IsEnabled = false;
                chk.Checked -= checkBox_Checked;
                chk.Unchecked -= checkBox_Checked;
                if (txt == null || chk == null) continue;
                txt.Text = "";
                chk.IsChecked = false;

                //txt.TextChanged += txt_TextChanged;
                //chk.Checked += checkBox_Checked;
                //chk.Unchecked += checkBox_Checked;

            }
            txtTitle.TextChanged -= txt_TextChanged;
            txtTitle.Text = topicInfo.Title;
            txtTitle.TextChanged += txt_TextChanged;

            txtScore.TextChanged -= txt_TextChanged;
            txtScore.Text = topicInfo.Score.ToString();
            txtScore.TextChanged += txt_TextChanged;

            cmbList.SelectionChanged -= cmbList_SelectionChanged;
            cmbList.SelectedIndex = (int)topicInfo.TopicType;
            cmbList.SelectionChanged += cmbList_SelectionChanged;

            txtNumber.TextChanged -= txtNumber_TextChanged;
            txtNumber.Text = topicInfo.OptionCount.ToString();
            txtNumber.TextChanged += txtNumber_TextChanged;

            i = 0;

            foreach (var v in topicInfo.TopicOptionList)
            {
                txt = this.FindName("txt" + i.ToString()) as TextBox;
                chk = this.FindName("checkBox" + i.ToString()) as CheckBox;
                if (txt == null || chk == null) continue;
                txt.Text = v.Title;
                chk.IsChecked = v.Right;
                txt.TextChanged += txt_TextChanged;
                chk.Checked += checkBox_Checked;
                chk.Unchecked += checkBox_Checked;
                txt.IsEnabled = true;
                chk.IsEnabled = true;
                i++;
            }

            for (i = topicInfo.OptionCount; i <= 5; i++)
            {
                txt = this.FindName("txt" + i.ToString()) as TextBox;
                chk = this.FindName("checkBox" + i.ToString()) as CheckBox;
                txt.Text = "";
                chk.IsChecked = false;
                txt.IsEnabled = false;
                chk.IsEnabled = false;
                txt.TextChanged += txt_TextChanged;
                chk.Checked += checkBox_Checked;
                chk.Unchecked += checkBox_Checked;

            }


        }

        void txtScore_TextChanged(object sender, TextChangedEventArgs e)
        {
            float score = 0;

            if (!float.TryParse(txtScore.Text, out score) || score < 1)
            {
                txtScore.Text = "1";
                txtScore.Focus();
                txtScore.SelectAll();
            }
            topicControl.TopicInfo.Score = score;
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
                    txtNumber.Text = "3";
                    txtNumber.IsReadOnly = false;
                    topicControl.TopicInfo.TopicType = Topic.TopicType.Single;                    
                    break;
                case 2:
                    txtNumber.Text = "4";
                    txtNumber.IsReadOnly = false;
                    topicControl.TopicInfo.TopicType = Topic.TopicType.Multiple;
                    break;
            }
            RefreshTopicInfo();
        }

        private void txtNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt;
            CheckBox checkbox;
            int count = 0;
            if (!int.TryParse(txtNumber.Text, out count))
            {
                txtNumber.Focus();
                txtNumber.SelectAll();
                return;
            }
            topicInfo.OptionCount = count;
            for (int i = 0; i <= count; i++)
            {
                txt = this.FindName("txt" + i.ToString()) as TextBox;
                checkbox = this.FindName("checkBox" + i.ToString()) as CheckBox;                
                if (txt == null || checkbox == null) continue;
                if (i == 0) checkbox.IsChecked = true;
                if (i < count)
                {
                    txt.IsEnabled = true;
                    checkbox.IsEnabled = true; 
                    checkbox.IsChecked = false;


                    if (txt.Text == "")
                        txt.Text = "选项" + (i + 1).ToString();
                }
                else
                {
                    txt.Text = "";
                    txt.IsEnabled = false;
                    checkbox.IsEnabled = false;
                    checkbox.IsChecked = false;


                }                
            }
            RefreshTopicInfo();
        }

        private void txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshTopicInfo();
        }

        void RefreshTopicInfo()
        {
            TextBox txt;
            CheckBox chk;
            topicControl.TopicInfo.Title = txtTitle.Text;
            
            for(int i =0;i< topicInfo.TopicOptionList.Count ;i++)
            {
                txt = this.FindName("txt" + i.ToString()) as TextBox;
                chk = this.FindName("checkBox" + i.ToString()) as CheckBox;
                if(txt == null || chk==null) continue;
                topicControl.TopicInfo.TopicOptionList[i].Title = txt.Text;
                if (chk.IsChecked == true)
                    topicControl.TopicInfo.TopicOptionList[i].Right = true;
                else
                   topicControl.TopicInfo.TopicOptionList[i].Right = false;
            }
            topicControl.RefreshControl();
            topicControl.ReSize();
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            CheckBox chkobj;

            switch (cmbList.SelectedIndex)
            {
                case 0:
                case 1:
                    for (int i = 0; i <= 5; i++)
                    {
                        chkobj = this.FindName("checkBox" + i.ToString()) as CheckBox;
                        if (chkobj == null) continue;
                        if (chk.Name == chkobj.Name) continue;
                        chkobj.Checked -= checkBox_Checked;
                        chkobj.Unchecked -= checkBox_Checked;
                        chkobj.IsChecked = false;
                        chkobj.Checked += checkBox_Checked;
                        chkobj.Unchecked += checkBox_Checked;
                    }
                    break;
            }

            RefreshTopicInfo();
        }



    }
}