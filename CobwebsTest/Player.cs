using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CobwebsTest
{
    public class Player
    {
        public string Name { get; set; }
        internal int attemps;
        public int closeDistanceGuess { get; set; }
        internal int guess;
        public int closeDistanceGuessInAttemps { get; set; }
        internal int BasketWeight;
        internal List<int> allPlayersGuess;
        public Player(string name, int basketWeight, List<int> allPlayersGuess)
        {
            Name = name;
            BasketWeight = basketWeight;
            closeDistanceGuess = -1;
            this.allPlayersGuess = allPlayersGuess;
        }
        public virtual async Task<int> Guess(List<int> allPlayersGuess)
        {
            return 0;
        }
        internal int setAttempNum(int guess)
        {
            attemps++;
            allPlayersGuess.Add(guess);
            int distance = guess > BasketWeight ? guess - BasketWeight : BasketWeight - guess;
            setDistanceGuessAndAttemp(distance, attemps, guess);
            return distance;
        }
        void setDistanceGuessAndAttemp(int distance, int attemp, int guess)
        {
            closeDistanceGuess = distance;
            closeDistanceGuessInAttemps = attemp;
            this.guess = guess;
        }
        public override string ToString()
        {
            if (closeDistanceGuess == 0)
                return $"{Name} wins with {attemps} attemps";
            else
                return $"{Name} was the closest player to guess with {guess}";
        }

    }
    public class RandomPlayer : Player
    {
        public RandomPlayer(string name, int basketWeight, List<int> allPlayersGuess) : base(name, basketWeight, allPlayersGuess)
        {

        }
        public override async Task<int> Guess(List<int> allPlayersGuess)
        {
            return base.setAttempNum(new Random().Next(40, 140));
        }
    }
    public class MemoryPlayer : Player
    {
        List<int> previesGuess;
        public MemoryPlayer(string name, int basketWeight, List<int> allPlayersGuess) : base(name, basketWeight, allPlayersGuess)
        {
            previesGuess = new List<int>();
        }
        public override async Task<int> Guess(List<int> allPlayersGuess)
        {
            bool isGuessBefore = false;
            int num = 0;
            while (!isGuessBefore)
            {
                num = new Random().Next(40, 140);
                if (!previesGuess.Contains(num))
                {
                    isGuessBefore = true;
                    previesGuess.Add(num);
                }
            }
            return base.setAttempNum(num);
        }
    }
    public class ThoroughPlayer : Player
    {
        int orderNum;
        public ThoroughPlayer(string name, int basketWeight, List<int> allPlayersGuess) : base(name, basketWeight, allPlayersGuess)
        {
            orderNum = 40;
        }
        public override async Task<int> Guess(List<int> allPlayersGuess)
        {
            return base.setAttempNum(orderNum++);
        }
    }
    public class CheaterPlayer : Player
    {
        public CheaterPlayer(string name, int basketWeight, List<int> allPlayersGuess) : base(name, basketWeight, allPlayersGuess)
        {

        }
        public override async Task<int> Guess(List<int> allPlayersGuess)
        {
            bool isGuessBefore = false;
            int num = 0;
            while (!isGuessBefore)
            {
                num = new Random().Next(40, 140);
                if (!allPlayersGuess.Contains(num))
                {
                    isGuessBefore = true;
                }
            }
            return base.setAttempNum(num);
        }
    }
    public class ThoroughCheaterPlayer : Player
    {
        int orderNum;
        public ThoroughCheaterPlayer(string name, int basketWeight, List<int> allPlayersGuess) : base(name, basketWeight, allPlayersGuess)
        {
            orderNum = 40;
        }
        public override async Task<int> Guess(List<int> allPlayersGuess)
        {
            bool isGuessBefore = false;
            int num = 0;
            while (!isGuessBefore)
            {
                num = orderNum++;
                if (!allPlayersGuess.Contains(num))
                {
                    isGuessBefore = true;
                }
            }
            return base.setAttempNum(num);
        }
    }
}
