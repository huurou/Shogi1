namespace Shogi1.Domain.Model.Pieces
{
    public enum Piece
    {
        /// <summary>
        /// 空
        /// </summary>
        None,
        /// <summary>
        /// 玉将(先手)
        /// </summary>
        KingB,
        /// <summary>
        /// 飛車(先手)
        /// </summary>
        RookB,
        /// <summary>
        /// 龍王(先手)
        /// </summary>
        DragonB,
        /// <summary>
        /// 角行(先手)
        /// </summary>
        BishopB,
        /// <summary>
        /// 龍馬(先手)
        /// </summary>
        HorseB,
        /// <summary>
        /// 金将(先手)
        /// </summary>
        GoldB,
        /// <summary>
        /// 銀将(先手)
        /// </summary>
        SilverB,
        /// <summary>
        /// 成銀(先手)
        /// </summary>
        ProShilverB,
        /// <summary>
        /// 桂馬(先手)
        /// </summary>
        KnightB,
        /// <summary>
        /// 成桂(先手)
        /// </summary>
        ProKnightB,
        /// <summary>
        /// 香車(先手)
        /// </summary>
        LanceB,
        /// <summary>
        /// 成香(先手)
        /// </summary>
        ProLanceB,
        /// <summary>
        /// 歩兵(先手)
        /// </summary>
        PawnB,
        /// <summary>
        /// と金(先手)
        /// </summary>
        ProPownB,
        /// <summary>
        /// 玉将(後手)
        /// </summary>
        KingW,
        /// <summary>
        /// 飛車(後手)
        /// </summary>
        RookW,
        /// <summary>
        /// 龍王(後手)
        /// </summary>
        DragonW,
        /// <summary>
        /// 角行(後手)
        /// </summary>
        BishopW,
        /// <summary>
        /// 龍馬(後手)
        /// </summary>
        HorseW,
        /// <summary>
        /// 金将(後手)
        /// </summary>
        GoldW,
        /// <summary>
        /// 銀将(後手)
        /// </summary>
        ShilverW,
        /// <summary>
        /// 成銀(後手)
        /// </summary>
        ProShilverW,
        /// <summary>
        /// 桂馬(後手)
        /// </summary>
        KnightW,
        /// <summary>
        /// 成桂(後手)
        /// </summary>
        ProKnightW,
        /// <summary>
        /// 香車(後手)
        /// </summary>
        LanceW,
        /// <summary>
        /// 成香(後手)
        /// </summary>
        ProLanceW,
        /// <summary>
        /// 歩兵(後手)
        /// </summary>
        PawnW,
        /// <summary>
        /// と金(後手)
        /// </summary>
        ProPawnW,

        /// <summary>
        /// for用
        /// </summary>
        PieceLast,
    }
}