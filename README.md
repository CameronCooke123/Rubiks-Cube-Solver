# Rubiks-Cube-Solver

These are scripts to be used in Unity3D, along with a sample scene. 

To run, simply import all 3 folders into a Unity project, open SampleScene from the scenes folder, 
and hit play (it is better to look through the scene view rather than the game view).
Click on the "Rubik's Cube" object in the hierarchy to access the controls for the cube.

"Rubik's Cube" Inspector:
Generate Cube: immediately resets the cube to a solved state
Up, Down, etc: move the corresponding side of the cube 90 degrees clockwise
Up Prime, Down Prime, etc: move the corresponding side of the cube 90 degrees counterclockwise
Rotation Speed: adjusts how fast the sides of the cube turn
Pause After Move: pauses the game in the editor after each turn
Solve: will solve the cube
Scramble: will scramble the cube
Num Turns: how many moves to make when scrambling the cube (default 25)

Keyboard controls (with game view selected):
I: stop the solving process after the currently running turn
Q: up side
A: down side
W: front side
S: back side
E: right side
D: left side
Z: middle layer
X: equatorial layer
C: standing layer
Shift: hold to make move prime/inverted
