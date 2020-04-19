using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardDrawSim
{
    class DrawSimulator
    {
        private List<Card> CardsToDraw { get; }
        private List<Card> CardsDrawn { get; }
        private int BaseDamage { get; }

        private Random rng = new Random();

        public DrawSimulator(List<Card> deck, int baseDamage)
        {
            CardsToDraw = new List<Card>(deck);
            CardsDrawn = new List<Card>();
            BaseDamage = baseDamage;

            CardsToDraw.Shuffle(rng);
        }

        public (float averageHitPoint, float averageNumberOfRollingApplied) SimulateHouseRules(int numberOfDraws)
        {
            List<DrawResult> drawResults = new List<DrawResult>();

            for (int drawNumber = 0 ; drawNumber < numberOfDraws; ++drawNumber)
            {
                drawResults.Add(DrawModifierCardsWithHouseRules());
            }

            float averageHitPoint = drawResults.Aggregate(0, (agg, c) => agg + c.HitPointResult) / (float)numberOfDraws;
            float averageNumberOfRollingApplied = drawResults.Aggregate(0, (agg, c) => agg + c.NumberOfRollingCardsApplied) / (float)numberOfDraws;

            return (averageHitPoint, averageNumberOfRollingApplied);
        }

        public (float averageHitPoint, float averageNumberOfRollingApplied) SimulateRawRules(int numberOfDraws)
        {
            List<DrawResult> drawResults = new List<DrawResult>();

            for (int drawNumber = 0; drawNumber < numberOfDraws; ++drawNumber)
            {
                drawResults.Add(DrawModifierCardsWithRawRules());
            }

            float averageHitPoint = drawResults.Aggregate(0, (agg, c) => agg + c.HitPointResult) / (float)numberOfDraws;
            float averageNumberOfRollingApplied = drawResults.Aggregate(0, (agg, c) => agg + c.NumberOfRollingCardsApplied) / (float)numberOfDraws;

            return (averageHitPoint, averageNumberOfRollingApplied);
        }

        private DrawResult DrawModifierCardsWithHouseRules()
        {
            List<Card> currentDrawnList = new List<Card>();

            Card firstDrawnCard = this.DrawOneCard();
            Card secondDrawnCard = this.DrawOneCard();

            int remainingNonRolling = 2;

            if (!firstDrawnCard.IsRolling)
            {
                --remainingNonRolling;
            }
            if (!secondDrawnCard.IsRolling)
            {
                --remainingNonRolling;
            }
            currentDrawnList.Add(firstDrawnCard);
            currentDrawnList.Add(secondDrawnCard);

            while (remainingNonRolling > 0)
            {
                Card nextCard = this.DrawOneCard();
                if(!nextCard.IsRolling)
                {
                    --remainingNonRolling;
                    currentDrawnList.Add(nextCard);
                }
            }

            int totalDamage = ComputeDamageOnAdvantage(currentDrawnList);
            int rollingCardsApplied = currentDrawnList.Count(c => c.IsRolling);

            if(currentDrawnList.Any(c => c.IsCrit || c.IsMiss))
            {
                this.ShuffleAllCards();
            }

            //Debug
            //string cardsDrawnStr = string.Join(" -> ", currentDrawnList.Select(c => c.ToString()));
            //Console.WriteLine($"{cardsDrawnStr} ({totalDamage}, {rollingCardsApplied})");
            //Console.WriteLine(string.Join(" -> ", this.CardsToDraw.Select(c => c.ToString())));
            //Console.WriteLine();

            return new DrawResult(totalDamage, rollingCardsApplied);
        }

        private DrawResult DrawModifierCardsWithRawRules()
        {
            List<Card> currentDrawnList = new List<Card>();

            Card firstDrawnCard = this.DrawOneCard();
            Card secondDrawnCard = this.DrawOneCard();

            int remainingNonRolling = 1;

            if (!firstDrawnCard.IsRolling)
            {
                --remainingNonRolling;
            }
            if (!secondDrawnCard.IsRolling)
            {
                --remainingNonRolling;
            }
            currentDrawnList.Add(firstDrawnCard);
            currentDrawnList.Add(secondDrawnCard);

            while (remainingNonRolling > 0)
            {
                Card nextCard = this.DrawOneCard();
                if (!nextCard.IsRolling)
                {
                    --remainingNonRolling;
                }
                currentDrawnList.Add(nextCard);
            }

            int totalDamage = ComputeDamageOnAdvantage(currentDrawnList);
            int rollingCardsApplied = currentDrawnList.Count(c => c.IsRolling);

            if (currentDrawnList.Any(c => c.IsCrit || c.IsMiss))
            {
                this.ShuffleAllCards();
            }

            //Debug
            //string cardsDrawnStr = string.Join(" -> ", currentDrawnList.Select(c => c.ToString()));
            //Console.WriteLine($"{cardsDrawnStr} ({totalDamage}, {rollingCardsApplied})");
            //Console.WriteLine(string.Join(" -> ", this.CardsToDraw.Select(c => c.ToString())));
            //Console.WriteLine();

            return new DrawResult(totalDamage, rollingCardsApplied);
        }

        private Card DrawOneCard()
        {
            if (CardsToDraw.Count == 0)
            {
                this.ShuffleAllCards();
            }

            Card drawnCard = CardsToDraw.PopLast();
            CardsDrawn.Add(drawnCard);
            return drawnCard;
        }

        private void ShuffleAllCards()
        {
            CardsToDraw.AddRange(CardsDrawn);
            CardsDrawn.Clear();
            CardsToDraw.Shuffle(rng);
        }

        private int ComputeDamageOnAdvantage(List<Card> cardList)
        {
            int rollingDamageModifier = cardList.Where(c => c.IsRolling).Aggregate(0, (agg, c) => agg + c.Modifier);
            IEnumerable<Card> nonRollingCards = cardList.Where(c => !c.IsRolling);

            int damageBeforeNonRolling = this.BaseDamage + rollingDamageModifier;

            List<int> possibleResults = new List<int>();

            foreach(Card nonRollingCard in nonRollingCards)
            {
                if(nonRollingCard.IsCrit)
                {
                    possibleResults.Add(damageBeforeNonRolling * 2);
                }
                else if (nonRollingCard.IsMiss)
                {
                    possibleResults.Add(0);
                }
                else
                {
                    possibleResults.Add(damageBeforeNonRolling + nonRollingCard.Modifier);
                }
            }

            return possibleResults.OrderByDescending(i => i).First();
        }
    }


    public static class ListExtensions
    {
        public static void Shuffle<T>(this IList<T> list, Random rng)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static T PopLast<T>(this IList<T> list)
        {
            int last = list.Count - 1;
            T item = list[last];
            list.RemoveAt(last);
            return item;
        }
    }
}
