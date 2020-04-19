using System;
using System.Collections.Generic;
using System.Text;

namespace CardDrawSim
{
    internal class DrawResult
    {
        internal int HitPointResult { get; }
        internal int NumberOfRollingCardsApplied { get; }

        public DrawResult(int hitPointResult, int numberOfRollingCardsApplied)
        {
            HitPointResult = hitPointResult;
            NumberOfRollingCardsApplied = numberOfRollingCardsApplied;
        }
    }
}
