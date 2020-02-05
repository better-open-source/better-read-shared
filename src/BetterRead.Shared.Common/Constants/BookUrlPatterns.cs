namespace BetterRead.Shared.Common.Constants
{
    /// <summary>
    /// This file contains all URL patterns.
    /// </summary>
    public static class BookUrlPatterns
    {
        public const string BaseUrl = "http://loveread.ec";
        
        /// <summary>
        /// Book URL pattern: 0 - book ID.
        /// </summary>
        public const string General = BaseUrl + "/view_global.php?id={0}";

        /// <summary>
        /// Book contents URL pattern: 0 - book ID.
        /// </summary>
        public const string Contents = BaseUrl + "/contents.php?id={0}";
        
        /// <summary>
        /// Book notes URL pattern: 0 - book ID.
        /// </summary>
        public const string Notes = BaseUrl + "/notes.php?id={0}";
        
        /// <summary>
        /// Read book URL pattern: 0 - book ID; 1 - current page number.
        /// </summary>
        public const string Read = BaseUrl + "/read_book.php?id={0}&p={1}";
        
        /// <summary>
        /// Book image URL pattern: 0 - book ID.
        /// </summary>
        public const string Cover = BaseUrl + "/img/photo_books/{0}.jpg";
        
        /// <summary>
        /// Book image URL pattern: 0 - book ID; 1 - Image number.
        /// </summary>
        public const string ContentImage = BaseUrl + "/img/photo_books/{0}/i_{1:000}.jpg";

        /// <summary>
        /// Book search URL pattern.
        /// </summary>
        public const string Search = BaseUrl + "/search.php";
    }
}