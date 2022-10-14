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

        public Card Card1;
        public Card Card2;

        public async Task StartGameAsync()
        {
            Cards = new List<Card>(CardImages.Animals);
            GameIsValid();
            List<Card> CardCopy = new();
            foreach (var item in Cards)
            {
                CardCopy.Add(new Card(item.Image));
            }
            Cards.AddRange(CardCopy);
            Cards = Shuffle(Cards).ToList();
            status = GameStatus.Started;
            await StartTimerAsync();
        }

        public void Mark(Card card)
        {
            if (card.Selected || card == Card1 || card == Card2)
            {
                return;
            }

            if (Card1 == null || Card2 == null)
            {
                if (Card1 == null)
                {
                    Card1 = card;
                }
                else
                {
                    Card2 = card;
                }

                if (Card1 != null && Card2 != null)
                {
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(1500);
                        Card1.Match(Card2);
                        Card1 = null;
                        Card2 = null;
                    });
                }

                StateHasChanged();
            }
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
