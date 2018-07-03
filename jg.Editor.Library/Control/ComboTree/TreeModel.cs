using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace jg.Editor.Library.Control.ComboTree
{

    public class TreeModel : ITreeViewItemModel
    {
        private string _selectedvaluepath;
        private string _displayvaluepath;
        private bool _isexpanded = false;
        private bool _isselected = false;
        private List<TreeModel> _children = new List<TreeModel>();
        public string SelectedValuePath { get { return _selectedvaluepath; } set { _selectedvaluepath = value; } }

        public string DisplayValuePath { get { return _displayvaluepath; } set { _displayvaluepath = value; } }

        public bool IsExpanded { get { return _isexpanded; } set { _isexpanded = value; } }

        public bool IsSelected { get { return _isselected; } set { _isselected = value; } }


        public List<TreeModel> Children { get { return _children; } set { _children = value; } }
        public IEnumerable<ITreeViewItemModel> GetHierarchy()
        {
            return GetAscendingHierarchy();
        }
        public TreeModel Parent { get; set; }
        public IEnumerable<ITreeViewItemModel> GetChildren()
        {
            if (this.Children != null)
            {
                return this.Children;
            }

            return null;
        }
        private IEnumerable<TreeModel> GetAscendingHierarchy()
        {
            var vm = this;

            yield return vm;
            while (vm.Parent != null)
            {
                yield return Parent;
                vm = vm.Parent;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
