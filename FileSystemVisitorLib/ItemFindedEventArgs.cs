namespace FileSystemVisitorLib
{
    /// <summary>
    /// Provides data for FileSystemVisitor class events 
    /// </summary>
    public class ItemFindedEventArgs
    {
        /// <summary>
        /// Gets or sets flag to cancel searching
        /// </summary>
        public bool CancelSearching { get; set; }
        /// <summary>
        /// Gets or sets flag to except item from result
        /// </summary>
        public bool ExceptItemFromResult { get; set; }
        /// <summary>
        /// Gets or sets full path to found item
        /// </summary>
        public string FullName { get; set; }
    }
}
