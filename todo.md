https://www.owlree.blog/posts/simulating-a-rope.html
https://toqoz.fyi/game-rope.html

Idea for reducing tunneling (getting stuck inside collider): let every link go back the same direction it came from (last position) on detected collision.
  Only do this when speed is high and the correction direction seems to be going the wrong way?
  Keep track of last non-colliding position?
    Calculate nearest point on collider edge from last non-colliding link position instead of current position?
