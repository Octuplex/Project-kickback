Instagib template
Created by Octuplex
Version 1.0
8/1/18

Has Propspawner from Hardlight's toybox and the raycast from Jetdog's prefabs

Make sure you have unity standard assets and the vrcsdk imported before importing this.

Feel free to change anything and everything as long as you ample credit is given, and a link to the discord isn't necessary but would be appreciated. Also, if you make technical improvements make sure you share them with the class so we can all benefit from them.

If you're using the prefabs instead of the example scene, you're going to have to add the propspawner's spawneditem to your dynamic prefabs. In the scene descriptor component of the vrcworld prefab, there's a drop down called dynamic prefabs. In the behind-the-scenes prefab, there's an object called SpawnedItem in the propspawner  under player trackers. Drag that into the dynamic prefabs, and you should be good.



Misc:
If you want to change the beam color, reload time or haptic pattern, they're all handled by the animations under "guns" in the animation folder.

One of the prefabs activates the trigger on its child object when it gets shot. Useful for if you want any interactables in the arena, like buttons or traps. It'll be obvious which prefab it is.

The music in the example scene is all empty by default, for obvious reasons. You can add your own or remove the music player from the scene.

When you import a new map or add the prefabs to an existing map, make sure to update the spawn cycle animations to be the places you want players to spawn, otherwise players might find themselves getting placed inside in trees and walls.