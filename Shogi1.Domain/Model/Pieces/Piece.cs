using System;

namespace Shogi1.Domain.Model.Pieces
{
    public abstract class Piece
    {
        public virtual bool Teban { get; private set; }

        protected Piece(bool teban) => Teban = teban;

        /// <summary>
        /// 駒の先後を入れ替える
        /// </summary>
        internal void Reverse() => Teban = !Teban;
    }

    public abstract class Promotable : Piece
    {
        protected Promotable(bool teban)
            : base(teban) { }

        public bool Promoted { get; private set; }

        internal void Promote()
        {
            if (Promoted) throw new InvalidOperationException();
            Promoted = true;
        }

        internal void Unpromote()
        {
            if (!Promoted) throw new InvalidOperationException();
            Promoted = false;
        }
    }

    public abstract class Unpromotable : Piece
    {
        protected Unpromotable(bool teban)
            : base(teban) { }
    }

    /// <summary>
    /// 王将 / 玉将
    /// </summary>
    public class Gyoku : Unpromotable
    {
        public Gyoku(bool teban)
            : base(teban) { }
    }

    /// <summary>
    /// 飛車 / 龍王
    /// </summary>
    public class Hisya : Promotable
    {
        public Hisya(bool teban)
            : base(teban) { }
    }

    /// <summary>
    /// 角行 / 龍馬
    /// </summary>
    public class Kaku : Promotable
    {
        public Kaku(bool teban)
            : base(teban) { }
    }

    /// <summary>
    /// 金将
    /// </summary>
    public class Kin : Unpromotable
    {
        public Kin(bool teban)
            : base(teban) { }
    }

    /// <summary>
    /// 銀将 / 成銀
    /// </summary>
    public class Gin : Promotable
    {
        public Gin(bool teban)
            : base(teban) { }
    }

    /// <summary>
    /// 桂馬 / 成桂
    /// </summary>
    public class Kei : Promotable
    {
        public Kei(bool teban)
            : base(teban) { }
    }

    /// <summary>
    /// 香車 / 成香
    /// </summary>
    public class Kyou : Promotable
    {
        public Kyou(bool teban)
            : base(teban) { }
    }

    /// <summary>
    /// 歩兵 / と金
    /// </summary>
    public class Hu : Promotable
    {
        public Hu(bool teban)
            : base(teban) { }
    }
}