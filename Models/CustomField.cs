namespace DocumentSummarization.Models
{
    public class CustomField
    {
        public string Name { get; set; }
        public string Content { get; set; }

        public CustomField(string name, string content)
        {
            this.Name = name;
            this.Content = content;
        }
    }
}