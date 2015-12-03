using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;

namespace ModelTests
{
    [TestClass]
    public class UnitTest1
    {
        /*************************************** CUBE TESTS ******************************************/
        [TestMethod]
        public void TestCubeLeft()
        {
            Cube c = new Cube(200, 2, 2, 2, 2, false, "", 2);
            Assert.AreEqual(200, c.Left);
        }

        [TestMethod]
        public void TestCubeRight()
        {
            Cube c = new Cube(200, 2, 2, 2, 2, false, "", 9);
            Assert.AreEqual(c.Right, 203);
        }

        [TestMethod]
        public void TestCubeTop()
        {
            Cube c = new Cube(0, 100, 2, 2, 2, false, "", 9);
            Assert.AreEqual(100, c.Top);
        }

        [TestMethod]
        public void TestCubeBottom()
        {
            Cube c = new Cube(0, 100, 2, 2, 2, false, "", 9);
            Assert.AreEqual(103, c.Bottom);
        }

        [TestMethod]
        public void TestCubeCopy()
        {
            Cube a = new Cube(0, 100, 2, 2, 2, false, "Hey", 9);
            Cube b = Cube.Copy(a);
            Assert.AreEqual(a.Top, b.Top);
            Assert.AreEqual(a.Bottom, b.Bottom);
            Assert.AreEqual(a.Left, b.Left);
            Assert.AreEqual(a.Right, b.Right);
            Assert.AreEqual(a.Color, b.Color);
            Assert.AreEqual(a.Food, b.Food);
            Assert.AreEqual(a.GetCenterX(), b.GetCenterX());
            Assert.AreEqual(a.GetCenterY(), b.GetCenterY());
            Assert.AreEqual(a.IsVirus(), b.IsVirus());
            Assert.AreEqual(a.Mass, b.Mass);
            Assert.AreEqual(a.Width, b.Width);
            Assert.AreEqual(a.Name, b.Name);
            Assert.AreEqual(a.team_id, b.team_id);
            Assert.AreEqual(a.UID, b.UID);
            Assert.AreEqual(a.X, b.X);
            Assert.AreEqual(a.Y, b.Y);
        }
            
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
        public void TestUpdateWorld()
        {
            World w = new World();
            Cube player = w.AddPlayerCube("Richie");
            w.AddVirusCube();
            for(int i = 0; i < w.MAX_FOOD; i++)
            {
                w.AddFoodCube();
            }

            player.X = 500;
            player.Y = 500;
            System.Diagnostics.Stopwatch s = new System.Diagnostics.Stopwatch();
            s.Start();
            w.Update();
            s.Stop();
            long elapsed = s.ElapsedMilliseconds;
            Assert.AreEqual(true, elapsed < (1000 / w.HEARTBEATS_PER_SECOND));
            Assert.AreEqual(true, w.food_cubes.Count < w.MAX_FOOD);
            Assert.AreEqual(true, player.Mass > w.PLAYER_START_MASS);
        }

        [TestMethod]
        public void TestProcessSlowAttrition()
        {
            World w = new World();
            Cube player = w.AddPlayerCube("Richie");
            player.Mass = 700;
            w.ProcessAttrition();
            Assert.AreEqual(698.75, player.Mass);
        }

        [TestMethod]
        public void TestProcessFastAttrition()
        {
            World w = new World();
            Cube player = w.AddPlayerCube("Richie");
            player.Mass = 1300;
            w.ProcessAttrition();
            Assert.AreEqual(1297.5, player.Mass);
        }

        [TestMethod]
        public void TestGetNextUID()
        {
            World w = new World();
            int uid = w.GetNextUID();
            int uid2 = w.GetNextUID();
            Assert.AreNotEqual(uid, uid2);
            w.ProcessCube(new Cube(0, 0, 0, uid, uid, false, "asdf", 10));
            int uid3 = 0;
            for (int i = 0; i< 100; i++)
            {
                uid3 = w.GetNextUID();
                Assert.AreNotEqual(uid, uid3);
                Assert.AreNotEqual(uid2, uid3);
            }
        }

        [TestMethod]
        public void TestProcessAttritionBelowMinimumMass()
        {
            World w = new World();
            Cube player = w.AddPlayerCube("Richie");
            player.Mass = 100;
            w.ProcessAttrition();
            Assert.AreEqual(100, player.Mass);
        }
            
        /// <summary>
        /// This tests to make sure the gameplay file can be read and used.
        /// </summary>
        [TestMethod]
        public void TestReadGameplayFile()
        {
            World w = new World("world_parameters.xml");
            Assert.AreEqual(999, w.HEIGHT);
            Assert.AreEqual(999, w.WIDTH);
            Assert.AreEqual(19, w.HEARTBEATS_PER_SECOND);
            Assert.AreEqual(2, w.LOW_SPEED);
            Assert.AreEqual(3, w.MAXIMUM_SPLITS);
            Assert.AreEqual(49, w.MAXIMUM_SPLIT_DISTANCE);
            Assert.AreEqual(499, w.MINIMUM_SPLIT_MASS);
            Assert.AreEqual(6, w.TOP_SPEED);
            Assert.AreEqual(999, w.PLAYER_START_MASS);
            Assert.AreEqual(499, w.MAX_FOOD);
            Assert.AreEqual(199, w.ATTRITION_RATE);
            Assert.AreEqual(4, w.FOOD_VALUE);
            Assert.AreEqual(0.33, w.ABSORB_DISTANCE_DELTA);
        }

        [TestMethod]
        public void TestProcessCube()
        {
            World w = new World();
            Cube c = new Cube(0, 0, 3000, 2, 2, false, "hey", 1000);
            w.ProcessCube(c);
            Assert.AreEqual(c, w.GetCube(2));
        }

        [TestMethod]
        public void TestProcessVirusCube()
        {
            World w = new World();
            Cube c = new Cube(0, 0, World.VIRUS_COLOR, 0, 0, true, "", 10);
            w.ProcessCube(c);
            Assert.AreEqual(c, w.GetCube(0));
        }

        [TestMethod]
        public void TestProcessMoveData()
        {
            World w = new World();
            Cube player = w.AddPlayerCube("Richie");
            double startX, startY;
            startX = startY = player.X = player.Y = 100;
            string moveRequest = "(move, 544, 433)\n";
            w.ProcessData(moveRequest);
            Assert.AreNotEqual(startX, player.X);
        }

        
        [TestMethod]
        public void TestAddPlayerCube()
        {
            World w = new World();
            Cube player = w.AddPlayerCube("Richie");
            Assert.AreEqual(w.GetCube(player.UID), player);
            Assert.AreEqual(true, w.player_cubes.Contains(player));
        }
        
        [TestMethod]
        public void TestIsPlayer()
        {
            World world = new World();
            Cube food = new Cube(0, 0, 0, 0, 0, true, "", 10);
            Cube virus = new Cube(0, 0, World.VIRUS_COLOR, 0, 0, true, "", 100);
            Cube player = world.AddPlayerCube("Richie");
            Assert.AreEqual(false, food.IsPlayer());
            Assert.AreEqual(false, virus.IsPlayer());
            Assert.AreEqual(true, player.IsPlayer());
            Assert.AreEqual(false, player.IsFood());
            Assert.AreEqual(false, player.IsVirus());
        }

        [TestMethod]
        public void TestIsFood()
        {
            Cube cube = new Cube(0, 0, 0, 0, 0, false, "", 10);
            Assert.AreEqual(false, cube.IsFood());
        }

        [TestMethod]
        public void TestGetCubeDoesntExist()
        {
            World w = new World();
            Cube c = w.GetCube(200);
            Assert.AreEqual(null, c);
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
