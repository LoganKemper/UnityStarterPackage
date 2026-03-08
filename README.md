# Unity Starter Package 

This repository contains a collection of scripts and assets for game development in Unity 6. 
It has been created for game development classes at the University of Florida's **Digital Worlds Institute**. 

Included is...
- A collection of scripts for a 2D side-scrolling or top-down game. 
- A collection of scripts for a 3D first-person or third-person game. 
- A dialogue system for use in 2D or 3D. 
- Documention PDFs for each script collection. 

All scripts were written by Logan Kemper. Feel free to use, fork, remix, or share however you would like. 

## How to Use

- Create a new 2D or 3D Unity 6 project using the Universal Render Pipeline. The package was last tested with Unity 6.3. 
- Import TextMesh Pro. In the scene hierarchy, `right-click > UI > Text - TextMesh Pro`. If it's not in the project yet, a TMP Importer dialogue will pop up. Choose `Import TMP Essentials`. 
- Download the desired `.unitypackage` files from this repo. Import them to Unity by choosing `Assets > Import Package > Custom Package...` then select the `.unitypackage` file. Make sure everything is checked on and click `Import`.
- Check out the example scenes to see the scripts in action! 

## Additional Material

A series of companion videos to this repository can be found on YouTube. There are two playlists — one for 2D and one for 3D — that cover project setup, how to use the scripts, Unity editor tips and tricks, game design fundamentals, and more. 
- 2D playlist (11 videos): https://www.youtube.com/playlist?list=PLF2N1cgwCCA-_XQ4gzKNaEb05ZdRGjnhh
- 3D playlist (14 videos): https://www.youtube.com/playlist?list=PLF2N1cgwCCA8uORuRpZVFAvCczmABgZWn

### A Note About Input Handling
Since [Version 3](https://github.com/LoganKemper/UnityStarterPackage/tree/version-3.0), this package has been updated to use Unity's `Input System Package`. The tutorial videos still show the older `Input Manager`. The `Input System Package (New)` implementation has been designed to be as similar to the old setup as possible, and the `Input Manager (Old)` setup can still be used with [Version 2](https://github.com/LoganKemper/UnityStarterPackage/tree/version-2-final) or earlier.
- To change input in Unity, go `Edit > Project Settings... > Player > Other Settings`, and set `Active Input Handling` to `Both` or whichever is desired.
