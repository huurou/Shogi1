using Shogi1.Application;
using Shogi1.Application.Moves;
using System;

namespace Shogi1.Presentation.ConsoleApp
{
    internal class Program
    {
        private readonly ShogiApplicationService appService_ = new();

        private static readonly ConsoleColor bg_ = ConsoleColor.DarkYellow;
        private static readonly ConsoleColor fg_ = ConsoleColor.Magenta;
        private static readonly ConsoleColor blackPiece_ = ConsoleColor.Black;
        private static readonly ConsoleColor whitePiece_ = ConsoleColor.White;
        private static readonly ConsoleColor empty_ = ConsoleColor.DarkGray;
        private static readonly ConsoleColor pieceBg_ = ConsoleColor.DarkCyan;

        private static void Main()
        {
            Console.WriteLine("Hello World!");
        }
        private static void DrawBoard(Board board, MoveBase? moveBase = null, int? eval = null)
        {

        }

    }
}