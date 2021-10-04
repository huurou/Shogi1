using Shogi1.Domain.Model.Games;
using Shogi1.Domain.Model.Moves;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static Shogi1.Infrastructure.Games.Piece;

namespace Shogi1.Infrastructure.Games
{
    public class KifGameRepository : IGameRepository
    {
        private readonly string outputDir_ = @"..\..\kifu";

        public void Save(Game game)
        {
            var info = ToInfo(game);
            var filePath = Path.Combine(outputDir_, $"{info.Start:yyyyMMdd_HHmmss}.kif");
            if (!Directory.Exists(outputDir_)) Directory.CreateDirectory(outputDir_);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            File.WriteAllText(filePath, Serialize(info),Encoding.GetEncoding("Shift_JIS"));
        }

        private static GameInfo ToInfo(Game game)
        {
            var moves = game.Board.Stack.Reverse().ToList();
            var moveInfos = new List<MoveInfo>();
            for (var i = 0; i <= moves.Count; i++)
            {
                if (i == moves.Count)
                {
                    moveInfos.Add(new(game.Start < game.End ? MoveType.Resign : MoveType.Suspend,
                                      i + 1, new(0, 0), new(0, 0), None, default, default));
                }
                else
                {
                    var moveBase = moves[i];
                    var to = moveBase.To;
                    if (moveBase is Move move)
                    {
                        var from = move.From;
                        moveInfos.Add(new(move.Promoted ? MoveType.Promote : MoveType.Normal, i + 1, new(from.X, from.Y), new(to.X, to.Y), ToPiece(move.Piece), default, default));
                    }
                    else if (moves[i] is Drop drop)
                    {
                        moveInfos.Add(new(MoveType.Drop, i + 1, new(0, 0), new(to.X, to.Y), ToPiece(drop.Piece), default, default));
                    }
                }
            }
            return new GameInfo(game.Start, game.End, game.Black.ToString() ?? "", game.White.ToString() ?? "", moveInfos);

            static Piece ToPiece(Domain.Model.Pieces.Piece piece) => piece switch
            {
                Domain.Model.Pieces.Piece.KingB or
                Domain.Model.Pieces.Piece.KingW => 玉,
                Domain.Model.Pieces.Piece.RookB or
                Domain.Model.Pieces.Piece.RookW => 飛,
                Domain.Model.Pieces.Piece.DragonB or
                Domain.Model.Pieces.Piece.DragonW => 龍,
                Domain.Model.Pieces.Piece.BishopB or
                Domain.Model.Pieces.Piece.BishopW => 角,
                Domain.Model.Pieces.Piece.HorseB or
                Domain.Model.Pieces.Piece.HorseW => 馬,
                Domain.Model.Pieces.Piece.GoldB or
                Domain.Model.Pieces.Piece.GoldW => 金,
                Domain.Model.Pieces.Piece.SilverB or
                Domain.Model.Pieces.Piece.ShilverW => 銀,
                Domain.Model.Pieces.Piece.ProShilverB or
                Domain.Model.Pieces.Piece.ProShilverW => 成銀,
                Domain.Model.Pieces.Piece.KnightB or
                Domain.Model.Pieces.Piece.KnightW => 桂,
                Domain.Model.Pieces.Piece.ProKnightB or
                Domain.Model.Pieces.Piece.ProKnightW => 成桂,
                Domain.Model.Pieces.Piece.LanceB or
                Domain.Model.Pieces.Piece.LanceW => 香,
                Domain.Model.Pieces.Piece.ProLanceB or
                Domain.Model.Pieces.Piece.ProLanceW => 成香,
                Domain.Model.Pieces.Piece.PawnB or
                Domain.Model.Pieces.Piece.PawnW => 歩,
                Domain.Model.Pieces.Piece.ProPownB or
                Domain.Model.Pieces.Piece.ProPawnW => と,
                Domain.Model.Pieces.Piece.None => throw new InvalidOperationException(),
                _ => throw new NotImplementedException(),
            };
        }

        private static string Serialize(GameInfo info)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"# ----- generated Shogi1 @{DateTime.Now} -----");
            sb.AppendLine($"開始日時：{info.Start:yyyy/MM/dd/(ddd) HH:mm:ss}");
            sb.AppendLine($"終了日時：{info.End:yyyy/MM/dd/(ddd) HH:mm:ss}");
            sb.AppendLine($"先手：{info.Black}");
            sb.AppendLine($"後手：{info.White}");
            for (var i = 0; i < info.MoveInfos.Count; i++)
            {
                sb.AppendLine(ConvMove(info.MoveInfos[i], i == 0 ? null : info.MoveInfos[i - 1].To));
            }
            return sb.ToString();

            static string ConvMove(MoveInfo move, Position? PreTo)
            {
                var sb = new StringBuilder();
                sb.Append(move.Turns);
                sb.Append(' ');
                if (move.Type is MoveType.Normal or MoveType.Promote &&
                    PreTo is not null &&
                    PreTo == move.To) sb.Append('同');
                else if (move.Type is MoveType.Normal or MoveType.Promote or MoveType.Drop) sb.Append(move.To.ToString());
                if (move.Type is MoveType.Normal or MoveType.Promote or MoveType.Drop) sb.Append(move.Piece);
                if (move.Type is MoveType.Promote) sb.Append('成');
                if (move.Type is MoveType.Drop) sb.Append('打');
                if (move.Type is MoveType.Normal or MoveType.Promote) sb.Append($"({move.From.X}{move.From.Y})");
                if (move.Type is MoveType.Suspend) sb.Append("中断");
                if (move.Type is MoveType.Resign) sb.Append("投了");
                return sb.ToString();
            }
        }
    }
}