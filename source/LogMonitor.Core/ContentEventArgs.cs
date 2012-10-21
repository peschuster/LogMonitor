using System;

namespace LogMonitor
{
    public class ContentEventArgs : EventArgs
    {
        public ContentEventArgs(string fileName, string addedContent)
        {
            this.FileName = fileName;
            this.AddedContent = addedContent;            
        }

        public string FileName { get; private set; }

        public string AddedContent { get; private set; }
    }
}