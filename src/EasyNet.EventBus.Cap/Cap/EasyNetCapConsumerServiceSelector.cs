using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotNetCore.CAP;
using DotNetCore.CAP.Internal;
using EasyNet.Extensions;
using Microsoft.Extensions.Options;

namespace EasyNet.EventBus.Cap
{
    public class EasyNetCapConsumerServiceSelector : ConsumerServiceSelector
    {
        private readonly EasyNetOptions _options;

        public EasyNetCapConsumerServiceSelector(IServiceProvider serviceProvider, IOptions<EasyNetOptions> options) : base(serviceProvider)
        {
            _options = options.Value;
        }

        /// <summary>
        /// 从接口<see cref="ICapSubscribe"/>和<see cref="IEasyNetEventSubscribe{TEventData}"/>中获取消费者
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        protected override IEnumerable<ConsumerExecutorDescriptor> FindConsumersFromInterfaceTypes(IServiceProvider provider)
        {
            // 从DotNetCore.CAP.ICapSubscribe中获取消费者
            var executorDescriptorList = base.FindConsumersFromInterfaceTypes(provider).ToList();

            // 从IEasyNetEventSubscribe<>中获取消费者
            if (_options.Assemblies != null && _options.Assemblies.Any())
            {
                var subscriberTypeInfo = typeof(IEasyNetEventSubscribe<>);

                foreach (var assembly in _options.Assemblies)
                {
                    foreach (var publicType in assembly.GetExportedTypes().Where(p => p.IsClass))
                    {
                        var subscriberInterface = publicType.GetImplementedRawGeneric(subscriberTypeInfo);
                        if (subscriberInterface != null)
                        {
                            executorDescriptorList.Add(GetSubscriberAttributesDescription(publicType, subscriberInterface));
                        }
                    }
                }
            }

            return executorDescriptorList;
        }

        protected ConsumerExecutorDescriptor GetSubscriberAttributesDescription(Type type, Type subscriberInterfaceType)
        {
            var subscriberAttr = type.GetCustomAttribute<EasyNetCapSubscribeAttribute>();
            if (subscriberAttr == null)
                throw new EasyNetException($"在{type.AssemblyQualifiedName}中需要指定{typeof(EasyNetCapSubscribeAttribute).AssemblyQualifiedName}.");

            var receiveMethodInfo = subscriberInterfaceType.GetRuntimeMethods().FirstOrDefault();
            if (receiveMethodInfo == null)
                throw new EasyNetException($"在{subscriberInterfaceType.AssemblyQualifiedName}中没有找到Receive方法.");

            SetSubscribeAttribute(subscriberAttr);

            return new ConsumerExecutorDescriptor
            {
                Attribute = subscriberAttr,
                ClassAttribute = null,
                MethodInfo = receiveMethodInfo,
                ImplTypeInfo = type.GetTypeInfo(),
                ServiceTypeInfo = null,
                Parameters = receiveMethodInfo.GetParameters().Select(p => new ParameterDescriptor
                {
                    Name = p.Name,
                    ParameterType = p.ParameterType,
                    IsFromCap = p.GetCustomAttributes(typeof(FromCapAttribute)).Any()
                }).ToList()
            };
        }
    }
}
