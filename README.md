# Test Kinetix

Repo url : https://github.com/Thanadev/TestKinetix

## Time taken to perform the exercise

Coding the exercice in itself “Quick and dirty” took around **7h**

I added 4h of refactoring / improvements and bug fixing

1h for this document

So around **12h** to fully perform the exercise

## Difficulties encountered

**Getting the keyframes**

I tried to find a way to get animation curves / keyframes at runtime but after a lot of reasearch, the only answer I found to that is that Unity is not providing any means to do it at runtime, only in Editor using AnimationUtility.

So in a production environment, we would have to already have the keyframes serialized to be able to achieve a blending like expected for the legacy animations.

**Blending from the legacy animation to the animator animation**

Due to the differences in the constitution of the curves of the humanoid animations, and to be able to achieve the goal in the time constraint, I had to put in place a “revert” system where at the end of the animation (or if the animation is cancelled by moving) we blend back to the last position before the animation was executed.

It works in the context of the test, but please see in “Improvements” what I would have liked to do instead.

**The C# reflection system**

I had to learn the C# reflection system to be able to create an elegant way to read and apply the properties found in the animation curves. It comes with its own subtilities, like the fact that everything needs to be cast as object for the properties to be applied. It was fun but took a bit of time to get it right.

## Improvements

**Humanoid Animation Clip Reader**

The most wanted change. It would be able to translate the really verbose animation curve properties like “Spine3 Stretch” to Quaternion and Vector3 properties, making them usable in the interpolator.

Once developed, it would have been simple to blend the legacy animations with any animator animation and get rid of the “revert system”. It would also allow for an improvement in performances via the caching of the target state at the first keyframe of the animation, instead of saving the current state before playing the legacy animation.

**Key binding to emote**

Instead of an “if” in the update method of the AnimationController, register a matching (Dictionary) of inputs (KeyCode) and emotes.

**Emote provider**

Instead of creating the emotes in the AnimationController, create an EmoteProvider. This allows for a fetching from different sources in the future.

**Remove the division per 10 in the LinearInterpolator**

Weirdly, the interpolation seems to be finished way before the timeRatio of the Lerp function gets to 1. Maybe there is an easing ? Didn’t have time to look more into it and created the illusion by dividing by ten (sorry, magic number).

**Use UnityEvent for OnMove**

Currently the AnimationController and the MovementController reference each other. We could use UnityEvents instead, allowing other systems to listen to movement instead.

**Continue to implement interfaces and providers for ClipReaders and Interpolators**

Always following the SOLID principles, I would have liked to create interfaces (and providers) for the LinearInterpolator and the LegacyAnimationClipReader, allowing to test multiple systems and switch them easily.

**Have a class instead of the awful Dictionary<Transform, Dictionary<string, object>> to pass animation information**

The title is self explanatory on this one… This would allow more flexibility on the transformation of information from other sources to the format we want to use, would help to significantly reduce method signatures length (more readability) and ease the refactoring in general.

**Create unit tests**

A good code coverage would help to significantly stabilize the codebase

## Various comments

### Coding conventions

**SerializeField and private properties**

Instead of putting the properties as “public” to show them in the inspector, I prefer to use the attribute [SerializeField] which does exactly that but respects the encapsulation.

**No underscore**

As written in the documentation of Unity in the code example, I usually don’t put underscores before the name of my private variables. This is mostly due to my experience in web development and the coding style I acquired.

That said, I fully understand that it depends on the existing codebase and the usual C# way of doing it is “m_” or just the underscore.

### Packages used (or not used)

**Camera**

For the camera orbiting the player, I chose not to use Cinemachine as :

- This is a test
- The time required to code this system was equivalent to the time to implement the cinemachine camera
- No vertical movement was required

**Old Input sytem**

I chose to use the old input system as it is the one I’m the more familiar with.

**A small error?**

The humanoid emote was not set as humanoid, I had to correct it

**That’s it, thanks for your time ! ☺**