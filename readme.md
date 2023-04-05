# Cryptica-Solver

A brute-force solver for the puzzle game Cryptica.

To use it, simply run `dotnet run`.

The `input.json` defines the level, and the current file shows an example level `Hard-1`.
For example, the `"walls": [13, 44]` means there are walls with `x-y` coordinates `(1,3)` and `(4,4)`, from lower-left to upper-right.

The results `URLD` means the solution is to go `U`p, `R`ight, `L`eft, and `D`own, resp.
