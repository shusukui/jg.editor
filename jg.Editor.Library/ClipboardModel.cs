using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jg.Editor.Library
{
    [Serializable]
    public class Clipboard_TreeViewInfo
    {
        public Clipboard_TreeViewInfo()
        {
            SavePageList = new List<SavePageInfo>();
        }
        public TreeViewItemInfo TreeViewItemInfo { get; set; }
        public List<SavePageInfo> SavePageList { get; set; }
    }

}