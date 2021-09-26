using Shogi1.Application;
using Shogi1.Domain.Model.AIs;
using Shogi1.Domain.Model.Boards;
using Shogi1.Domain.Model.Games;
using Shogi1.Domain.Model.Moves;
using Shogi1.Domain.Model.Pieces;
using System;
using System.Linq;
using static Shogi1.Domain.Model.Pieces.Piece;

namespace Shogi1.Presentation.ConsolApp
{
    internal class Program
    {
        private static readonly ShogiApplicationService appService_
            = new(
                new PieceValueAlphaBetaAI(5),
                new PieceAndEffectValueAlphaBetaAI(4)
                );

        private static readonly ConsoleColor bg_ = ConsoleColor.DarkYellow;
        private static readonly ConsoleColor fg_ = ConsoleColor.Magenta;
        private static readonly ConsoleColor blackPiece_ = ConsoleColor.Black;
        private static readonly ConsoleColor whitePiece_ = ConsoleColor.White;
        private static readonly ConsoleColor empty_ = ConsoleColor.DarkGray;
        private static readonly ConsoleColor pieceBg_ = ConsoleColor.DarkCyan;

        private static void Main()
        {
            Console.CursorVisible = false;
            Console.ResetColor();
            appService_.GameStart += (s, e) =>
            {
                DrawBoard(e);
                Console.WriteLine();
            };
            appService_.Moved += (s, e) =>
            {
                DrawBoard(e.board, e.move, e.eval);
                Console.WriteLine();
            };
            appService_.GameEnd += (s, e) =>
            {
                Console.WriteLine($"{(e == Result.Win ? "先手勝ち" : "後手勝ち")}");
            };
            appService_.LoopEnd += (s, e) => Console.WriteLine($"先手勝ち:{e.win} 後手勝ち:{e.lose} 引き分け:{e.draw}");

            appService_.GameLoop(1);
        }

        private static void DrawBoard(Board board, MoveBase? moveBase = null, double? eval = null)
        {
            Console.Write(new string(' ', 50));
            Console.SetCursorPosition( 0, Console.CursorTop);
            Console.WriteLine(eval is null ? "" : $"{moveBase} eval:{eval}");
            Console.BackgroundColor = bg_;
            Console.ForegroundColor = fg_;
            Console.WriteLine("９８７６５４３２１");
            for (var y = 1; y <= 9; y++)
            {
                for (var x = 9; x >= 1; x--)
                {
                    if (moveBase is Move move)
                    {
                        var (from, to) = (move.From, move.To);
                        Console.BackgroundColor = from.X == x && from.Y == y || to.X == x && to.Y == y
                            ? pieceBg_ : bg_;
                    }
                    else if (moveBase is Drop drop)
                    {
                        var to = drop.To;
                        Console.BackgroundColor = to.X == x && to.Y == y
                            ? pieceBg_ : bg_;
                    }
                    var piece = board.Pieces[new Position(x, y)];
                    Console.ForegroundColor = piece.IsPiece()
                        ? piece.GetTeban() ? blackPiece_ : whitePiece_
                        : empty_;
                    Console.Write(piece.IsPiece() ? piece switch
                    {
                        王B => "玉",
                        王W => "王",
                        飛B or 飛W => "飛",
                        龍王B or 龍王W => "龍",
                        角B or 角W => "角",
                        龍馬B or 龍馬W => "馬",
                        金B or 金W => "金",
                        銀B or 銀W => "銀",
                        成銀B or 成銀W => "全",
                        桂B or 桂W => "桂",
                        成桂B or 成桂W => "圭",
                        香B or 香W => "香",
                        成香B or 成香W => "杏",
                        歩B or 歩W => "歩",
                        と金B or と金W => "と",
                        _ => "・",
                    } : "・");
                    Console.BackgroundColor = bg_;
                }
                Console.ForegroundColor = fg_;
                Console.WriteLine(y switch
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
            Console.ForegroundColor = fg_;
            Console.Write(new string(' ', 50));
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write("先手持ち駒:");
            foreach (var h in board.HandsBlack.GroupBy(x => x))
            {
                Console.Write($"{h.Key}{h.Count()} ");
            }
            Console.WriteLine();
            Console.Write(new string(' ', 50));
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write("後手持ち駒:");
            foreach (var h in board.HandsWhite.GroupBy(x => x))
            {
                Console.Write($"{h.Key}{h.Count()} ");
            }
            Console.ResetColor();
            //Console.SetCursorPosition(0, 0);
        }
    }
}