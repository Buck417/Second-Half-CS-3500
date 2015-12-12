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
            Game game = new Game(0, 25, 1987, 13010, DateTime.Now, "Richard", 1);
            List<Player_Eaten> eaten = new List<Player_Eaten>();
            eaten.Add(new Player_Eaten("Theo"));
            eaten.Add(new Player_Eaten("Amanda"));
            Database.AddGameToDB(game, eaten);

            Game game2 = new Game(0, 26, 1998, 12970, DateTime.Now, "Ryan", 2);
            List<Player_Eaten> eaten2 = new List<Player_Eaten>();
            eaten2.Add(new Player_Eaten("Jake"));
            eaten2.Add(new Player_Eaten("Tiffany"));
            Database.AddGameToDB(game2, eaten2);

            Game game3 = new Game(0, 22, 1000, 10010, DateTime.Now, "Dave", 3);
            List<Player_Eaten> eaten3 = new List<Player_Eaten>();
            eaten3.Add(new Player_Eaten("Romeo"));
            eaten3.Add(new Player_Eaten("Juliet"));
            Database.AddGameToDB(game3, eaten3);

            Game game4 = new Game(0, 21, 999, 10000, DateTime.Now, "John", 4);
            List<Player_Eaten> eaten4 = new List<Player_Eaten>();
            eaten4.Add(new Player_Eaten("Ronald"));
            eaten4.Add(new Player_Eaten("Ronnie"));
            Database.AddGameToDB(game4, eaten4);

            Game game5 = new Game(0, 20, 980, 9000, DateTime.Now, "Sally", 5);
            List<Player_Eaten> eaten5 = new List<Player_Eaten>();
            eaten5.Add(new Player_Eaten("Jennifer"));
            eaten5.Add(new Player_Eaten("Chris"));
            eaten5.Add(new Player_Eaten("Christina"));
            Database.AddGameToDB(game5, eaten5);

            LinkedList<Game> high_scores = Database.GetHighScores();
            int i = 1;

            //Go through each of the high scores and make sure the rank is
            //correct, and that all the players eaten are correctly retrieved
            //from the database.
            foreach(Game score in high_scores)
            {
                switch (score.player_name)
                {
                    case "Richard":
                        Assert.AreEqual(1, score.rank);
                        Assert.AreEqual(1, i);
                        i++;

                        foreach(Player_Eaten player_eaten in Database.GetPlayersEaten(score.game_id))
                        {
                            //Assertions are contrived, the switch statement makes sure the names are the same
                            switch (player_eaten.name)
                            {
                                case "Theo":
                                    Assert.AreEqual(true, true);
                                    break;
                                case "Amanda":
                                    Assert.AreEqual(true, true);
                                    break;
                                default:
                                    Assert.Fail(player_eaten.name + " is not supposed to be on this players eaten list.");
                                    break;
                            }
                        }
                        Database.Delete_Game(score.game_id);
                        break;
                    case "Ryan":
                        Assert.AreEqual(2, score.rank);
                        Assert.AreEqual(2, i);
                        i++;

                        foreach (Player_Eaten player_eaten in Database.GetPlayersEaten(score.game_id))
                        {
                            //Assertions are contrived, the switch statement makes sure the names are the same
                            switch (player_eaten.name)
                            {
                                case "Jake":
                                    Assert.AreEqual(true, true);
                                    break;
                                case "Tiffany":
                                    Assert.AreEqual(true, true);
                                    break;
                                default:
                                    Assert.Fail(player_eaten.name + " is not supposed to be on this players eaten list.");
                                    break;
                            }
                        }
                        Database.Delete_Game(score.game_id);
                        break;
                    case "Dave":
                        Assert.AreEqual(3, score.rank);
                        Assert.AreEqual(3, i);
                        i++;

                        foreach (Player_Eaten player_eaten in Database.GetPlayersEaten(score.game_id))
                        {
                            //Assertions are contrived, the switch statement makes sure the names are the same
                            switch (player_eaten.name)
                            {
                                case "Romeo":
                                    Assert.AreEqual(true, true);
                                    break;
                                case "Juliet":
                                    Assert.AreEqual(true, true);
                                    break;
                                default:
                                    Assert.Fail(player_eaten.name + " is not supposed to be on this players eaten list.");
                                    break;
                            }
                        }
                        Database.Delete_Game(score.game_id);
                        break;
                    case "John":
                        Assert.AreEqual(4, score.rank);
                        Assert.AreEqual(4, i);
                        i++;

                        foreach (Player_Eaten player_eaten in Database.GetPlayersEaten(score.game_id))
                        {
                            //Assertions are contrived, the switch statement makes sure the names are the same
                            switch (player_eaten.name)
                            {
                                case "Ronald":
                                    Assert.AreEqual(true, true);
                                    break;
                                case "Ronnie":
                                    Assert.AreEqual(true, true);
                                    break;
                                default:
                                    Assert.Fail(player_eaten.name + " is not supposed to be on this players eaten list.");
                                    break;
                            }
                        }
                        Database.Delete_Game(score.game_id);
                        break;
                    case "Sally":
                        Assert.AreEqual(5, score.rank);
                        Assert.AreEqual(5, i);
                        i++;

                        foreach (Player_Eaten player_eaten in Database.GetPlayersEaten(score.game_id))
                        {
                            //Assertions are contrived, the switch statement makes sure the names are the same
                            switch (player_eaten.name)
                            {
                                case "Jennifer":
                                    Assert.AreEqual(true, true);
                                    break;
                                case "Chris":
                                    Assert.AreEqual(true, true);
                                    break;
                                case "Christina":
                                    Assert.AreEqual(true, true);
                                    break;
                                default:
                                    Assert.Fail(player_eaten.name + " is not supposed to be on this players eaten list.");
                                    break;
                            }
                        }
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
