public class ClaudeReadme
{
    /**
    Important information for Claude or other AI agents and my group members

    Overview:
        - 2D Automation Game With Nodes
        - Single player, Simple
        - Grid layout. Everything is square.
        - Machine Types: Delivery, Flipper, Junction, Merger, Processor, Splitter, and Wire
        - Can have multiple types of these except for delivery and wire
        - Processors can consume inputs, but can also produce resources from nothing
        - Wires can only move straight right, up, or down
        - Wires are either fully connected or fully removed
        - Wire min size is 1 x 1
        - Machines take inputs on left and output to the right
        - Default width of machine is 5 units
        - Default height is 1 + (2 x Max(inputs, outputs))
        - Junctions can take inputs on top or bottom and output to top or bottom
        - Two types of junctions exist. Both input left and output left
        - One input top and output bottom, other output top and input bottom
        - Junctions act as mergers and splitters at the same time
        - Nothing can overlap, and wires cannot cross
        - Delivered resources count as currency to buy machines
        - Delivered resources also used as part of the game objective based on the level
        - Very simple project required. A simple node factory game should be enough
        - Plenty of complexity already in the code base

    State of the Project:
        - Core data structures are finished
            --- May need a little bit of work. Possibly some small bugs or missing functionality
            --- Enough content exists to do a minimal test of the game
            --- Empty folders were created as hints for good design
            --- There is nothing to run or test at the present time
            --- Most of the code is done
            --- It requires Menus, UI, and later real content
            --- I/O and debugging are supported

    Machine Operations and Other Actions:
        - The core operations the player does are add and remove machines, and connect and disconnect wires
        - These are handled by the Machine Operations class
        - You can also buy new machines

    *** Suggested Future Content Below ***

    Menus
        - Can start by actually doing something the player can see
        - Use the scripts/Menu folder
        - A main menu. Include version info from Game Constants
        - An exit menu
        - An about menu
        - Some way to start the game
        - Possibly add real save and load system and new game with level select (That is the full option)
        - For now maybe just play and start the first level only?
        - A possibility is only one save for each level with a reset option?
        - Claude should ask the developer for feedback here on final useage

    Game Engine
        - Monobehaviour class needed to actually update the game
        - Core logic runs at a fix frame rate set in game constants
        - Engine should be forced to run at this speed
        - We can ignore if it runs slower or faster

    Display
        - Use the scripts / display folder
        - Drawing of the level itself
        - The map class contains everything in the level
        - The map view determines the visible area and has conversion tools to switch between screen units and grid units
        - Map Machines is the grid of all machines on the map
        - It has helper methods for getting the machines in an area
        - Experiment with the grid size in pixels for now. AKA how big one grid is on the screen at zoom = 1
        - Core display should be quite simple
        - Wires fill area but are slightly thinner
        - For example a right pointing wire would have slightly reduced height
        - An up or down wire would have slightly reduced width
        - Display minimal information and put details in a hud overlay
        - Need basic rects. Possibly solid colors, possibly with borders
        - Possibly distinquish machine types by color maybe not. Distinquish wires probably
        - Can draw nodes. don't draw disabled nodes. pretend they don't exist
        - Possibly show if the node is empty or not. 3 states exist, but maybe only distinguish 2
        - Progress bars on processors would be nice. Just a simple solid rect is probably all you can fit

    Hud & Game Menus
        - Use the scripts / hud folder
        - Need a way to exit
        - Probably open a menu with button in top left and or pressing escape
        - From there have an exit command, possibly saving. No need to be able to load from within the game
        - An overlay hud that shows details of machines would be very nice to have
        - Ability to select a processor and assign a recipe or remove it is needed
        - Require the shop. Ability to buy machines using delivered resources.
        - Use the Item Requirements class to prepare the data.
        - Require the abilty to select machines and drag them into the world
        - Require the ability to see the level objectives
        - Should have some sort of level completion, but allow them to keep playing
        - A hud showing level progress would be very good. Stored in Game Data
        - Some sort of popup on level completion would be good. Warning in Game Data where it needs to be added
        - A minimap would be ok, but isn't absolutely necessary

    Game Logic
        - Ensure the game is functional
        - Machines can be placed
        - Assemblers can have recipes assigned
        - Machines can be removed
        - Wires can be connected. Easiest method would be clicking on two nodes. Or even just one and let it try extend outwards and connect from there.
        - Wires can be removed
        - Machines can be purchased in the shop
        - Game completion is shown

    Testing
        - Each machine will need to be tested to ensure it is working
        - Maybe automate it (Claude or whatever). Maybe a person

    Real Game Content
        - Michael should be responsible for making this into a real game
        - He can come up with a theme
        - He can create content

    Level Suggestions
        - We should have 3 levels
        - The entire thing should be demonstrable in under 5 mins
        - I suggest 1 super simple level. Place a miner, smelter, and delivery machine. Let it run for a few seconds. Win.
        - Then a slightly harder mission that requires like 1 or 2 minutes
        - Then a free play mission that is difficult and has real game play

    Finalization
        - Michael should be responsible for finalization
        - The other two can pitch in where needed
    */
}
