## Tetris

https://user-images.githubusercontent.com/45336771/138013812-9f8e55a8-f16f-4d74-8d4a-d9678c824845.mp4

This Tetris game was made in C# using the MonoGame framework. To run this game, [.NET](https://dotnet.microsoft.com/) and 
[MonoGame](https://docs.monogame.net/index.html) must be installed. I used .NET Core 3.1.

If no user input is detected on the main menu screen for ten seconds, the game will enter Attract Mode. In this mode, the AI will play the game by itself
until the AI loses the game or the user presses an input.

<details open>
  <summary> Controls </summary>
  <ul>
    <li> <code>←</code> move piece left </li>
    <li> <code>→</code> move piece right </li>
    <li> <code>↓</code> move piece down one row</li>
    <li> <code>↑</code> drop piece directly to bottom </li>
    <li> <code>Q, Home</code> rotate piece counterclockwise
    <li> <code>E, PageUp</code> rotate piece clockwise
  </ul>
</details>

### Technical Notes

* The Tetris AI used in Attract Mode was based on results from Yiyuan Lee's Tetris AI training 
  (training details [here](https://codemyroad.wordpress.com/2013/04/14/tetris-ai-the-near-perfect-player/),
  GitHub repo [here](https://github.com/LeeYiyuan/tetrisai)).
* Game details:
  * Uses the [Original Nintendo scoring system](https://tetris.fandom.com/wiki/Scoring#Original_Nintendo_Scoring_System).
  * Uses [sticky gravity](https://tetris.fandom.com/wiki/Line_clear#Line_clear_gravity) on line clears.
  * New level reached after every 10 line clears.
* Persistent storage for this game stores high scores and control configs between sessions.
