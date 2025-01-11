namespace Learning_Backend.Contracts
{
    /// <summary>
    /// Defines the <see cref="IRepoWrapper" />.
    /// </summary>
    public interface IRepoWrapper
    {
        /// <summary>
        /// Gets the User.
        /// </summary>
        IUserRepo User { get; }
    }
}
