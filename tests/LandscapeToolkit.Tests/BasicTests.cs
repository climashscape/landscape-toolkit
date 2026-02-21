using NUnit.Framework;
using Rhino.Geometry;

namespace LandscapeToolkit.Tests
{
    public class BasicTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestMath()
        {
            Assert.AreEqual(4, 2 + 2);
        }

        /*
        [Test]
        public void TestPoint3dDistance()
        {
            // Simple RhinoCommon test that usually works without full Rhino context
            // Point3d is a struct and basic math usually works fine.
            var p1 = new Point3d(0, 0, 0);
            var p2 = new Point3d(3, 4, 0);
            Assert.AreEqual(5.0, p1.DistanceTo(p2), 0.0001);
        }
        */
    }
}
