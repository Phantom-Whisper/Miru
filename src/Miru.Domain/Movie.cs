namespace Miru.Domain
{
    public class Movie : Media
    {
        /// <summary>
        /// Duration of the movie in minutes.
        /// </summary>
        public TimeSpan Duration { get; private set; }

        /// <summary>
        /// Default constructor for ORM.
        /// </summary>
        protected Movie() { }
    }
}
