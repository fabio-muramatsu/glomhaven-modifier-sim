using System;
using System.Collections.Generic;
using System.Text;

namespace CardDrawSim
{
    internal class Card
    {
        private static int CRIT = 10;
        private static int MISS = -10;

        internal int Modifier { get; }
        internal bool IsRolling { get; }

        internal bool IsCrit => this.Modifier == Card.CRIT;
        internal bool IsMiss => this.Modifier == Card.MISS;
        internal bool IsShuffle => this.IsCrit || this.IsMiss;



        public Card(int modifier, bool isRolling)
        {
            Modifier = modifier;
            IsRolling = isRolling;
        }

        public override string ToString()
        {
            string modifier = this.IsCrit ? "Crit" : (this.IsMiss ? "Miss" : this.Modifier.ToString());
            string rolling = this.IsRolling ? "Rolling" : "Normal";
            return $"{modifier}_{rolling}";
        }
    }
}
