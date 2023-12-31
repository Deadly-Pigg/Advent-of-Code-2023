﻿using System.Text;
using System.Text.RegularExpressions;
public class Day10 : Days<int>
{
    public struct Coordinates
    {
        public int X;
        public int Y;
        public Coordinates(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    Dictionary<int, Coordinates> direct = new Dictionary<int, Coordinates>()
    {
        {0, new Coordinates(0,-1)}, //going north
        {1, new Coordinates(1,0)}, //going east
        {2, new Coordinates(0,1)}, //going south
        {3, new Coordinates(-1,0)} //going west
    };

    HashSet<Coordinates> loop = new();

    public override int Answer(bool part2) 
    { 
        string[] file = File.ReadAllLines("AoC10.txt"); 
        Coordinates startPos = new();

        for(int y = 0; y < file.Length; y++)
        {
            for(int x = 0; x < file[y].Length; x++)
            {
                if(file[y][x] == 'S')
                {
                    startPos = new Coordinates(x,y);
                    break;
                }
            }
        }
        if(file[startPos.Y][startPos.X] != 'S')
            throw new Exception("start (S) was not found in the input.");

        if(part2)
        {
            file[startPos.Y] = PipeType(file, startPos);
            Console.WriteLine(file[startPos.Y][startPos.X] + "eee");
            return Part2(file);
        }
        
        return Part1(file, startPos);
    }
    private int Part2(string[] file) //got a hint from reddit sadly. I aint smart enough to do this on my own :/
    {
        int count = 0;
        bool isLoop;
        char last = '|';
        List<char> aboveChars = new List<char> {'7','F','|'};
        List<char> belowChars = new List<char> {'L','J','|'};
        for(int y = 0; y < file.Length; y++)
        {
            isLoop = false;
            last = '|';
            for(int x = 0; x < file[y].Length; x++)
            {
                if(loop.Contains(new Coordinates(x,y)))
                    (isLoop, last) = CheckForValidity(file[y][x], x, y);
                else if(isLoop)
                    count++;
            }
        }
        return count;

        (bool, char) CheckForValidity(char chosen, int x, int y)
        {
            if(aboveChars.Contains(chosen) && aboveChars.Contains(last))
                return (!isLoop, chosen);
            if(belowChars.Contains(chosen) && belowChars.Contains(last))
                return (!isLoop, chosen);

            return (isLoop, last);
        }
    }
    private int Part1(string[] file, Coordinates curr)
    {
        int count = 0;
        (curr, int direction) = ChooseDirection(file, curr);
        loop.Add(curr);
        bool valid = true;
        Console.WriteLine(direction);
        
        while(valid)
        {
            (curr, valid, direction) = MoveOnce(file, curr, direction);
            loop.Add(curr);
            count++;
        }
        return count/2;
    }
    private (Coordinates, int) ChooseDirection(string[] file, Coordinates curr)
    {
        if(curr.Y > 0 && curr.Y < file.Length) //incase the node is in the corner or something stupid
        {
            char temp = file[curr.Y-1][curr.X];
            if(temp == '|' || temp == 'F' || temp == '7')
                return (new Coordinates(curr.X,curr.Y - 1), 0);

            temp = file[curr.Y+1][curr.X];
            if(temp == 'L' || temp == 'J')
                return (new Coordinates(curr.X,curr.Y + 1), 2);
        }
        
        if(curr.X <= 0 || curr.X >= file[0].Length)
            throw new Exception("the input string is not in the valid format. Check if there are at least 2 other paths from the start node.");
        return (new Coordinates(curr.X + 1,curr.Y), 1); //I would check for edge validation here, but if S is at an edge, the loop connector will be either below or above
        //the S position. If this throws an error
        
    }
    
    private (Coordinates, bool, int) MoveOnce(string[] file, Coordinates curr, int d) //just for moving. First chooses the direction specified by
    { //the current position, then moves down said direction. At no point should there
        bool valid = true;

        switch(file[curr.Y][curr.X])
        {
            case 'L':
            {
                d = d == 3 ? 0 : 1;
                break;
            }
            case 'J':
            {
                d = d == 2 ? 3 : 0;
                break;
            }
            case '7':
            {
                d = d == 1 ? 2 : 3;
                break;
            }
            case 'F':
            {
                d = d == 0 ? 1 : 2;
                break;
            }
            case 'S':
            {
                valid = false;
                break;
            }
        }
        curr = new Coordinates(curr.X + direct[d].X, curr.Y + direct[d].Y);

        return (curr, valid, d);
    }

    private string ReplaceAt(string str, int index, char replacement)
    {
        char[] charray = str.ToCharArray();
        charray[index] = replacement;
        return new string(charray);
    }

    private string PipeType(string[] file, Coordinates curr)
    {
        Coordinates tempDir = curr;
        Dictionary<int[], char> choosePipe = new Dictionary<int[], char>()
        {
            {new int[] {0,1}, 'L'},
            {new int[] {1,2}, 'F'},
            {new int[] {2,3}, '7'},
            {new int[] {0,3}, 'J'},
            {new int[] {0,2}, '|'},
            {new int[] {1,3}, '-'}
        };

        (tempDir, int direction) = ChooseDirection(file, curr);
        int direction2 = 0;

        if(curr.X > 0 && curr.X < file[0].Length) //same as ChooseDirection code, just in reverse.
        {
            char temp = file[curr.Y][curr.X-1];
            if(temp == '-' || temp == 'L' || temp == 'F')
                direction2 = 3;
            else
            {
                temp = file[curr.Y][curr.X+1];
                if(temp == '7' || temp == 'J')
                    direction2 = 1;
            }
        }
        else
            direction2 = 2;
        
        if(direction > direction2)
            (direction, direction2) = (direction2, direction);

        char chosen = 'T';
        switch(direction)
        {
            case 0:
            {
                switch(direction2)
                {
                    case 1:
                    {
                        chosen = 'L';
                        break;
                    }
                    case 2:
                    { 
                        chosen = 'J';
                        break;
                    }
                    case 3:
                    {
                        chosen = '|';
                        break;
                    }
                }
                break;
            }
            case 1:
            {
                switch(direction2)
                {
                    case 2:
                    {
                        chosen = 'F';
                        break;
                    }
                    case 3:
                    { 
                        chosen = '-';
                        break;
                    }
                }
                break;
            }
            case 2:
            {
                chosen = '7';
                break;
            }
        }

        file[curr.Y] = ReplaceAt(file[curr.Y], curr.X, chosen);

        return file[curr.Y];
    }
}
