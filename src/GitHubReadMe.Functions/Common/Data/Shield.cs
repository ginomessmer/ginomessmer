namespace GitHubReadMe.Functions.Common.Data
{
    public class Shield
    {
        /// <summary>
        /// The shield's title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The shield's message.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The shield's color.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// A complementary logo.
        /// </summary>
        public string Logo { get; set; }

        public Shield(string title, string value, string color = "inactive", string logo = "")
        {
            Title = title;
            Value = value;
            Color = color;
            Logo = logo;
        }
    }
}