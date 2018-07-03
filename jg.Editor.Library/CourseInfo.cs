
namespace jg.Editor.Library
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    class CourseInfo
    {
        ObservableCollection<PageInfo> PageList { get; set; }
    }
    class PageInfo
    {
        ObservableCollection<IAssetInfo> AssetList { get; set; }
    }


    interface IAssetInfo
    {
        // 素材编号
        Guid ItemId { get; set; }

        string ItemName { get; set; }

        // 素材类型
        AssetType AssetType { get; set; }

        // 缩略图
        string Thumbnails { get; set; }

        // 素材存储路径（本地路径）
        string AssetPath { get; set; }

        double TimeLength { get; set; }

    }

    // 同一时间段的联合动作
    public class TimeAction
    {
        public int TimeStart;
        public int TimeLength;

        ObservableCollection<IAction> ActionList { get; set; }
    }

    // 位移动作
    public class MoveAction : IAction
    {
        public void Start()
        {

        }
    }
    
    // 动作接口
    public interface IAction
    {
        void Start();
    }
}
