namespace GitHubReadMe.Functions.Data
{
    public class Shield
    {
        public string Title { get; set; }

        public string Value { get; set; }

        public string Color { get; set; }

        public string Logo { get; set; }

        public Shield(string title, string value, string color, string logo)
        {
            Title = title;
            Value = value;
            Color = color;
            Logo = logo;
        }
    }
}