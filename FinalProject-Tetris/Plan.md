# Game Dev Final Project - Tetris Tribute

## **Overview**
### **Menu**

Need to use mouse input for the menu.

* New Game
* High Scores
* Controls
    * Don't need to support key combinations, just a single button
* Credits

### **Update Loop**

* Check input with `InputHandler`
* Update the game with `TetrisGameController`
* Update visuals with `Draw()`

### **Input Handler**

* Use debouncing buttons (like last project)
* Mouse input:
    1. Get mouse down / up state
    2. Get mouse absolute pixel coordinates, convert to game window coordinates
    3. Using game window coordinates, check to see if any MouseInputButtons are hovered or clicked (with debouncing)
* `InputHandler` only knows about inputs and changes which buttons are pressed, it does \*not\* do anything with this knowledge
  
### **Tetris Game Controller**
* Needs to store:
  * Number of lines cleared
  * Score
    * Scoring: 
    * 1 line clear: 40 * (level + 1)
    * 2 line clear: 100 * (level + 1)
    * 3 line clear: 300 * (level + 1)
    * 4 line clear: 1200 * (level + 1)
    * Each piece also awards 1 point for each grid square it fell vertically
  * Level (will be total lines cleared divided by 10)
  * Game state (running, game over, attract mode)
    * When game is running, whether we are in "piece free fall" mode or not
  * All `Square` objects on the board, stored in a 10 col x 20 row array (elements can be null if no Square here), where (0, 0) is the bottom-left corner of the board
  * Current `Piece` object
  * Next `Piece` object
  * Elapsed time since last "gravity update tick"
    * Pieces can be rotated and translated at any time, but gravity only is active on certain ticks
    * Soft / hard dropping will also reset this elapsed time
  * Reference to an `InputHandler`
  * Reference to a `ParticleController`
  * Reference to a `SoundController`
  * Reference to a `MenuController`
  * Reference to an `AIController`
  * Game window size: 30 x 30
  * Reference table to level / timespan for gravity
* Game update loop:
  * If running or attract mode:
    * If not in free fall mode
      * In running mode:
        * Check for movement of the piece from user (if it exists)
        * Down-movements reset the elapsed time since last piece drop
      * In attract mode:
        * Check for movement of the piece from the AI (if it exists)
        * Down-movements reset the elapsed time since last piece drop
      * Get elapsed time since last piece drop
      * If greater than / equal to reference table timespan for gravity update tick:
        * If no piece exists, then spawn a piece (from Next Piece)
        * Apply a gravity tick to falling piece (if we can; this might not be possible if we rotate onto touching)
        * If piece is if touching piece below:
          * Clear the current piece (so that next gravity update) will spawn new piece
          * Check for line clears, if clear:
            * Remove the lines from screen
            * Play "Line Clear" sound effect using `SoundController`
            * Generate particle effects on edges of lines that were cleared
            * Enter Free Fall mode
          * Otherwise: 
            * Play "Piece hit" sound effect
            * Generate particle effects on edges of this piece
            * Check for a game over (if square (5, 20) is filled)
    * If in free fall mode
      * Create (10, 20) `visited` array of booleans
      * Set starting group number to 0
      * Strating from (0, 0), then iterating up-right for each square:
        1. If this board position does not have a Square, then continue
        2. If this Square is `visited` then continue
        3. Mark square as `visited`, mark group as the group number, recurse at 3 for each neighboring `Square` that is not visited
        4. Increase group number
      * For each group number between 0 and the max group number:
        * Check if we can move the squares down, if we can then move all squares down to lowest position
      * Clear all Square object groups
      * Wait for next gravity tick
      * Check for line clears again (call a function, if no line clears then we go back to non-free fall mode)
    * Update `ParticleController`
  * If in the menu:
    * Hand over control to the `MenuController`
  * If in the game over: 
    * Wait for user to press the key to exit back to main menu
* To start a new game:
  * Reset board to empty (all elements null)
  * Reset score to 0
  * Reset level to 1
  * Reset line clear count to 0
  * Generate a starting `Piece` and a next `Piece`
  * Reset `ParticleController`
  
#### **Square**

* Has coordinates for the board (between (0, 0) and (10, 20))
* Has a `Color` property to tell the color
  * Colors are: Red, Orange, Yellow, Green, Blue, Indigo, Violet
* Has a `SquareGroup` int to tell which square group this square will be in
  * Used for Free Fall mode
  
#### **Piece**

* Has four `Square` objects
* Has an orientation: Up, Right, Down, Left
* Has a `PieceType`: 
  * lowercaseL
  * leftCapitalL
  * rightCapitalL
  * plus
  * square
  * leftZ
  * rightZ

### **Particle Controller**
* Exposes function drawParticle(gameBoardPointX, gameBoardPointY, ParticleType)
* ParticleType:
  * White - simple white particle, for placing a piece
  * Yellow - yellow particle, for clearing lines
* Has a list of ParticleEmitters, initially empty
* Update function:
  * For each ParticleEmitter, update it
  * For each ParticleEmitter that's been alive for 5 seconds, remove it

### **Sound Controller**
* Exposes functions:
  * `PlayLineClear()`
  * `PlayPieceDrop()`
  * `PlayGameOver()`
  * `StartMusic()`
  * `StopMusic()`
* Each function is "smart", as in, the sound will only play once, no matter how many frames it's called for

### **Menu Controller**
* Substantially similar to last time

### **Draw() Function**
* Substantially similar to last time
* Just check for mousing over a button to highlight

## **Progress**

- [x] Basic rendering (pieces, in one state only)
    - 2 hours
- [x] Piece Movement / Rotation
    - 5 hours
- [x] Row Clearing / Gravity falling / Game Updating
    - 4 hours
- [x] Scoring, level increasing
    - 1 hour
- [ ] Menuing
    - 4 hours
- [ ] Persistent Storage (High Scores, controls)
    - 2 hours
- [ ] Attract Mode
    - 4 hours
- [x] Particles
    - 3 hours
- [x] Sound
    - 3 hours
- [x] Final Touches (text, piece preview)
    - 3 hours