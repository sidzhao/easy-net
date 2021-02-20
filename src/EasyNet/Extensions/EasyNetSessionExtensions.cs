using System;
using EasyNet.Runtime.Session;

namespace EasyNet.Extensions
{
    /// <summary>
    /// Extension methods in a <see cref="IEasyNetSession" />.
    /// </summary>
    public static class EasyNetSessionExtensions
    {
        /// <summary>
        /// Get user id and return int.
        /// </summary>
        /// <param name="session">The <see cref="IEasyNetSession"/></param>
        /// <returns>The user id.</returns>
        /// <exception cref="EasyNetException">Throw exception if EasyNetSession.UserId is null.</exception>
        /// <exception cref="FormatException">Throw exception if the user id is not in the correct format.</exception>
        /// <exception cref="OverflowException">Throw exception if the user id is greater than <see cref="Int32.MaxValue"/>.</exception>
        public static int GetIntUserId(this IEasyNetSession session)
        {
            Check.NotNull(session, nameof(session));

            if (string.IsNullOrEmpty(session.UserId)) throw new EasyNetException("EasyNetSession.UserId is null. Probably, user is not logged in.");

            return int.Parse(session.UserId);
        }

        /// <summary>
        /// Get user id and return int.
        /// </summary>
        /// <param name="session">The <see cref="IEasyNetSession"/></param>
        /// <returns>The user id or null.</returns>
        /// <exception cref="FormatException">Throw exception if the user id is not in the correct format.</exception>
        /// <exception cref="OverflowException">Throw exception if the user id is greater than <see cref="Int32.MaxValue"/>.</exception>
        public static int? GetIntUserIdOrNull(this IEasyNetSession session)
        {
            Check.NotNull(session, nameof(session));

            if (string.IsNullOrEmpty(session.UserId)) return null;

            return int.Parse(session.UserId);
        }

        /// <summary>
        /// Get user id and return long.
        /// </summary>
        /// <param name="session">The <see cref="IEasyNetSession"/></param>
        /// <returns>The user id.</returns>
        /// <exception cref="EasyNetException">Throw exception if EasyNetSession.UserId is null.</exception>
        /// <exception cref="FormatException">Throw exception if the user id is not in the correct format.</exception>
        /// <exception cref="OverflowException">Throw exception if the user id is greater than <see cref="Int64.MaxValue"/>.</exception>
        public static long GetLongUserId(this IEasyNetSession session)
        {
            Check.NotNull(session, nameof(session));

            if (string.IsNullOrEmpty(session.UserId)) throw new EasyNetException("EasyNetSession.UserId is null. Probably, user is not logged in.");

            return long.Parse(session.UserId);
        }

        /// <summary>
        /// Get user id and return int.
        /// </summary>
        /// <param name="session">The <see cref="IEasyNetSession"/></param>
        /// <returns>The user id or null.</returns>
        /// <exception cref="FormatException">Throw exception if the user id is not in the correct format.</exception>
        /// <exception cref="OverflowException">Throw exception if the user id is greater than <see cref="Int64.MaxValue"/>.</exception>
        public static long? GetLongUserIdOrNull(this IEasyNetSession session)
        {
            Check.NotNull(session, nameof(session));

            if (string.IsNullOrEmpty(session.UserId)) return null;

            return long.Parse(session.UserId);
        }

        /// <summary>
        /// Get user id and return Guid.
        /// </summary>
        /// <param name="session">The <see cref="IEasyNetSession"/></param>
        /// <returns>The user id.</returns>
        /// <exception cref="EasyNetException">Throw exception if EasyNetSession.UserId is null.</exception>
        /// <exception cref="FormatException">Throw exception if the user id is not in the correct format.</exception>
        public static Guid GetGuidUserId(this IEasyNetSession session)
        {
            Check.NotNull(session, nameof(session));

            if (string.IsNullOrEmpty(session.UserId)) throw new EasyNetException("EasyNetSession.UserId is null. Probably, user is not logged in.");

            return Guid.Parse(session.UserId);
        }

        /// <summary>
        /// Get user id and return int.
        /// </summary>
        /// <param name="session">The <see cref="IEasyNetSession"/></param>
        /// <returns>The user id or null.</returns>
        /// <exception cref="FormatException">Throw exception if the user id is not in the correct format.</exception>
        public static Guid? GetGuidUserIdOrNull(this IEasyNetSession session)
        {
            Check.NotNull(session, nameof(session));

            if (string.IsNullOrEmpty(session.UserId)) return null;

            return Guid.Parse(session.UserId);
        }

        /// <summary>
        /// Get tenant id and return int.
        /// </summary>
        /// <param name="session">The <see cref="IEasyNetSession"/></param>
        /// <returns>The tenant id.</returns>
        /// <exception cref="EasyNetException">Throw exception if EasyNetSession.TenantId is null.</exception>
        /// <exception cref="FormatException">Throw exception if the user id is not in the correct format.</exception>
        /// <exception cref="OverflowException">Throw exception if the user id is greater than <see cref="Int32.MaxValue"/>.</exception>
        public static int GetIntTenantId(this IEasyNetSession session)
        {
            Check.NotNull(session, nameof(session));

            if (string.IsNullOrEmpty(session.TenantId)) throw new EasyNetException("EasyNetSession.TenantId is null. Probably, user is not logged in.");

            return int.Parse(session.TenantId);
        }

        /// <summary>
        /// Get tenant id and return int.
        /// </summary>
        /// <param name="session">The <see cref="IEasyNetSession"/></param>
        /// <returns>The tenant id or null.</returns>
        /// <exception cref="FormatException">Throw exception if the tenant id is not in the correct format.</exception>
        /// <exception cref="OverflowException">Throw exception if the tenant id is greater than <see cref="Int32.MaxValue"/>.</exception>
        public static int? GetIntTenantOrNull(this IEasyNetSession session)
        {
            Check.NotNull(session, nameof(session));

            if (string.IsNullOrEmpty(session.TenantId)) return null;

            return int.Parse(session.TenantId);
        }

        /// <summary>
        /// Get tenant id and return long.
        /// </summary>
        /// <param name="session">The <see cref="IEasyNetSession"/></param>
        /// <returns>The tenant id.</returns>
        /// <exception cref="EasyNetException">Throw exception if EasyNetSession.TenantId is null.</exception>
        /// <exception cref="FormatException">Throw exception if the tenant id is not in the correct format.</exception>
        /// <exception cref="OverflowException">Throw exception if the tenant id is greater than <see cref="Int64.MaxValue"/>.</exception>
        public static long GetLongTenantId(this IEasyNetSession session)
        {
            Check.NotNull(session, nameof(session));

            if (string.IsNullOrEmpty(session.TenantId)) throw new EasyNetException("EasyNetSession.TenantId is null. Probably, user is not logged in.");

            return long.Parse(session.TenantId);
        }

        /// <summary>
        /// Get tenant id and return long.
        /// </summary>
        /// <param name="session">The <see cref="IEasyNetSession"/></param>
        /// <returns>The tenant id or null.</returns>
        /// <exception cref="FormatException">Throw exception if the tenant id is not in the correct format.</exception>
        /// <exception cref="OverflowException">Throw exception if the tenant id is greater than <see cref="Int64.MaxValue"/>.</exception>
        public static long? GetLongTenantOrNull(this IEasyNetSession session)
        {
            Check.NotNull(session, nameof(session));

            if (string.IsNullOrEmpty(session.TenantId)) return null;

            return long.Parse(session.TenantId);
        }

        /// <summary>
        /// Get tenant id and return Guid.
        /// </summary>
        /// <param name="session">The <see cref="IEasyNetSession"/></param>
        /// <returns>The user id.</returns>
        /// <exception cref="EasyNetException">Throw exception if EasyNetSession.TenantId is null.</exception>
        /// <exception cref="FormatException">Throw exception if the tenant id is not in the correct format.</exception>
        public static Guid GetGuidTenantId(this IEasyNetSession session)
        {
            Check.NotNull(session, nameof(session));

            if (string.IsNullOrEmpty(session.TenantId)) throw new EasyNetException("EasyNetSession.TenantId is null. Probably, user is not logged in.");

            return Guid.Parse(session.TenantId);
        }

        /// <summary>
        /// Get tenant id and return Guid.
        /// </summary>
        /// <param name="session">The <see cref="IEasyNetSession"/></param>
        /// <returns>The user id or null.</returns>
        /// <exception cref="FormatException">Throw exception if the user id is not in the correct format.</exception>
        public static Guid? GetGuidTenantIdOrNull(this IEasyNetSession session)
        {
            Check.NotNull(session, nameof(session));

            if (string.IsNullOrEmpty(session.TenantId)) return null;

            return Guid.Parse(session.TenantId);
        }
    }
}
