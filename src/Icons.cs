using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace LandscapeToolkit
{
    public static class Icons
    {
        private static Bitmap CreateIcon(Action<Graphics> drawAction)
        {
            Bitmap bmp = new Bitmap(24, 24);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                drawAction(g);
            }
            return bmp;
        }

        public static Bitmap MainIcon => CreateIcon(g =>
        {
            // Background
            g.FillEllipse(Brushes.ForestGreen, 1, 1, 22, 22);
            // Text LT
            using (Font font = new Font("Arial", 10, FontStyle.Bold))
            {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                g.DrawString("LT", font, Brushes.White, 12, 13, sf);
            }
        });

        public static Bitmap Hydrology => CreateIcon(g =>
        {
            // Water drop
            Point[] points = { new Point(12, 2), new Point(18, 14), new Point(12, 22), new Point(6, 14) };
            g.FillClosedCurve(Brushes.DodgerBlue, points, FillMode.Winding, 0.5f);
        });

        public static Bitmap SlopeAnalysis => CreateIcon(g =>
        {
            // Triangle slope
            Point[] points = { new Point(2, 22), new Point(22, 22), new Point(22, 2) };
            g.FillPolygon(Brushes.OrangeRed, points);
            // Gradient lines
            g.DrawLine(Pens.White, 8, 22, 22, 8);
            g.DrawLine(Pens.White, 14, 22, 22, 14);
        });

        public static Bitmap SolarAnalysis => CreateIcon(g =>
        {
            // Sun
            g.FillEllipse(Brushes.Gold, 6, 6, 12, 12);
            // Rays
            for (int i = 0; i < 8; i++)
            {
                double angle = i * Math.PI / 4;
                float x1 = 12 + (float)(Math.Cos(angle) * 7);
                float y1 = 12 + (float)(Math.Sin(angle) * 7);
                float x2 = 12 + (float)(Math.Cos(angle) * 10);
                float y2 = 12 + (float)(Math.Sin(angle) * 10);
                g.DrawLine(Pens.Orange, x1, y1, x2, y2);
            }
        });

        public static Bitmap Steps => CreateIcon(g =>
        {
            // Stairs
            Point[] points = { 
                new Point(2, 22), new Point(8, 22), new Point(8, 16), 
                new Point(14, 16), new Point(14, 10), new Point(20, 10), 
                new Point(20, 4), new Point(22, 4), new Point(22, 22)
            };
            g.FillPolygon(Brushes.Gray, points);
        });

        public static Bitmap Wall => CreateIcon(g =>
        {
            // Brick wall pattern
            g.FillRectangle(Brushes.Firebrick, 2, 6, 20, 12);
            g.DrawLine(Pens.White, 2, 10, 22, 10);
            g.DrawLine(Pens.White, 2, 14, 22, 14);
            g.DrawLine(Pens.White, 8, 6, 8, 10);
            g.DrawLine(Pens.White, 16, 6, 16, 10);
            g.DrawLine(Pens.White, 5, 10, 5, 14);
            g.DrawLine(Pens.White, 13, 10, 13, 14);
            g.DrawLine(Pens.White, 21, 10, 21, 14);
            g.DrawLine(Pens.White, 8, 14, 8, 18);
            g.DrawLine(Pens.White, 16, 14, 16, 18);
        });

        public static Bitmap RoadNetwork => CreateIcon(g =>
        {
            // Road intersection
            g.FillRectangle(Brushes.DimGray, 8, 0, 8, 24);
            g.FillRectangle(Brushes.DimGray, 0, 8, 24, 8);
            // Dashed lines
            Pen dashPen = new Pen(Color.White, 1);
            dashPen.DashStyle = DashStyle.Dash;
            g.DrawLine(dashPen, 12, 0, 12, 24);
            g.DrawLine(dashPen, 0, 12, 24, 12);
        });

        public static Bitmap PlotGenerator => CreateIcon(g =>
        {
            // Plots
            g.FillRectangle(Brushes.LightGreen, 2, 2, 9, 9);
            g.FillRectangle(Brushes.LightGreen, 13, 2, 9, 9);
            g.FillRectangle(Brushes.LightGreen, 2, 13, 9, 9);
            g.FillRectangle(Brushes.LightGreen, 13, 13, 9, 9);
            g.DrawRectangle(Pens.DarkGreen, 2, 2, 9, 9);
            g.DrawRectangle(Pens.DarkGreen, 13, 2, 9, 9);
            g.DrawRectangle(Pens.DarkGreen, 2, 13, 9, 9);
            g.DrawRectangle(Pens.DarkGreen, 13, 13, 9, 9);
        });

        public static Bitmap Terrain => CreateIcon(g =>
        {
            // Mountains
            Point[] p1 = { new Point(2, 22), new Point(10, 6), new Point(18, 22) };
            Point[] p2 = { new Point(10, 22), new Point(16, 10), new Point(22, 22) };
            g.FillPolygon(Brushes.SaddleBrown, p1);
            g.FillPolygon(Brushes.Sienna, p2);
        });

        public static Bitmap PathOptimizer => CreateIcon(g =>
        {
            // Path finding
            g.FillEllipse(Brushes.Red, 2, 18, 4, 4); // Start
            g.FillEllipse(Brushes.Green, 18, 2, 4, 4); // End
            // Path
            Point[] path = { new Point(4, 20), new Point(10, 18), new Point(14, 6), new Point(20, 4) };
            g.DrawCurve(Pens.Blue, path);
        });

        public static Bitmap WoolyPathOptimizer => CreateIcon(g =>
        {
            // Wooly path
            g.FillEllipse(Brushes.Red, 2, 18, 4, 4); // Start
            g.FillEllipse(Brushes.Green, 18, 2, 4, 4); // End
            // Path
            using (Pen p = new Pen(Color.Purple, 2))
            {
                 Point[] path = { new Point(4, 20), new Point(8, 12), new Point(16, 14), new Point(20, 4) };
                 g.DrawCurve(p, path);
            }
        });
    }
}
