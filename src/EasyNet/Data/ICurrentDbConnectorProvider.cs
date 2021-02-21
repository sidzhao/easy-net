namespace EasyNet.Data
{
    /// <summary>
    /// Used to get/set current <see cref="IDbConnector"/>. 
    /// </summary>
    public interface ICurrentDbConnectorProvider
    {
        /// <summary>
        /// Gets/sets current <see cref="IDbConnector"/>.
        /// </summary>
        IDbConnector Current { get; set; }
    }
}
