using Shogi1.Application;
using Shogi1.Application.Events;
using Shogi1.Application.Moves;
using Shogi1.Application.Plays;
using System;
using System.Collections.Generic;
using System.Linq;
using static Shogi1.Application.Games.Result;
using static System.Console;

namespace Shogi1.Presentation.ConsoleApp
{
    internal class Program
    {
        private static readonly ShogiApplicationService appService_ = new();

        private static readonly ConsoleColor boardBg_ = ConsoleColor.DarkYellow;
        private static readonly ConsoleColor boardFg_ = ConsoleColor.Magenta;
        private static readonly ConsoleColor blackPieceFg_ = ConsoleColor.Black;
        private static readonly ConsoleColor whitePieceFg_ = ConsoleColor.White;
        private static readonly ConsoleColor emptyFg_ = ConsoleColor.DarkGray;
        private static readonly ConsoleColor movedPieceBg_ = ConsoleColor.DarkCyan;

        private static void Main()
        {
            CursorVisible = false;
            ResetColor();
            appService_.GameStarted += (s, e) => Draw(e);
            appService_.Moved += (s, e) => Draw(e);
            appService_.GameFinished += (s, e) => WriteLine($"{(e.Result == BlackWin ? "先手勝ち" : "後手勝ち")}");

            appService_.Run();
        }

        private static void Draw(ShogiEventArgs e)
        {
            if (e is GameStartedEventArgs gse)
            {
                WriteLine($"先手 {gse.Black}");
                WriteLine($"後手 {gse.White}");
                DrawBoard(e.BlackBoard, e.WhiteBoard);
            }
            else if (e is MovedEventArgs me)
            {
                DrawBoard(e.BlackBoard, e.WhiteBoard, me.Move);
                DrawHands(e.BlackHands, e.WhiteHands);
            }
        }

        private static void DrawBoard(Piece[] blackBoard, Piece[] whiteBoard, MoveBase? moveBase = null)
        {
            BackgroundColor = boardBg_;
            ForegroundColor = boardFg_;
            WriteLine("９８７６５４３２１");
            for (var y = 1; y <= 9; y++)
            {
                for (var x = 9; x >= 1; x--)
                {
                    if (moveBase is Move move)
                    {
                        var (from, to) = (move.From, move.To);
                        BackgroundColor = from.X == x && from.Y == y || to.X == x && to.Y == y
                            ? movedPieceBg_ : boardBg_;
                    }
                    else if (moveBase is Drop drop)
                    {
                        var to = drop.To;
                        BackgroundColor = to.X == x && to.Y == y
                            ? movedPieceBg_ : boardBg_;
                    }

                    var position = new Position(x, y);
                    Piece piece;
                    ForegroundColor = (piece = blackBoard[position]).IsPiece() ? blackPieceFg_
                        : (piece = whiteBoard[position]).IsPiece() ? whitePieceFg_ : emptyFg_;
                    Write(piece.ToOneLetter());
                    BackgroundColor = boardBg_;
                }
                ForegroundColor = boardFg_;
                WriteLine(y switch
                {
                    1 => "一",
                    2 => "二",
                    3 => "三",
                    4 => "四",
                    5 => "五",
                    6 => "六",
                    7 => "七",
                    8 => "八",
                    9 => "九",
                    _ => "",
                });
            }
            ResetColor();
        }

        private static void DrawHands(List<Piece> blackHands, List<Piece> whiteHands)
        {
            ForegroundColor = boardFg_;
            BackgroundColor = boardBg_;
            Write("先手持ち駒:");
            foreach (var h in blackHands.GroupBy(x => x))
            {
                Write($"{h.Key.ToOneLetter()}{h.Count()} ");
            }
            WriteLine();
            Write("後手持ち駒:");
            foreach (var h in whiteHands.GroupBy(x => x))
            {
                Write($"{h.Key.ToOneLetter()}{h.Count()} ");
            }
            WriteLine();
            ResetColor();
        }
    }
}