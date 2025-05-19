Thank you for downloading Facial Animation Tool.

F.A.T can blend more than 2 facial expressions to make your model has various look.
To execute F.A.T smoothly, it has extra feature: Automatically rename animation track from FBX file if it has only one track.

Main function of F.A.T is very simple.
Create Blend Tree for various expression, control it through Animator component.


To use F.A.T, you need two files.
1. 3D Humanoid model (FBX file)
2. Facial expression animation track for model (FBX file)


How do I make it work?
1. Import your model and facial expressions.
2. Place the model into scene and select it.
3. Open "Blend Tree Creator" window and assign your expression animations. The window is at "Window -> EMP Studio -> Facial Animation Tool" or you can open Facial Animatino Tool with layout presets.
4. Click "Generate scripts" button.
5. If there's no error, Click "Apply components" button to finalize the F.A.T initialization.

All set! You can preview how it looks like with generated components and make your own facial animation on Animation Tab.



===Advanced option===

-Facial Preset
F.A.T has preset function to help you to manage your facial works.
If you assign presets to your facial animation clip before apply it, you can quickly manage to your components.
Do not worry that you forgot to assign it. You can re-assign them anytime you want by re-apply the component.

F.A.T has 25 default presets that you cannot edit unless customize the scripts.

Default,	EyeClose,		MouthOpen,	Sleep,		Tired,
Happy,		Smile,			Flush,		Excited,	Fresh,
Nervous,	Frustrated,		Despair,	Sad,		Cry,
Angry,		Shout,			Confounded,	Unpleasant,	Grimace,
A,			I,				U,			E,			O


-Add Facial Preset
F.A.T support your own presets too. Add your own presets on "Facial Preset Setting" window.
If you import your own preset icon (PNG file only) with matching name to icon directory, F.A.T will add yours on Facial Icon preset window.



===Technical information===
1. We cannot approve that F.A.T will be working on MAC OS or Linux since our studio doesn't own any computers for testing.
2. F.A.T uses Animator.Update function which can cause serious bone tree problem. However, we simply found that problem can be solved by replacing certain bone tree. If your model doesn't include "Head" or "head" in its bone tree, F.A.T cannot operate properaly.
2-1. If your model stuck at one of animation clips, try Play mode or re-open the project. Even neither work, you might reimport your prefab or asset to scene.
3. Avatar Mask will be created only when new layer from Animator Controller has been created. If you want to keep your Avatar Mask, generate scripts at existing layer.


If you have any question about F.A.T, please don't hesitate to contect us!
https://www.empst.com/con

Primer font is published from 1001Fonts under 1001Fonts General Font Usage Terms
https://www.1001fonts.com/licenses/general-font-usage-terms.html
https://www.1001fonts.com/primer-font.html

3D model Billy and EMP Studio logo are owned by EMP Studio. Please Contact us before use them for your project.