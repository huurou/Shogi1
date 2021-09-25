using Shogi1.Domain.Model.Boards;
using System;
using System.Collections.Generic;
using System.Linq;
using static Shogi1.Domain.Consts;
using static Shogi1.Domain.Model.Pieces.Piece;

namespace Shogi1.Domain.Model.Pieces
{
    public static class PieceExtension
    {
        public static bool IsEmpty(this Piece piece) => piece == 空;

        public static bool IsPiece(this Piece piece) => piece != 空;

        public static bool GetTeban(this Piece piece)
        {
            return piece switch
            {
                王B or 飛B or 龍王B or
                角B or 龍馬B or 金B or
                銀B or 成銀B or 桂B or
                成桂B or 香B or 成香B or
                歩B or と金B => BLACK,

                王W or 飛W or 龍王W or
                角W or 龍馬W or 金W or
                銀W or 成銀W or 桂W or
                成桂W or 香W or 成香W or
                歩W or と金W => WHITE,

                空 => throw new InvalidOperationException(),
                _ => throw new NotSupportedException(),
            };
        }

        public static Piece ReverseTeban(this Piece piece) => piece switch
        {
            王B => 王W,
            飛B => 飛W,
            龍王B => 龍王W,
            角B => 角W,
            龍馬B => 龍馬W,
            金B => 金W,
            銀B => 銀W,
            成銀B => 成銀W,
            桂B => 桂W,
            成桂B => 成桂W,
            香B => 香W,
            成香B => 成香W,
            歩B => 歩W,
            と金B => と金W,
            王W => 王B,
            飛W => 飛B,
            龍王W => 龍王B,
            角W => 角B,
            龍馬W => 龍馬B,
            金W => 金B,
            銀W => 銀B,
            成銀W => 成銀B,
            桂W => 桂B,
            成桂W => 成桂B,
            香W => 香B,
            成香W => 成香B,
            歩W => 歩B,
            と金W => と金B,
            空 => throw new InvalidOperationException(),
            _ => throw new NotSupportedException(),
        };

        public static bool IsPromotable(this Piece piece)
        {
            return piece switch
            {
                飛B or 角B or 銀B or
                桂B or 香B or 歩B or
                飛W or 角W or 銀W or
                桂W or 香W or 歩W => true,

                王B or 龍王B or 龍馬B or
                金B or 成銀B or 成桂B or
                成香B or と金B or 王W or
                龍王W or 龍馬W or 金W or
                成銀W or 成桂W or 成香W or
                と金W => false,

                空 => throw new InvalidOperationException(),
                _ => throw new NotSupportedException(),
            };
        }

        public static bool IsPromoted(this Piece piece) => piece switch
        {
            龍王B or 龍馬B or 成銀B or
            成桂B or 成香B or と金B or
            龍王W or 龍馬W or 成銀W or
            成桂W or 成香W or と金W => true,

            王B or 飛B or 角B or
            金B or 銀B or 桂B or
            香B or 歩B or 王W or
            飛W or 角W or 金W or
            銀W or 桂W or 香W or 歩W => false,

            空 => throw new InvalidOperationException(),
            _ => throw new NotSupportedException(),
        };

        public static Piece Promote(this Piece piece) => piece switch
        {
            飛B => 龍王B,
            角B => 龍馬B,
            銀B => 成銀B,
            桂B => 成桂B,
            香B => 成香B,
            歩B => と金B,
            飛W => 龍王W,
            角W => 龍馬W,
            銀W => 成銀W,
            桂W => 成桂W,
            香W => 成香W,
            歩W => と金W,

            王B or 龍王B or 龍馬B or
            金B or 成銀B or 成桂B or
            成香B or と金B or 王W or
            龍王W or 龍馬W or 金W or
            成銀W or 成桂W or 成香W or
            と金W or 空 => throw new InvalidOperationException(),
            _ => throw new NotSupportedException(),
        };

        public static Piece Unpromote(this Piece piece) => piece switch
        {
            飛B or 龍王B => 飛B,
            角B or 龍馬B => 角B,
            金B => 金B,
            銀B or 成銀B => 銀B,
            桂B or 成桂B => 桂B,
            香B or 成香B => 香B,
            歩B or と金B => 歩B,
            飛W or 龍王W => 飛W,
            角W or 龍馬W => 角W,
            金W => 金W,
            銀W or 成銀W => 銀W,
            桂W or 成桂W => 桂W,
            香W or 成香W => 香W,
            歩W or と金W => 歩W,
            王B or 王W or 空 => throw new InvalidOperationException(),
            _ => throw new NotSupportedException(),
        };

        public static List<Position> GetPositions(this Piece piece, Position position)
        {
            switch (piece)
            {
                case 王B:
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

                case 飛B:
                    return UpToEnd(position)
                        .Concat(RightToEnd(position))
                        .Concat(DownToEnd(position))
                        .Concat(LeftToEnd(position)).ToList();

                case 龍王B:
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

                case 角B:
                    return UpRightToEnd(position)
                        .Concat(DownRightToEnd(position))
                        .Concat(DownLeftToEnd(position))
                        .Concat(UpLeftToEnd(position)).ToList();

                case 龍馬B:
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

                case 金B:
                    return new Position[]
                    {
                        position.Up(),
                        position.UpRight(),
                        position.Right(),
                        position.Down(),
                        position.Left(),
                        position.UpLeft(),
                    }.Where(x => x.IsOnBoard).ToList();

                case 銀B:
                    return new Position[]
                    {
                        position.Up(),
                        position.UpRight(),
                        position.DownRight(),
                        position.DownLeft(),
                        position.UpLeft(),
                    }.Where(x => x.IsOnBoard).ToList();

                case 成銀B:
                    return new Position[]
                    {
                        position.Up(),
                        position.UpRight(),
                        position.Right(),
                        position.Down(),
                        position.Left(),
                        position.UpLeft(),
                    }.Where(x => x.IsOnBoard).ToList();

                case 桂B:
                    return new Position[]
                    {
                        position.JumpUpLeft(),
                        position.JumpUpRight(),
                    }.Where(x => x.IsOnBoard).ToList();

                case 成桂B:
                    return new Position[]
                    {
                        position.Up(),
                        position.UpRight(),
                        position.Right(),
                        position.Down(),
                        position.Left(),
                        position.UpLeft(),
                    }.Where(x => x.IsOnBoard).ToList();

                case 香B:
                    return UpToEnd(position).ToList();

                case 成香B:
                    return new Position[]
                    {
                        position.Up(),
                        position.UpRight(),
                        position.Right(),
                        position.Down(),
                        position.Left(),
                        position.UpLeft(),
                    }.Where(x => x.IsOnBoard).ToList();

                case 歩B:
                    return new Position[]
                    {
                        position.Up(),
                    }.Where(x => x.IsOnBoard).ToList();

                case と金B:
                    return new Position[]
                    {
                        position.Up(),
                        position.UpRight(),
                        position.Right(),
                        position.Down(),
                        position.Left(),
                        position.UpLeft(),
                    }.Where(x => x.IsOnBoard).ToList();

                case 王W:
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

                case 飛W:
                    return UpToEnd(position)
                        .Concat(RightToEnd(position))
                        .Concat(DownToEnd(position))
                        .Concat(LeftToEnd(position)).ToList();

                case 龍王W:
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

                case 角W:
                    return UpRightToEnd(position)
                        .Concat(DownRightToEnd(position))
                        .Concat(DownLeftToEnd(position))
                        .Concat(UpLeftToEnd(position)).ToList();

                case 龍馬W:
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

                case 金W:
                    return new Position[]
                    {
                        position.Up(),
                        position.Right(),
                        position.DownRight(),
                        position.Down(),
                        position.DownLeft(),
                        position.Left(),
                    }.Where(x => x.IsOnBoard).ToList();

                case 銀W:
                    return new Position[]
                    {
                        position.UpRight(),
                        position.DownRight(),
                        position.Down(),
                        position.DownLeft(),
                        position.UpLeft(),
                    }.Where(x => x.IsOnBoard).ToList();

                case 成銀W:
                    return new Position[]
                    {
                        position.Up(),
                        position.Right(),
                        position.DownRight(),
                        position.Down(),
                        position.DownLeft(),
                        position.Left(),
                    }.Where(x => x.IsOnBoard).ToList();

                case 桂W:
                    return new Position[]
                    {
                        position.JumpDownLeft(),
                        position.JumpDownRight(),
                    }.Where(x => x.IsOnBoard).ToList();

                case 成桂W:
                    return new Position[]
                    {
                        position.Up(),
                        position.Right(),
                        position.DownRight(),
                        position.Down(),
                        position.DownLeft(),
                        position.Left(),
                    }.Where(x => x.IsOnBoard).ToList();

                case 香W:
                    return DownToEnd(position).ToList();

                case 成香W:
                    return new Position[]
                    {
                        position.Up(),
                        position.Right(),
                        position.DownRight(),
                        position.Down(),
                        position.DownLeft(),
                        position.Left(),
                    }.Where(x => x.IsOnBoard).ToList();

                case 歩W:
                    return new Position[]
                    {
                        position.Down(),
                    }.Where(x => x.IsOnBoard).ToList();

                case と金W:
                    return new Position[]
                    {
                        position.Up(),
                        position.Right(),
                        position.DownRight(),
                        position.Down(),
                        position.DownLeft(),
                        position.Left(),
                    }.Where(x => x.IsOnBoard).ToList();

                case 空:
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