Package made by Phasedragon#7157. My discord DMs are always open if you need help understanding or modifying this prefab.

part A: Using the prefab

There are 3 different ways that you can change the value. The scene includes examples of how to do each of these.

1: Let the animator handle it
	Pros: easy to set up, includes multiplication and set
	Cons: cannot receive more than one input per frame.

	Setup: Use a vrc trigger to set animationfloat "Modifier" to the desired change, then use the desired animation trigger: Set, Add, Subtract, or Multiply.

	This is the easiest method because all you do is set the modifier and choose the operation you want to do. Modifier and operation can 
	be chosen separately, like a calculator, or both at the same time. This also gives the option of multiplication.

2: Addition buffer
	Pros: Can send and receive more than one input per frame
	Cons: Takes time to process, addition/subtraction only
	
	Setup: Use a vrc trigger to do animationintadd on the "AnimationBuffer" integer.

	This method is meant for combat systems and other uses where inputs are more freeform and can happen at any time, even if it's dozens per second.

3: Preset changes
	Pros: can receive more than one input per frame
	Cons: cannot send more than one input per frame, addition/subtraction only, requires more setup

	Setup: Create a new gameobject inside the handle object, and set it's Y position to the value you want to add/subtract. Then set up
	a button to change the handle's autocam target to that gameobject and manual update it. Check the scene for a more comprehensive example.

	This method is best used when you have many different sources of change all going into one central animator. Each source can only activate once
	per frame, but the central animator can receive any number of them at once. This requires you to make a new gameobject inside the counter's handle
	object for each different amount.


Part B: Modifying the prefab

If you want to change how the value is displayed, you can freely modify the animator's display layer and modifier layer to whatever you want.
The value is stored in the jump parameter.

If you want to change the minimum and maximum values, go the "operations" layer and change the condition to transition to clampmin and clampmax,
then change the Y position of the Counter/Animator/Maxposition and Counter/Animator/MinPosition gameobjects.

