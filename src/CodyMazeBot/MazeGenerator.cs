﻿using CodyMazeBot.StoreModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodyMazeBot {
    public static class MazeGenerator {

        public const int LastMove = 13;

        public static (GridCoordinate Target, string Code) GenerateInstructions(GridCoordinate start, int move, IDictionary<string, GridCell> grid) {
            return move switch {
                1 => GenerateMove1(start, grid),
                2 => GenerateMove2(start, grid),
                3 => GenerateMove1(start, grid),
                4 => GenerateMove4(start, grid),
                5 => GenerateMove5(start, grid),
                6 => GenerateMove6(start, grid),
                7 => GenerateMove7(start, grid),
                8 => GenerateMove8(start, grid),
                9 => GenerateMove9(start, grid),
                10 => GenerateMove10(start, grid),
                11 => GenerateMove11(start, grid),
                12 => GenerateMove12(start, grid),
                13 => GenerateMove13(start, grid),
                _ => throw new ArgumentException("Unsupported move")
            };
        }

        private static (GridCoordinate Target, string Code) GenerateMove1(GridCoordinate start, IDictionary<string, GridCell> grid) {
            var target = start.Advance();
            if(!target.HasValue) {
                throw new ArgumentException($"Cannot generate move 1 from position {start}");
            }

            return (target.Value, "f");
        }

        private static (GridCoordinate Target, string Code) GenerateMove2(GridCoordinate start, IDictionary<string, GridCell> grid) {
            // TODO pick randomly
            if(start.TurnLeft().CanAdvance()) {
                return (start.TurnLeft(), "l");
            }
            else if(start.TurnRight().CanAdvance()) {
                return (start.TurnRight(), "r");
            }
            else {
                throw new ArgumentException($"Cannot turn right nor left from position {start}");
            }
        }

        private static (GridCoordinate Target, string Code) GenerateMove4(GridCoordinate start, IDictionary<string, GridCell> grid) {
            if (start.TurnLeft().CanAdvance()) {
                return (start.TurnLeft().Advance().Value, "lf");
            }
            else if (start.TurnRight().CanAdvance()) {
                return (start.TurnRight().Advance().Value, "rf");
            }
            else {
                throw new ArgumentException($"Cannot turn right nor left from position {start}");
            }
        }

        private static (GridCoordinate Target, string Code) GenerateMove5(GridCoordinate start, IDictionary<string, GridCell> grid) {
            if(!start.CanAdvance(2)) {
                throw new ArgumentException($"Cannot advance two positions from position {start}");
            }
            return (start.Advance().Value.Advance().Value, "2{f}");
        }

        private static (GridCoordinate Target, string Code) GenerateMove6(GridCoordinate start, IDictionary<string, GridCell> grid) {
            int leftAdvancements = start.TurnLeft().MaxAdvancements();
            int rightAdvancements = start.TurnRight().MaxAdvancements();
            if(leftAdvancements > rightAdvancements) {
                return (start.TurnLeft().Advance(leftAdvancements).Value, $"l{leftAdvancements}{{f}}");
            }
            else {
                return (start.TurnRight().Advance(rightAdvancements).Value, $"r{rightAdvancements}{{f}}");
            }
        }

        private static (GridCoordinate Target, string Code) GenerateMove7(GridCoordinate start, IDictionary<string, GridCell> grid) {
            GridCoordinate current = start;
            GridCoordinate? next;
            if(start.CanAdvanceLeft()) {
                for(int c = 0; c < 3; ++c) {
                    next = current.TurnLeft().Advance();
                    if(!next.HasValue) {
                        return (current, $"{c}{{lf}}");
                    }
                    current = next.Value;
                }
                return (current, "3{lf}");
            }
            else {
                for (int c = 0; c < 3; ++c) {
                    next = current.TurnRight().Advance();
                    if (!next.HasValue) {
                        return (current, $"{c}{{rf}}");
                    }
                    current = next.Value;
                }
                return (current, "3{rf}");
            }
        }

        private static (GridCoordinate Target, string Code) GenerateMove8(GridCoordinate start, IDictionary<string, GridCell> grid) {
            bool hasStar = grid[start.CoordinateString.ToLowerInvariant()].HasStar;
            string starCondition = hasStar ? "star" : "no star";

            if (start.TurnLeft().CanAdvance()) {
                return (start.TurnLeft().Advance().Value, $"if({starCondition}){{lf}}");
            }
            else if (start.TurnRight().CanAdvance()) {
                return (start.TurnRight().Advance().Value, $"if({starCondition}){{rf}}");
            }
            else {
                throw new ArgumentException($"Cannot turn right nor left from position {start}");
            }
        }

        private static (GridCoordinate Target, string Code) GenerateMove9(GridCoordinate start, IDictionary<string, GridCell> grid) {
            bool hasStar = grid[start.CoordinateString.ToLowerInvariant()].HasStar;
            string starCondition = hasStar ? "no star" : "star"; // inverse, do on else condition

            if (start.TurnLeft().CanAdvance()) {
                return (start.TurnLeft().Advance().Value, $"if({starCondition}){{rf}}else{{lf}}");
            }
            else if (start.TurnRight().CanAdvance()) {
                return (start.TurnRight().Advance().Value, $"if({starCondition}){{lf}}else{{rf}}");
            }
            else {
                throw new ArgumentException($"Cannot turn right nor left from position {start}");
            }
        }

        private static (GridCoordinate Target, string Code) GenerateMove10(GridCoordinate start, IDictionary<string, GridCell> grid) {
            var rnd = new Random();
            GridCoordinate target = start;
            int count = rnd.Next(2, 5);

            for(int i = 0; i < count; ++i) {
                target = target.CrawlPreferRight();
            }

            return (target, $"{count}{{if(path ahead){{f}}else{{if(path right){{r}}else{{l}}}}");
        }

        private static (GridCoordinate Target, string Code) GenerateMove11(GridCoordinate start, IDictionary<string, GridCell> grid) {
            var rnd = new Random();
            GridCoordinate target = start;
            int count = rnd.Next(3, 6);

            for (int i = 0; i < count; ++i) {
                target = target.CrawlPreferLeft();
            }

            return (target, $"{count}{{if(path ahead){{f}}else{{if(path left){{l}}else{{r}}}}");
        }

        private static (GridCoordinate Target, string Code) GenerateMove12(GridCoordinate start, IDictionary<string, GridCell> grid) {
            int maxAdvances = start.MaxAdvancements();
            return (start.Advance(maxAdvances).Value, "while(path ahead){f}");
        }

        private static (GridCoordinate Target, string Code) GenerateMove13(GridCoordinate start, IDictionary<string, GridCell> grid) {
            while(!grid[start.CoordinateString.ToLowerInvariant()].HasStar) {
                start = start.CrawlPreferRight();
            }

            return (start, "while(no star){if(path ahead){f}else{if(path right){r}else{l}}");
        }

    }
}