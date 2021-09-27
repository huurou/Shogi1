using Shogi1.Application.Moves;
using System;

namespace Shogi1.Application.Events
{
    /// <summary>
    /// 探索中の最良手が変化した
    /// </summary>
    public class BestMoveChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 現在の最良手
        /// </summary>
        public MoveBase Move { get; }

        public BestMoveChangedEventArgs(MoveBase move) => Move = move;
    }
}