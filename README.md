# BehaveAsSakura

**BehaveAsSakura** is a [behavior tree](http://www.gamasutra.com/blogs/ChrisSimpson/20140717/221339/Behavior_trees_for_AI_How_they_work.php) implementation for .NET and Unity. BehaveAsSakura provides several features to build multiplayer networking games with behavior trees easier.

## Features

* Behavior trees are event-driven to avoid traversal of entire trees every frame and improve execution performance significantly.
* Traversal of trees and execution of tasks are deterministic which means simulations on client and server will give exactly the same result.
* Configuration of behavior trees are serialized with [FlatBuffers](https://google.github.io/flatbuffers/) so it is insanely fast to load trees into memory.
* Internal states of behavior trees are also serializable in case a full synchronization is necessary.
* A intuitive visual editor is provided in Unity with runtime debugging support.

## Licensing

**BehaveAsSakura** is licensed under the Apache License, Version 2.0. See [LICENSE](https://github.com/wuyuntao/BehaveAsSakura/blob/master/LICENSE) for the full license text.
