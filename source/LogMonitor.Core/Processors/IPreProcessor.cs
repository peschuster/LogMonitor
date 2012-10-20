namespace LogMonitor.Processors
{
    public interface IPreProcessor
    {
        FileChange Process(string fileName, string content);
    }
}
