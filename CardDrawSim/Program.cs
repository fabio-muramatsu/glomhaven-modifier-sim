using System;
using System.Collections.Generic;

namespace CardDrawSim
{
    class Program
    {
        static void Main(string[] args)
        {
            int attackBaseDamage = 5;

            string[] text = System.IO.File.ReadAllLines(@"ModifierDecks/Doomstalker.csv");

            List<Card> deck = new List<Card>();

            foreach(string line in text)
            {
                string[] lineContents = line.Split(',');
                deck.Add(new Card(int.Parse(lineContents[0]), bool.Parse(lineContents[1])));
            }

            DrawSimulator sim = new DrawSimulator(deck, attackBaseDamage);
            //(float averageHitPoint, float averageRollingApplied) = sim.SimulateHouseRules(1000000);
            (float averageHitPoint, float averageRollingApplied) = sim.SimulateRawRules(1000000);
            Console.WriteLine($"Average hit point: {averageHitPoint} ; Average rolling applied: {averageRollingApplied}");

            Console.ReadLine();
        }

    }
}
