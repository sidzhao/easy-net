using System.Runtime.CompilerServices;

// Allow the following projects to access internal resources
[assembly: InternalsVisibleTo("EasyNet.EntityFrameworkCore")]
[assembly: InternalsVisibleTo("EasyNet.Dapper")]

[assembly: InternalsVisibleTo("EasyNet.Core.Tests")]
[assembly: InternalsVisibleTo("EasyNet.Tests")]
[assembly: InternalsVisibleTo("EasyNet.SqlLite.Tests")]
[assembly: InternalsVisibleTo("EasyNet.EntityFrameworkCore.Tests")]