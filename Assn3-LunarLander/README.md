## Lunar Lander

https://user-images.githubusercontent.com/45336771/138014760-9b475f84-1bab-48e0-b19c-f6900b14aeb9.mp4

This Lunar Lander game was made in C# using the MonoGame framework. To run this game, [.NET](https://dotnet.microsoft.com/) and 
[MonoGame](https://docs.monogame.net/index.html) must be installed. I used .NET Core 3.1.

<details open>
  <summary> Controls </summary>
  <ul>
    <li> <code>←→↑↓ Enter</code> navigate menu </li>
    <li> <code>↑</code> thrust </li>
    <li> <code>←</code> turn left</li>
    <li> <code>→</code> turn right </li>
  </ul>
</details>

### Technical Notes

* Terrain for this game is generated using the [midpoint displacement algorithm](https://bitesofcode.wordpress.com/2016/12/23/landscape-generation-using-midpoint-displacement/).
* The lander's physics are calculated using a real-time physics simulation. 
  * Lander thrust acceleration: 7 m/s^2.
  * Moon's gravity acceleration: -1.62 m/s^2.
* Persistent storage for this game stores high scores and control configs between sessions.
