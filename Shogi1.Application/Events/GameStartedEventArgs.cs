using Shogi1.Application.Plays;
using System.Collections.Generic;

namespace Shogi1.Application.Events
{
    /// <summary>
    /// 対局開始
    /// </summary>
    public class GameStartedEventArgs : ShogiEventArgs
    {
        /// <summary>
        /// 対局者名(先手)
        /// </summary>
        public string Black { get; }

        /// <summary>
        /// 対局者名(後手)
        /// </summary>
        public string White { get; }

        public GameStartedEventArgs(Color color, Piece[] blackBoard, Piece[] whiteBoard,
                                    List<Piece> blackHands, List<Piece> whiteHands, string black, string white)
            : base(color, blackBoard, whiteBoard, blackHands, whiteHands)
        {
            Black = black;
            White = white;
        }
    }
}