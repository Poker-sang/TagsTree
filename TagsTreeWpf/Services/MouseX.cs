using System;
using System.Windows;

namespace TagsTreeWpf.Services
{
    public static class MouseX
    {
        /// <summary>
        /// 鼠标上一次的位置
        /// </summary>
        public static Point LastMousePos { get; set; }

        /// <summary>
        /// 鼠标位移是否超过一定距离
        /// </summary>
        /// <param name="distance">位移阈值</param>
        /// <param name="currentPos">现在鼠标位置</param>
        /// <returns>是否超过阈值</returns>
        public static bool MouseDisplace(double distance, Point currentPos) => Math.Abs(currentPos.X - LastMousePos.X) > distance || Math.Abs(currentPos.Y - LastMousePos.Y) > distance;
    }
}