using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using LandscapeToolkit;

class Program
{
    static void Main(string[] args)
    {
        string outputDir = "docs/assets/icons";
        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        Console.WriteLine($"Exporting high-res icons (128x128) to {outputDir}...");

        try {
            // Modeling
            SaveIcon(Icons.WithBackground(Icons.CategoryColors.Modeling, Icons.DrawRoadNetwork), "road_network", outputDir);
            SaveIcon(Icons.WithBackground(Icons.CategoryColors.Modeling, Icons.DrawTerrain), "terrain", outputDir);
            SaveIcon(Icons.WithBackground(Icons.CategoryColors.Modeling, Icons.DrawPlotGenerator), "plot_generator", outputDir);
            SaveIcon(Icons.WithBackground(Icons.CategoryColors.Modeling, Icons.DrawSteps), "steps", outputDir);
            SaveIcon(Icons.WithBackground(Icons.CategoryColors.Modeling, Icons.DrawWall), "wall", outputDir);
            SaveIcon(Icons.WithBackground(Icons.CategoryColors.Modeling, Icons.DrawBoardwalk), "boardwalk", outputDir);
            SaveIcon(Icons.WithBackground(Icons.CategoryColors.Modeling, Icons.DrawScatter), "scatter", outputDir);
            SaveIcon(Icons.WithBackground(Icons.CategoryColors.Modeling, Icons.DrawMinimalSurface), "minimal_surface", outputDir);

            // Analysis
            SaveIcon(Icons.WithBackground(Icons.CategoryColors.Analysis, Icons.DrawSlopeAnalysis), "slope_analysis", outputDir);
            SaveIcon(Icons.WithBackground(Icons.CategoryColors.Analysis, Icons.DrawSolarAnalysis), "solar_analysis", outputDir);
            SaveIcon(Icons.WithBackground(Icons.CategoryColors.Analysis, Icons.DrawWindShadowAnalysis), "wind_shadow_analysis", outputDir);
            SaveIcon(Icons.WithBackground(Icons.CategoryColors.Analysis, Icons.DrawCarbonAnalysis), "carbon_analysis", outputDir);
            
            // Hydrology
            SaveIcon(Icons.WithBackground(Icons.CategoryColors.Hydrology, Icons.DrawHydrology), "hydrology", outputDir);

            // Optimization
            SaveIcon(Icons.WithBackground(Icons.CategoryColors.Optimization, Icons.DrawPathOptimizer), "path_optimizer", outputDir);
            SaveIcon(Icons.WithBackground(Icons.CategoryColors.Optimization, Icons.DrawWoolyPathOptimizer), "wooly_path_optimizer", outputDir);

            // Utility
            SaveIcon(Icons.WithBackground(Icons.CategoryColors.Utility, Icons.DrawRhinoPicker), "rhino_picker", outputDir);
            SaveIcon(Icons.WithBackground(Icons.CategoryColors.Utility, Icons.DrawChangelog), "changelog", outputDir);
            SaveIcon(Icons.WithBackground(Icons.CategoryColors.Utility, Icons.DrawProjectDoc), "project_doc", outputDir);
            SaveIcon(Icons.WithBackground(Icons.CategoryColors.Utility, Icons.DrawUIStandards), "ui_standards", outputDir);

            // Workflows (Using Modeling Color)
            SaveIcon(Icons.WithBackground(Icons.CategoryColors.Modeling, Icons.DrawSketchToRoad), "sketch_to_road", outputDir);
            SaveIcon(Icons.WithBackground(Icons.CategoryColors.Modeling, Icons.DrawTerrainPlanting), "terrain_planting", outputDir);

            // Core Logic Impl (Using Utility Color)
            SaveIcon(Icons.WithBackground(Icons.CategoryColors.Utility, Icons.DrawRoadNetwork), "roads_impl", outputDir);
            SaveIcon(Icons.WithBackground(Icons.CategoryColors.Utility, Icons.DrawTerrain), "surfaces_impl", outputDir);
            SaveIcon(Icons.WithBackground(Icons.CategoryColors.Utility, Icons.DrawSteps), "features_impl", outputDir);
            SaveIcon(Icons.WithBackground(Icons.CategoryColors.Utility, Icons.DrawSolarAnalysis), "analysis_impl", outputDir);

            // Logo (Keep original)
            SaveIcon(Icons.DrawMainIcon, "logo", outputDir);
            
        } catch (Exception ex) {
            Console.WriteLine("Error: " + ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
    }

    static void SaveIcon(Action<Graphics> drawAction, string name, string dir)
    {
        if (drawAction == null) 
        {
            Console.WriteLine($"Warning: Icon action for {name} is null.");
            return;
        }
        
        // Create 128x128 high-res icon
        using (Bitmap icon = Icons.CreateIcon(drawAction, 128, 128))
        {
            string filePath = Path.Combine(dir, name + ".png");
            // Ensure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            
            icon.Save(filePath, ImageFormat.Png);
            Console.WriteLine($"Saved {filePath}");
        }
    }
}
