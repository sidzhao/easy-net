using System.Runtime.CompilerServices;

// Allow the following projects to access internal resources
[assembly: InternalsVisibleTo("EasyNet.EntityFrameworkCore")]
[assembly: InternalsVisibleTo("EasyNet.Dapper")]

[assembly: InternalsVisibleTo("EasyNet.Core.Tests")]
[assembly: InternalsVisibleTo("EasyNet.Tests")]
[assembly: InternalsVisibleTo("EasyNet.Data.Tests")]
[assembly: InternalsVisibleTo("EasyNet.Sqlite.Tests")]
[assembly: InternalsVisibleTo("EasyNet.SqlServer.Tests")]
[assembly: InternalsVisibleTo("EasyNet.EntityFrameworkCore.Tests")]