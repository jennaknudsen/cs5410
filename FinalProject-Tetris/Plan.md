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
  * Game state (running, paused, game over, attract mode)
    * When game is running, whether we are in "piece free fall" mode or not
  * All `Square` objects on the board, stored in a 10 col x 20 row array, where (0, 0) is the bottom-left corner of the board
  * Current `Piece` object
  * Next `Piece` object
  * Elapsed time since last "gravity update tick"
    * Pieces can be rotated and translated at any time, but gravity only is active on certain ticks
    * Soft / hard dropping will also reset this elapsed time
  * Reference to an `InputHandler`
  * Reference to a `ParticleController`
  * Reference to a `SoundController`
  * Game window size: 30 x 30
  * Reference table to level / timespan for gravity
* Game update loop:
  * If running:
    * If not in free fall mode
      * Check for movement of the piece (if it exists)
        * Down-movements reset the elapsed time since last piece drop
      * Get elapsed time since last piece drop
      * If greater than / equal to reference table timespan for gravity update tick:
        * If no piece exists, then spawn a piece (from Next Piece)
        * Apply a gravity tick to falling piece
        * If piece is if touching piece below:
          * Clear the current piece (so that next gravity update) will spawn new piece
          * Check for line clears, if clear:
            * Remove the lines from screen
            * Play "Line Clear" sound effect using `SoundController`
            * Generate particle effects on edges of lines that were cleared
            * Enter Free Fall mode
          * Otherwise: 
            * Play "Piece hit" sound effect
    * If in free fall mode
      * Create (10, 20) `visited` array of booleans, (10, 20) ref list of `Square` objects
      * Set starting group number to 0
      * Strating from (0, 0), then iterating up-right for each square:
        1. If this board position does not have a Square, then continue
        2. If this Square is `visited` then continue
        3. Mark square as `visited`, mark group as the group number, recurse at 3 for each neighboring `Square` that is not visited
      *  
  
#### **Square**

* Has coordinates for the board (between (0, 0) and (10, 20))
* Has a `Color` property to tell the color
  * Colors are: Red, Orange, Yellow, Green, Blue, Indigo, Violet
* Has a `SquareGroup` int to tell which square group this square will be in
  * Used for Free Fall mode