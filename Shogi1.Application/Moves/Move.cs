using Shogi1.Application.Plays;
using static Shogi1.Application.Plays.Piece;

namespace Shogi1.Application.Moves
{
    /// <summary>
    /// 駒の移動
    /// </summary>
    public class Move : MoveBase
    {
        /// <summary>
        /// 駒の移動元
        /// </summary>
        public Position From { get; }

        /// <summary>
        /// 成ったかどうか
        /// </summary>
        public bool IsPromoted { get; }

        /// <summary>
        /// 成った後の駒
        /// </summary>
        public Piece PromotedPiece { get; }

        /// <summary>
        /// 駒を捕獲したかどうか
        /// </summary>
        public bool IsCaptured { get; }

        /// <summary>
        /// 捕獲された駒<para/>
        /// 捕獲された後の状態 例.と金を捕獲→歩
        /// </summary>
        public Piece CapturedPiece { get; }

        public Move(Color color, Position from, Position to, Piece piece, int eval,
                    bool isPromoted = false, Piece promotedPiece = None,
                    bool isCaptured = false, Piece capturedPiece = None)
            : base(color, to, piece, eval)
        {
            From = from;
            IsPromoted = isPromoted;
            PromotedPiece = promotedPiece;
            IsCaptured = isCaptured;
            CapturedPiece = capturedPiece;
        }
    }
}