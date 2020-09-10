# RealCityGen

This Project was for an University Project. A [Video](https://drive.google.com/file/d/1RaybepdfP0eSiYIlqkqS8SMPMzD0wfhH/view?usp=sharing) of it in action

# Intro

Procedural generation is a widely explored field in computer science. Not only games but also science use procedural generation increasingly often in the last few years. The most prominent and in my opinion excessively explored topic in that field is about the generation of terrain. 

But true procedural generation of cities and its buildings can rarely be found in the field. When exploring the few different city generation techniques it may become evident that most, -while being realistic, complex and sophisticated- become costly, clunky and unwieldy. It seems that a compact and flexible real-time city generator that is easy to be implemented and fast to use, is difficult to be found. 

The RealCityGen aims to allow dynamic city generation in and around an preexisting environment in real time. 
It does not need a big setup, you can simply import the tool, designate the areas where random buildings should be generated and press play. 
However the fast implementation does not equal sparse customization:
You are able to alter every aspect of the generation.

# How does it work

I recommend downloading the whole project and exploring the Example Scene to become fluent in handling the software.

But to be able use the software you just need to do the following: 

1. import all the scripts from the [Scripts Folder](https://github.com/s4safeld/RealCityGen/tree/master/Assets/Scripts) into your project.

2. Make an empty GameObject and drag the GlobalInformation Script onto it

3. Take any cube and add the GridGenerator Script to it

4. Make the size of the cube so that it covers the area where you want a city generated

5. Add a few empty GameObjects as childs to the Cube 

6. add the BuildingGenerator Script to each of them

7. Now fill out all the values of the Scripts

If you need help with the last point check out the documentation below or contact me.

Note that the Code is 100% written by me except for the [Triangulator.cs](https://github.com/s4safeld/RealCityGen/blob/master/Assets/Scripts/Triangulator.cs), a class that creates surface-triangles for any 2D mesh.
[Source](http://wiki.unity3d.com/index.php?title=Triangulator&_ga=2.97540694.871866967.1597588282-744620994.1584369047)

# Documentation

## Global Information:
A class that needs to be in the Scene and inherits variables and functions that needs to be accessible from anywhere in the scene.

**Set View Distance:**
The Range  in which streets and buildings are generated, if they are further away they will be destroyed.

**Set Player:**
The Gameobject of the Player, used to compute the render distances.
This can be left empty, then there won't be any View Frustum Filling happening and all the Buildings will be generated once.

**Set World Seed:**
A string that will serve as the world seed, to generate a random seed: Type "null".

**Set Terrain:**
The gameobject ontop of which the city will be generated.

## Generation Cube:
Inherits the GridGenerator script which manages the generation of the cellgrid and streetgrid. The area of a city is defined by the rotation and dimensions of the cube. 
The height of the cube does not matter. 
Additionally it must inherit an arbitrary number of BuildingGenerators as child objects. 
The prefab comes with an transparent material, but the apperance does not play a role since the Mesh Renderer will be disabled at runtime anyway.
The Generation Cube also saves an array of colliders and does not generate anything inside of any collider which does not have the "ignore" tag.
Every child of an GenerationCube will automatically have the ignore tag assigned.

**Cell Size:**
if automatic cellsize is not ticked this will dictate the size of the cells/lots. Note that the BuildingGenerators do not care for  cellsize, and might generate buildings on and over streets. If you don't know how big the Buildings will be, ignore this and tick the Automatic Cellsize box.

**Automatic Cellsize:**
If this is ticked the size of a cell/lot is computed automatically by calculating the biggest possible building dimensions.
Dont mind the "CellSize" field if this is ticked.

**Road Width:**
the width of a standart street object. The CellSize will be fitted to this in the background.

**Perlin Noise Multiplier:**
building types are distributed over the grid by using the Perlin Noise function. This Value determines what area of the Perlin Noise image is used. If the values are too high or too low the results may appear as if its not working. The best results are observed using a value between 0 and 1. 

**Street:**
A Street object, its dimensions should match the Road Width value. The orientation is important, and should match the one in the exampleset.

**Crossroad:**
A Crossroad connecting four street objects.

**T-Junction:**
A T-Junction connecting three streets. The orientation is important, and should match the one in the exampleset.

**Street Corner:**
A Corner connecting two streets. The orientation is important, and should match the one in the exampleset.

**Street End:**
An End of a street. The orientation is important, and should match the one in the exampleset.

**Connection:**
A straight connection of two streets facing each other vertically or horizontically. The orientation is important, and should match the one in the exampleset.

## Building Generator:
Defines a type of a building by its material, the number of different layers/steps and their values generated randomly using the cellSeed. Each Cell will have one Building Generator assigned by the Generation Cube (using Perlin Noise). And will call the generate function of the BuildingGenerator with its localSeed as a paramter.
Important: A BuildingGenerator needs to be attached to an empty Gameobject and then set as child of a GenerationCube.

**Steps -> Size:**
Determines how many Steps the generated building should have.Theoretically the number of steps is arbitrary and only limited by the Computation Power of your System.
If this is set to zero no building will be generated and you have an emtpy lot. 

**MinFloors, MaxFloors:**
How Many Floors the step should have, this will set the height of a step. 
If you dont want the number to be random, just put the same value two times.
A floor is 2 units tall.
The floors of each step combined will set the total height of a building.
For example two steps where each floor number is 3, will result in the total height of 12 units ((3+3)*2).

**MinRotation, MaxRotation:**
Define the roation of the FloorPlan.
If you dont want the number to be random, just put the same value two times.

**IsPolygon:**
This determines if the step will be a simple Rectangle or a simple Polygon. With that it determines which of the following values will be needed:
If the desired floor plan is a Polygon, Min-Max Vertices and Min-Max Radius will define its dimensions. MinMaxWidth and Min-Max Length will then be ignored!
Otherwise, if isPolygon is false, Min-MaxWidth and Min-Max Length will define the the dimensions of a resulting Rectangle, while vertices and radius will be ignored.

**MinVertices, MaxVertices:**
Defines how many Corners the polygon shaped step/floorplan should have. Will be ignored if isPolygon is false.
If you dont want the number to be random, just put the same value two times.

**MinRadius, MaxRadius:**
Defines the radius and thus the Size of the polygon shaped step/floorplan. Will be ignored if isPolygon is false.
If you dont want the number to be random, just put the same value two times.

**MinWidth, MaxWidth:**
Defines the width of the step/floorplan which will be shaped like a rectangle. Will be ignored if isPolygon is true.
If you dont want the number to be random, just put the same value two times.

**MinLength, MaxLength:**
Defines the Length of the step/floorplan which will be shaped like a rectangle. Will be ignored if isPolygon is true.
If you dont want the number to be random, just put the same value two times.

**Material:**
This will set the Material of the Building. Note that this is not final. In the finished version this will take a texture of a fassade as input in which the building should appear.

**Grid Generator:**
Ignore this, it will be set automatically.

