using System;
using System.Collections.Generic;
using System.Linq;
using static Shogi1.Domain.Consts;

namespace Shogi1.Domain.Model.Boards
{
    public class Move
    {
        /// <summary>
        /// どこから 負:持ち駒
        /// </summary>
        public int From { get; }
        /// <summary>
        /// どこへ
        /// </summary>
        public int To { get; }
        /// <summary>
        /// 何の駒を
        /// </summary>
        public int Piece { get; }
        /// <summary>
        /// 成ったかどうか
        /// </summary>
        public bool Promoted { get; }
        /// <summary>
        /// 何の駒を取ったか 0:何も取ってない
        /// 取る前の駒について
        /// </summary>
        public int Captured { get; }

        public Move(int from, int to, int piece, bool promoted = false, int captured = 0)
        {
            From = from;
            To = to;
            Piece = piece;
            Promoted = promoted;
            Captured = captured;
        }
    }

    public class Board
    {
        #region consts

        // SQ11: 1一 SQ12:1二 SQ21: 2一 SQ22:2二

        public const int SQ11 = 20;
        public const int SQ12 = 31;
        public const int SQ13 = 42;
        public const int SQ14 = 53;
        public const int SQ15 = 64;
        public const int SQ16 = 75;
        public const int SQ17 = 86;
        public const int SQ18 = 97;
        public const int SQ19 = 108;
        public const int SQ21 = 19;
        public const int SQ22 = 30;
        public const int SQ23 = 41;
        public const int SQ24 = 52;
        public const int SQ25 = 63;
        public const int SQ26 = 74;
        public const int SQ27 = 85;
        public const int SQ28 = 96;
        public const int SQ29 = 107;
        public const int SQ31 = 18;
        public const int SQ32 = 29;
        public const int SQ33 = 40;
        public const int SQ34 = 51;
        public const int SQ35 = 62;
        public const int SQ36 = 73;
        public const int SQ37 = 84;
        public const int SQ38 = 95;
        public const int SQ39 = 106;
        public const int SQ41 = 17;
        public const int SQ42 = 28;
        public const int SQ43 = 39;
        public const int SQ44 = 50;
        public const int SQ45 = 61;
        public const int SQ46 = 72;
        public const int SQ47 = 83;
        public const int SQ48 = 94;
        public const int SQ49 = 105;
        public const int SQ51 = 16;
        public const int SQ52 = 27;
        public const int SQ53 = 38;
        public const int SQ54 = 49;
        public const int SQ55 = 60;
        public const int SQ56 = 71;
        public const int SQ57 = 82;
        public const int SQ58 = 93;
        public const int SQ59 = 104;
        public const int SQ61 = 15;
        public const int SQ62 = 26;
        public const int SQ63 = 37;
        public const int SQ64 = 48;
        public const int SQ65 = 59;
        public const int SQ66 = 70;
        public const int SQ67 = 81;
        public const int SQ68 = 92;
        public const int SQ69 = 103;
        public const int SQ71 = 14;
        public const int SQ72 = 25;
        public const int SQ73 = 36;
        public const int SQ74 = 47;
        public const int SQ75 = 58;
        public const int SQ76 = 69;
        public const int SQ77 = 80;
        public const int SQ78 = 91;
        public const int SQ79 = 102;
        public const int SQ81 = 13;
        public const int SQ82 = 24;
        public const int SQ83 = 35;
        public const int SQ84 = 46;
        public const int SQ85 = 57;
        public const int SQ86 = 68;
        public const int SQ87 = 79;
        public const int SQ88 = 90;
        public const int SQ89 = 101;
        public const int SQ91 = 12;
        public const int SQ92 = 23;
        public const int SQ93 = 34;
        public const int SQ94 = 45;
        public const int SQ95 = 56;
        public const int SQ96 = 67;
        public const int SQ97 = 78;
        public const int SQ98 = 89;
        public const int SQ99 = 100;

        private static readonly int[] sqs_ = new int[]
        {
            SQ91, SQ81, SQ71, SQ61, SQ51, SQ41, SQ31, SQ21, SQ11,
            SQ92, SQ82, SQ72, SQ62, SQ52, SQ42, SQ32, SQ22, SQ12,
            SQ93, SQ83, SQ73, SQ63, SQ53, SQ43, SQ33, SQ23, SQ13,
            SQ94, SQ84, SQ74, SQ64, SQ54, SQ44, SQ34, SQ24, SQ14,
            SQ95, SQ85, SQ75, SQ65, SQ55, SQ45, SQ35, SQ25, SQ15,
            SQ96, SQ86, SQ76, SQ66, SQ56, SQ46, SQ36, SQ26, SQ16,
            SQ97, SQ87, SQ77, SQ67, SQ57, SQ47, SQ37, SQ27, SQ17,
            SQ98, SQ88, SQ78, SQ68, SQ58, SQ48, SQ38, SQ28, SQ18,
            SQ99, SQ89, SQ79, SQ69, SQ59, SQ49, SQ39, SQ29, SQ19,
        };

        private const int HANDS_P = -1;
        private const int HANDS_O = -2;

        #endregion consts

        private readonly int[] board_ = new int[]
        {
            WALL,  WALL,  WALL, WALL, WALL,   WALL, WALL, WALL,  WALL,  WALL,WALL,
            WALL,KYOU_O, KEI_O,GIN_O,KIN_O,GYOKU_O,KIN_O,GIN_O, KEI_O,KYOU_O,WALL,
            WALL, EMPTY,  HI_O,EMPTY,EMPTY,  EMPTY,EMPTY,EMPTY,KAKU_O, EMPTY,WALL,
            WALL,  HU_O,  HU_O, HU_O, HU_O,   HU_O, HU_O, HU_O,  HU_O,  HU_O,WALL,
            WALL, EMPTY, EMPTY,EMPTY,EMPTY,  EMPTY,EMPTY,EMPTY, EMPTY, EMPTY,WALL,
            WALL, EMPTY, EMPTY,EMPTY,EMPTY,  EMPTY,EMPTY,EMPTY, EMPTY, EMPTY,WALL,
            WALL, EMPTY, EMPTY,EMPTY,EMPTY,  EMPTY,EMPTY,EMPTY, EMPTY, EMPTY,WALL,
            WALL,  HU_P,  HU_P, HU_P, HU_P,   HU_P, HU_P, HU_P,  HU_P,  HU_P,WALL,
            WALL, EMPTY,KAKU_P,EMPTY,EMPTY,  EMPTY,EMPTY,EMPTY,  HI_P, EMPTY,WALL,
            WALL,KYOU_P, KEI_P,GIN_P,KIN_P,GYOKU_P,KIN_P,GIN_P, KEI_P,KYOU_P,WALL,
            WALL,  WALL,  WALL, WALL, WALL,   WALL, WALL, WALL,  WALL,  WALL,WALL,
        };

        private readonly List<int> handsPlayer_ = new();
        private readonly List<int> handsOpponent_ = new();
        private readonly Stack<Move> history_ = new();

        public bool Teban { get; private set; }
        public int Turns { get; private set; }

        public void Move(Move move)
        {
            // Move
            if (IsOnBoard(move.From))
            {
                if (move.Piece != board_[move.From]) throw new InvalidOperationException();
                if (move.Captured != EMPTY && move.Captured != board_[move.To]) throw new InvalidOperationException();
                if (move.Promoted && IsPromotableSq(Teban, move.From, move.To)) throw new InvalidOperationException();
                board_[move.From] = EMPTY;
                board_[move.To] = move.Promoted ? Promote(move.Piece) : move.Piece;
                if (move.Captured != EMPTY) (Teban ? handsPlayer_ : handsOpponent_).Add(Captured(move.Captured));
                if (move.Captured != EMPTY) (Teban ? handsPlayer_ : handsOpponent_).Add(move.Captured);
            }
            // Drop
            else if (move.From == HANDS_P)
            {
                if (!Teban) throw new InvalidOperationException();
            }
            else if (move.From == HANDS_O)
            {
                if (Teban) throw new InvalidOperationException();
            }
            else throw new InvalidOperationException();
        }

        private static int Upper(int sq) => sq - 11;

        private static int UpperRight(int sq) => sq - 10;

        private static int Right(int sq) => sq + 1;

        private static int LowerRight(int sq) => sq + 12;

        private static int Lower(int sq) => sq + 11;

        private static int LowerLeft(int sq) => sq + 10;

        private static int Left(int sq) => sq - 1;

        private static int UpperLeft(int sq) => sq - 12;

        private static int JumpUpperLeft(int sq) => sq - 23;

        private static int JumpUpperRight(int sq) => sq - 21;

        private static int JumpLowerLeft(int sq) => sq + 21;

        private static int JumpLowerRight(int sq) => sq + 23;

        private static int ToSQ(int x, int y) => x * -1 + y * 11 + 10;

        /// <summary>
        /// 盤内かどうか
        /// </summary>
        /// <param name="sq"></param>
        /// <returns></returns>
        private static bool IsOnBoard(int sq) => sqs_.Contains(sq);

        /// <summary>
        /// 王手がかかっているかどうか
        /// </summary>
        /// <param name="teban">どちら側の王についてか</param>
        /// <returns></returns>
        private bool IsCheck(bool teban) => IsCheck(teban, teban
                ? Array.IndexOf(board_, GYOKU_P)
                : Array.IndexOf(board_, GYOKU_O));

        private bool IsCheck(bool teban, int sq)
        {
            var kingSq = sq;
            if (teban)
            {
                // 上
                // 玉の一つ上を確認
                var u = Upper(kingSq);
                if (board_[u] is GYOKU_O or HI_O or RYUU_O or UMA_O or KIN_O or
                    GIN_O or NARIGIN_O or NARIKEI_O or KYOU_O or NARIKYOU_O or
                    HU_O or TOKIN_O) return true;
                // 空のマスではなくなるまで上に移動
                while (board_[u] == EMPTY) u = Upper(u);
                // 香、飛、龍があればtrue
                if (board_[u] is KYOU_O or HI_O or RYUU_O) return true;

                // 左上
                var ul = UpperLeft(kingSq);
                if (board_[ul] is GYOKU_O or RYUU_O or KAKU_O or UMA_O or
                    KIN_O or GIN_O or NARIGIN_O or NARIKEI_O or NARIKYOU_O or
                    TOKIN_O) return true;
                while (board_[ul] == EMPTY) ul = UpperLeft(ul);
                if (board_[ul] is KAKU_O or UMA_O) return true;

                // 右上
                var ur = UpperRight(kingSq);
                if (board_[ur] is GYOKU_O or RYUU_O or KAKU_O or UMA_O or
                    KIN_O or GIN_O or NARIGIN_O or NARIKEI_O or NARIKYOU_O or
                    TOKIN_O) return true;
                while (board_[ur] == EMPTY) ur = UpperRight(ur);
                if (board_[ur] is KAKU_O or UMA_O) return true;

                // 左
                var l = Left(kingSq);
                if (board_[l] is GYOKU_O or HI_O or RYUU_O or UMA_O or
                    KIN_O or NARIGIN_O or NARIKEI_O or NARIKYOU_O or
                    TOKIN_O) return true;
                while (board_[l] == EMPTY) l = Left(l);
                if (board_[l] is HI_O or RYUU_O) return true;

                // 右
                var r = Right(kingSq);
                if (board_[r] is GYOKU_O or HI_O or RYUU_O or UMA_O or
                    KIN_O or NARIGIN_O or NARIKEI_O or NARIKYOU_O or
                    TOKIN_O) return true;
                while (board_[r] == EMPTY) r = Right(r);
                if (board_[r] is HI_O or RYUU_O) return true;

                // 左下
                var ll = LowerLeft(kingSq);
                if (board_[ll] is GYOKU_O or RYUU_O or KAKU_O or UMA_O or
                    GIN_O) return true;
                while (board_[ll] == EMPTY) ll = LowerLeft(ll);
                if (board_[ll] is KAKU_O or UMA_O) return true;

                // 右下
                var lr = LowerRight(kingSq);
                if (board_[lr] is GYOKU_O or RYUU_O or KAKU_O or UMA_O or
                    GIN_O) return true;
                while (board_[lr] == EMPTY) lr = LowerRight(lr);
                if (board_[lr] is KAKU_O or UMA_O) return true;

                // 下
                l = Lower(kingSq);
                if (board_[l] is GYOKU_O or HI_O or RYUU_O or UMA_O or
                    KIN_O or NARIGIN_O or NARIKEI_O or NARIKYOU_O or
                    TOKIN_O) return true;
                while (board_[l] == EMPTY) l = Lower(l);
                if (board_[l] is HI_O or RYUU_O) return true;

                // 桂馬左
                var jul = JumpUpperLeft(kingSq);
                if (IsOnBoard(jul) && board_[jul] == KEI_O) return true;

                // 桂馬右
                var jur = JumpUpperRight(kingSq);
                if (IsOnBoard(jur) && board_[jur] == KEI_O) return true;
            }
            else
            {
                // 下
                var l = Lower(kingSq);
                if (board_[l] is GYOKU_P or HI_P or RYUU_P or UMA_P or
                    KIN_P or GIN_P or NARIGIN_P or NARIKEI_P or
                    KYOU_P or NARIKYOU_P or HU_P or TOKIN_P) return true;
                while (board_[l] == EMPTY) l = Lower(l);
                if (board_[l] is HI_P or RYUU_P or KYOU_P) return true;

                // 左下
                var ll = LowerLeft(kingSq);
                if (board_[ll] is GYOKU_P or RYUU_P or KAKU_P or UMA_P or
                    KIN_P or GIN_P or NARIGIN_P or NARIKEI_P or NARIKYOU_P or
                    TOKIN_P) return true;
                while (board_[ll] == EMPTY) ll = LowerLeft(ll);
                if (board_[ll] is KAKU_P or UMA_P) return true;

                // 右下
                var lr = LowerRight(kingSq);
                if (board_[lr] is GYOKU_P or RYUU_P or KAKU_P or UMA_P or
                    KIN_P or GIN_P or NARIGIN_P or NARIKEI_P or NARIKYOU_P or
                    TOKIN_P) return true;
                while (board_[lr] == EMPTY) lr = LowerLeft(lr);
                if (board_[lr] is KAKU_P or UMA_P) return true;

                // 左
                l = Left(kingSq);
                if (board_[l] is GYOKU_P or HI_P or RYUU_P or UMA_P or
                    KIN_P or NARIGIN_P or NARIKEI_P or NARIKYOU_P or
                    TOKIN_P) return true;
                while (board_[l] == EMPTY) l = Left(l);
                if (board_[l] is HI_P or RYUU_P) return true;

                // 右
                var r = Right(kingSq);
                if (board_[r] is GYOKU_P or HI_P or RYUU_P or UMA_P or
                    KIN_P or NARIGIN_P or NARIKEI_P or NARIKYOU_P or
                    TOKIN_P) return true;
                while (board_[r] == EMPTY) r = Right(r);
                if (board_[r] is HI_P or RYUU_P) return true;

                // 左上
                var ul = UpperLeft(kingSq);
                if (board_[ul] is GYOKU_P or RYUU_P or KAKU_P or UMA_P or
                    GIN_P) return true;
                while (board_[ul] == EMPTY) ul = UpperLeft(ul);
                if (board_[ul] is KAKU_P or UMA_P) return true;

                // 右上
                var ur = UpperRight(kingSq);
                if (board_[ur] is GYOKU_P or RYUU_P or KAKU_P or UMA_P or
                    GIN_P) return true;
                while (board_[ur] == EMPTY) ur = UpperRight(ur);
                if (board_[ur] is KAKU_P or UMA_P) return true;

                // 上
                var u = Upper(kingSq);
                if (board_[u] is GYOKU_P or HI_P or RYUU_P or UMA_P or
                    KIN_P or NARIGIN_P or NARIKEI_P or NARIKYOU_P or
                    TOKIN_P) return true;
                while (board_[u] == EMPTY) u = Upper(u);
                if (board_[u] is HI_P or RYUU_P) return true;

                // 桂馬左
                var jll = JumpLowerLeft(kingSq);
                if (IsOnBoard(jll) && board_[jll] == KEI_P) return true;

                // 桂馬右
                var jlr = JumpLowerRight(kingSq);
                if (IsOnBoard(jlr) && board_[jlr] == KEI_P) return true;
            }
            return false;
        }

        private static bool IsPromotableSq(bool teban, int from, int to)
        {
            if (teban)
            {
                var p = new int[]
                {
                    SQ91, SQ81, SQ71, SQ61, SQ51, SQ41, SQ31, SQ21, SQ11,
                    SQ92, SQ82, SQ72, SQ62, SQ52, SQ42, SQ32, SQ22, SQ12,
                    SQ93, SQ83, SQ73, SQ63, SQ53, SQ43, SQ33, SQ23, SQ13,
                };
                return p.Contains(from) || p.Contains(to);
            }
            else
            {
                var p = new int[]
                {
                    SQ97, SQ87, SQ77, SQ67, SQ57, SQ47, SQ37, SQ27, SQ17,
                    SQ98, SQ88, SQ78, SQ68, SQ58, SQ48, SQ38, SQ28, SQ18,
                    SQ99, SQ89, SQ79, SQ69, SQ59, SQ49, SQ39, SQ29, SQ19,
                };
                return p.Contains(from) || p.Contains(to);
            }
        }

        private static int Promote(int piece) => piece switch
        {
            HI_P => RYUU_P,
            KAKU_P => UMA_P,
            GIN_P => NARIGIN_P,
            KEI_P => NARIKEI_P,
            KYOU_P => NARIKYOU_P,
            HU_P => TOKIN_P,
            HI_O => RYUU_O,
            KAKU_O => UMA_O,
            GIN_O => NARIGIN_O,
            KEI_O => NARIKEI_O,
            KYOU_O => NARIKYOU_O,
            HU_O => TOKIN_O,
            _ => throw new InvalidOperationException(),
        };

        private static int Captured(int piece) => piece switch
        {
            RYUU_P => HI_O,
            UMA_P => KAKU_O,
            NARIGIN_P => GIN_O,
            NARIKEI_P => KEI_O,
            NARIKYOU_P => KYOU_O,
            TOKIN_P => HU_O,
            RYUU_O => HI_P,
            UMA_O => KAKU_P,
            NARIGIN_O => GIN_P,
            NARIKEI_O => KEI_P,
            NARIKYOU_O => NARIKYOU_P,
            TOKIN_O => HU_P,
            EMPTY or >= WALL or <= WALL * -1 => throw new InvalidOperationException();
            _ => piece * -1
        };
    }
}