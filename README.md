# Multiplayer-Prototype
This is a prototype for a multiplayer game. The game uses unity as an engine for simulation and rendering. Within this repository are two projects, One for the client and one for the server. These projects are the first version of the prototype and later versions are in development by myself. There is a third project that acts as an update launcher using XAML and Windows .NET, however it is being used to advance a later version of this project.

## Noted Bugs
* Localhost must connect last in order for external players to join.
  * most likely due to a faulty SendPlayerIntoGame function.
    * This was fixed in a later version by implementing a lobby system.
* Bullets will lag heavily.
  * Later version contain client sided prediction to fix this.
* Bullets may sometimes not despawn.
  * Fixed by the same solution above.
* Game as a whole may have connection issues.
  * Players do not de-sync when this happens.

## Port
The server must have port 7020 open to allow external connections.
## Credits
Thanks to Tom Weiland for providing a tutorial for a 3D multiplayer framework!
