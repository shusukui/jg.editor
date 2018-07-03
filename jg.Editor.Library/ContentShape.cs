using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jg.Editor.Library
{
    /// <summary>
    /// 素材内容为内置形状类型的类
    /// </summary>
    [Serializable]
    public class ContentShape : abstractContent
    {
        public string Id { get; set; }
    }
}
