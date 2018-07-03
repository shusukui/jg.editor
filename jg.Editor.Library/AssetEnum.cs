using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jg.Editor.Library
{
    //素材类型
    public enum AssetType
    {
        /// <summary>
        /// 图片
        /// </summary>
        Image,
        /// <summary>
        /// 视频
        /// </summary>
        Movie,
     
        /// <summary>
        /// 声音
        /// </summary>
        Sound,
        /// <summary>
        /// 行状
        /// </summary>
        Shape,
        /// <summary>
        /// 线条
        /// </summary>
        Line,
        /// <summary>
        /// 选择题
        /// </summary>
        Topic,
        /// <summary>
        /// 连线题
        /// </summary>
        TopicDrag,
        /// <summary>
        /// 文本
        /// </summary>
        Text,
        /// <summary>
        /// 舞台
        /// </summary>
        Stage,
        /// <summary>
        /// 文本表格
        /// </summary>
        TextGrid,
        /// <summary>
        /// 文档
        /// </summary>
        Document,
        /// <summary>
        /// 消息框
        /// </summary>
        Message,
        /// <summary>
        /// HTML5
        /// </summary>
        HTML5,
        /// <summary>
        /// 图片组控件
        /// </summary>
        TPageGroup
    }

    //所有属性
    public enum AssetProperty
    {
        /// <summary>
        /// 舞台
        /// </summary>
        Stage,
        /// <summary>
        /// 字体
        /// </summary>
        Font,
        /// <summary>
        /// 布局
        /// </summary>
        Location,
        /// <summary>
        /// 变形
        /// </summary>
        Transform,
        /// <summary>
        /// 前景色
        /// </summary>
        ForeColor,
        /// <summary>
        /// 背景色
        /// </summary>
        BackColor,
        /// <summary>
        /// 动作
        /// </summary>
        Action,
        /// <summary>
        /// 表格控件
        /// </summary>
        TextGrid,
        /// <summary>
        /// 判断题、选择题
        /// </summary>
        Topic,
        /// <summary>
        /// 拖拽题
        /// </summary>
        TopicDrag,
        /// <summary>
        /// 线条控件
        /// </summary>
        Line,

        /// <summary>
        /// 图片组控件
        /// </summary>
        TPageGroup

    }
    //素材事件
    public enum enumAssetEvent
    {
        /// <summary>
        /// 鼠标进入
        /// </summary>
        MouseEnter = 0,
        /// <summary>
        /// 鼠标点击
        /// </summary>
        MouseClick = 1,
    }

    //素材动作
    public enum enumAssetAction
    {
        /// <summary>
        /// 淡入淡出
        /// </summary>
        Fade = 0,
        /// <summary>
        /// 左侧飞入
        /// </summary>
        Left = 1,
        /// <summary>
        /// 右侧飞入
        /// </summary>
        Right = 2,
        /// <summary>
        /// 页面跳转
        /// </summary>
        PageJump = 3

    }

    //舞台间的切换动作
    public enum enumStageSwitch
    {
        /// <summary>
        /// 无效果
        /// </summary>
        SwitchFF00003D = 0,
        /// <summary>
        /// 淡出
        /// </summary>
        SwitchFF00003E = 1,
        /// <summary>
        /// 推进
        /// </summary>
        SwitchFF00003F = 2,
        /// <summary>
        /// 擦除
        /// </summary>
        SwitchFF000040 = 3,
        /// <summary>
        /// 分割
        /// </summary>
        SwitchFF00008C = 4
    }

}
