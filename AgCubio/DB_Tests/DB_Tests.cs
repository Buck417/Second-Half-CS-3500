using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Database_Controller;
using System.Collections.Generic;

namespace DB_Tests
{
    [TestClass]
    public class DB_Tests
    {
        /// <summary>
        /// Even though the game server isn't working with the database at this point,
        /// we wrote this test to show that the database does work.
        /// </summary>
        [TestMethod]
        public void TestAddGame()
        {
            Game game = new Game(0, 25, 1987, 1301, DateTime.Now, "Richard", 1);
            List<Player_Eaten> eaten = new List<Player_Eaten>();
            eaten.Add(new Player_Eaten("Theo"));
            eaten.Add(new Player_Eaten("Amanda"));
            Database.AddGameToDB(game, eaten);

            Game game2 = new Game(0, 26, 1998, 1297, DateTime.Now, "Ryan", 2);
            List<Player_Eaten> eaten2 = new List<Player_Eaten>();
            eaten.Add(new Player_Eaten("Jake"));
            eaten.Add(new Player_Eaten("Tiffany"));
            Database.AddGameToDB(game2, eaten2);

            Game game3 = new Game(0, 22, 1000, 1001, DateTime.Now, "Dave", 3);
            List<Player_Eaten> eaten3 = new List<Player_Eaten>();
            eaten.Add(new Player_Eaten("Romeo"));
            eaten.Add(new Player_Eaten("Juliet"));
            Database.AddGameToDB(game3, eaten3);

            Game game4 = new Game(0, 21, 999, 1000, DateTime.Now, "John", 4);
            List<Player_Eaten> eaten4 = new List<Player_Eaten>();
            eaten.Add(new Player_Eaten("Ronald"));
            eaten.Add(new Player_Eaten("Ronnie"));
            Database.AddGameToDB(game4, eaten4);

            Game game5 = new Game(0, 20, 980, 900, DateTime.Now, "Sally", 5);
            List<Player_Eaten> eaten5 = new List<Player_Eaten>();
            eaten.Add(new Player_Eaten("Jennifer"));
            eaten.Add(new Player_Eaten("Chris"));
            eaten.Add(new Player_Eaten("Christina"));
            Database.AddGameToDB(game5, eaten5);

            LinkedList<Game> high_scores = Database.GetHighScores();
            int i = 1;
            foreach(Game score in high_scores)
            {
                switch (score.player_name)
                {
                    case "Richard":
                        Assert.AreEqual(1, score.rank);
                        Assert.AreEqual(1, i);
                        i++;
                        Database.Delete_Game(score.game_id);
                        break;
                    case "Ryan":
                        Assert.AreEqual(2, score.rank);
                        Assert.AreEqual(2, i);
                        i++;
                        Database.Delete_Game(score.game_id);
                        break;
                    case "Dave":
                        Assert.AreEqual(3, score.rank);
                        Assert.AreEqual(3, i);
                        i++;
                        Database.Delete_Game(score.game_id);
                        break;
                    case "John":
                        Assert.AreEqual(4, score.rank);
                        Assert.AreEqual(4, i);
                        i++;
                        Database.Delete_Game(score.game_id);
                        break;
                    case "Sally":
                        Assert.AreEqual(5, score.rank);
                        Assert.AreEqual(5, i);
                        i++;
                        Database.Delete_Game(score.game_id);
                        break;
                    default:
                        Assert.Fail(score.player_name + " shouldn't be in the top 5 high scores.");
                        break;
                }
            }
            Assert.AreEqual(5, high_scores.Count);
        }
    }
}
