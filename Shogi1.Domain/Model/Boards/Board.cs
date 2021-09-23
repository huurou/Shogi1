using Shogi1.Domain.Model.Moves;
using Shogi1.Domain.Model.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Shogi1.Domain.Consts;
using static Shogi1.Domain.Model.Pieces.Piece;

namespace Shogi1.Domain.Model.Boards
{
    public class Board
    {
        private readonly Piece[] board_;

        private readonly List<Piece> handsBlack_;
        private readonly List<Piece> handsWhite_;

        public bool Teban { get; private set; }
        public int Turns { get; private set; }

        internal Board()
        {
            board_ = new Piece[]
            {
                壁,  壁,  壁,  壁,  壁,  壁,  壁,  壁,  壁,  壁,  壁,
                壁, 香W, 桂W, 銀W, 金W, 王W, 金W, 銀W, 桂W, 香W,  壁,
                壁,  空, 飛W,  空,  空,  空,  空,  空, 角W,  空,  壁,
                壁, 歩W, 歩W, 歩W, 歩W, 歩W, 歩W, 歩W, 歩W, 歩W,  壁,
                壁,  空,  空,  空,  空,  空,  空,  空,  空,  空,  壁,
                壁,  空,  空,  空,  空,  空,  空,  空,  空,  空,  壁,
                壁,  空,  空,  空,  空,  空,  空,  空,  空,  空,  壁,
                壁, 歩B, 歩B, 歩B, 歩B, 歩B, 歩B, 歩B, 歩B, 歩B,  壁,
                壁,  空, 角B,  空,  空,  空,  空,  空, 飛B,  空,  壁,
                壁, 香B, 桂B, 銀B, 金B, 王B, 金B, 銀B, 桂B, 香B,  壁,
                壁,  壁,  壁,  壁,  壁,  壁,  壁,  壁,  壁,  壁,  壁,
            };
            handsBlack_ = new();
            handsWhite_ = new();

            Teban = BLACK;
            Turns = 0;
        }

        internal void ChangeTeban() => Teban = !Teban;

        /// <summary>
        /// 全ての指し手から盤外への移動を除いたもの
        /// 自殺手も含む
        /// 移動不可能になる場所への着手(敵陣1段目への歩等)は除く
        /// </summary>
        /// <returns></returns>
        internal List<MoveBase> GetPseudoMoves()
        {
            var moves = new List<MoveBase> { Capacity = 128 };
            // Move
            for (var y = 1; y <= 9; y++)
            {
                for (var x = 1; x <= 9; x++)
                {
                    var from = new Position(x, y);
                    var piece = board_[from];
                    if (!piece.IsPiece() || piece.GetTeban() != Teban) continue;
                    foreach (var to in piece.GetPositions(from))
                    {
                        // 自駒は捕獲できない
                        if (board_[to].IsPiece() && board_[to].GetTeban() == Teban) continue;
                        // 飛龍角馬香は駒を飛び越えて移動できない
                        if (piece is 飛B or 飛W or 龍王B or 龍王W &&
                            (from.X == to.X || from.Y == to.Y) ||
                            piece is 角B or 角W or 龍馬B or 龍馬W &&
                            (from.X + from.Y == to.X + to.Y || from.X - from.Y == to.X - to.Y) ||
                            piece is 香B && from.X == to.X && from.Y > to.Y ||
                            piece is 香W && from.X == to.X && from.Y < to.Y)
                        {
                            var range = Position.Range(from, to);
                            if (range.Any() && !range.All(x => board_[x] == 空)) continue;
                        }
                        // 不成
                        // 行き場のない場所への移動は不可
                        if (!(piece is 歩B or 香B or 桂B && to.Y == 1 ||
                            piece is 桂B && to.Y == 2 ||
                            piece is 桂W && to.Y == 8 ||
                            piece is 歩W or 香W or 桂W && to.Y == 9))
                        {
                            moves.Add(board_[to].IsPiece()
                            ? new Move(Teban, piece, to, from, captured: true, pieceCaptured: board_[to])
                            : new Move(Teban, piece, to, from));
                        }
                        // 成り
                        if (piece.IsPromotable() &&
                            (Teban && from.Y is >= 1 and <= 3 && to.Y is >= 1 and <= 3 ||
                            !Teban && from.Y is >= 7 and <= 9 && to.Y is >= 7 and <= 9))
                        {
                            moves.Add(board_[to].IsPiece()
                               ? new Move(Teban, piece, to, from, captured: true, pieceCaptured: board_[to], promoted: true)
                               : new Move(Teban, piece, to, from, promoted: true));
                        }
                    }
                }
            }

            // Drop
            foreach (var hand in (Teban ? handsBlack_ : handsWhite_).Distinct())
            {
                // 以下の駒は手駒にあってはいけない
                if (!hand.IsPiece() || hand.GetTeban() != Teban ||
                    hand is 王B or 王W or 龍王W or
                    龍王B or 龍馬B or 龍馬W or
                    成銀B or 成銀W or 成桂B or
                    成桂W or 成香B or 成香W or
                    と金B or と金W) throw new InvalidOperationException();
                for (var y = 1; y <= 9; y++)
                {
                    // 行きどころのない駒打ちは不可
                    if (hand is 歩B or 香B or 桂B && y == 1) continue;
                    if (hand is 桂B && y == 2) continue;
                    if (hand is 桂W && y == 8) continue;
                    if (hand is 歩W or 香W or 桂W && y == 9) continue;
                    for (var x = 1; x <= 9; x++)
                    {
                        // 空の場所にしか打てない
                        if (board_[new Position(x, y)].IsPiece()) continue;
                        // 二歩は不可
                        if (hand is 歩B && ExistsOnLine(歩B, x)) continue;
                        if (hand is 歩W && ExistsOnLine(歩W, x)) continue;
                        moves.Add(new Drop(Teban, hand, new(x, y)));
                    }
                }
            }
            return moves;

            bool ExistsOnLine(Piece piece, int x)
                => Enumerable.Range(1, 9).Select(y => board_[new Position(x, y)]).Any(x => x == piece);
        }

        internal void DoMove(MoveBase moveBase)
        {
            if (moveBase.Teban != Teban) throw new InvalidOperationException();
            var hands = Teban ? handsBlack_ : handsWhite_;
            if (moveBase is Move move)
            {
                if (move.Captured)
                {
                    hands.Add(move.PieceCaptured.Unpromote().ReverseTeban());
                }
                board_[move.To] = move.Piece;
                board_[move.From] = 空;
            }
            else if (moveBase is Drop drop)
            {
                hands.Remove(drop.Piece);
                board_[drop.To] = drop.Piece;
            }
            ChangeTeban();
            Turns++;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("９ ８ ７ ６ ５ ４ ３ ２ １ ");
            for (var y = 1; y <= 9; y++)
            {
                for (var x = 9; x >= 1; x--)
                {
                    var piece = board_[new Position(x, y)];
                    sb.Append(piece.IsPiece() ? piece : "・ ");
                }
                sb.AppendLine(y switch
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
            sb.Append("先手持ち駒:");
            foreach (var h in handsBlack_.GroupBy(x => x))
            {
                sb.Append($"{h.Key}{h.Count()} ");
            }
            sb.AppendLine();
            sb.Append("後手持ち駒:");
            foreach (var h in handsWhite_.GroupBy(x => x))
            {
                sb.Append($"{h.Key}{h.Count()} ");
            }

            return sb.ToString();
        }
    }
}