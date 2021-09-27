using System;
using System.Threading;

namespace Shogi1.Domain.Model
{
    internal static class RandomProvider
    {
        private static int seed_ = Environment.TickCount;

        private static readonly ThreadLocal<Random> randomWrapper_ = new(() => new(Interlocked.Increment(ref seed_)));

        internal static int Next(int count) => randomWrapper_.Value!.Next(count);
        internal static double NextDouble() => randomWrapper_.Value!.NextDouble();
    }
}