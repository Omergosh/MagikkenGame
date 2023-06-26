# Magikken Game

Inspired by two games:
- FOOTSIES by HiFight (simplicity, game flow, rollback netcode in Unity/C#)
- Pokkén Tournament (complexity, two mechanically divergent battle phases)

Magikken aims to be a low-scope game that incorporates elements of both of these titles, and then puts its own spin on it.

Low scope, more depth.

If this game makes it to a fully functioning state locally, online rollback netcode will be the next goal.

Unique challenges:
- Camera-relative movement, and split-screen for local multiplayer.
- Deterministic game state, in a game where the mechanics + physics basically require using trigonometric functions.
- Interfacing with UGS (Unity Gaming Services) to facilitate online networking, if we get that far.
