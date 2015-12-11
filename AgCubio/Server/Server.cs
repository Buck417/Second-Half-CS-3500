using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Network_Controller;
using System.Drawing;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Timers;
using Database_Controller;
using System.Data;
using System.Threading;
using System.ComponentModel;

namespace Server
{
    public class AgServer
    {
        public volatile static World world;
        private static Dictionary<int, Player> players = new Dictionary<int, Player>();

        private const int QUEUE_MAX = 50;
        private static System.Timers.Timer heartbeatTimer = new System.Timers.Timer();
        private static System.Timers.Timer splitTimer = new System.Timers.Timer();
        private static System.Collections.Concurrent.ConcurrentQueue<Tuple<string, int, int, int>> moveQueue = new System.Collections.Concurrent.ConcurrentQueue<Tuple<string, int, int, int>>();
        private static System.Collections.Concurrent.ConcurrentQueue<Tuple<string, int, int, int>> splitQueue = new System.Collections.Concurrent.ConcurrentQueue<Tuple<string, int, int, int>>();

        private static bool Game_Started = false;

        public static void Main(string[] args)
        {
            world = new World("world_parameters.xml");
            splitTimer.Interval = world.SPLIT_INTERVAL;
            splitTimer.Elapsed += SplitTimer_Elapsed;

            heartbeatTimer.Interval = 1000 / world.HEARTBEATS_PER_SECOND;
            heartbeatTimer.Elapsed += HeartbeatTimer_Elapsed;
            heartbeatTimer.Start();

            //Game server listener
            Network.Server_Awaiting_Client_Loop(new Action<Preserved_State>(Handle_New_Client_Connections), 11000);
        }




        /******************************** HANDLE GAME SERVER COMMUNICATIONS **********************/
        /// <summary>
        /// Once the split timer finishes, make sure the mass goes back to normal.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void SplitTimer_Elapsed(object sender, ElapsedEventArgs e)
        {

        }

        /// <summary>
        /// This is the handler for every time the heartbeat timer elapses. Stop the timer
        /// at the beginning so that we don't spawn too many threads, and then process
        /// the world, before sending it to each client that's currently connected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void HeartbeatTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            heartbeatTimer.Stop();

            if (Game_Started)
            {
                LinkedList<Cube> cubes_eaten;
                LinkedList<Cube> players_eaten;
                world.ProcessAttrition();
                cubes_eaten = world.FoodConsumed();
                players_eaten = world.PlayersConsumed();

                Tuple<string, int, int, int> move;
                if (moveQueue.TryDequeue(out move))
                {
                    string type = move.Item1;
                    int x = move.Item2;
                    int y = move.Item3;
                    int player_uid = move.Item4;
                    world.ProcessData(type, x, y, player_uid);
                }

                Tuple<string, int, int, int> split;
                if (splitQueue.TryDequeue(out split))
                {
                    string type = split.Item1;
                    int x = split.Item2;
                    int y = split.Item3;
                    int player_uid = split.Item4;
                    world.ProcessData(type, x, y, player_uid);
                }

                //Send the world state to every connected client
                foreach (Player player in players.Values)
                {
                    Network.Send(player.socket, JsonConvert.SerializeObject(world.AddFoodCube()) + "\n");

                    foreach (Cube cube in cubes_eaten)
                    {
                        Network.Send(player.socket, JsonConvert.SerializeObject(cube) + "\n");
                    }

                    foreach (Cube cube in players_eaten)
                    {
                        Network.Send(player.socket, JsonConvert.SerializeObject(cube) + "\n");
                    }

                    foreach (Cube cube in world.player_cubes.Values)
                    {
                        Network.Send(player.socket, JsonConvert.SerializeObject(cube) + "\n");
                    }
                }

            }

            heartbeatTimer.Start();
        }
        /******************************** HANDLE GAME SERVER COMMUNICATIONS **********************/






        /********************************* HANDLE WEB SERVER COMMUNICATIONS **********************/
        private static void Handle_Web_Server_Connection(Preserved_State state)
        {
            state.callback = Process_Web_Server_Request;
            Network.i_want_more_data(state);
        }

        private static void Process_Web_Server_Request(Preserved_State state)
        {
            string http_request = state.sb.ToString();
            string uri, method;
            LinkedList<KeyValuePair<string, string>> parameters;
            bool validRequest = Try_Parse_Raw_HTTP_Request(http_request, out method, out uri, out parameters);

            StringBuilder result = new StringBuilder();

            //Start out by adding the necessary HTML
            result.Append(Get_HTML_Header());

            //Append the table HTML
            result.Append("<table class='table table-striped table-bordered agcubio'>");

            //Process valid requests
            try
            {
                switch (uri)
                {
                    case "scores":

                        break;
                    case "games":
                        result.Append(Get_Games_By_Player(parameters));
                        break;
                    case "eaten":

                        break;
                    default:
                        return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            //Always make sure to close the socket and form the HTML closing tags properly.
            finally
            {
                //Append the ending table HTML
                result.Append("</table>");

                //Append the closing HTML tags
                result.Append(Get_HTML_Footer());

                Network.Send(state.socket, result.ToString());
                state.socket.Close();
            }
        }

        /// <summary>
        /// This is the magic that parses a raw HTTP request, and returns usable data for routing the request.
        /// </summary>
        /// <param name="http"></param>
        /// <param name="method"></param>
        /// <param name="pathname"></param>
        /// <param name="parameters"></param>
        private static bool Try_Parse_Raw_HTTP_Request(string http, out string method, out string pathname, out LinkedList<KeyValuePair<string, string>> parameters)
        {
            method = "";
            pathname = "";
            parameters = new LinkedList<KeyValuePair<string, string>>();

            try
            {
                //First, split the raw HTTP request by line
                string[] parts = Regex.Split(http, "\r\n");

                //First line is the http method, as well as the pathname
                string[] request_parts = Regex.Split(parts[0], " ");
                if (request_parts[0].Equals("GET"))
                    method = request_parts[0];
                //Only handle GET requests for now
                else
                    return false;

                //The first part of the path is the URI/pathname
                string[] path_parts = request_parts[1].Split('?');
                pathname = path_parts[0].Substring(1);

                //See if there are any variable parameters in the request. If there are, add them as a key/value pair to the parameters linked list passed in.
                if (path_parts.Length > 1)
                {
                    foreach (string part in path_parts[1].Split('&'))
                    {
                        string[] var = part.Split('=');
                        parameters.AddLast(new KeyValuePair<string, string>(var[0], var[1]));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //Reset the variables passed in
                method = "";
                pathname = "";
                parameters = new LinkedList<KeyValuePair<string, string>>();
                return false;
            }

            return true;
        }

        /// <summary>
        /// This returns HTML for all the games by a certain player, to be displayed on the browser.
        /// Note - this includes the table header as well.
        /// </summary>
        /// <param name="player_name"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static string Get_Games_By_Player(LinkedList<KeyValuePair<string, string>> parameters)
        {
            StringBuilder result = new StringBuilder();

            //Look for the player name in the parameters
            string player_name = "";
            foreach (KeyValuePair<string, string> pair in parameters)
            {
                if (pair.Key.ToLower().Equals("player"))
                    player_name = pair.Value;
            }

            //Add the table header
            result.Append(Get_Games_Table_Header());

            //Go through each game and put it in some useful HTML
            string game_tpl = "<tr><td>game_id</td><td>player_name</td><td>max_mass</td>" +
                "<td>rank</td><td>time_of_death</td><td>time_alive</td><td>cubes_eaten</td></tr>";
            string row = "";
            foreach (Game game in Database.GetAllGamesByPlayer(player_name))
            {
                row = game_tpl.Replace("game_id", game.game_id.ToString())
                    .Replace("player_name", game.player_name)
                    .Replace("max_mass", game.max_mass.ToString())
                    .Replace("rank", game.rank.ToString())
                    .Replace("time_of_death", game.time_of_death.ToString())
                    .Replace("time_alive", game.time_alive.ToString())
                    .Replace("cubes_eaten", game.cubes_eaten.ToString());
                result.Append(row);
            }

            return result.ToString();
        }

        /// <summary>
        /// Required as the table header to when a list of games is requested
        /// </summary>
        /// <returns></returns>
        private static string Get_Games_Table_Header()
        {
            return "<tr class='header'><td>Game ID</td><td>Player Name</td><td>Max Mass</td><td>Highest Rank</td>" +
                "<td>Time of Death</td><td>Time Alive</td><td>Number of Cubes Eaten</td></tr>";
        }

        private static string Get_HTML_Header()
        {
            return "<html><head>" +
                    Get_Styles() +
                    "</head>" +
                "<body>"
                ;
        }

        /// <summary>
        /// This abstracts out the styles we need to format the HTML, to be stored in the header.
        /// </summary>
        /// <returns></returns>
        private static string Get_Styles()
        {
            return
                "<link rel='stylesheet' href='https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css' integrity='sha384-1q8mTJOASx8j1Au+a5WDVnPi2lkFfwwEAa8hDDdjZlpLegxhjVME1fgjWPGmkzs7' crossorigin='anonymous'>" +
                       "<style type='text/css'>" +
                "table.agcubio { width: 80%; margin-left: auto; margin-right: auto; margin-top: 5%;}" +
                "tr.header td {" +
                    "padding: 20px;" +
                    "font-size: 1.5em;" +
                    "text-align: center; " +
                    "font-weight: 'bold'" +
                  "}" +
                "td {" +
                    "padding: 20px;" +
                    "font-size: 1.3em;" +
                    "text-align: center; " +
                  "}" +
                "</style>";
        }

        private static string Get_HTML_Footer()
        {
            return "</body></html>";
        }
        /******************************* END HANDLE WEB SERVER COMMUNICATIONS ********************/





        /*********************************** HANDLE NETWORK COMMUNICATIONS **********************/
        /// <summary>
        /// Handle new client connections, and ask the client for more data (the player name).
        /// </summary>
        /// <param name="state"></param>
        private static void Handle_New_Client_Connections(Preserved_State state)
        {
            state.callback = new Action<Preserved_State>(Receive_Player_Name);
            Network.i_want_more_data(state);
        }

        /// <summary>
        /// Receive the player name from the client, and add that player to the list of players (including the connected socket)
        /// </summary>
        /// <param name="state"></param>
        private static void Receive_Player_Name(Preserved_State state)
        {
            //Preserved_State state = (Preserved_State)ar.AsyncState;
            state.callback = new Action<Preserved_State>(HandleData);
            string player_name = state.sb.ToString();

            Cube player_cube = world.AddPlayerCube(player_name);
            state.SetUID(player_cube.UID);

            Player player = new Player(state.socket, player_name);
            player.SetUID(player_cube.UID);

            players.Add(player_cube.UID, player);

            if (!Game_Started)
            {
                Game_Started = true;
                PopulateWorld();
            }

            //Sends the player cube and starting food cubes to the client
            lock (world)
            {
                SendInitialData(player, player_cube);
            }
            state.sb.Clear();
            Network.i_want_more_data(state);
        }

        /// <summary>
        /// Send the player cube, followed by the other cubes in the world.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="player_cube"></param>
        private static void SendInitialData(Player player, Cube player_cube)
        {
            Network.Send(player.socket, JsonConvert.SerializeObject(player_cube) + "\n");
            StringBuilder builder = new StringBuilder();
            foreach (Cube cube in world.food_cubes.Values)
            {
                builder.Append(JsonConvert.SerializeObject(cube) + "\n");
            }
            Network.Send(player.socket, builder.ToString());
        }

        /// <summary>
        /// Handle data from the client, and add it do a queue of operations to be performed during the heartbeat, one by one.
        /// Doesn't touch the "world" object.
        /// </summary>
        static void HandleData(Preserved_State state)
        {
            string movement = state.sb.ToString();
            int player_uid = state.UID;

            string type;
            int x, y;
            if (TryParseData(movement, out type, out x, out y))
            {
                if (type.Equals("move") && moveQueue.Count < QUEUE_MAX)
                    moveQueue.Enqueue(new Tuple<string, int, int, int>(type, x, y, player_uid));
                else if (type.Equals("split"))
                {
                    //Make sure we can only add one split request to the queue at a time
                    Tuple<string, int, int, int> existingSplit;
                    if (!splitQueue.TryPeek(out existingSplit))
                        splitQueue.Enqueue(new Tuple<string, int, int, int>(type, x, y, player_uid));
                }
            }
            state.sb.Clear();
            System.Threading.Thread.Sleep(1000 / world.HEARTBEATS_PER_SECOND);
            Network.i_want_more_data(state);
        }

        /// <summary>
        /// Puts the data into a usable format, and we only use the
        /// first request in the queue if it's a split.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static bool TryParseData(string data, out string type, out int x, out int y)
        {
            string[] parts = data.Split('\n');
            string temp_type = Regex.Replace(parts[0], "[()]", "");

            //Look for the split, if there is one
            if (data.Contains("split"))
            {
                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i].Contains("split"))
                    {
                        temp_type = Regex.Replace(parts[i], "[()]", "");
                        break;
                    }
                }
            }
            parts = temp_type.Split(',');
            type = "";
            x = y = 0;

            try
            {
                //Check to see if the request was either for move or split
                if (int.TryParse(parts[1], out x) && int.TryParse(parts[2], out y))
                {
                    if (parts[0].Equals("move"))
                    {
                        type = parts[0];
                        return true;
                    }
                    else if (parts[0].Equals("split"))
                    {
                        type = parts[0];
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }

            return false;
        }
        /********************************* END HANDLE NETWORK COMMUNICATIONS ********************/




        /************************************ HANDLE GAMEPLAY MECHANICS *************************/
        private static void PopulateWorld()
        {
            //Don't send all the food possible at once, we still want to have some to add during the game
            for (int i = 0; i < world.MAX_FOOD / 2; i++)
            {
                world.AddFoodCube();
            }
        }
        /********************************** END HANDLE GAMEPLAY MECHANICS ***********************/
    }
}