using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jg.Editor.Library
{
    public class AssetPropertyList : Dictionary<AssetType, List<AssetProperty>>
    {
        public AssetPropertyList()
        {
            Add(AssetType.Stage, new List<AssetProperty>() { AssetProperty.Stage });
            Add(AssetType.Sound, new List<AssetProperty>() { AssetProperty.Location });
            Add(AssetType.Image, new List<AssetProperty>() { AssetProperty.Location, AssetProperty.Transform, AssetProperty.Action });
            Add(AssetType.Movie, new List<AssetProperty>() { AssetProperty.Location, AssetProperty.Transform, AssetProperty.Action });
            Add(AssetType.Shape, new List<AssetProperty>() { AssetProperty.Location, AssetProperty.Transform, AssetProperty.Action });
            Add(AssetType.Topic, new List<AssetProperty>() { AssetProperty.Location, AssetProperty.Transform, AssetProperty.Font, AssetProperty.ForeColor, AssetProperty.BackColor, AssetProperty.Action, AssetProperty.Topic });
            Add(AssetType.TopicDrag, new List<AssetProperty>() { AssetProperty.Location, AssetProperty.Transform, AssetProperty.Font, AssetProperty.ForeColor, AssetProperty.BackColor, AssetProperty.Action, AssetProperty.TopicDrag });
            Add(AssetType.Text, new List<AssetProperty>() { AssetProperty.Font, AssetProperty.Location, AssetProperty.Transform, AssetProperty.Action, AssetProperty.BackColor, AssetProperty.ForeColor });
            Add(AssetType.TextGrid, new List<AssetProperty>() { AssetProperty.Location, AssetProperty.Transform, AssetProperty.Font, AssetProperty.ForeColor, AssetProperty.BackColor, AssetProperty.Action, AssetProperty.TextGrid });
            Add(AssetType.Message, new List<AssetProperty>() { AssetProperty.Location, AssetProperty.Transform, AssetProperty.Action, AssetProperty.Font, AssetProperty.ForeColor, AssetProperty.BackColor });
            Add(AssetType.Line, new List<AssetProperty>() { AssetProperty.Location, AssetProperty.Transform, AssetProperty.Action, AssetProperty.BackColor, AssetProperty.Line });
            Add(AssetType.TPageGroup, new List<AssetProperty>() { AssetProperty.Location, AssetProperty.Action, AssetProperty.TPageGroup  });
            Add(AssetType.HTML5, new List<AssetProperty>() { AssetProperty.Location, AssetProperty.Transform, AssetProperty.Action });
        }
    }
}
