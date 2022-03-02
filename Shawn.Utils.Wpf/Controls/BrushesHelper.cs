using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Shawn.Utils.Wpf.Image;

namespace Shawn.Utils.Wpf.Controls
{
    public static class BrushesHelper
    {
        private static readonly object Obj = new object();
        private static readonly Dictionary<int, ImageBrush> ChessboardBrushes = new Dictionary<int, ImageBrush>();

        public static ImageBrush ChessboardBrush(int blockPixSize = 32)
        {
            lock (Obj)
            {
                if (ChessboardBrushes.ContainsKey(blockPixSize))
                    return ChessboardBrushes[blockPixSize];
                // 绘制透明背景
                var wpen = System.Drawing.Brushes.White;
                var gpen = System.Drawing.Brushes.LightGray;
                int span = blockPixSize;
                var bg = new System.Drawing.Bitmap(span * 2, span * 2);
                using (var g = System.Drawing.Graphics.FromImage(bg))
                {
                    g.FillRectangle(wpen, new System.Drawing.Rectangle(0, 0, bg.Width, bg.Height));
                    for (var v = 0; v < span * 2; v += span)
                    {
                        for (int h = (v / (span)) % 2 == 0 ? 0 : span; h < span * 2; h += span * 2)
                        {
                            g.FillRectangle(gpen, new System.Drawing.Rectangle(h, v, span, span));
                        }
                    }
                }

                var b = new ImageBrush(bg.ToBitmapImage())
                {
                    Stretch = Stretch.None,
                    TileMode = TileMode.Tile,
                    AlignmentX = AlignmentX.Left,
                    AlignmentY = AlignmentY.Top,
                    Viewport = new Rect(new Point(0, 0), new Point(span * 2, span * 2)),
                    ViewportUnits = BrushMappingMode.Absolute
                };
                ChessboardBrushes.Add(blockPixSize, b);
                return b;
            }
        }
    }
}
