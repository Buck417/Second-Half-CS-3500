using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;


namespace Database_Controller
{
    /// <summary>
    /// This class acts as the controller for our database connections. In essence,
    /// it adds a layer of abstraction so that we can connect to a database without
    /// having to know what's under the covers, as well as read to/write from it.
    /// </summary>
    public class Database
    {
        private static string connectionString = "server=atr.eng.utah.edu;database=cs3500_rfrost;uid=cs3500_rfrost;password=PSWRD";



        /***************************************************** METHODS FOR INSERTING DATA ****************************************************/
        /// <summary>
        /// Adds a game to the DB, and players eaten during that game.
        /// </summary>
        /// <param name="game">The game structure to be added to the DB. The game ID will be ignored here, as it will be adding a new game, 
        /// and the database takes care of the game's ID.</param>
        /// <param name="players_eaten"></param>
        /// <returns></returns>
        public static bool AddGameToDB(Game game, IEnumerable<Player_Eaten> players_eaten)
        {
            bool result = false;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string sql = "insert into Game (cubes_eaten, max_mass, time_alive, time_of_death, player_name) values(" + game.cubes_eaten + ", " + game.max_mass + ", " +
                        game.time_alive + ", " + game.time_of_death + ", " + game.player_name + ")";
                    MySqlCommand command = new MySqlCommand(sql, conn);
                    command.ExecuteNonQuery();

                    //Get the most recently updated game's id
                    sql = "select game_id from Game order by game_id desc limit 1";
                    command = new MySqlCommand(sql, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    long game_id;
                    if (long.TryParse(reader["game_id"].ToString(), out game_id)){
                        foreach (Player_Eaten player in players_eaten)
                        {
                            sql = "insert into Players_Eaten (eaten_name, game_id) values(" + player.name + ", " + game_id + ")";
                            command = new MySqlCommand(sql, conn);
                            command.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        throw new Exception("Error updating players eaten for this game");
                    }
                    
                    conn.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return result;
        }
        /*************************************************** END METHODS FOR INSERTING DATA **************************************************/





        /****************************************************** METHODS FOR READING DATA *****************************************************/
        /// <summary>
        /// This gives us a list of all the games that were played by this one player.  If none were played by
        /// this player, it returns an empty linked list.
        /// </summary>
        /// <param name="player_name"></param>
        /// <returns>A Linked List of Game objects, to be parsed by whoever is calling this.</returns>
        public static LinkedList<Game> GetAllGamesByPlayer(string player_name)
        {
            LinkedList<Game> games = new LinkedList<Game>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    // Open a connection
                    conn.Open();

                    // Create a command
                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = "select * from Game where player_name = '" + player_name + "'";

                    // Execute the command and cycle through the DataReader object
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string[] parts = reader["time_of_death"].ToString().Split(' ');
                            string[] date = parts[0].Split('-');
                            string[] time = parts[1].Split(':');
                            DateTime datetime = new DateTime(int.Parse(date[0]), int.Parse(date[1]), int.Parse(date[2]), int.Parse(time[0]), int.Parse(time[1]), int.Parse(time[2]));
                            games.AddLast(new Game(long.Parse(reader["game_id"].ToString()), int.Parse(reader["cubes_eaten"].ToString()), long.Parse(reader["time_alive"].ToString()), int.Parse(reader["max_mass"].ToString()), datetime, reader["player_name"].ToString()));
                        }
                    }

                    conn.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return games;
        }

        /// <summary>
        /// This returns the top 5 scores in the database.
        /// </summary>
        /// <returns></returns>
        public static LinkedList<Game> GetHighScores()
        {
            LinkedList<Game> games = new LinkedList<Game>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    // Open a connection
                    conn.Open();

                    // Create a command
                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = "select * from Game order by max_mass desc limit 5";

                    // Execute the command and cycle through the DataReader object
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string[] parts = reader["time_of_death"].ToString().Split(' ');
                            string[] date = parts[0].Split('-');
                            string[] time = parts[1].Split(':');
                            DateTime datetime = new DateTime(int.Parse(date[0]), int.Parse(date[1]), int.Parse(date[2]), int.Parse(time[0]), int.Parse(time[1]), int.Parse(time[2]));
                            games.AddLast(new Game(long.Parse(reader["game_id"].ToString()), int.Parse(reader["cubes_eaten"].ToString()), long.Parse(reader["time_alive"].ToString()), int.Parse(reader["max_mass"].ToString()), datetime, reader["player_name"].ToString()));
                        }
                    }

                    conn.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return games;
        }

        /// <summary>
        /// Return all the players that were eaten in a given game.
        /// </summary>
        /// <param name="game_id"></param>
        /// <returns></returns>
        public LinkedList<Player_Eaten> GetPlayersEaten(int game_id)
        {
            LinkedList<Player_Eaten> players = new LinkedList<Player_Eaten>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    // Open a connection
                    conn.Open();

                    // Create a command
                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = "select eaten_name from Players_Eaten where game_id = '" + game_id + "'";

                    // Execute the command and cycle through the DataReader object
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            players.AddLast(new Player_Eaten(reader["eaten_name"].ToString()));
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return players;
        }
        /************************************************** END METHODS FOR READING DATA *************************************************/
    }

    /// <summary>
    /// This is just the structure to the game in code form, as stored in the database.
    /// </summary>
    public class Game
    {
        public readonly int cubes_eaten, max_mass;
        public readonly long game_id, time_alive;
        public readonly DateTime time_of_death;
        public readonly string player_name;

        public Game(long _game_id, int _cubes_eaten, long _time_alive, int _max_mass, DateTime _time_of_death, string _player_name)
        {
            this.game_id = _game_id;
            this.cubes_eaten = _cubes_eaten;
            this.time_alive = _time_alive;
            this.max_mass = _max_mass;
            this.time_of_death = _time_of_death;
            this.player_name = _player_name;
        }
    }

    /// <summary>
    /// Structure for the player that was eaten, according to the structure in the database.
    /// Doing it this way instead of by an array of strings allows for more fields to be added
    /// in the future.
    /// </summary>
    public class Player_Eaten
    {
        public readonly string name;

        public Player_Eaten(string _name)
        {
            this.name = _name;
        }
    }
}
