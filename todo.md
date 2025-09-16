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

State machine for (grabbing/)ungrabbing.
  Go through (most of) release animation first, then release. Otherwise multiple collisions causes jerkiness during release animation.