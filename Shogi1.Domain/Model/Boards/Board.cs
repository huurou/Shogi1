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
            LanceW, KnightW, ShilverW, GoldW, KingW, GoldW, ShilverW, KnightW, LanceW,
             None, RookW,  None,  None,  None,  None,  None, BishopW,  None,
            PawnW, PawnW, PawnW, PawnW, PawnW, PawnW, PawnW, PawnW, PawnW,
             None,  None,  None,  None,  None,  None,  None,  None,  None,
             None,  None,  None,  None,  None,  None,  None,  None,  None,
             None,  None,  None,  None,  None,  None,  None,  None,  None,
            PawnB, PawnB, PawnB, PawnB, PawnB, PawnB, PawnB, PawnB, PawnB,
             None, BishopB,  None,  None,  None,  None,  None, RookB,  None,
            LanceB, KnightB, SilverB, GoldB, KingB, GoldB, SilverB, KnightB, LanceB,
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
                        if (piece is RookB or RookW or DragonB or DragonW &&
                            (from.X == to.X || from.Y == to.Y) ||
                            piece is BishopB or BishopW or HorseB or HorseW &&
                            (from.X + from.Y == to.X + to.Y || from.X - from.Y == to.X - to.Y) ||
                            piece is LanceB && from.X == to.X && from.Y > to.Y ||
                            piece is LanceW && from.X == to.X && from.Y < to.Y)
                        {
                            var range = Position.Range(from, to);
                            if (range.Any() && !range.All(x => Pieces[x] == None)) continue;
                        }
                        // 不成
                        // 行き場のない場所への移動は不可
                        if (!(piece is PawnB or LanceB or KnightB && to.Y == 1 ||
                            piece is KnightB && to.Y == 2 ||
                            piece is KnightW && to.Y == 8 ||
                            piece is PawnW or LanceW or KnightW && to.Y == 9 ||
                            // 飛角歩は成らない手は考えない
                            piece is RookB or BishopB or PawnB && (from.Y is >= 1 and <= 3 || to.Y is >= 1 and <= 3) ||
                            piece is RookW or BishopW or PawnW && (from.Y is >= 7 and <= 9 || to.Y is >= 7 and <= 9)))
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
                    hand is KingB or KingW or DragonW or
                    DragonB or HorseB or HorseW or
                    ProShilverB or ProShilverW or ProKnightB or
                    ProKnightW or ProLanceB or ProLanceW or
                    ProPownB or ProPawnW) throw new InvalidOperationException();
                for (var y = 1; y <= 9; y++)
                {
                    // 行きどころのない駒打ちは不可
                    if (hand is PawnB or LanceB or KnightB && y == 1) continue;
                    if (hand is KnightB && y == 2) continue;
                    if (hand is KnightW && y == 8) continue;
                    if (hand is PawnW or LanceW or KnightW && y == 9) continue;
                    for (var x = 1; x <= 9; x++)
                    {
                        // 空の場所にしか打てない
                        if (Pieces[new Position(x, y)].IsPiece()) continue;
                        // 二歩は不可
                        if (hand is PawnB && ExistsOnLine(PawnB, x)) continue;
                        if (hand is PawnW && ExistsOnLine(PawnW, x)) continue;
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
                Pieces[move.From] = None;
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
                Pieces[drop.To] = None;
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
                var kingPos = new Position(Array.IndexOf(Pieces, KingB));

                // 上
                var position = kingPos.Up();
                if (position.IsOnBoard)
                {
                    var piece = Pieces[position];
                    if (piece is KingW or RookW or DragonW or
                        HorseW or GoldW or ShilverW or
                        ProShilverW or ProKnightW or LanceW or
                        ProLanceW or PawnW or ProPawnW) return true;

                    while (position.IsOnBoard)
                    {
                        piece = Pieces[position];
                        if (piece.IsEmpty()) position = position.Up();
                        else if (piece is RookW or DragonW or LanceW) return true;
                        else break;
                    }
                }

                // 右上
                position = kingPos.UpRight();
                if (position.IsOnBoard)
                {
                    var piece = Pieces[position];
                    if (piece is KingW or DragonW or BishopW or
                        HorseW or GoldW or ShilverW or
                        ProShilverW or ProKnightW or ProLanceW or
                        ProPawnW) return true;

                    while (position.IsOnBoard)
                    {
                        piece = Pieces[position];
                        if (piece.IsEmpty()) position = position.UpRight();
                        else if (piece is BishopW or HorseW) return true;
                        else break;
                    }
                }

                // 右
                position = kingPos.Right();
                if (position.IsOnBoard)
                {
                    var piece = Pieces[position];
                    if (piece is KingW or RookW or DragonW or
                        HorseW or GoldW or ProShilverW or
                        ProKnightW or ProLanceW or ProPawnW) return true;

                    while (position.IsOnBoard)
                    {
                        piece = Pieces[position];
                        if (piece.IsEmpty()) position = position.Right();
                        else if (piece is RookW or DragonW) return true;
                        else break;
                    }
                }

                // 右下
                position = kingPos.DownRight();
                if (position.IsOnBoard)
                {
                    var piece = Pieces[position];
                    if (piece is KingW or DragonW or BishopW or
                        HorseW or ShilverW) return true;

                    while (position.IsOnBoard)
                    {
                        piece = Pieces[position];
                        if (piece.IsEmpty()) position = position.DownRight();
                        else if (piece is BishopW or HorseW) return true;
                        else break;
                    }
                }

                // 下
                position = kingPos.Down();
                if (position.IsOnBoard)
                {
                    var piece = Pieces[position];
                    if (piece is KingW or RookW or DragonW or
                        HorseW or GoldW or ProShilverW or
                        ProKnightW or ProLanceW or ProPawnW) return true;

                    while (position.IsOnBoard)
                    {
                        piece = Pieces[position];
                        if (piece.IsEmpty()) position = position.Down();
                        else if (piece is RookW or DragonW) return true;
                        else break;
                    }
                }

                // 左下
                position = kingPos.DownLeft();
                if (position.IsOnBoard)
                {
                    var piece = Pieces[position];
                    if (piece is KingW or DragonW or BishopW or
                        HorseW or ShilverW) return true;

                    while (position.IsOnBoard)
                    {
                        piece = Pieces[position];
                        if (piece.IsEmpty()) position = position.DownLeft();
                        else if (piece is BishopW or HorseW) return true;
                        else break;
                    }
                }

                // 左
                position = kingPos.Left();
                if (position.IsOnBoard)
                {
                    var piece = Pieces[position];
                    if (piece is KingW or RookW or DragonW or
                        HorseW or GoldW or ProShilverW or
                        ProKnightW or ProLanceW or ProPawnW) return true;

                    while (position.IsOnBoard)
                    {
                        piece = Pieces[position];
                        if (piece.IsEmpty()) position = position.Left();
                        else if (piece is RookW or DragonW) return true;
                        else break;
                    }
                }

                // 左上
                position = kingPos.UpLeft();
                if (position.IsOnBoard)
                {
                    var piece = Pieces[position];
                    if (piece is KingW or DragonW or BishopW or
                        HorseW or GoldW or ShilverW or
                        ProShilverW or ProKnightW or ProLanceW or
                        ProPawnW) return true;

                    while (position.IsOnBoard)
                    {
                        piece = Pieces[position];
                        if (piece.IsEmpty()) position = position.UpLeft();
                        else if (piece is BishopW or HorseW) return true;
                        else break;
                    }
                }

                //桂馬
                if (new Position[] { kingPos.JumpUpLeft(), kingPos.JumpUpRight() }.Any(x => x.IsOnBoard && Pieces[x] == KnightW)) return true;
            }
            else
            {
                var kingPos = new Position(Array.IndexOf(Pieces, KingW));

                // 上
                var position = kingPos.Up();
                if (position.IsOnBoard)
                {
                    var piece = Pieces[position];
                    if (piece is KingB or RookB or DragonB or
                        HorseB or GoldB or ProShilverB or
                        ProKnightB or ProLanceB or ProPownB) return true;

                    while (position.IsOnBoard)
                    {
                        piece = Pieces[position];
                        if (piece.IsEmpty()) position = position.Up();
                        else if (piece is RookB or DragonB) return true;
                        else break;
                    }
                }

                // 右上
                position = kingPos.UpRight();
                if (position.IsOnBoard)
                {
                    var piece = Pieces[position];
                    if (piece is KingB or DragonB or BishopB or
                        HorseB or SilverB) return true;

                    while (position.IsOnBoard)
                    {
                        piece = Pieces[position];
                        if (piece.IsEmpty()) position = position.UpRight();
                        else if (piece is BishopB or HorseB) return true;
                        else break;
                    }
                }

                // 右
                position = kingPos.Right();
                if (position.IsOnBoard)
                {
                    var piece = Pieces[position];
                    if (piece is KingB or RookB or DragonB or
                        HorseB or GoldB or ProShilverB or
                        ProKnightB or ProLanceB or ProPownB) return true;

                    while (position.IsOnBoard)
                    {
                        piece = Pieces[position];
                        if (piece.IsEmpty()) position = position.Right();
                        else if (piece is RookB or DragonB) return true;
                        else break;
                    }
                }

                // 右下
                position = kingPos.DownRight();
                if (position.IsOnBoard)
                {
                    var piece = Pieces[position];
                    if (piece is KingB or DragonB or BishopB or
                        HorseB or GoldB or SilverB or
                        ProShilverB or ProKnightB or ProLanceB or
                        ProPownB) return true;

                    while (position.IsOnBoard)
                    {
                        piece = Pieces[position];
                        if (piece.IsEmpty()) position = position.DownRight();
                        else if (piece is BishopB or HorseB) return true;
                        else break;
                    }
                }

                // 下
                position = kingPos.Down();
                if (position.IsOnBoard)
                {
                    var piece = Pieces[position];
                    if (piece is KingB or RookB or DragonB or
                        HorseB or GoldB or SilverB or
                        ProShilverB or ProKnightB or LanceB or
                        ProLanceB or PawnB or ProPownB) return true;

                    while (position.IsOnBoard)
                    {
                        piece = Pieces[position];
                        if (piece.IsEmpty()) position = position.Down();
                        else if (piece is RookB or DragonB or LanceB) return true;
                        else break;
                    }
                }

                // 左下
                position = kingPos.DownLeft();
                if (position.IsOnBoard)
                {
                    var piece = Pieces[position];
                    if (piece is KingB or DragonB or BishopB or
                        HorseB or GoldB or SilverB or
                        ProShilverB or ProKnightB or ProLanceB or
                        ProPownB) return true;

                    while (position.IsOnBoard)
                    {
                        piece = Pieces[position];
                        if (piece.IsEmpty()) position = position.DownLeft();
                        else if (piece is BishopB or HorseB) return true;
                        else break;
                    }
                }

                // 左
                position = kingPos.Left();
                if (position.IsOnBoard)
                {
                    var piece = Pieces[position];
                    if (piece is KingB or RookB or DragonB or
                        HorseB or GoldB or ProShilverB or
                        ProKnightB or ProLanceB or ProPownB) return true;

                    while (position.IsOnBoard)
                    {
                        piece = Pieces[position];
                        if (piece.IsEmpty()) position = position.Left();
                        else if (piece is RookB or DragonB) return true;
                        else break;
                    }
                }

                // 左上
                position = kingPos.UpLeft();
                if (position.IsOnBoard)
                {
                    var piece = Pieces[position];
                    if (piece is KingB or DragonB or BishopB or
                        HorseB or SilverB) return true;

                    while (position.IsOnBoard)
                    {
                        piece = Pieces[position];
                        if (piece.IsEmpty()) position = position.UpLeft();
                        else if (piece is BishopB or HorseB) return true;
                        else break;
                    }
                }

                //桂馬
                if (new Position[] { kingPos.JumpDownLeft(), kingPos.JumpDownRight() }.Any(x => x.IsOnBoard && Pieces[x] == KnightB)) return true;
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
                if (piece is KingB or HorseB or GoldB or
                    ProShilverB or ProKnightB or ProLanceB or
                    ProPownB) b++;
                if (piece is KingW or HorseW or GoldW or
                    ShilverW or ProShilverW or ProKnightW or
                    ProLanceW or PawnW or ProPawnW) w++;
                while (p.IsOnBoard)
                {
                    piece = Pieces[p];
                    if (piece.IsEmpty()) p = p.Up();
                    else if (piece is RookB or DragonB) { b++; break; }
                    else if (piece is RookW or DragonW or LanceW) { w++; break; }
                    else break;
                }
            }

            // 右上
            p = position.UpRight();
            if (p.IsOnBoard)
            {
                var piece = Pieces[p];
                if (piece is KingB or DragonB or SilverB) b++;
                if (piece is KingW or DragonW or GoldW or
                    ShilverW or ProShilverW or ProKnightW or
                    ProLanceW or ProPawnW) w++;

                while (p.IsOnBoard)
                {
                    piece = Pieces[p];
                    if (piece.IsEmpty()) p = p.UpRight();
                    else if (piece is BishopB or HorseB) { b++; break; }
                    else if (piece is BishopW or HorseW) { w++; break; }
                    else break;
                }
            }

            // 右
            p = position.Right();
            if (p.IsOnBoard)
            {
                var piece = Pieces[p];
                if (piece is KingB or HorseB or GoldB or
                    ProShilverB or ProKnightB or ProLanceB or
                    ProPownB) b++;
                if (piece is KingW or HorseW or GoldW or
                    ProShilverW or ProKnightW or ProLanceW or
                    ProPawnW) w++;

                while (p.IsOnBoard)
                {
                    piece = Pieces[p];
                    if (piece.IsEmpty()) p = p.Right();
                    else if (piece is RookB or DragonB) { b++; break; }
                    else if (piece is RookW or DragonW) { w++; break; }
                    else break;
                }
            }

            // 右下
            p = position.DownRight();
            if (p.IsOnBoard)
            {
                var piece = Pieces[p];
                if (piece is KingB or DragonB or GoldB or
                    SilverB or ProShilverB or ProKnightB or
                    ProLanceB or ProPownB) b++;
                if (piece is KingW or DragonW or ShilverW) w++;

                while (p.IsOnBoard)
                {
                    piece = Pieces[p];
                    if (piece.IsEmpty()) p = p.DownRight();
                    else if (piece is BishopB or HorseB) { b++; break; }
                    else if (piece is BishopW or HorseW) { w++; break; }
                    else break;
                }
            }

            // 下
            p = position.Down();
            if (p.IsOnBoard)
            {
                var piece = Pieces[p];
                if (piece is KingB or HorseB or GoldB or
                    SilverB or ProShilverB or ProKnightB or
                    ProLanceB or PawnB or ProPownB) b++;
                if (piece is KingW or HorseW or GoldW or
                    ProShilverW or ProKnightW or ProLanceW or
                    ProPawnW) w++;

                while (p.IsOnBoard)
                {
                    piece = Pieces[p];
                    if (piece.IsEmpty()) p = p.Down();
                    else if (piece is RookB or DragonB or LanceB) { b++; break; }
                    else if (piece is RookW or DragonW) { w++; break; }
                    else break;
                }
            }

            // 左下
            p = position.DownLeft();
            if (p.IsOnBoard)
            {
                var piece = Pieces[p];
                if (piece is KingB or DragonB or GoldB or
                    SilverB or ProShilverB or ProKnightB or
                    ProLanceB or ProPownB) b++;
                if (piece is KingW or DragonW or ShilverW) w++;

                while (p.IsOnBoard)
                {
                    piece = Pieces[p];
                    if (piece.IsEmpty()) p = p.DownLeft();
                    else if (piece is BishopB or HorseB) { b++; break; }
                    else if (piece is BishopW or HorseW) { w++; break; }
                    else break;
                }
            }

            // 左
            p = position.Left();
            if (p.IsOnBoard)
            {
                var piece = Pieces[p];
                if (piece is KingB or HorseB or GoldB or
                    ProShilverB or ProKnightB or ProLanceB or
                    ProPownB) b++;
                if (piece is KingW or HorseW or GoldW or
                    ProShilverW or ProKnightW or ProLanceW or
                    ProPawnW) w++;

                while (p.IsOnBoard)
                {
                    piece = Pieces[p];
                    if (piece.IsEmpty()) p = p.Left();
                    else if (piece is RookB or DragonB) { b++; break; }
                    else if (piece is RookW or DragonW) { w++; break; }
                    else break;
                }
            }

            // 左上
            p = position.UpLeft();
            if (p.IsOnBoard)
            {
                var piece = Pieces[p];
                if (piece is KingB or DragonB or SilverB) b++;
                if (piece is KingW or DragonW or GoldW or
                    ShilverW or ProShilverW or ProKnightW or
                    ProLanceW or ProPawnW) w++;

                while (p.IsOnBoard)
                {
                    piece = Pieces[p];
                    if (piece.IsEmpty()) p = p.UpLeft();
                    else if (piece is BishopB or HorseB) { b++; break; }
                    else if (piece is BishopW or HorseW) { w++; break; }
                    else break;
                }
            }

            //桂馬
            if (position.JumpDownLeft().IsOnBoard && Pieces[position.JumpDownLeft()] == KnightB) b++;
            if (position.JumpDownRight().IsOnBoard && Pieces[position.JumpDownRight()] == KnightB) b++;
            if (position.JumpUpRight().IsOnBoard && Pieces[position.JumpUpRight()] == KnightW) w++;
            if (position.JumpUpRight().IsOnBoard && Pieces[position.JumpUpRight()] == KnightW) w++;

            return (b, w);
        }
    }
}