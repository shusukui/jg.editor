using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jg.Editor.Library
{
    /// <summary>
    /// 素材内容为文本属性的类
    /// </summary>
    [Serializable]
    public class ContentText : abstractContent
    {
        public string Text { get; set; }
    }
}
