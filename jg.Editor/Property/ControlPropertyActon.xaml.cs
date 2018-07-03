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
    public partial class ControlPropertyActon : UserControl
    {
        double time = 0.3;

        public ControlPropertyActon()
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
                DesignerItem item;
                if (canvas == null) return;
                cmbAssetList.Items.Clear();
                cmbAssetList.Items.Add("");
                foreach (var v in canvas.Children)
                {
                    item = v as DesignerItem;
                    if (item == null) continue;
                    if (item.ItemId == value.ItemId) continue;
                    if (string.IsNullOrEmpty(item.ItemName)) continue;
                    cmbAssetList.Items.Add(item.ItemName);
                }
                if (_source.assetActionInfo != null)
                {
                    cmbEvent.SelectedIndex = (int)_source.assetActionInfo.AssetEvent;
                    cmbAction.SelectedIndex = (int)_source.assetActionInfo.AssetAction;
                    if (!string.IsNullOrEmpty(_source.assetActionInfo.AssetName))
                        cmbAssetList.SelectedItem = _source.assetActionInfo.AssetName;
                    txtTime.TextChanged -= txtTime_TextChanged;
                    txtTime.Text = _source.assetActionInfo.Time.ToString();
                    txtTime.TextChanged += txtTime_TextChanged;
                }
            }
        }
        
        private void cmbAssetList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {            
            if (_source == null) return;
            if (cmbAssetList.SelectedIndex == -1) return;

            SetAction();
        }

        private void txtTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            TextBox txt = sender as TextBox;
            if (txt == null) return;
            if (!double.TryParse(txt.Text, out time))
            {
                txtTime.Focus();
                txtTime.SelectAll();
                return;
            }
            SetAction();
        }

        private void SetAction()
        {
            AssetActionInfo assetactioninfo;

            assetactioninfo = new AssetActionInfo();
            assetactioninfo.AssetEvent = (enumAssetEvent)cmbEvent.SelectedIndex;
            assetactioninfo.AssetAction = (enumAssetAction)cmbAction.SelectedIndex;
            assetactioninfo.AssetName = cmbAssetList.SelectedItem.ToString();
            assetactioninfo.Time = time;
            _source.assetActionInfo = assetactioninfo;
        }
    }
}
