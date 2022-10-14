namespace MemoryGame.Models
{
    public class Card
    {
        public string Image { get; private set; }
        public bool Selected { get; private set; }
        public Card(string image)
        {
            Image = image;
        }

        public void Match(Card card)
        {
            if (card.Image == Image)
            {
                Selected = true;
                if (!card.Selected)
                {
                    card.Match(this);
                }
            }
        }

        public void Reset()
        {
            Selected = false;
        }
    }
}
