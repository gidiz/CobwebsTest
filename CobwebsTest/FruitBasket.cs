using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CobwebsTest
{
    public class FruitBasket
    {
        int BasketWeight;
        bool stopCount;
        List<int> allPlayersGuess;
        static object wait;
        Player win;
        List<string> names;

        public FruitBasket()
        {
            BasketWeight = new Random().Next(40, 140);
            allPlayersGuess = new List<int>();
            wait = new object();
            names = new List<string>();
        }


        public void Play()
        {
            List<Player> players = new List<Player>();
            int participating = 0;
            bool isCurrectNumOfPlayer = false;

            while (!isCurrectNumOfPlayer)
            {
                Console.WriteLine("Please enter The number of participating players – 2 through 8");
                isCurrectNumOfPlayer = validParticipating(Console.ReadLine(), out participating);
            }
            int indexPlayer = 0;
            string name = string.Empty;
            while (indexPlayer < participating)
            {
                Console.WriteLine("Please enter the player's name");
                name = Console.ReadLine();
                indexPlayer = setPlayer(name, indexPlayer, players);
            }

            startGame(players);
        }
        private int setPlayer(string name, int indexPlayer, List<Player> players)
        {

            bool isValid = false;
            int playerType = 0;
            while (!isValid)
            {
                Console.WriteLine("Please enter a number for the player type");
                Console.WriteLine("1 - Random Player");
                Console.WriteLine("2 - Memory Player");
                Console.WriteLine("3 - Thorough Player");
                Console.WriteLine("4 - Cheater Player");
                Console.WriteLine("5 - Thorough Cheater Player");
                isValid = int.TryParse(Console.ReadLine(), out playerType);
                if (!isValid)
                    Console.WriteLine("Please enter a valid real number");
                else
                {
                    switch (playerType)
                    {
                        case 1:
                            players.Add(new RandomPlayer(name, BasketWeight, allPlayersGuess));
                            break;
                        case 2:
                            players.Add(new MemoryPlayer(name, BasketWeight, allPlayersGuess));

                            break;
                        case 3:
                            players.Add(new ThoroughPlayer(name, BasketWeight, allPlayersGuess));

                            break;
                        case 4:
                            players.Add(new CheaterPlayer(name, BasketWeight, allPlayersGuess));

                            break;
                        case 5:
                            players.Add(new ThoroughCheaterPlayer(name, BasketWeight, allPlayersGuess));

                            break;
                        default:
                            isValid = false;
                            Console.WriteLine("Invalid type. Please choose again");
                            break;

                    }
                }

            }
            return ++indexPlayer;
        }
        private bool validParticipating(string val, out int participating)
        {
            bool isValid = false;
            isValid = int.TryParse(val, out participating);

            if (isValid)
            {
                isValid = participating >= 2 && participating <= 8;

            }
            else
                Console.WriteLine("Please enter a valid real number");
            return isValid;

        }
        private void startGame(List<Player> players)
        {
            Task.Run(async () =>
            {
                await Task.Delay(1500);
                if (!stopCount)
                {
                    Console.WriteLine("Stopped after 1500 milliseconds");
                    stopCount = true;
                }

            });
            Task[] tasks = new Task[players.Count];

            var cancelToken = new CancellationTokenSource();



            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(() => guessFruitWeight(players[i], cancelToken.Token), cancelToken.Token);
                Thread.Sleep(10);
            }

            Task.WaitAll(tasks, cancelToken.Token);


            checkTheWinner(players);
            Console.ReadKey();

        }
        private void checkTheWinner(List<Player> players)
        {
            Console.WriteLine($"The real weight of the basket is: {BasketWeight}");
            if (win != null)
                Console.WriteLine(win.ToString());
            else
            {
                Player winByCloseGuess = players.OrderBy(x => x.closeDistanceGuess).ThenBy(x => x.closeDistanceGuessInAttemps).First();
                Console.WriteLine(winByCloseGuess.ToString());
            }
        }
        private async void guessFruitWeight(Player player, CancellationToken token)
        {
            while (!stopCount)
            {
                try
                {
                    Monitor.Enter(wait);
                    int distance = await player.Guess(allPlayersGuess);

                    if (distance == 0)
                    {
                        stopCount = true;
                        win = player;
                    }
                    else if (allPlayersGuess.Count == 100)
                    {
                        stopCount = true;
                    }
                    else
                    {
                        Monitor.Wait(wait, distance);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    Monitor.Exit(wait);
                    // Thread.Sleep(50);
                }
                token.ThrowIfCancellationRequested();
            }
        }
    }


}
