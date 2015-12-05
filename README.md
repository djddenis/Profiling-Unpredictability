# Profiling-Unpredictability

In this project I have designed and implemented a learning AI for a simple game.  The game is designed to be purely about predicting the opponents moves, to simplify the situation.  The AI is composed of two parts- profiling, and learning to be unpredictable.  
The AI keeps a record of the moves its opponents have made and the game states in which those moves were made.  It uses this information to generate a probability distribution for the opponents current possible moves.
Because being unpredictable is required to win this game, the AI also learned a strategy for how to act on the probability distribution created by profiling, using genetic algorithms.
