using System;
using System.Threading;

namespace Shogi1.Domain.Model
{
    public static class RandomProvider
    {
        private static int seed_ = Environment.TickCount;

        private static readonly ThreadLocal<Random> randomWrapper_ = new(() => new(Interlocked.Increment(ref seed_)));

        public static int Next(int count) => randomWrapper_.Value!.Next(count);
        public static double NextDouble() => randomWrapper_.Value!.NextDouble();
    }
}