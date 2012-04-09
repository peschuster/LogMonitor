using System;

namespace LogMonitor
{
    public class ContentEventArgs : EventArgs
    {
        public ContentEventArgs(string fullName, string addedContent)
        {
            this.FullName = fullName;
            this.AddedContent = addedContent;
        }

        public string FullName { get; private set; }

        public string AddedContent { get; private set; }
    }
}
