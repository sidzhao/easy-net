using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DotNetCore.CAP;
using DotNetCore.CAP.Internal;
using DotNetCore.CAP.Messages;
using DotNetCore.CAP.Monitoring;
using DotNetCore.CAP.MySql;
using DotNetCore.CAP.Persistence;
using DotNetCore.CAP.Serialization;
using Microsoft.Extensions.Options;
using MySqlConnector;
using EasyNet.Extensions.DependencyInjection;

namespace EasyNet.EventBus.Cap.MySql
{
    public class EasyNetCapMySqlDataStorage : IDataStorage
    {
        private readonly IOptions<MySqlOptions> _options;
        private readonly IOptions<CapOptions> _capOptions;
        private readonly ISerializer _serializer;
        private readonly string _pubName;
        private readonly string _recName;
        private readonly IDataStorage _capMySqlDataStorage;
        private bool _isFirstLoadMessagesOfNeedRetry = true;

        public EasyNetCapMySqlDataStorage(
            IOptions<MySqlOptions> options,
            IOptions<CapOptions> capOptions,
            IStorageInitializer initializer,
            ISerializer serializer)
        {
            _options = options;
            _capOptions = capOptions;
            _serializer = serializer;
            _pubName = initializer.GetPublishedTableName();
            _recName = initializer.GetReceivedTableName();

            _capMySqlDataStorage = new MySqlDataStorage(options, capOptions, initializer, serializer);
        }

        public Task ChangePublishStateAsync(MediumMessage message, StatusName state)
        {
            return _capMySqlDataStorage.ChangePublishStateAsync(message, state);
        }

        public Task ChangeReceiveStateAsync(MediumMessage message, StatusName state)
        {
            return _capMySqlDataStorage.ChangeReceiveStateAsync(message, state);
        }

        public MediumMessage StoreMessage(string name, Message content, object dbTransaction = null)
        {
            return _capMySqlDataStorage.StoreMessage(name, content, dbTransaction);
        }

        public void StoreReceivedExceptionMessage(string name, string @group, string content)
        {
            _capMySqlDataStorage.StoreReceivedExceptionMessage(name, @group, content);
        }

        public MediumMessage StoreReceivedMessage(string name, string @group, Message content)
        {
            return _capMySqlDataStorage.StoreReceivedMessage(name, @group, content);
        }

        public Task<int> DeleteExpiresAsync(string table, DateTime timeout, int batchCount = 1000,
            CancellationToken token = new CancellationToken())
        {
            return _capMySqlDataStorage.DeleteExpiresAsync(table, timeout, batchCount, token);
        }

        public Task<IEnumerable<MediumMessage>> GetPublishedMessagesOfNeedRetry()
        {
            return GetMessagesOfNeedRetryAsync(_pubName);
        }

        public Task<IEnumerable<MediumMessage>> GetReceivedMessagesOfNeedRetry()
        {
            return GetMessagesOfNeedRetryAsync(_recName);
        }

        public IMonitoringApi GetMonitoringApi()
        {
            return _capMySqlDataStorage.GetMonitoringApi();
        }

        private async Task<IEnumerable<MediumMessage>> GetMessagesOfNeedRetryAsync(string tableName)
        {
            // 在系统启动时，CAP会同时开启多个后台线程，有可能在获取到待处理数据后，CAP并没有完成注册消费者.
            // 所以会导致在处理前几条数据时找不到消费者的情况.
            // 定义该字段来标识是否是第一次读取数据，如果是则延后30秒再执行.
            if (_isFirstLoadMessagesOfNeedRetry)
            {
                await Task.Delay(1000 * 30);
                _isFirstLoadMessagesOfNeedRetry = false;
            }

            var fourMinAgo = DateTime.Now.AddMinutes(-4).ToString("O");
            var sql =
                "SELECT * FROM ( " +
                $"SELECT `Id`,`Content`,`Retries`,`Added` FROM `{tableName}` WHERE `Version`='{_capOptions.Value.Version}' AND `StatusName` = '{StatusName.Scheduled}' " +
                "UNION " +
                $"SELECT `Id`,`Content`,`Retries`,`Added` FROM `{tableName}` WHERE `Version`='{_capOptions.Value.Version}' AND `Retries`<3 AND `StatusName` = '{StatusName.Failed}' " +
                "UNION " +
                $"SELECT `Id`,`Content`,`Retries`,`Added` FROM `{tableName}` WHERE `Retries`>=3 AND `Retries`<{_capOptions.Value.FailedRetryCount} " +
                $"AND `Version`='{_capOptions.Value.Version}' AND `Added`<'{fourMinAgo}' AND `StatusName` = '{StatusName.Failed}' " +
                ") AS t1 LIMIT 200;";

            using (var connection = new MySqlConnection(_options.Value.ConnectionString))
            {
                var result = connection.ExecuteReader(sql, reader =>
                {
                    var messages = new List<MediumMessage>();
                    while (reader.Read())
                    {
                        messages.Add(new MediumMessage
                        {
                            DbId = reader.GetInt64(0).ToString(),
                            Origin = _serializer.Deserialize(reader.GetString(1)),
                            Retries = reader.GetInt32(2),
                            Added = reader.GetDateTime(3)
                        });
                    }

                    return messages;
                });

                return result;
            }
        }
    }
}
