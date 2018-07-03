using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace jg.Editor.Library.Control.ComboTree
{
    public interface ITreeViewItemModel : INotifyPropertyChanged
    {
        string SelectedValuePath { get; set; }

        string DisplayValuePath { get; set; }

        bool IsExpanded { get; set; }

        bool IsSelected { get; set; }

        IEnumerable<ITreeViewItemModel> GetHierarchy();

        IEnumerable<ITreeViewItemModel> GetChildren();
       

    }
}
