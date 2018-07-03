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
    /// ControlPropertyActon.xaml 的交互逻辑
    /// </summary>
    public partial class ControlPropertyAction : UserControl
    {
        double time = 0.3;

        public ControlPropertyAction()
        {
            InitializeComponent();
        }
        
        private DesignerItem _source = null;
        public DesignerItem Source
        {
            get { return _source; }
            set
            {
                double _time = time;
                _source = value;
                DesignerCanvas canvas = _source.Parent as DesignerCanvas;
                DesignerItem item;
                if (canvas == null) return;
                if (_source.assetActionInfo != null)
                    _time = _source.assetActionInfo.Time;

                cmbEvent.SelectionChanged -= cmbAssetList_SelectionChanged;
                cmbAction.SelectionChanged -= cmbAssetList_SelectionChanged;
                cmbAssetList.SelectionChanged -= cmbAssetList_SelectionChanged;
                cmbTree.SelectionChanged -= cmbAssetList_SelectionChanged;
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
                cmbEvent.SelectedIndex = 0;
                cmbTree.Visibility = System.Windows.Visibility.Hidden;
                cmbAssetList.Visibility = System.Windows.Visibility.Visible;
                cmbAssetList.SelectedIndex = 0;



                if (_source.assetActionInfo != null)
                {
                    cmbEvent.SelectedIndex = (int)_source.assetActionInfo.AssetEvent;
                    cmbAction.SelectedIndex = (int)_source.assetActionInfo.AssetAction;

                    switch(cmbAction.SelectedIndex)
                    {
                        case (int)enumAssetAction.PageJump:
                            cmbTree.Visibility = System.Windows.Visibility.Visible;
                            cmbAssetList.Visibility = System.Windows.Visibility.Hidden;

                            foreach(Tuple<string,string> vv in cmbTree.Items)
                                if (vv.Item2 == _source.assetActionInfo.AssetName)
                                    cmbTree.SelectedItem = vv;
                            break;
                        default:
                            cmbTree.Visibility = System.Windows.Visibility.Hidden;
                            cmbAssetList.Visibility = System.Windows.Visibility.Visible;
                            if (!string.IsNullOrEmpty(_source.assetActionInfo.AssetName))
                                cmbAssetList.SelectedItem = _source.assetActionInfo.AssetName;
                            break;
                    }


                    txtTime.TextChanged -= txtTime_TextChanged;
                    txtTime.Text = _time.ToString();
                    _source.assetActionInfo.Time = _time;
                    txtTime.TextChanged += txtTime_TextChanged;
                }
                if (cmbAssetList.SelectedIndex == -1)
                    txtTime.IsEnabled = false;


                cmbEvent.SelectionChanged += cmbAssetList_SelectionChanged;
                cmbAction.SelectionChanged += cmbAssetList_SelectionChanged;
                cmbAssetList.SelectionChanged += cmbAssetList_SelectionChanged;
                cmbTree.SelectionChanged += cmbAssetList_SelectionChanged;

            }
        }
        private List<jg.Editor.Library.Control.ComboTree.TreeModel> _treemodellist = new List<Control.ComboTree.TreeModel>();
        public List<jg.Editor.Library.Control.ComboTree.TreeModel> TreeModelList
        {
            get { return _treemodellist; }
            set
            {
                _treemodellist = value;

                cmbTree.SelectionChanged -= cmbAssetList_SelectionChanged;

                cmbTree.DisplayMemberPath = "Item1";
                cmbTree.SelectedValuePath = "Item2";
                cmbTree.Items.Clear();
                FillTreeList(value);
                cmbTree.SelectionChanged += cmbAssetList_SelectionChanged;
            }
        }
        void FillTreeList(List<jg.Editor.Library.Control.ComboTree.TreeModel> list, int i = 0)
        {
            i++;
            foreach (var v in list)
            {
                if (v.Children != null)
                {
                    cmbTree.Items.Add(new Tuple<string, string>(v.DisplayValuePath.PadLeft(i * 8), v.SelectedValuePath));
                    FillTreeList(v.Children, i);
                }
            }
        }
        private void cmbAssetList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_source == null) return;
            _source.assetActionInfo = null;
            if (cmbAction.SelectedIndex == (int)enumAssetAction.PageJump)
            {
                cmbTree.Visibility = System.Windows.Visibility.Visible;
                cmbAssetList.Visibility = System.Windows.Visibility.Hidden;

                if (cmbTree.SelectedIndex == -1) return;
                SetAction();
            }
            else
            {
                cmbTree.Visibility = System.Windows.Visibility.Hidden;
                cmbAssetList.Visibility = System.Windows.Visibility.Visible;
                if (cmbAssetList.SelectedIndex == -1) return;
                txtTime.IsEnabled = true;
                SetAction();
            }



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
            switch (assetactioninfo.AssetAction)
            {
                case enumAssetAction.PageJump:
                    assetactioninfo.AssetName = ((Tuple<string, string>)cmbTree.SelectedItem).Item2;
                    assetactioninfo.Time = 0;
                    break;
                default:
                    assetactioninfo.AssetName = cmbAssetList.SelectedItem.ToString();
                    assetactioninfo.Time = time;
                    break;
            }
            _source.assetActionInfo = assetactioninfo;
        }
    }
}
