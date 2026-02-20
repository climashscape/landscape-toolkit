using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace LandscapeToolkit
{
    public static class Icons
    {
        public static class CategoryColors
        {
            public static Color Modeling = Color.ForestGreen;
            public static Color Analysis = Color.OrangeRed;
            public static Color Hydrology = Color.DodgerBlue;
            public static Color Optimization = Color.Purple;
            public static Color Utility = Color.Gray;
        }

        public static Action<Graphics> WithBackground(Color bgColor, Action<Graphics> foregroundAction)
        {
            return g =>
            {
                // Calculate gradient colors for 3D effect
                // Lighter top-left, Darker bottom-right
                Color lightColor = Color.FromArgb(
                    Math.Min(255, bgColor.R + 40),
                    Math.Min(255, bgColor.G + 40),
                    Math.Min(255, bgColor.B + 40));
                Color darkColor = Color.FromArgb(
                    Math.Max(0, bgColor.R - 40),
                    Math.Max(0, bgColor.G - 40),
                    Math.Max(0, bgColor.B - 40));

                int w = 24;
                int h = 24;
                // Full size drawing area, no margin
                // We use PenAlignment.Inset for the border to prevent clipping
                RectangleF rect = new RectangleF(0, 0, w, h);
                float radius = 4f;
                float d = radius * 2;
                
                GraphicsPath path = new GraphicsPath();
                path.AddArc(rect.X, rect.Y, d, d, 180, 90);
                path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
                path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
                path.CloseFigure();

                // Fill with Gradient
                using (LinearGradientBrush brush = new LinearGradientBrush(rect, lightColor, darkColor, 45f))
                {
                    g.FillPath(brush, path);
                }
                
                // Add a subtle border/bezel with Inset alignment
                using (Pen pen = new Pen(darkColor, 1))
                {
                    pen.Alignment = PenAlignment.Inset;
                    g.DrawPath(pen, path);
                }

                // Inner highlight (optional, for extra "glassy" or "plastic" look)
                // Draw a faint white arc at top
                // using (Pen p = new Pen(Color.FromArgb(100, 255, 255, 255), 1))
                // {
                //     g.DrawArc(p, 2, 2, w - 4, h - 4, 180, 90);
                // }
                
                // Draw Foreground
                // We might want to scale down the foreground slightly to fit inside the padding?
                // The current icons assume 24x24 canvas.
                // If we draw them directly, they might touch the edges.
                // Let's apply a slight scale (0.8) and center it.
                
                GraphicsState state = g.Save();
                g.TranslateTransform(12, 12); // Move to center
                g.ScaleTransform(0.8f, 0.8f); // Scale down
                g.TranslateTransform(-12, -12); // Move back
                
                foregroundAction(g);
                
                g.Restore(state);
            };
        }

        public static Bitmap CreateIcon(Action<Graphics> drawAction, int width = 24, int height = 24)
        {
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.Clear(Color.Transparent);

                if (width != 24 || height != 24)
                {
                    float scaleX = width / 24f;
                    float scaleY = height / 24f;
                    g.ScaleTransform(scaleX, scaleY);
                }

                drawAction(g);
            }
            return bmp;
        }

        public static Action<Graphics> DrawMainIcon = g =>
        {
            // Background
            g.FillEllipse(Brushes.ForestGreen, 1, 1, 22, 22);
            // Text LT
            using (Font font = new Font("Arial", 10, FontStyle.Bold))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString("LT", font, Brushes.White, 12, 13, sf);
            }
        };

        public static Action<Graphics> DrawHydrology = g =>
        {
            // Water drop
            Point[] points = { new Point(12, 2), new Point(18, 14), new Point(12, 22), new Point(6, 14) };
            g.FillClosedCurve(Brushes.White, points, FillMode.Winding, 0.5f);
        };

        public static Action<Graphics> DrawSlopeAnalysis = g =>
        {
            // Triangle slope
            Point[] points = { new Point(2, 22), new Point(22, 22), new Point(22, 2) };
            g.FillPolygon(Brushes.White, points);
            // Gradient lines
            using (Pen p = new Pen(Color.OrangeRed, 2))
            {
                g.DrawLine(p, 8, 22, 22, 8);
                g.DrawLine(p, 14, 22, 22, 14);
            }
        };

        public static Action<Graphics> DrawSolarAnalysis = g =>
        {
            // Sun
            g.FillEllipse(Brushes.White, 6, 6, 12, 12);
            // Rays
            using (Pen p = new Pen(Color.White, 2))
            {
                for (int i = 0; i < 8; i++)
                {
                    double angle = i * Math.PI / 4;
                    float x1 = 12 + (float)(Math.Cos(angle) * 7);
                    float y1 = 12 + (float)(Math.Sin(angle) * 7);
                    float x2 = 12 + (float)(Math.Cos(angle) * 10);
                    float y2 = 12 + (float)(Math.Sin(angle) * 10);
                    g.DrawLine(p, x1, y1, x2, y2);
                }
            }
        };

        public static Action<Graphics> DrawSteps = g =>
        {
            // Stairs
            Point[] points = { 
                new Point(2, 22), new Point(8, 22), new Point(8, 16), 
                new Point(14, 16), new Point(14, 10), new Point(20, 10), 
                new Point(20, 4), new Point(22, 4), new Point(22, 22)
            };
            g.FillPolygon(Brushes.White, points);
        };

        public static Action<Graphics> DrawWall = g =>
        {
            // Brick wall pattern
            g.FillRectangle(Brushes.White, 2, 6, 20, 12);
            using (Pen p = new Pen(Color.ForestGreen, 1)) // Use background color for lines to "cut out"
            {
                g.DrawLine(p, 2, 10, 22, 10);
                g.DrawLine(p, 2, 14, 22, 14);
                g.DrawLine(p, 8, 6, 8, 10);
                g.DrawLine(p, 16, 6, 16, 10);
                g.DrawLine(p, 5, 10, 5, 14);
                g.DrawLine(p, 13, 10, 13, 14);
                g.DrawLine(p, 21, 10, 21, 14);
                g.DrawLine(p, 8, 14, 8, 18);
                g.DrawLine(p, 16, 14, 16, 18);
            }
        };

        public static Action<Graphics> DrawRoadNetwork = g =>
        {
            // Road intersection
            g.FillRectangle(Brushes.White, 8, 0, 8, 24);
            g.FillRectangle(Brushes.White, 0, 8, 24, 8);
            // Dashed lines
            Pen dashPen = new Pen(Color.ForestGreen, 1) // Use bg color
            {
                DashPattern = new float[] { 2, 2 }
            };
            g.DrawLine(dashPen, 12, 0, 12, 24);
            g.DrawLine(dashPen, 0, 12, 24, 12);
        };

        public static Action<Graphics> DrawBoardwalk = g =>
        {
            // Wooden planks path
            Point[] path = { new Point(6, 22), new Point(10, 14), new Point(16, 8), new Point(22, 2) };
            // Draw path (thick line)
            using (Pen p = new Pen(Color.White, 6))
            {
                g.DrawCurve(p, path);
            }
            // Draw planks (perpendicular lines) - use bg color
            using (Pen p = new Pen(Color.ForestGreen, 1))
            {
                g.DrawLine(p, 4, 20, 8, 24);
                g.DrawLine(p, 8, 16, 12, 12);
                g.DrawLine(p, 14, 10, 18, 6);
            }
        };

        public static Action<Graphics> DrawScatter = g =>
        {
            // Random dots
            Random r = new Random(42);
            for (int i = 0; i < 15; i++)
            {
                int x = r.Next(2, 22);
                int y = r.Next(2, 22);
                g.FillEllipse(Brushes.White, x, y, 3, 3);
            }
        };

        public static Action<Graphics> DrawMinimalSurface = g =>
        {
            // Minimal Surface (Saddle shape)
            using (Pen p = new Pen(Color.White, 2f))
            {
                g.DrawArc(p, 2, 2, 20, 10, 180, 180); // Top curve
                g.DrawArc(p, 2, 12, 20, 10, 0, 180);  // Bottom curve
                g.DrawLine(p, 2, 7, 2, 17);           // Left connector
                g.DrawLine(p, 22, 7, 22, 17);         // Right connector
            }
        };

        public static Action<Graphics> DrawPlotGenerator = g =>
        {
            // Plots
            using (Pen p = new Pen(Color.White, 2))
            {
                g.DrawRectangle(p, 2, 2, 9, 9);
                g.DrawRectangle(p, 13, 2, 9, 9);
                g.DrawRectangle(p, 2, 13, 9, 9);
                g.DrawRectangle(p, 13, 13, 9, 9);
            }
        };

        public static Action<Graphics> DrawTerrain = g =>
        {
            // Mountains
            Point[] p1 = { new Point(2, 22), new Point(10, 6), new Point(18, 22) };
            Point[] p2 = { new Point(10, 22), new Point(16, 10), new Point(22, 22) };
            g.FillPolygon(Brushes.White, p1);
            g.FillPolygon(Brushes.White, p2);
            // Outline to separate
             using (Pen p = new Pen(Color.ForestGreen, 1))
             {
                 g.DrawPolygon(p, p1);
                 g.DrawPolygon(p, p2);
             }
        };

        public static Action<Graphics> DrawPathOptimizer = g =>
        {
            // Path finding
            g.FillEllipse(Brushes.White, 2, 18, 4, 4); // Start
            g.FillEllipse(Brushes.White, 18, 2, 4, 4); // End
            // Path
            using (Pen p = new Pen(Color.White, 2))
            {
                Point[] path = { new Point(4, 20), new Point(10, 18), new Point(14, 6), new Point(20, 4) };
                g.DrawCurve(p, path);
            }
        };

        public static Action<Graphics> DrawWoolyPathOptimizer = g =>
        {
            // Wooly path
            g.FillEllipse(Brushes.White, 2, 18, 4, 4); // Start
            g.FillEllipse(Brushes.White, 18, 2, 4, 4); // End
            // Path
            using (Pen p = new Pen(Color.White, 2))
            {
                 Point[] path = { new Point(4, 20), new Point(8, 12), new Point(16, 14), new Point(20, 4) };
                 g.DrawCurve(p, path);
            }
        };

        public static Action<Graphics> DrawRhinoPicker = g =>
        {
            // Cursor + Box
            g.DrawRectangle(Pens.White, 4, 4, 16, 16);
            Point[] arrow = { new Point(14, 14), new Point(22, 22), new Point(14, 22) };
            g.FillPolygon(Brushes.White, arrow);
        };

        public static Action<Graphics> DrawCarbonAnalysis = g =>
        {
            // Tree + C
            g.FillEllipse(Brushes.White, 6, 2, 12, 12); // Tree top
            g.FillRectangle(Brushes.White, 10, 14, 4, 8); // Trunk
            // CO2 cloud
            using (Font font = new Font("Arial", 8, FontStyle.Bold))
            {
                 g.DrawString("CO2", font, Brushes.OrangeRed, 0, 0); // Use bg color
            }
        };

        public static Action<Graphics> DrawWindShadowAnalysis = g =>
        {
            // Wind -> Box -> Shadow
            g.FillRectangle(Brushes.White, 10, 8, 4, 8); // Obstacle
            // Wind lines
            using (Pen p = new Pen(Color.White, 1))
            {
                g.DrawLine(p, 2, 10, 8, 10);
                g.DrawLine(p, 2, 14, 8, 14);
                // Shadow area
                g.DrawLine(p, 14, 10, 22, 6);
                g.DrawLine(p, 14, 14, 22, 18);
            }
        };

        // Standard GH Component Properties
        public static readonly Bitmap MainIcon = CreateIcon(WithBackground(CategoryColors.Modeling, DrawMainIcon));
        
        // Modeling
        public static readonly Bitmap RoadNetwork = CreateIcon(WithBackground(CategoryColors.Modeling, DrawRoadNetwork));
        public static readonly Bitmap Terrain = CreateIcon(WithBackground(CategoryColors.Modeling, DrawTerrain));
        public static readonly Bitmap PlotGenerator = CreateIcon(WithBackground(CategoryColors.Modeling, DrawPlotGenerator));
        public static readonly Bitmap Steps = CreateIcon(WithBackground(CategoryColors.Modeling, DrawSteps));
        public static readonly Bitmap Wall = CreateIcon(WithBackground(CategoryColors.Modeling, DrawWall));
        public static readonly Bitmap Boardwalk = CreateIcon(WithBackground(CategoryColors.Modeling, DrawBoardwalk));
        public static readonly Bitmap Scatter = CreateIcon(WithBackground(CategoryColors.Modeling, DrawScatter));
        public static readonly Bitmap MinimalSurface = CreateIcon(WithBackground(CategoryColors.Modeling, DrawMinimalSurface));
        
        // Analysis
        public static readonly Bitmap SlopeAnalysis = CreateIcon(WithBackground(CategoryColors.Analysis, DrawSlopeAnalysis));
        public static readonly Bitmap SolarAnalysis = CreateIcon(WithBackground(CategoryColors.Analysis, DrawSolarAnalysis));
        public static readonly Bitmap WindShadowAnalysis = CreateIcon(WithBackground(CategoryColors.Analysis, DrawWindShadowAnalysis));
        public static readonly Bitmap CarbonAnalysis = CreateIcon(WithBackground(CategoryColors.Analysis, DrawCarbonAnalysis));
        
        // Hydrology
        public static readonly Bitmap Hydrology = CreateIcon(WithBackground(CategoryColors.Hydrology, DrawHydrology));
        
        // Optimization
        public static readonly Bitmap PathOptimizer = CreateIcon(WithBackground(CategoryColors.Optimization, DrawPathOptimizer));
        public static readonly Bitmap WoolyPathOptimizer = CreateIcon(WithBackground(CategoryColors.Optimization, DrawWoolyPathOptimizer));
        
        // Utility
        public static readonly Bitmap RhinoPicker = CreateIcon(WithBackground(CategoryColors.Utility, DrawRhinoPicker));
    }
}
