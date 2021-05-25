# Flagger
Flagger - An easy way to efficiently store 8 bools per byte using bit flags - why waste 63 bits per bool?

Let's say you have 8 values you want to store. You arent really doing much with them, just keeping them for later.
This will create 8 blocks of 64 bits with one of those for the bool. This is wasteful.

Instead, Flagger stores these 8 values as the bits in one byte, and provides utility functions to get and set bits.
