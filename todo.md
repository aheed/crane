https://www.owlree.blog/posts/simulating-a-rope.html
https://toqoz.fyi/game-rope.html

Idea for reducing tunneling (getting stuck inside collider): let every link go back the same direction it came from (last position) on detected collision.
  Only do this when speed is high and the correction direction seems to be going the wrong way?
  Keep track of last non-colliding position?
    Calculate nearest point on collider edge from last non-colliding link position instead of current position?

Improved chain link orientation:
  Direction from next link bottom edge to previous link top edge

---
I don't understand composite colliders :(

Another weird thing: c.otherCollider is the collider on this object in OnCollisionEnter2D(Collision2D c). c.collider is the object you collided with!!

Potential problem:
Edge colliders do not collide with other edge colliders


claw detector and grabable objects:
 separate layer: grab
   grabable object is a "shadow" of the visible object with same geometry, separate layer
     => jaws and chain will interact with the visible object, detector only with grabable "shadow"
       Possible to achieve the same with collider layer overrides?
 rigibody2D
   necessary for the claw detector, not for the grabbable object
 kinematic
 full kinematic contacts
   OnCollisionEnter2D is not triggered but there is a list of contacts during overlap
 on closing jaws: check for contacts
   If contact pick up contacted grabbable.
     Keep it's position locked to claw
     Adjust claw's mass ratio
     (Disable appropriate parts of grabbed object to prevent the jaws from reacting in a weird way.)
     
Introduce new interface IGrabbable?*
  GetGrabbedObject (usually parent)
  GetGrabPoint/object/offset
  Grab
    Turn off some colliders, like the visible part of grabbable object
      Reduces risk of weird chain or claw interaction behavior
    Turn off physics if applicable
  Release
  GetMassRatio (compared to a chain link)

State machine for (grabbing/)ungrabbing.*
  Go through (most of) release animation first, then release. Otherwise multiple collisions causes jerkiness during release animation.

Levels
  Do not make the player decide difficulty level explicitly.
    Some levels can return as near-carbon-copies of earlier levels, but harder

Improvements
  Do not make the player choose upgrades explicitly. Somewhat random upgrade "cards" must be collected to have a chance to succeed on later levels.


Fans, magnets, vertical flappy-birds
  Fans affect the chain
  magnets only affects cargo or claw
   Example: magnet only affects cargo => you might need to carry a magnetic marble to get through an opening with the claw. Then drop it and collect your actual target marble.
   Example: Need to push a magnetic marble even though it's behind a wall

Underwater levels with buoyancy related challenges. Need for the claw to travel upward.

Parachutes?

Bank shots.
  Need to drop a marble at the correct height/position to get the right bounce.

Explosions
  Place bombs at appropriate places to get a reaction

Drop a non-goal marble into a shute to push the goal marble into position
  Trampolines?

Zombies pushing whatever is placed in front of them.

Drop a marble at speed to make it roll over a bump.

Throw the marble through a basketball hoop

Scale factor:
  Everything, including the crane, should be smaller?
    4K resolution is possible

Timer, high score (best time)

Make collapsed empty claw thinner than claw holding marble
  Makes it possible to make levels with openings narrow enough for empty closed claw to get through, but not with grabbed marble.
  Set closed angle depending on grabbed object or not.
  Attempt to pull a ball through a narrow opening should result in dropped ball.
    Long enough distance between claw and next link in the chain should make the grabbed object lost.

Game idea:
Scorched Earths
  Like Scorched Earth, but players are at different planets. Shots are deflected by the gravity of other planets and moons in interplanetary space between players.

Problem:
why does the claw interact with Finish collider (isTrigger)?
  Compare to marble

---
Claw jaws should be a separate layer, interacting w default*
Marble collider:
  interact w default and claws when not grabbed
  interact w default but not claws when grabbed
  Either have separate sub-gameobject, mutually exclusively active
    or change layer on grab/release
  
Level scaling:
  Frame should adapt to the main camera's zoom level. Use view-to-screen conversion.

More for-editor code:
  RoundedBar: make the circular ends match bar length
  Marble: Make starting position match current position

