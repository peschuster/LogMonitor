namespace LogMonitor.Processors
{
    public class DefaultPreProcessor : IPreProcessor
    {
        public FileChange Process(string fileName, string content)
        {
            return new FileChange
            {
                File = fileName,
                Content = content,
            };
        }
    }
}
