using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Collections.ObjectModel;
namespace jg.Editor.Library.Property
{
    public class FontCollection : List<FontFamily>
    {
        public FontCollection()
        {
            System.Drawing.Text.InstalledFontCollection fonts = new System.Drawing.Text.InstalledFontCollection();
            Clear();
            foreach (var family in fonts.Families)
                Add(new FontFamily(family.Name));
        }
    }
}