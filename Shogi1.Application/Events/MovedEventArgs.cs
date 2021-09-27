using Shogi1.Application.Moves;
using Shogi1.Application.Plays;
using System.Collections.Generic;

namespace Shogi1.Application.Events
{
    /// <summary>
    /// 一手指された
    /// </summary>
    public class MovedEventArgs : ShogiEventArgs
    {
        /// <summary>
        /// 指し手
        /// </summary>
        public MoveBase Move { get; }

        public MovedEventArgs(Color color, Piece[] blackBoard, Piece[] whiteBoard,
                              List<Piece> blackHands, List<Piece> whiteHands, MoveBase move)
            : base(color, blackBoard, whiteBoard, blackHands, whiteHands) => Move = move;
    }
}