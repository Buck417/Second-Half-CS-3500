Name: Richie Frost
Organization: CS 3500
Date: October 1st, 2015

This is an empty project that contains the resources for a Spreadsheet.

The Spreadsheet has an infinite number of cells, which can contain strings, numbers, or formulas. 
Cells can have names, contents, and values. A cell's contents differs from its values in that with 
formulas, a cell's value is the result of the formula's evaluation.



UPDATE: Saturday October 10th, 2015

As part of the PS5 implementation of this spreadsheet, we had to get the cell's value. I chose to recursively 
lookup the variable's value until there was a variable that returned a double.

Name: Richie Frost
Name: Ryan Fletcher
Organization: CS 3500
UPDATE: Wednesday November 4th, 2015

We are officially released! The front end interface has been built so there is now a functioning spreadsheet application
that can be ran and used to do important work for your business.

Important Implementations: The spreadsheet is now capable of holding names, numbers, or formulas and can handle dependency with 
other cells of the spreadsheet. When one cell is changed, all cells that use the changed cell are updated as well and everything is 
displayed the screen. The spreadsheet is able to open .sprd files and save new files as .sprd. Multiple spreadsheet windows can be
open but there is not interactivity between spreadsheet windows, that will be implemented in a future update. Our spreadsheet will also
keep track of any changes made to your spreadsheet, ensuring that you dont accidentally close out of your spreadsheet without saving. We
believe that all instructions given by our customer's API were followed exactly as asked. 

Special Feature: What makes our spreadsheet application so unique is the addition of hotkeys that allow for quick command of your 
spreadsheet. Pressing CTRL+O will bring up the open menu, pressing CTRL+S will bring up the save menu, CTRL+N will open a new spreadsheet
and CTRL+W will close the current spreadsheet window.
