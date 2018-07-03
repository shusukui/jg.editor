using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace jg.Editor.Library.Control.ComboTree
{
    public class ComboBoxTreeView : ComboBox
    {
        private ExtendedTreeView _treeView;
        private ContentPresenter _contentPresenter;
        private object _ModelType;

        public object ModelType
        {
            get { return _ModelType; }
            set { _ModelType = value; }
        }
        /// <summary>
        /// 可输入的TextBox
        /// </summary>
        private System.Windows.Controls.TextBox popupTextBox;

        public ComboBoxTreeView()
        {
            this.DefaultStyleKey = typeof(ComboBoxTreeView);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            //don't call the method of the base class
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _treeView = (ExtendedTreeView)this.GetTemplateChild("treeView");
            popupTextBox = (TextBox)this.GetTemplateChild("PART_EditableTextBox");
            _treeView.OnHierarchyMouseUp += new MouseEventHandler(OnTreeViewHierarchyMouseUp);
            _contentPresenter = (ContentPresenter)this.GetTemplateChild("ContentPresenter");

            this.SetSelectedItemToHeader();
        }

        protected override void OnDropDownClosed(EventArgs e)
        {
            base.OnDropDownClosed(e);
            this.SelectedItem = _treeView.SelectedItem;
            this.SetSelectedItemToHeader();
        }

        protected override void OnDropDownOpened(EventArgs e)
        {
            base.OnDropDownOpened(e);
            this.SetSelectedItemToHeader();
        }

        /// <summary>
        /// Handles clicks on any item in the tree view
        /// </summary>
        private void OnTreeViewHierarchyMouseUp(object sender, MouseEventArgs e)
        {
            //This line isn't obligatory because it is executed in the OnDropDownClosed method, but be it so
            this.SelectedItem = _treeView.SelectedItem;

            this.IsDropDownOpen = false;
        }

        public new IEnumerable<ITreeViewItemModel> ItemsSource
        {
            get { return (IEnumerable<ITreeViewItemModel>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly new DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<ITreeViewItemModel>), typeof(ComboBoxTreeView), new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));

        private static void OnItemsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((ComboBoxTreeView)sender).UpdateItemsSource();
        }

        /// <summary>
        /// Selected item of the TreeView
        /// </summary>
        public new object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public new static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(ComboBoxTreeView), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedItemChanged)));

        private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((ComboBoxTreeView)sender).UpdateSelectedItem();
        }

        /// <summary>
        /// Selected hierarchy of the treeview
        /// </summary>
        public IEnumerable<string> SelectedHierarchy
        {
            get { return (IEnumerable<string>)GetValue(SelectedHierarchyProperty); }
            set { SetValue(SelectedHierarchyProperty, value); }
        }

        public static readonly DependencyProperty SelectedHierarchyProperty =
            DependencyProperty.Register("SelectedHierarchy", typeof(IEnumerable<string>), typeof(ComboBoxTreeView), new PropertyMetadata(null, OnSelectedHierarchyChanged));

        private static void OnSelectedHierarchyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((ComboBoxTreeView)sender).UpdateSelectedHierarchy();
        }

        private void UpdateItemsSource()
        {
            var allItems = new List<ITreeViewItemModel>();

            Action<IEnumerable<ITreeViewItemModel>> selectAllItemsRecursively = null;
            selectAllItemsRecursively = items =>
            {
                if (items == null)
                {
                    return;
                }

                foreach (var item in items)
                {
                    allItems.Add(item);
                    selectAllItemsRecursively(item.GetChildren());
                }
            };

            selectAllItemsRecursively(this.ItemsSource);

            base.ItemsSource = allItems.Count > 0 ? allItems : null;
        }

        private void UpdateSelectedItem()
        {
            if (_treeView != null && _treeView.SelectedItem != null)
            {
                if (this._treeView.SelectedItem is TreeViewItem)
                {
                    //I would rather use a correct object instead of TreeViewItem
                    this.SelectedItem = ((TreeViewItem)this.SelectedItem).DataContext;
                }
                else
                {
                    //Update the selected hierarchy and displays
                    var model = this._treeView.SelectedItem as ITreeViewItemModel;
                    if (model != null)
                    {
                        this.SelectedHierarchy = model.GetHierarchy().Select(h => h.SelectedValuePath).ToList();
                    }

                    this.SetSelectedItemToHeader();

                    base.SelectedItem = this._treeView.SelectedItem;
                }
            }
            else
            {
                if (this.SelectedItem is TreeViewItem)
                {
                    //I would rather use a correct object instead of TreeViewItem
                    this.SelectedItem = ((TreeViewItem)this.SelectedItem).DataContext;
                }
                else
                {
                    //Update the selected hierarchy and displays
                    var model = this.SelectedItem as ITreeViewItemModel;
                    if (model != null)
                    {
                        this.SelectedHierarchy = model.GetHierarchy().Select(h => h.SelectedValuePath).ToList();
                    }

                    this.SetSelectedItemToHeader();

                    base.SelectedItem = this.SelectedItem;
                }
            }
        }

        private void UpdateSelectedHierarchy()
        {
            if (ItemsSource != null && this.SelectedHierarchy != null)
            {
                //Find corresponding items and expand or select them
                var source = this.ItemsSource.OfType<ITreeViewItemModel>();
                var item = SelectItem(source, this.SelectedHierarchy);
                this.SelectedItem = item;
            }
        }

        /// <summary>
        /// Searches the items of the hierarchy inside the items source and selects the last found item
        /// </summary>
        private static ITreeViewItemModel SelectItem(IEnumerable<ITreeViewItemModel> items, IEnumerable<string> selectedHierarchy)
        {
            if (items == null || selectedHierarchy == null || !items.Any() || !selectedHierarchy.Any())
            {
                return null;
            }

            var hierarchy = selectedHierarchy.ToList();
            var currentItems = items;
            ITreeViewItemModel selectedItem = null;

            for (int i = 0; i < hierarchy.Count; i++)
            {
                // get next item in the hierarchy from the collection of child items
                var currentItem = GetFind(currentItems.ToList(), hierarchy[i]);
                if (currentItem == null)
                {
                    break;
                }

                selectedItem = currentItem;

                // rewrite the current collection of child items
                currentItems = selectedItem.GetChildren();
                if (currentItems == null)
                {
                    break;
                }

                // the intermediate items will be expanded
                if (i != hierarchy.Count - 1)
                {
                    selectedItem.IsExpanded = true;
                }
            }

            if (selectedItem != null)
            {
                selectedItem.IsSelected = true;
            }

            return selectedItem;
        }


        private static ITreeViewItemModel GetFind(List<ITreeViewItemModel> currentItems,string hierarchy)
        {
            ITreeViewItemModel selectedItem = null;

            if (hierarchy != null && hierarchy.Length > 0)
            {
                
                    for (int i = 0; i < currentItems.Count; i++)
                    {


                        if (currentItems[i].SelectedValuePath == hierarchy)
                        {
                            selectedItem = currentItems[i];
                            break;
                        }


                       selectedItem= GetFind(currentItems[i].GetChildren().ToList(), hierarchy);

                        if (selectedItem != null)
                        {
                            break;
                        }

                    }
                
            }
            return selectedItem;
        }
        /// <summary>
        /// Gets the hierarchy of the selected tree item and displays it at the combobox header
        /// </summary>
        private void SetSelectedItemToHeader()
        {
            string content = null;

            var item = this.SelectedItem as ITreeViewItemModel;
            if (item != null)
            {
                var hierarchy = this.SelectedItem.ToString();
                if (_treeView != null && _treeView.SelectedItem != null)
                {
                    //Get hierarchy and display it as the selected item
                    hierarchy = _treeView.SelectedItem.ToString();
                }

                if (hierarchy.Length > 0)
                {
                    content = string.Join(".", hierarchy);
                }
            }


            this.SetContentAsTextBlock(content);
        }

        /// <summary>
        /// Gets the combobox header and displays the specified content there
        /// </summary>
        private void SetContentAsTextBlock(string content)
        {
            if (_contentPresenter == null)
            {
                return;
            }

            var tb = _contentPresenter.Content as TextBlock;
            if (tb == null)
            {
                _contentPresenter.Content = tb = new TextBlock();
            }
            tb.Text = content ?? ' '.ToString();
            _contentPresenter.ContentTemplate = null;

        }
    }

}
