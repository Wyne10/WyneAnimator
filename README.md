# WyneAnimator

__Useful tool for creating tween animations, mainly targeted to UI.__

![Alt Text](https://i.imgur.com/GpmZjod.gif)

## Requirements
- Installed DOTween

## Download
To load this project, simply drag and drop the WyneAnimator folder into your Unity project.

## Features
- Parses any component
- Conditional start

## Supproted animation types
- Int
- Long
- Float
- Double
- Color
- Vector2
- Vector3
- Bool

_I  will probably add more supported types later_

## Animation conditions
- OnStart
- OnEnable
- OnDisable
- OnDestroy
- OnClick (Only GameObject with 'Button' component)
- OnTrigger (Manual start)
- OnUIHover (If cursor is pointed over UI element)
- OnUIUnHover (If cursor lost focus from UI element)

## Creating animation

To create an animation you have to add a "WAnimator" component to any GameObject. After that you need to open the animation editor:

![Alt Text](https://i.imgur.com/FUsrGxf.png)

You will see a window where you can add and delete animations:

![Alt Text](https://i.imgur.com/dyupOWk.png)

The animation element looks like this:

![Alt Text](https://i.imgur.com/rx9vRdR.png)

Here you must select the animation condition (OnTrigger condition must be called manually by using TriggerAnimations(string triggerName) method in WAnimator):

![Alt Text](https://i.imgur.com/yn6v2z0.png)

Then you have to pass a condition object (object that being used to check condition) and animated component (component that will be animated). Animator will parse the entire component and show you something like this:

![Alt Text](https://i.imgur.com/bFqtNlF.png)

If you open one of these values, you can change its animation parameters, all used values will be animated when when the condition is met:

![Alt Text](https://i.imgur.com/iTppvvN.png)

Also, used values will be highlighted in blue:

![Alt Text](https://i.imgur.com/O4FZePJ.png)

__That's it!__

_P.S. I apologize for any mistake in my text, I am not a native English speaker._
