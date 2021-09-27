using System;
using static Shogi1.Application.Plays.Piece;

namespace Shogi1.Application.Plays
{
    /// <summary>
    /// 駒
    /// </summary>
    public enum Piece
    {
        None,
        王将,
        玉将,
        飛車,
        龍王,
        角行,
        龍馬,
        金将,
        銀将,
        成銀,
        桂馬,
        成桂,
        香車,
        成香,
        歩兵,
        と金,
    }

    public static class PieceExtension
    {
        /// <summary>
        /// 一文字で表示する
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public static string ToOneLetter(this Piece piece) => piece switch
        {
            None => "・",
            王将 => "王",
            玉将 => "玉",
            飛車 => "飛",
            龍王 => "龍",
            角行 => "角",
            龍馬 => "馬",
            金将 => "金",
            銀将 => "銀",
            成銀 => "全",
            桂馬 => "桂",
            成桂 => "圭",
            香車 => "香",
            成香 => "杏",
            歩兵 => "歩",
            と金 => "と",
            _ => throw new NotImplementedException(),
        };
    }
}