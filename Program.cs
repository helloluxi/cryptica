using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

class Program
{
    static void Main()
    {
        string s = File.ReadAllText("input.json");
        LevelData data = JsonSerializer.Deserialize<LevelData>(s);
        Solver solver = new Solver(data);
        solver.Solution();
    }
}

class LevelData{
    public int[] walls { get; set; }
    public int[] quads { get; set; }
    public int[] goals { get; set; }
}

class Solver
{
    readonly ulong wall = 0x7fe030180c0603ffUL;
    readonly int target = 0, total = 0;
    readonly ulong startState = 0, endState = 0;

    const int xx = 9, yy = 7;
    const string dirs = "RLUD";
    
    public Solver(LevelData data)
    {
        foreach (int w in data.walls)
            wall |= 1UL << XY2Pos(w);
        foreach (int q in data.quads)
            startState |= (ulong)XY2Pos(q) << (total++ * 6);
        foreach (int g in data.goals)
            endState |= (ulong)XY2Pos(g) << (target++ * 6);
    }

    static int XY2Pos(int xy) => (xy / 10) + (xy % 10) * xx;
    
    int Go(int pos, int dir)
    {
        if (dir == 0) return pos + 1;
        else if (dir == 1) return pos - 1;
        else if (dir == 2) return pos + xx;
        else return pos - xx;
    }

    ulong FromPos(int[] poss){
        ulong quadPos = 0UL;
        for(int i = 0; i < poss.Length; ++i)
        {
            quadPos |= (ulong)poss[i] << (i * 6);
        }
        return quadPos;
    }

    ulong Pos2Wall(ulong quadPos){
        ulong quadAsWall = 0;
        for (int i = 0; i < total; i++)
        {
            int pos = (int)(quadPos >> (i * 6)) & 63;
            quadAsWall |= 1UL << pos;
        }
        return quadAsWall;
    }

    ulong Move(ulong quadPos, ulong quadAsWall, int dir){
        int[] nextPoss = new int[total];
        for (int i = 0; i < total; i++)
        {
            int pos = (int)(quadPos >> (i * 6)) & 63;
            int nextPos = Go(pos, dir);

            int tmpPos = nextPos;
            while(((quadAsWall >> tmpPos) & 1) == 1){
                tmpPos = Go(tmpPos, dir);
            }
            if(((wall >> tmpPos) & 1) == 0){
                quadPos ^= (ulong)(nextPos ^ pos) << (i * 6);
            }
        }
        return quadPos;
    }

    public void Solution()
    {
        Stopwatch sw = new();
        Queue<ulong> front = new();
        Dictionary<ulong, (ulong, int)> history = new();
        ulong comparator = (1UL << (target * 6)) - 1UL;

        front.Enqueue(startState);
        history[startState] = default;

        int count = 0;
        while (front.Any())
        {
            ulong quadPos = front.Dequeue();
            ulong quadAsWall = Pos2Wall(quadPos);
            for (int dir = 0; dir < dirs.Length; dir++)
            {
                ulong nextState = Move(quadPos, quadAsWall, dir);
                if(((nextState ^ endState) & comparator) == 0){
                    StringBuilder sb = new();
                    sb.Append(dirs[dir]);

                    (ulong, int) pair = history[quadPos];
                    while(pair.Item1 != 0){
                        sb.Insert(0, dirs[pair.Item2]);
                        pair = history[pair.Item1];
                    }

                    Console.WriteLine();
                    Console.WriteLine($"Serach complete in {sw.Elapsed:c}");
                    Console.WriteLine($"Solution = {sb} ({sb.Length} moves)");
                    return;
                }
                if(quadPos != nextState && !history.ContainsKey(nextState)){
                    front.Enqueue(nextState);
                    history[nextState] = (quadPos, dir);
                }
            }
            if((++count) % 100000 == 0){
                System.Console.WriteLine($"Searched {count} states in {sw.Elapsed:c}");
            }
        }
        System.Console.WriteLine($"Serach failed in {sw.Elapsed:c}");
    }
}