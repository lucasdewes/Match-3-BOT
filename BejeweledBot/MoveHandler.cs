using System;
using System.Collections.Generic;
using System.Drawing;

namespace BejeweledBot
{
    public static class MoveHandler
    {
        public static Move GetBestMove(int[,] board)
        {
            List<Move> moves = new List<Move>();
            Move move;
            int width = board.GetLength(0);
            int height = board.GetLength(1);
            for (int row = 0; row < width; row++)
                for (int col = 0; col < height; col++)
                {
                    List<Point> neighbours = getMatchingNeighbours(board, row, col, 2, board[row, col]);
                    moves.AddRange(getValidMoves(new Point(row, col), neighbours, board));
                }
            if (moves.Count != 0)
            {
                Random rnd = new Random();
                move = moves[rnd.Next(moves.Count)];
            }
            else
            {
                move = null; //TODO: clean up
            }

            return move;
        }

        private static List<Point> getMatchingNeighbours(int[,] arr, int row, int col, int distance, int targetValue)
        {
            List<Point> pointList = new List<Point>();

            pointList.AddRange(getHorizontalMatch(arr, row, col, distance, targetValue));
            pointList.AddRange(getVerticalMatch(arr, row, col, distance, targetValue));

            return pointList;
        }

        private static List<Point> getHorizontalMatch(int[,] arr, int row, int col, int distance, int targetValue)
        {
            int height = arr.GetLength(0);
            int width = arr.GetLength(1);
            if (width != height)
                throw new ArgumentException("Provided array was not square");

            List<Point> pointList = new List<Point>();

            for (int j = col - distance; j <= col + distance; j++)
            {
                if (j >= 0 && j < width && j != col)
                {
                    if (arr.TryGetValue(row, j) == targetValue)
                    {
                        pointList.Add(new Point(row, j));
                    }
                }
            }
            return pointList;
        }

        private static List<Point> getVerticalMatch(int[,] arr, int row, int col, int distance, int targetValue)
        {
            int height = arr.GetLength(0);
            int width = arr.GetLength(1);
            if (width != height)
                throw new ArgumentException("Provided array was not square");

            List<Point> pointList = new List<Point>();
            //get vertical matching neigbours
            for (int i = row - distance; i <= row + distance; i++)
            {
                if (i >= 0 && i < height && i != row)
                {
                    if (arr.TryGetValue(i, col) == targetValue)
                    {
                        pointList.Add(new Point(i, col));
                    }
                }
            }
            return pointList;
        }

        private static List<Point> getAllMatches(int[,] arr, int row, int col, int distance, int targetValue)
        {
            List<Point> matches = new List<Point>();
            matches.AddRange(getHorizontalMatch(arr, row, col, distance, targetValue));
            matches.AddRange(getVerticalMatch(arr, row, col, distance, targetValue));
            return matches;
        }

        private static List<Move> getMoves(List<Point> matches, Point source)
        {
            List<Move> moves = new List<Move>();
            foreach (Point pt in matches)
            {
                if (pt.X < source.X)
                {
                    moves.Add(new Move(pt, Direction.DOWN));
                }
                else if (pt.X > source.X)
                {
                    moves.Add(new Move(pt, Direction.UP));
                }
                else if (pt.Y < source.Y)
                {
                    moves.Add(new Move(pt, Direction.RIGHT));
                }
                else if (pt.Y > source.Y)
                {
                    moves.Add(new Move(pt, Direction.LEFT));
                }
            }
            return moves;
        }

        private static List<Move> getValidMoves(Point target, List<Point> validNeighbours, int[,] board)
        {
            //x - row
            //y - col
            List<Move> moves = new List<Move>();
            int value = board[target.X, target.Y];
            foreach (Point neighbour in validNeighbours)
            {
                //one index of neighbour is always the same as the index of target
                //horizontal check
                if (neighbour.X == target.X)
                {
                    int minY = Math.Min(neighbour.Y, target.Y);
                    int maxY = Math.Max(neighbour.Y, target.Y);
                    if (maxY - minY == 1) //points are next to one another
                    {
                        Point leftPoint = new Point(target.X, minY - 1);
                        Point rightPoint = new Point(target.X, maxY + 1);

                        List<Point> leftMatches = getAllMatches(board, leftPoint.X, leftPoint.Y, 1, value);
                        moves.AddRange(getMoves(leftMatches, leftPoint));

                        List<Point> rightMatches = getAllMatches(board, rightPoint.X, rightPoint.Y, 1, value);
                        moves.AddRange(getMoves(rightMatches, rightPoint));

                        moves.RemoveAll(mv => (mv.Pt == neighbour || mv.Pt == target));
                    }
                    else //points have an empty tile between them
                    {
                        Point midPoint = new Point(target.X, minY + 1);
                        List<Point> midMatches = getVerticalMatch(board, midPoint.X, midPoint.Y, 1, value);
                        moves.AddRange(getMoves(midMatches, midPoint));
                    }
                }
                else //vertical check TODO: refactor
                {
                    int minX = Math.Min(neighbour.X, target.X);
                    int maxX = Math.Max(neighbour.X, target.X);
                    if (maxX - minX == 1)//points are next to one another
                    {
                        Point topPoint = new Point(minX - 1, target.Y);
                        Point bottomPoint = new Point(maxX + 1, target.Y);

                        List<Point> topMatches = getAllMatches(board, topPoint.X, topPoint.Y, 1, value);
                        moves.AddRange(getMoves(topMatches, topPoint));

                        List<Point> bottomMatches = getAllMatches(board, bottomPoint.X, bottomPoint.Y, 1, value);
                        moves.AddRange(getMoves(bottomMatches, bottomPoint));

                        moves.RemoveAll(mv => (mv.Pt == neighbour || mv.Pt == target));
                    }
                    else //points have an empty tile between them
                    {
                        Point midPoint = new Point(minX + 1, target.Y);
                        List<Point> midMatches = getHorizontalMatch(board, midPoint.X, midPoint.Y, 1, value);
                        moves.AddRange(getMoves(midMatches, midPoint));
                    }
                }
            }
            return moves;
        }
    }
}