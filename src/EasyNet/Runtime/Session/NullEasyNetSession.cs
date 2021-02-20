namespace EasyNet.Runtime.Session
{
	/// <summary>
	/// Implements null object pattern for <see cref="IEasyNetSession"/>.
	/// </summary>
	public class NullEasyNetSession : EasyNetSessionBase
	{
		/// <summary>
		/// Singleton instance.
		/// </summary>
		public static NullEasyNetSession Instance { get; } = new NullEasyNetSession();

		public override string UserId => string.Empty;

		public override string TenantId => string.Empty;

		public override string UserName => string.Empty;

		public override string Role => string.Empty;

		public override string ImpersonatorUserId => string.Empty;

        public override string ImpersonatorTenantId => string.Empty;
    }
}
