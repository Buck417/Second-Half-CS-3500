using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;

namespace ModelTests
{
    [TestClass]
    public class UnitTest1
    {
        /*************************************** CUBE TESTS ******************************************/
        /// <summary>
        /// This makes sure the cube width is calculated correctly
        /// </summary>
        [TestMethod]
        public void TestCubeWidthCalculation()
        {
            Cube c = new Cube(0, 0, 3000, 2, 2, false, "hey", 1000);
            Assert.AreEqual((int)(Math.Sqrt(1000)), c.Width);
        }

        /// <summary>
        /// This just makes sure that the cube.create() method makes a cube based on the json you give it
        /// </summary>
        [TestMethod]
        public void TestCubeCreate()
        {
            string json = "{\"loc_x\":926.0,\"loc_y\":682.0,\"argb_color\":-65536,\"uid\":5571,\"team_id\":5571,\"food\":false,\"Name\":\"3500 is love\",\"Mass\":1000.0}";
            Cube c = Cube.Create(json);
            Assert.AreEqual(926, c.X);
            Assert.AreEqual(682, c.Y);
            Assert.AreEqual(-65536, c.Color);
            Assert.AreEqual(5571, c.UID);
            Assert.AreEqual(5571, c.team_id);
            Assert.AreEqual(false, c.Food);
            Assert.AreEqual("3500 is love", c.Name);
            Assert.AreEqual(1000, c.Mass);
        }

        /// <summary>
        /// Make sure invalid JSON returns null when trying to call cube.Create()
        /// </summary>
        [TestMethod]
        public void TestCubeCreateInvalidJson()
        {
            string json = "{\"team\": Hey there ";
            Cube c = Cube.Create(json);
            Assert.IsNull(c);
        }

        [TestMethod]
        public void TestGetCenterX()
        {
            string json = "{\"loc_x\":500.0,\"loc_y\":500.0,\"argb_color\":-65536,\"uid\":5571,\"team_id\":5571,\"food\":false,\"Name\":\"3500 is love\",\"Mass\":900.0}";
            Cube c = Cube.Create(json);
            Assert.AreEqual(515, c.GetCenterX());
        }

        [TestMethod]
        public void TestGetCenterY()
        {
            string json = "{\"loc_x\":500.0,\"loc_y\":600.0,\"argb_color\":-65536,\"uid\":5571,\"team_id\":5571,\"food\":false,\"Name\":\"3500 is love\",\"Mass\":900.0}";
            Cube c = Cube.Create(json);
            Assert.AreEqual(615, c.GetCenterY());
        }

        [TestMethod]
        public void TestCubeNotVirus()
        {
            string json = "{\"loc_x\":500.0,\"loc_y\":600.0,\"argb_color\":-65536,\"uid\":5571,\"team_id\":5571,\"food\":false,\"Name\":\"3500 is love\",\"Mass\":900.0}";
            Cube c = Cube.Create(json);
            Assert.AreEqual(false, c.IsVirus());
        }

        [TestMethod]
        public void TestCubeIsVirus()
        {
            Cube c = new Cube(2, 2, World.VIRUS_COLOR, 2, 2, true, "", 1000);
            Assert.AreEqual(true, c.IsVirus());
        }
        /************************************* END CUBE TESTS ****************************************/





        /*************************************** WORLD TESTS *****************************************/
        [TestMethod]
        public void TestProcessCube()
        {
            World w = new World();
            Cube c = new Cube(0, 0, 3000, 2, 2, false, "hey", 1000);
            w.ProcessCube(c);
            Assert.AreEqual(c, w.GetCube(2));
        }

        [TestMethod]
        public void TestAddPlayerCube()
        {
            World w = new World();
            string json = "{\"loc_x\":500.0,\"loc_y\":600.0,\"argb_color\":-65536,\"uid\":5571,\"team_id\":5571,\"food\":false,\"Name\":\"3500 is love\",\"Mass\":900.0}";
            Cube c = Cube.Create(json);
            w.AddPlayerCube(json);
            Assert.AreEqual(c.UID, w.GetPlayerCube().UID);
        }

        [TestMethod]
        public void TestGetCubeDoesntExist()
        {
            World w = new World();
            Cube c = w.GetCube(200);
            Assert.AreEqual(null, c);
        }

        [TestMethod]
        public void TestGetPlayerMass()
        {
            World w = new World();
            string json = "{\"loc_x\":500.0,\"loc_y\":600.0,\"argb_color\":-65536,\"uid\":5571,\"team_id\":5571,\"food\":false,\"Name\":\"3500 is love\",\"Mass\":900.0}";
            w.AddPlayerCube(json);
            Assert.AreEqual(900, w.GetPlayerMass());
        }

        [TestMethod]
        public void TestProcessIncomingCubeZeroMass()
        {
            World world = new World();
            Cube c = new Cube(2, 2, 2, 2, 2, false, "hasf", 10);
            world.ProcessCube(c);
            Assert.AreEqual(2, world.GetCube(2).UID);

            //Now delete the cube
            c = new Cube(2, 2, 2, 2, 2, false, "hasf", 0);
            world.ProcessCube(c);
            Assert.AreEqual(null, world.GetCube(2));
        }

        [TestMethod]
        public void TestGetPlayerCubeDoesntExist()
        {
            World w = new World();
            Cube c = w.GetPlayerCube();
            Assert.AreEqual(null, c);
        }

        [TestMethod]
        public void TestProcessIncomingCubeChangedSize()
        {
            World w = new World();
            Cube c = new Cube(2, 2, 2, 2, 2, false, "hey", 20);
            w.ProcessCube(c);
            c = new Cube(2, 2, 2, 2, 2, false, "hey", 30);
            w.ProcessCube(c);
            Assert.AreEqual(30, w.GetCube(2).Mass);
        }
        /************************************* END WORLD TESTS ***************************************/
    }

}
