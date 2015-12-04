Authors: Richie Frost and Ryan Fletcher
Organization: CS 3500
Date: December 3, 2015
Status: Done

	This is the server application for a computer game called AgCubio. This program talks to a server, receiving data for a player
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

Design decisions:
	1.) For the world updating algorithm, we decided to check each player against each food cube, and add those "eaten" cubes to a 
	buffer that's read after all the player and food cubes are processed. The reason we're doing the buffer is because we couldn't
	update the collection of cubes we were working on in-line, or there would be a run-time error.
	2.) When checking to see if a player cube overlaps a food cube, we "artificially" increased the size of the player cube. The
	reason for this was because even though the math was correct for calculating an overlap, the client still wasn't showing the food
	being eaten when it was overlapped. So, we artificially changed the overlap size so that the food would look like it was being
	eaten, even though the client wasn't showing it correctly in the first place.
	3.) We're using IPv6 in our client connections.
	4.) We're using Jim's client for our testing. Please don't use our client, it doesn't work. We had to change our model significantly,
	and our networking code had to change, so we decided to stop trying to fix our client and just use Jim's client instead.
	5.) We added some constants beyond the ones suggested to be read in the XML, namely:
		SPLIT_INTERVAL: Recognized in the XML as "split_interval", this determines how long the split lasts, in seconds.
		MINIMUM_ATTRITION_MASS: Recognized in the XML as "min_attrition_mass", this determines the minimum amount of mass a player cube 
			must have in order to have attrition effects.
		MINIMUM_FAST_ATTRITION: Recognized in the XML as "min_fast_attrition", this determines the minimum amount of mass a player cube
			must have in order to have the attrition effect at a faster rate, which is also a constant (see next).
		FAST_ATTRITION_RATE: Recognized in the XML as "fast_attrition_rate", this determines the rate at which attrition occurs on a player
			cube that has a mass higher than the MINIMUM_FAST_ATTRITION constant.
		We also included (and used) the constants suggested in the assignment specifications, such as HEARTBEATS_PER_SECOND, WIDTH, HEIGHT, etc.

Not working: 
	