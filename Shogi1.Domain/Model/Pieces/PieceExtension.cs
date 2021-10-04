using Shogi1.Domain.Model.Boards;
using System;
using System.Collections.Generic;
using System.Linq;
using static Shogi1.Domain.Consts;
using static Shogi1.Domain.Model.Pieces.Piece;

namespace Shogi1.Domain.Model.Pieces
{
    internal static class PieceExtension
    {
        internal static bool IsEmpty(this Piece piece) => piece == None;

        internal static bool IsPiece(this Piece piece) => piece != None;

        internal static bool GetTeban(this Piece piece)
        {
            return piece switch
            {
                KingB or RookB or DragonB or
                BishopB or HorseB or GoldB or
                SilverB or ProShilverB or KnightB or
                ProKnightB or LanceB or ProLanceB or
                PawnB or ProPownB => BLACK,

                KingW or RookW or DragonW or
                BishopW or HorseW or GoldW or
                ShilverW or ProShilverW or KnightW or
                ProKnightW or LanceW or ProLanceW or
                PawnW or ProPawnW => WHITE,

                None => throw new InvalidOperationException(),
                _ => throw new NotSupportedException(),
            };
        }

        internal static Piece ReverseTeban(this Piece piece) => piece switch
        {
            KingB => KingW,
            RookB => RookW,
            DragonB => DragonW,
            BishopB => BishopW,
            HorseB => HorseW,
            GoldB => GoldW,
            SilverB => ShilverW,
            ProShilverB => ProShilverW,
            KnightB => KnightW,
            ProKnightB => ProKnightW,
            LanceB => LanceW,
            ProLanceB => ProLanceW,
            PawnB => PawnW,
            ProPownB => ProPawnW,
            KingW => KingB,
            RookW => RookB,
            DragonW => DragonB,
            BishopW => BishopB,
            HorseW => HorseB,
            GoldW => GoldB,
            ShilverW => SilverB,
            ProShilverW => ProShilverB,
            KnightW => KnightB,
            ProKnightW => ProKnightB,
            LanceW => LanceB,
            ProLanceW => ProLanceB,
            PawnW => PawnB,
            ProPawnW => ProPownB,
            None => throw new InvalidOperationException(),
            _ => throw new NotSupportedException(),
        };

        internal static bool IsPromotable(this Piece piece)
        {
            return piece switch
            {
                RookB or BishopB or SilverB or
                KnightB or LanceB or PawnB or
                RookW or BishopW or ShilverW or
                KnightW or LanceW or PawnW => true,

                KingB or DragonB or HorseB or
                GoldB or ProShilverB or ProKnightB or
                ProLanceB or ProPownB or KingW or
                DragonW or HorseW or GoldW or
                ProShilverW or ProKnightW or ProLanceW or
                ProPawnW => false,

                None => throw new InvalidOperationException(),
                _ => throw new NotSupportedException(),
            };
        }

        internal static bool IsPromoted(this Piece piece) => piece switch
        {
            DragonB or HorseB or ProShilverB or
            ProKnightB or ProLanceB or ProPownB or
            DragonW or HorseW or ProShilverW or
            ProKnightW or ProLanceW or ProPawnW => true,

            KingB or RookB or BishopB or
            GoldB or SilverB or KnightB or
            LanceB or PawnB or KingW or
            RookW or BishopW or GoldW or
            ShilverW or KnightW or LanceW or PawnW => false,

            None => throw new InvalidOperationException(),
            _ => throw new NotSupportedException(),
        };

        internal static Piece Promote(this Piece piece) => piece switch
        {
            RookB => DragonB,
            BishopB => HorseB,
            SilverB => ProShilverB,
            KnightB => ProKnightB,
            LanceB => ProLanceB,
            PawnB => ProPownB,
            RookW => DragonW,
            BishopW => HorseW,
            ShilverW => ProShilverW,
            KnightW => ProKnightW,
            LanceW => ProLanceW,
            PawnW => ProPawnW,

            KingB or DragonB or HorseB or
            GoldB or ProShilverB or ProKnightB or
            ProLanceB or ProPownB or KingW or
            DragonW or HorseW or GoldW or
            ProShilverW or ProKnightW or ProLanceW or
            ProPawnW or None => throw new InvalidOperationException(),
            _ => throw new NotSupportedException(),
        };

        internal static Piece Unpromote(this Piece piece) => piece switch
        {
            RookB or DragonB => RookB,
            BishopB or HorseB => BishopB,
            GoldB => GoldB,
            SilverB or ProShilverB => SilverB,
            KnightB or ProKnightB => KnightB,
            LanceB or ProLanceB => LanceB,
            PawnB or ProPownB => PawnB,
            RookW or DragonW => RookW,
            BishopW or HorseW => BishopW,
            GoldW => GoldW,
            ShilverW or ProShilverW => ShilverW,
            KnightW or ProKnightW => KnightW,
            LanceW or ProLanceW => LanceW,
            PawnW or ProPawnW => PawnW,
            KingB or KingW or None => throw new InvalidOperationException(),
            _ => throw new NotSupportedException(),
        };

        internal static List<Position> GetPositions(this Piece piece, Position position)
        {
            switch (piece)
            {
                case KingB:
                    return new Position[]
                    {
                        position.Up(),
                        position.UpRight(),
                        position.Right(),
                        position.DownRight(),
                        position.Down(),
                        position.DownLeft(),
                        position.Left(),
                        position.UpLeft(),
                    }.Where(x => x.IsOnBoard).ToList();

                case RookB:
                    return UpToEnd(position)
                        .Concat(RightToEnd(position))
                        .Concat(DownToEnd(position))
                        .Concat(LeftToEnd(position)).ToList();

                case DragonB:
                    return new Position[]
                    {
                        position.UpRight(),
                        position.DownRight(),
                        position.DownLeft(),
                        position.UpLeft(),
                    }.Where(x => x.IsOnBoard)
                    .Concat(UpToEnd(position))
                    .Concat(RightToEnd(position))
                    .Concat(DownToEnd(position))
                    .Concat(LeftToEnd(position)).ToList();

                case BishopB:
                    return UpRightToEnd(position)
                        .Concat(DownRightToEnd(position))
                        .Concat(DownLeftToEnd(position))
                        .Concat(UpLeftToEnd(position)).ToList();

                case HorseB:
                    return new Position[]
                    {
                        position.Up(),
                        position.Right(),
                        position.Down(),
                        position.Left(),
                    }.Where(x => x.IsOnBoard)
                    .Concat(UpRightToEnd(position))
                    .Concat(DownRightToEnd(position))
                    .Concat(DownLeftToEnd(position))
                    .Concat(UpLeftToEnd(position)).ToList();

                case GoldB:
                    return new Position[]
                    {
                        position.Up(),
                        position.UpRight(),
                        position.Right(),
                        position.Down(),
                        position.Left(),
                        position.UpLeft(),
                    }.Where(x => x.IsOnBoard).ToList();

                case SilverB:
                    return new Position[]
                    {
                        position.Up(),
                        position.UpRight(),
                        position.DownRight(),
                        position.DownLeft(),
                        position.UpLeft(),
                    }.Where(x => x.IsOnBoard).ToList();

                case ProShilverB:
                    return new Position[]
                    {
                        position.Up(),
                        position.UpRight(),
                        position.Right(),
                        position.Down(),
                        position.Left(),
                        position.UpLeft(),
                    }.Where(x => x.IsOnBoard).ToList();

                case KnightB:
                    return new Position[]
                    {
                        position.JumpUpLeft(),
                        position.JumpUpRight(),
                    }.Where(x => x.IsOnBoard).ToList();

                case ProKnightB:
                    return new Position[]
                    {
                        position.Up(),
                        position.UpRight(),
                        position.Right(),
                        position.Down(),
                        position.Left(),
                        position.UpLeft(),
                    }.Where(x => x.IsOnBoard).ToList();

                case LanceB:
                    return UpToEnd(position).ToList();

                case ProLanceB:
                    return new Position[]
                    {
                        position.Up(),
                        position.UpRight(),
                        position.Right(),
                        position.Down(),
                        position.Left(),
                        position.UpLeft(),
                    }.Where(x => x.IsOnBoard).ToList();

                case PawnB:
                    return new Position[]
                    {
                        position.Up(),
                    }.Where(x => x.IsOnBoard).ToList();

                case ProPownB:
                    return new Position[]
                    {
                        position.Up(),
                        position.UpRight(),
                        position.Right(),
                        position.Down(),
                        position.Left(),
                        position.UpLeft(),
                    }.Where(x => x.IsOnBoard).ToList();

                case KingW:
                    return new Position[]
                    {
                        position.Up(),
                        position.UpRight(),
                        position.Right(),
                        position.DownRight(),
                        position.Down(),
                        position.DownLeft(),
                        position.Left(),
                        position.UpLeft(),
                    }.Where(x => x.IsOnBoard).ToList();

                case RookW:
                    return UpToEnd(position)
                        .Concat(RightToEnd(position))
                        .Concat(DownToEnd(position))
                        .Concat(LeftToEnd(position)).ToList();

                case DragonW:
                    return new Position[]
                    {
                        position.UpRight(),
                        position.DownRight(),
                        position.DownLeft(),
                        position.UpLeft(),
                    }.Where(x => x.IsOnBoard)
                    .Concat(UpToEnd(position))
                    .Concat(RightToEnd(position))
                    .Concat(DownToEnd(position))
                    .Concat(LeftToEnd(position)).ToList();

                case BishopW:
                    return UpRightToEnd(position)
                        .Concat(DownRightToEnd(position))
                        .Concat(DownLeftToEnd(position))
                        .Concat(UpLeftToEnd(position)).ToList();

                case HorseW:
                    return new Position[]
                    {
                        position.Up(),
                        position.Right(),
                        position.Down(),
                        position.Left(),
                    }.Where(x => x.IsOnBoard)
                    .Concat(UpRightToEnd(position))
                    .Concat(DownRightToEnd(position))
                    .Concat(DownLeftToEnd(position))
                    .Concat(UpLeftToEnd(position)).ToList();

                case GoldW:
                    return new Position[]
                    {
                        position.Up(),
                        position.Right(),
                        position.DownRight(),
                        position.Down(),
                        position.DownLeft(),
                        position.Left(),
                    }.Where(x => x.IsOnBoard).ToList();

                case ShilverW:
                    return new Position[]
                    {
                        position.UpRight(),
                        position.DownRight(),
                        position.Down(),
                        position.DownLeft(),
                        position.UpLeft(),
                    }.Where(x => x.IsOnBoard).ToList();

                case ProShilverW:
                    return new Position[]
                    {
                        position.Up(),
                        position.Right(),
                        position.DownRight(),
                        position.Down(),
                        position.DownLeft(),
                        position.Left(),
                    }.Where(x => x.IsOnBoard).ToList();

                case KnightW:
                    return new Position[]
                    {
                        position.JumpDownLeft(),
                        position.JumpDownRight(),
                    }.Where(x => x.IsOnBoard).ToList();

                case ProKnightW:
                    return new Position[]
                    {
                        position.Up(),
                        position.Right(),
                        position.DownRight(),
                        position.Down(),
                        position.DownLeft(),
                        position.Left(),
                    }.Where(x => x.IsOnBoard).ToList();

                case LanceW:
                    return DownToEnd(position).ToList();

                case ProLanceW:
                    return new Position[]
                    {
                        position.Up(),
                        position.Right(),
                        position.DownRight(),
                        position.Down(),
                        position.DownLeft(),
                        position.Left(),
                    }.Where(x => x.IsOnBoard).ToList();

                case PawnW:
                    return new Position[]
                    {
                        position.Down(),
                    }.Where(x => x.IsOnBoard).ToList();

                case ProPawnW:
                    return new Position[]
                    {
                        position.Up(),
                        position.Right(),
                        position.DownRight(),
                        position.Down(),
                        position.DownLeft(),
                        position.Left(),
                    }.Where(x => x.IsOnBoard).ToList();

                case None:
                    throw new InvalidOperationException();
                default: break;
            }
            throw new NotSupportedException();

            IEnumerable<Position> UpToEnd(Position position)
            {
                while ((position = position.Up()).IsOnBoard) yield return position;
            }

            IEnumerable<Position> UpRightToEnd(Position position)
            {
                while ((position = position.UpRight()).IsOnBoard) yield return position;
            }

            IEnumerable<Position> RightToEnd(Position position)
            {
                while ((position = position.Right()).IsOnBoard) yield return position;
            }

            IEnumerable<Position> DownRightToEnd(Position position)
            {
                while ((position = position.DownRight()).IsOnBoard) yield return position;
            }

            IEnumerable<Position> DownToEnd(Position position)
            {
                while ((position = position.Down()).IsOnBoard) yield return position;
            }

            IEnumerable<Position> DownLeftToEnd(Position position)
            {
                while ((position = position.DownLeft()).IsOnBoard) yield return position;
            }

            IEnumerable<Position> LeftToEnd(Position position)
            {
                while ((position = position.Left()).IsOnBoard) yield return position;
            }

            IEnumerable<Position> UpLeftToEnd(Position position)
            {
                while ((position = position.UpLeft()).IsOnBoard) yield return position;
            }
        }
    }
}