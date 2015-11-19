Authors: Richie Frost and Ryan Fletcher
Organization: CS 3500
Date: November 17th, 2015
Status: Done

	This is the client application for a computer game called AgCubio. This program talks to a server, receiving data for a player
cube which can be moved across the screen, swallowing up food and smaller player cubes. The player cube can be destroyed by other
cubes which are larger than it. The player cube is unique in a way that it is controllable with the mouse and displays the player
name on the player cube.

Typical Functionality:
	The client will first open up, asking for the player's name and their server address that it wants
to connect to. If connection is not made, it will pop up saying that no server exists with that address and request a new
server address. If it does connect, the game will load in after a few seconds and display the world with the player cube and
all of the food that is capable of being swallowed. The cube will grow while it swallows up more cubes, but there is a decay
involved with the cube which shrinks the cube continually. The game will only end once the player cube is swallowed up by a 
larger cube. When the game ends, the client will ask if the player would like to play again. 

Not working:
	Scaling to enlarge the player cube and its world when the cube is small is not functioning at the moment.