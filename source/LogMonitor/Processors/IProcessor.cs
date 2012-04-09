namespace LogMonitor.Processors
{
    /// <summary>
    /// Processor for added content.
    /// </summary>
    public interface IProcessor
    {
        /// <summary>
        /// Called when [content added].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="LogMonitor.ContentEventArgs" /> instance containing the event data.</param>
        void OnContentAdded(object sender, ContentEventArgs e);
    }
}
