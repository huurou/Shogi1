using Shogi1.Application.Games;
using System;

namespace Shogi1.Application.Events
{
    /// <summary>
    /// 対局終了
    /// </summary>
    public class GameFinishedEventArgs : EventArgs
    {
        public Result Result { get; }

        public GameFinishedEventArgs(Result result) => Result = result;
    }
}