Authors: Richie Frost and Ryan Fletcher
Organization: CS 3500
Date: December 11th, 2015
Status: Database: Done, Web Server: Done, Client Server: In Progress

	This is the database update for the AgCubio project.

Database Structure:
	
	The table structure for our database used two tables, one that kept track of most of the game information. This table 
is named Game. The Game table was intended to keep track of each individual game that every player played, from their start
to their death. A player could have multiple games. The game_id is used as the primary key. The Game table included the 
following columns of data:

game_id: Auto generated id used to identify each unique game that every player played

cubes_eaten: The number of total cubes each player has eaten

time_alive: The total time that the player lived in the game

max_mass: The maximum mass the player achieved while playing the game

time_of_death: The time that the player died, in DateTime format

player_name: The name of the player

rank: The rank the player had when compared to every other player that existed in the game, where rank is determined by the max_mass of every player.

	The second table was the Players_Eaten table. This table tracks every other player that a single player has eaten. This table's primary key is the 
eaten_id, which is auto incremented for each entry added to the table. This table has the following columns:

eaten_id: Auto incremented id used to identify each unique name a player has eaten

eaten_name: The name of the player that was eaten

game_id: the id of the game from the Game table to identify the specific game where the player had eaten others



MYSQL Queries:

Insert: insert into Game (cubes_eaten, max_mass, time_alive, time_of_death, player_name) values(" + game.cubes_eaten + ", " + game.max_mass + ", " +
        game.time_alive + ", " + game.time_of_death + ", " + game.player_name;
		
		"select game_id from Game order by game_id desc limit 1"

		"insert into Players_Eaten (eaten_name, game_id) values(" + player.name + ", " + game_id + ")"

		We insert a game object that was created holding all the information we want to store in the database and insert that into the Game table. The game_id is 
		auto generated when a new game is added. We then select the newest game_id which is the game we just added and add that to the Players_Eaten table and loop 
		through an IEnumerable set that contains all the player names that were eaten during that specific game.

GetAllGamesByPlayer: "select * from Game where player_name = '" + player_name + "';"
				     
					 We select all games in the Games table that contain the name of the player that we want to find.

GetHighScores: "select * from Game order by max_mass desc limit 5"

				We get the top 5 games that were added to the Game table where the rank is determined by the max_mass of the player

GetPlayersEaten: "select eaten_name from Players_Eaten where game_id = '" + game_id + "'"

			      We get the names of all players that were eaten by the player that was queried. 



AgCubio Design:
	
	Our server isn't able to handle multiple clients at the moment.