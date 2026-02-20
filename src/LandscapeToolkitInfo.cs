using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace LandscapeToolkit
{
    public class LandscapeToolkitInfo : GH_AssemblyInfo
    {
        public override string Name => "Landscape Toolkit";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => Icons.MainIcon;

        //Return a short string describing the library.
        public override string Description => "A toolkit for high-standard landscape modeling and analysis (Class-A surfaces, Quad meshes, Bio-mimetics).";

        public override Guid Id => new Guid("7a8b9c0d-1e2f-3a4b-5c6d-7e8f9a0b1c2d");

        public override string AuthorName => "Landscape Toolkit Team";

        public override string AuthorContact => "";

        public override string Version => "1.2.2";
    }
}
