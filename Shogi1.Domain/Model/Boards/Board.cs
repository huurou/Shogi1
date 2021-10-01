using Shogi1.Domain.Model.Moves;
using Shogi1.Domain.Model.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using static Shogi1.Domain.Consts;
using static Shogi1.Domain.Model.Pieces.Piece;

namespace Shogi1.Domain.Model.Boards
{
    /// <summary>
    /// 盤
    /// </summary>
    public class Board
    {
        /// <summary>
        /// 指し手のスタック
        /// </summary>
        public Stack<MoveBase> Stack { get; } = new();

        /// <summary>
        /// 盤上の駒
        /// </summary>
        internal Piece[] Pieces { get; }

        /// <summary>
        /// 先手の持ち駒
        /// </summary>
        internal List<Piece> HandsBlack { get; } = new();

        /// <summary>
        /// 後手の持ち駒
        /// </summary>
        internal List<Piece> HandsWhite { get; } = new();

        /// <summary>
        /// 手番 True:先手/False:後手
        /// </summary>
        internal bool Teban { get; private set; } = BLACK;
        /// <summary>
        /// 手数
        /// </summary>
        internal int Turns { get; private set; }

        internal bool IsChackMate => GetLegalMoves().Count == 0;

        /// <summary>
        /// 平手初期局面
        /// </summary>
        internal Board() => Pieces = new Piece[]
        {
            香W, 桂W, 銀W, 金W, 王W, 金W, 銀W, 桂W, 香W,
             空, 飛W,  空,  空,  空,  空,  空, 角W,  空,
            歩W, 歩W, 歩W, 歩W, 歩W, 歩W, 歩W, 歩W, 歩W,
             空,  空,  空,  空,  空,  空,  空,  空,  空,
             空,  空,  空,  空,  空,  空,  空,  空,  空,
             空,  空,  空,  空,  空,  空,  空,  空,  空,
            歩B, 歩B, 歩B, 歩B, 歩B, 歩B, 歩B, 歩B, 歩B,
             空, 角B,  空,  空,  空,  空,  空, 飛B,  空,
            香B, 桂B, 銀B, 金B, 王B, 金B, 銀B, 桂B, 香B,
        };

        /// <summary>
        /// 手番を入れ替える
        /// </summary>
        internal void ChangeTeban() => Teban = !Teban;

        /// <summary>
        /// 盤外への移動と自駒を取る手、行き場のない場所への着手、二歩を除いたもの
        /// 自殺手を含む
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
                    var piece = Pieces[from];
                    if (!piece.IsPiece() || piece.GetTeban() != Teban) continue;
                    foreach (var to in piece.GetPositions(from))
                    {
                        // 自駒は捕獲できない
                        if (Pieces[to].IsPiece() && Pieces[to].GetTeban() == Teban) continue;
                        // 飛龍角馬香は駒を飛び越えて移動できない
                        if (piece is 飛B or 飛W or 龍王B or 龍王W &&
                            (from.X == to.X || from.Y == to.Y) ||
                            piece is 角B or 角W or 龍馬B or 龍馬W &&
                            (from.X + from.Y == to.X + to.Y || from.X - from.Y == to.X - to.Y) ||
                            piece is 香B && from.X == to.X && from.Y > to.Y ||
                            piece is 香W && from.X == to.X && from.Y < to.Y)
                        {
                            var range = Position.Range(from, to);
                            if (range.Any() && !range.All(x => Pieces[x] == 空)) continue;
                        }
                        // 不成
                        // 行き場のない場所への移動は不可
                        if (!(piece is 歩B or 香B or 桂B && to.Y == 1 ||
                            piece is 桂B && to.Y == 2 ||
                            piece is 桂W && to.Y == 8 ||
                            piece is 歩W or 香W or 桂W && to.Y == 9 ||
                            // 飛角歩は成らない手は考えない
                            piece is 飛B or 角B or 歩B && (from.Y is >= 1 and <= 3 || to.Y is >= 1 and <= 3) ||
                            piece is 飛W or 角W or 歩W && (from.Y is >= 7 and <= 9 || to.Y is >= 7 and <= 9)))
                        {
                            moves.Add(Pieces[to].IsPiece()
                            ? new Move(Teban, piece, to, from, captured: true, pieceCaptured: Pieces[to])
                            : new Move(Teban, piece, to, from));
                        }
                        // 成り
                        if (piece.IsPromotable() &&
                            (Teban && (from.Y is >= 1 and <= 3 || to.Y is >= 1 and <= 3) ||
                            !Teban && (from.Y is >= 7 and <= 9 || to.Y is >= 7 and <= 9)))
                        {
                            moves.Add(Pieces[to].IsPiece()
                               ? new Move(Teban, piece, to, from, captured: true, pieceCaptured: Pieces[to], promoted: true)
                               : new Move(Teban, piece, to, from, promoted: true));
                        }
                    }
                }
            }

            // Drop
            foreach (var hand in (Teban ? HandsBlack : HandsWhite).Distinct())
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
                        if (Pieces[new Position(x, y)].IsPiece()) continue;
                        // 二歩は不可
                        if (hand is 歩B && ExistsOnLine(歩B, x)) continue;
                        if (hand is 歩W && ExistsOnLine(歩W, x)) continue;
                        moves.Add(new Drop(Teban, hand, new(x, y)));
                    }
                }
            }
            return moves;

            bool ExistsOnLine(Piece piece, int x)
                => Enumerable.Range(1, 9).Select(y => Pieces[new Position(x, y)]).Any(x => x == piece);
        }

        /// <summary>
        /// 指し手が自殺手かどうか
        /// </summary>
        /// <param name="moveBase">指し手</param>
        /// <returns>自殺手でない/自殺手</returns>
        internal bool IsLegalMove(MoveBase moveBase)
        {
            DoMove(moveBase);
            var res = !IsCheck(moveBase.Teban);
            UndoMove();
            return res;
        }

        internal List<MoveBase> GetLegalMoves()
        {
            if (legalMoves_ is not null) return legalMoves_;
            var lms = GetPseudoMoves().Where(x => IsLegalMove(x)).ToList();
            var moves = lms.OfType<Move>();
            var drops = lms.OfType<Drop>();
            var cps = moves.Where(x => x.Captured && x.Promoted).OfType<MoveBase>();
            var cs = moves.Where(x => x.Captured && !x.Promoted).OfType<MoveBase>();
            var ps = moves.Where(x => !x.Captured && x.Promoted).OfType<MoveBase>();
            var others = moves.Where(x => !x.Captured && !x.Promoted).OfType<MoveBase>();
            legalMoves_ = cps.Concat(cs).Concat(ps).Concat(drops).Concat(others).ToList();
            return legalMoves_;
        }

        private List<MoveBase>? legalMoves_;

        /// <summary>
        /// 手を進める
        /// </summary>
        /// <param name="moveBase">指し手</param>
        internal void DoMove(MoveBase moveBase)
        {
            Stack.Push(moveBase);
            legalMoves_ = null;
            if (moveBase.Teban != Teban) throw new InvalidOperationException();
            var hands = moveBase.Teban ? HandsBlack : HandsWhite;
            if (moveBase is Move move)
            {
                if (move.Captured) hands.Add(move.PieceCaptured.Unpromote().ReverseTeban());
                Pieces[move.To] = move.Promoted ? move.Piece.Promote() : move.Piece;
                Pieces[move.From] = 空;
            }
            else if (moveBase is Drop drop)
            {
                hands.Remove(drop.Piece);
                Pieces[drop.To] = drop.Piece;
            }
            ChangeTeban();
            Turns++;
        }

        /// <summary>
        /// 手を戻す
        /// </summary>
        internal void UndoMove()
        {
            var moveBase = Stack.Pop();
            legalMoves_ = null;
            if (moveBase.Teban == Teban) throw new InvalidOperationException();
            var hands = moveBase.Teban ? HandsBlack : HandsWhite;
            if (moveBase is Move move)
            {
                if (move.Captured) hands.Remove(move.PieceCaptured.Unpromote().ReverseTeban());
                Pieces[move.To] = move.PieceCaptured;
                Pieces[move.From] = move.Promoted ? move.Piece.Unpromote() : move.Piece;
            }
            else if (moveBase is Drop drop)
            {
                hands.Add(drop.Piece);
                Pieces[drop.To] = 空;
            }
            ChangeTeban();
            Turns--;
        }

        /// <summary>
        /// 王手がかかっているかどうか
        /// </summary>
        /// <param name="teban">どちらの玉に対する王手を調べたいか</param>
        /// <returns>王手がかかっている/かかっていない</returns>
        internal bool IsCheck(bool teban)
        {
            if (teban)
            {
                var kingPos = new Position(Array.IndexOf(Pieces, 王B));

                // 上
                var position = kingPos.Up();
                if (position.IsOnBoard)
                {
                    var piece = Pieces[position];
                    if (piece is 王W or 飛W or 龍王W or
                        龍馬W or 金W or 銀W or
                        成銀W or 成桂W or 香W or
                        成香W or 歩W or と金W) return true;

                    while (position.IsOnBoard)
                    {
                        piece = Pieces[position];
                        if (piece.IsEmpty()) position = position.Up();
                        else if (piece is 飛W or 龍王W or 香W) return true;
                        else break;
                    }
                }

                // 右上
                position = kingPos.UpRight();
                if (position.IsOnBoard)
                {
                    var piece = Pieces[position];
                    if (piece is 王W or 龍王W or 角W or
                        龍馬W or 金W or 銀W or
                        成銀W or 成桂W or 成香W or
                        と金W) return true;

                    while (position.IsOnBoard)
                    {
                        piece = Pieces[position];
                        if (piece.IsEmpty()) position = position.UpRight();
                        else if (piece is 角W or 龍馬W) return true;
                        else break;
                    }
                }

                // 右
                position = kingPos.Right();
                if (position.IsOnBoard)
                {
                    var piece = Pieces[position];
                    if (piece is 王W or 飛W or 龍王W or
                        龍馬W or 金W or 成銀W or
                        成桂W or 成香W or と金W) return true;

                    while (position.IsOnBoard)
                    {
                        piece = Pieces[position];
                        if (piece.IsEmpty()) position = position.Right();
                        else if (piece is 飛W or 龍王W) return true;
                        else break;
                    }
                }

                // 右下
                position = kingPos.DownRight();
                if (position.IsOnBoard)
                {
                    var piece = Pieces[position];
                    if (piece is 王W or 龍王W or 角W or
                        龍馬W or 銀W) return true;

                    while (position.IsOnBoard)
                    {
                        piece = Pieces[position];
                        if (piece.IsEmpty()) position = position.DownRight();
                        else if (piece is 角W or 龍馬W) return true;
                        else break;
                    }
                }

                // 下
                position = kingPos.Down();
                if (position.IsOnBoard)
                {
                    var piece = Pieces[position];
                    if (piece is 王W or 飛W or 龍王W or
                        龍馬W or 金W or 成銀W or
                        成桂W or 成香W or と金W) return true;

                    while (position.IsOnBoard)
                    {
                        piece = Pieces[position];
                        if (piece.IsEmpty()) position = position.Down();
                        else if (piece is 飛W or 龍王W) return true;
                        else break;
                    }
                }

                // 左下
                position = kingPos.DownLeft();
                if (position.IsOnBoard)
                {
                    var piece = Pieces[position];
                    if (piece is 王W or 龍王W or 角W or
                        龍馬W or 銀W) return true;

                    while (position.IsOnBoard)
                    {
                        piece = Pieces[position];
                        if (piece.IsEmpty()) position = position.DownLeft();
                        else if (piece is 角W or 龍馬W) return true;
                        else break;
                    }
                }

                // 左
                position = kingPos.Left();
                if (position.IsOnBoard)
                {
                    var piece = Pieces[position];
                    if (piece is 王W or 飛W or 龍王W or
                        龍馬W or 金W or 成銀W or
                        成桂W or 成香W or と金W) return true;

                    while (position.IsOnBoard)
                    {
                        piece = Pieces[position];
                        if (piece.IsEmpty()) position = position.Left();
                        else if (piece is 飛W or 龍王W) return true;
                        else break;
                    }
                }

                // 左上
                position = kingPos.UpLeft();
                if (position.IsOnBoard)
                {
                    var piece = Pieces[position];
                    if (piece is 王W or 龍王W or 角W or
                        龍馬W or 金W or 銀W or
                        成銀W or 成桂W or 成香W or
                        と金W) return true;

                    while (position.IsOnBoard)
                    {
                        piece = Pieces[position];
                        if (piece.IsEmpty()) position = position.UpLeft();
                        else if (piece is 角W or 龍馬W) return true;
                        else break;
                    }
                }

                //桂馬
                if (new Position[] { kingPos.JumpUpLeft(), kingPos.JumpUpRight() }.Any(x => x.IsOnBoard && Pieces[x] == 桂W)) return true;
            }
            else
            {
                var kingPos = new Position(Array.IndexOf(Pieces, 王W));

                // 上
                var position = kingPos.Up();
                if (position.IsOnBoard)
                {
                    var piece = Pieces[position];
                    if (piece is 王B or 飛B or 龍王B or
                        龍馬B or 金B or 成銀B or
                        成桂B or 成香B or と金B) return true;

                    while (position.IsOnBoard)
                    {
                        piece = Pieces[position];
                        if (piece.IsEmpty()) position = position.Up();
                        else if (piece is 飛B or 龍王B) return true;
                        else break;
                    }
                }

                // 右上
                position = kingPos.UpRight();
                if (position.IsOnBoard)
                {
                    var piece = Pieces[position];
                    if (piece is 王B or 龍王B or 角B or
                        龍馬B or 銀B) return true;

                    while (position.IsOnBoard)
                    {
                        piece = Pieces[position];
                        if (piece.IsEmpty()) position = position.UpRight();
                        else if (piece is 角B or 龍馬B) return true;
                        else break;
                    }
                }

                // 右
                position = kingPos.Right();
                if (position.IsOnBoard)
                {
                    var piece = Pieces[position];
                    if (piece is 王B or 飛B or 龍王B or
                        龍馬B or 金B or 成銀B or
                        成桂B or 成香B or と金B) return true;

                    while (position.IsOnBoard)
                    {
                        piece = Pieces[position];
                        if (piece.IsEmpty()) position = position.Right();
                        else if (piece is 飛B or 龍王B) return true;
                        else break;
                    }
                }

                // 右下
                position = kingPos.DownRight();
                if (position.IsOnBoard)
                {
                    var piece = Pieces[position];
                    if (piece is 王B or 龍王B or 角B or
                        龍馬B or 金B or 銀B or
                        成銀B or 成桂B or 成香B or
                        と金B) return true;

                    while (position.IsOnBoard)
                    {
                        piece = Pieces[position];
                        if (piece.IsEmpty()) position = position.DownRight();
                        else if (piece is 角B or 龍馬B) return true;
                        else break;
                    }
                }

                // 下
                position = kingPos.Down();
                if (position.IsOnBoard)
                {
                    var piece = Pieces[position];
                    if (piece is 王B or 飛B or 龍王B or
                        龍馬B or 金B or 銀B or
                        成銀B or 成桂B or 香B or
                        成香B or 歩B or と金B) return true;

                    while (position.IsOnBoard)
                    {
                        piece = Pieces[position];
                        if (piece.IsEmpty()) position = position.Down();
                        else if (piece is 飛B or 龍王B or 香B) return true;
                        else break;
                    }
                }

                // 左下
                position = kingPos.DownLeft();
                if (position.IsOnBoard)
                {
                    var piece = Pieces[position];
                    if (piece is 王B or 龍王B or 角B or
                        龍馬B or 金B or 銀B or
                        成銀B or 成桂B or 成香B or
                        と金B) return true;

                    while (position.IsOnBoard)
                    {
                        piece = Pieces[position];
                        if (piece.IsEmpty()) position = position.DownLeft();
                        else if (piece is 角B or 龍馬B) return true;
                        else break;
                    }
                }

                // 左
                position = kingPos.Left();
                if (position.IsOnBoard)
                {
                    var piece = Pieces[position];
                    if (piece is 王B or 飛B or 龍王B or
                        龍馬B or 金B or 成銀B or
                        成桂B or 成香B or と金B) return true;

                    while (position.IsOnBoard)
                    {
                        piece = Pieces[position];
                        if (piece.IsEmpty()) position = position.Left();
                        else if (piece is 飛B or 龍王B) return true;
                        else break;
                    }
                }

                // 左上
                position = kingPos.UpLeft();
                if (position.IsOnBoard)
                {
                    var piece = Pieces[position];
                    if (piece is 王B or 龍王B or 角B or
                        龍馬B or 銀B) return true;

                    while (position.IsOnBoard)
                    {
                        piece = Pieces[position];
                        if (piece.IsEmpty()) position = position.UpLeft();
                        else if (piece is 角B or 龍馬B) return true;
                        else break;
                    }
                }

                //桂馬
                if (new Position[] { kingPos.JumpDownLeft(), kingPos.JumpDownRight() }.Any(x => x.IsOnBoard && Pieces[x] == 桂B)) return true;
            }
            return false;
        }

        internal (int b, int w) Effects(Position position)
        {
            // 先手の駒の利きの数
            var b = 0;
            //後手の駒の利きの数
            var w = 0;
            // 上
            var p = position.Up();
            if (p.IsOnBoard)
            {
                var piece = Pieces[p];
                if (piece is 王B or 龍馬B or 金B or
                    成銀B or 成桂B or 成香B or
                    と金B) b++;
                if (piece is 王W or 龍馬W or 金W or
                    銀W or 成銀W or 成桂W or
                    成香W or 歩W or と金W) w++;
                while (p.IsOnBoard)
                {
                    piece = Pieces[p];
                    if (piece.IsEmpty()) p = p.Up();
                    else if (piece is 飛B or 龍王B) { b++; break; }
                    else if (piece is 飛W or 龍王W or 香W) { w++; break; }
                    else break;
                }
            }

            // 右上
            p = position.UpRight();
            if (p.IsOnBoard)
            {
                var piece = Pieces[p];
                if (piece is 王B or 龍王B or 銀B) b++;
                if (piece is 王W or 龍王W or 金W or
                    銀W or 成銀W or 成桂W or
                    成香W or と金W) w++;

                while (p.IsOnBoard)
                {
                    piece = Pieces[p];
                    if (piece.IsEmpty()) p = p.UpRight();
                    else if (piece is 角B or 龍馬B) { b++; break; }
                    else if (piece is 角W or 龍馬W) { w++; break; }
                    else break;
                }
            }

            // 右
            p = position.Right();
            if (p.IsOnBoard)
            {
                var piece = Pieces[p];
                if (piece is 王B or 龍馬B or 金B or
                    成銀B or 成桂B or 成香B or
                    と金B) b++;
                if (piece is 王W or 龍馬W or 金W or
                    成銀W or 成桂W or 成香W or
                    と金W) w++;

                while (p.IsOnBoard)
                {
                    piece = Pieces[p];
                    if (piece.IsEmpty()) p = p.Right();
                    else if (piece is 飛B or 龍王B) { b++; break; }
                    else if (piece is 飛W or 龍王W) { w++; break; }
                    else break;
                }
            }

            // 右下
            p = position.DownRight();
            if (p.IsOnBoard)
            {
                var piece = Pieces[p];
                if (piece is 王B or 龍王B or 金B or
                    銀B or 成銀B or 成桂B or
                    成香B or と金B) b++;
                if (piece is 王W or 龍王W or 銀W) w++;

                while (p.IsOnBoard)
                {
                    piece = Pieces[p];
                    if (piece.IsEmpty()) p = p.DownRight();
                    else if (piece is 角B or 龍馬B) { b++; break; }
                    else if (piece is 角W or 龍馬W) { w++; break; }
                    else break;
                }
            }

            // 下
            p = position.Down();
            if (p.IsOnBoard)
            {
                var piece = Pieces[p];
                if (piece is 王B or 龍馬B or 金B or
                    銀B or 成銀B or 成桂B or
                    成香B or 歩B or と金B) b++;
                if (piece is 王W or 龍馬W or 金W or
                    成銀W or 成桂W or 成香W or
                    と金W) w++;

                while (p.IsOnBoard)
                {
                    piece = Pieces[p];
                    if (piece.IsEmpty()) p = p.Down();
                    else if (piece is 飛B or 龍王B or 香B) { b++; break; }
                    else if (piece is 飛W or 龍王W) { w++; break; }
                    else break;
                }
            }

            // 左下
            p = position.DownLeft();
            if (p.IsOnBoard)
            {
                var piece = Pieces[p];
                if (piece is 王B or 龍王B or 金B or
                    銀B or 成銀B or 成桂B or
                    成香B or と金B) b++;
                if (piece is 王W or 龍王W or 銀W) w++;

                while (p.IsOnBoard)
                {
                    piece = Pieces[p];
                    if (piece.IsEmpty()) p = p.DownLeft();
                    else if (piece is 角B or 龍馬B) { b++; break; }
                    else if (piece is 角W or 龍馬W) { w++; break; }
                    else break;
                }
            }

            // 左
            p = position.Left();
            if (p.IsOnBoard)
            {
                var piece = Pieces[p];
                if (piece is 王B or 龍馬B or 金B or
                    成銀B or 成桂B or 成香B or
                    と金B) b++;
                if (piece is 王W or 龍馬W or 金W or
                    成銀W or 成桂W or 成香W or
                    と金W) w++;

                while (p.IsOnBoard)
                {
                    piece = Pieces[p];
                    if (piece.IsEmpty()) p = p.Left();
                    else if (piece is 飛B or 龍王B) { b++; break; }
                    else if (piece is 飛W or 龍王W) { w++; break; }
                    else break;
                }
            }

            // 左上
            p = position.UpLeft();
            if (p.IsOnBoard)
            {
                var piece = Pieces[p];
                if (piece is 王B or 龍王B or 銀B) b++;
                if (piece is 王W or 龍王W or 金W or
                    銀W or 成銀W or 成桂W or
                    成香W or と金W) w++;

                while (p.IsOnBoard)
                {
                    piece = Pieces[p];
                    if (piece.IsEmpty()) p = p.UpLeft();
                    else if (piece is 角B or 龍馬B) { b++; break; }
                    else if (piece is 角W or 龍馬W) { w++; break; }
                    else break;
                }
            }

            //桂馬
            if (position.JumpDownLeft().IsOnBoard && Pieces[position.JumpDownLeft()] == 桂B) b++;
            if (position.JumpDownRight().IsOnBoard && Pieces[position.JumpDownRight()] == 桂B) b++;
            if (position.JumpUpRight().IsOnBoard && Pieces[position.JumpUpRight()] == 桂W) w++;
            if (position.JumpUpRight().IsOnBoard && Pieces[position.JumpUpRight()] == 桂W) w++;

            return (b, w);
        }
    }
}