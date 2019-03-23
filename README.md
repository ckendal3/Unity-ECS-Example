# Unity-ECS-Example

## Purpose
This repo is a testing ground for Unity's ECS system. It will contain examples for key systems such as movement and lifetime systems. 
Hopefully it helps others taking the first step into the realm of "Performance by Default"...

## Preview
![Youtube Link](https://i.imgur.com/xtVkaqV.png)
https://www.youtube.com/watch?v=nqOs8C-LRIE

## Instructions
There is only one scene in the project. The boostrap file handles the creation of entities (for now) with multiple settings.
While most values are random within a given range, the settings exposed to the inspector are used for testing workload specifically.

#### SpawnType
- This controls how the cubes spawn. The options are:
  1. *None* which will not spawn any cubes.
  2. *Input* which will spawn cubes when the space bar is released.
  3. *Timer* which will spawn cubes on a timer.

#### cubeSpawnRate
- This determines **how many seconds in between batches of cubes** being spawned.

#### numCubeToSpawnAtOnce
- This determines **how many cubes to spawn in every batch.** *(This will be useful for batch instantion)*

#### rotationCube
- This bool determines if a **batch of cubes will have a rotation component added** to it and consequently - rotate.

#### inMesh
```diff
- This cannot be null/empty
```
- This is the mesh that will be "added" to an entity for rendering.

#### inMaterial_Primary
```diff
- This cannot be null/empty
```
- This is the primary instanced material that will be used on the inMesh.

#### inMaterial_Secondary
```diff
- This cannot be null/empty
```
- This is the secondary instanced material that will be used on the inMesh.


## Future Additions
- Utilize batch instantiations and batch destruction
- Implement Batch Raycast System for Hit Detection
  1. Setup Hit System (Setups data for batch raycast)
  2. Hit System (Perform Batch Raycast/Hits)


## From ckendal3

I will try and keep this updated as future versions release. 

Please feel free to let me know better ways of doing things or any bugs that you come across.



Created by ckendal3
