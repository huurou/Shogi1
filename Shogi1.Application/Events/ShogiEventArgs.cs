using Shogi1.Application.Plays;
using System;
using System.Collections.Generic;

namespace Shogi1.Application.Events
{
    /// <summary>
    /// GameStartedEventArgs, MovedEventArgaの基底クラス
    /// </summary>
    public abstract class ShogiEventArgs : EventArgs
    {
        /// <summary>
        /// 手番
        /// </summary>
        public Color Color { get; }

        /// <summary>
        /// 盤上の先手の駒
        /// </summary>
        public Piece[] BlackBoard { get; }

        /// <summary>
        /// 盤上の後手の駒
        /// </summary>
        public Piece[] WhiteBoard { get; }

        /// <summary>
        /// 先手の持ち駒
        /// </summary>
        public List<Piece> BlackHands { get; }

        /// <summary>
        /// 後手の持ち駒
        /// </summary>
        public List<Piece> WhiteHands { get; }

        public ShogiEventArgs(Color color, Piece[] blackBoard, Piece[] whiteBoard,
                              List<Piece> blackHands, List<Piece> whiteHands)
        {
            Color = color;
            BlackBoard = blackBoard;
            WhiteBoard = whiteBoard;
            BlackHands = blackHands;
            WhiteHands = whiteHands;
        }
    }
}