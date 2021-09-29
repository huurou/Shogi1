using Shogi1.Application.Events;
using Shogi1.Application.Moves;
using Shogi1.Application.Plays;
using Shogi1.Domain.Model.AIs;
using Shogi1.Domain.Model.Games;
using Shogi1.Domain.Model.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using static Shogi1.Application.Games.Result;
using static Shogi1.Application.Plays.Color;
using static Shogi1.Application.Plays.Piece;
using static Shogi1.Domain.Model.Pieces.Piece;
using ModelBoard = Shogi1.Domain.Model.Boards.Board;
using ModelMoveBase = Shogi1.Domain.Model.Moves.MoveBase;
using ModelPiece = Shogi1.Domain.Model.Pieces.Piece;
using ModelResult = Shogi1.Domain.Model.Games.Result;
using Piece = Shogi1.Application.Plays.Piece;

namespace Shogi1.Application
{
    public class ShogiApplicationService
    {
        public event EventHandler<GameStartedEventArgs>? GameStarted;
        public event EventHandler<MovedEventArgs>? Moved;
        //public event EventHandler<BestMoveChangedEventArgs>? BestMoveChanged;
        public event EventHandler<GameFinishedEventArgs>? GameFinished;

        // とりあえずここに書く
        // UIから選択できるようになるといいなあ
        private readonly IAI black_ = new PieceAndEffectValueAlphaBetaAI(5);

        private readonly IAI white_ = new PieceValueAlphaBetaAI(5);

        private readonly Game game_;

        public ShogiApplicationService()
        {
            game_ = new(black_, white_);

            game_.GameStart += (s, e) => GameStarted?.Invoke(s, ToGameStartedEventArgs(e));
            game_.Moved += (s, e) => Moved?.Invoke(s, ToMovedEventArgs(e.board, e.move, e.eval));
            game_.GameEnd += (s, e) => GameFinished?.Invoke(s,
                new(e == ModelResult.Win ? BlackWin : e == ModelResult.Lose ? WhiteWin : Draw));
        }

        public void Run() => game_.Run();

        private GameStartedEventArgs ToGameStartedEventArgs(ModelBoard board)
        {
            var (c, bb, wb, bh, wh) = ToShogiEventArgsParams(board);
            return new GameStartedEventArgs(c, bb, wb, bh, wh, black_.ToString() ?? "", white_.ToString() ?? "");
        }

        private static MovedEventArgs ToMovedEventArgs(ModelBoard board,
                                                       ModelMoveBase moveBase, int eval)
        {
            var (c, bb, wb, bh, wh) = ToShogiEventArgsParams(board);
            var m = ToMoveBase(moveBase, eval);
            return new(c, bb, wb, bh, wh, m);
        }

        private static (Color c, Piece[] bb, Piece[] wb, List<Piece> bh, List<Piece> wh)
            ToShogiEventArgsParams(ModelBoard board)
        {
            var c = board.Teban ? Black : White;
            var bb = new Piece[81];
            var wb = new Piece[81];
            var pieces = board.Pieces;
            for (var y = 1; y <= 9; y++)
            {
                for (var x = 1; x <= 9; x++)
                {
                    var pos = new Domain.Model.Boards.Position(x, y);
                    var piece = pieces[pos];
                    if (piece.IsEmpty()) continue;
                    if (piece.GetTeban()) bb[pos] = ToPiece(piece);
                    else wb[pos] = ToPiece(piece);
                }
            }
            var bh = board.HandsBlack.Select(ToPiece).ToList();
            var wh = board.HandsWhite.Select(ToPiece).ToList();
            return (c, bb, wb, bh, wh);
        }

        private static Piece ToPiece(ModelPiece piece) => piece switch
        {
            空 => None,
            王B => 玉将,
            飛B => 飛車,
            龍王B => 龍王,
            角B => 角行,
            龍馬B => 龍馬,
            金B => 金将,
            銀B => 銀将,
            成銀B => 成銀,
            桂B => 桂馬,
            成桂B => 成桂,
            香B => 香車,
            成香B => 成香,
            歩B => 歩兵,
            と金B => と金,
            王W => 王将,
            飛W => 飛車,
            龍王W => 龍王,
            角W => 角行,
            龍馬W => 龍馬,
            金W => 金将,
            銀W => 銀将,
            成銀W => 成銀,
            桂W => 桂馬,
            成桂W => 成桂,
            香W => 香車,
            成香W => 成香,
            歩W => 歩兵,
            と金W => と金,
            _ => throw new NotImplementedException(),
        };

        private static MoveBase ToMoveBase(ModelMoveBase moveBase, int eval)
        {
            var c = moveBase.Teban ? Black : White;
            var to = new Position(moveBase.To);
            var piece = ToPiece(moveBase.Piece);
            if (moveBase is Domain.Model.Moves.Move move)
            {
                var from = new Position(move.From);
                var isPromoted = move.Promoted;
                var promotePiece = isPromoted ? ToPiece(move.Piece.Promote()) : None;
                var isCaptured = move.Captured;
                var capturedPiece = isCaptured
                    ? ToPiece(move.PieceCaptured.Unpromote()) : None;
                return new Move(c, from, to, piece, eval, isPromoted, promotePiece, isCaptured, capturedPiece);
            }
            else
            {
                return new Drop(c, to, piece, eval);
            }
        }
    }
}