using MemoryGame.Models;
using Microsoft.AspNetCore.Components;
using MemoryGame.Contants;

namespace MemoryGame.Pages
{
    public class BoardBase : ComponentBase, IDisposable
    {
        public List<Card> Cards;
        public DateTime GameStarted;
        public PeriodicTimer timer;
        public GameStatus status = GameStatus.NotStarted;
        private static Random Random = new();
        public async Task StartGameAsync()
        {
            Cards = new List<Card>(CardImages.Animals);
            GameIsValid();

            Cards.AddRange(Cards);

            Cards = Shuffle(Cards).ToList();
            status = GameStatus.Started;
            await StartTimerAsync();
        }

        private async Task StartTimerAsync()
        {
            GameStarted = DateTime.Now;
            timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
            while (await timer.WaitForNextTickAsync())
            {
                await InvokeAsync(StateHasChanged);
            }
        }

        private void GameIsValid()
        {
            if (Cards == null)
            {
                throw new ArgumentNullException(nameof(Cards));
            }
            if (Cards.Count % 2 != 0)
            {
                throw new Exception("number of cards must be even");
            }

            if (Cards.GroupBy(x => x.Image).Any(y => y.Count() > 1))
            {
                throw new Exception("there are repeated cards");
            }
        }

        private static IEnumerable<Card> Shuffle(IEnumerable<Card> cards)
        {
            var el = cards.ToArray();
            for (int i = el.Length - 1; i >= 0; i--)
            {
                int swapIndex = Random.Next(i + 1);
                yield return el[swapIndex];
                el[swapIndex] = el[i];
                el[swapIndex].Reset();
            }
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
