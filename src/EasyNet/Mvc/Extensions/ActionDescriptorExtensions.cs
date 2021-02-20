using System.Reflection;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;

// ReSharper disable once CheckNamespace
namespace EasyNet.Extensions.DependencyInjection
{
	internal static class ActionDescriptorExtensions
	{
		public static ControllerActionDescriptor AsControllerActionDescriptor(this ActionDescriptor actionDescriptor)
		{
			if (!actionDescriptor.IsControllerAction())
			{
				throw new EasyNetException($"{nameof(actionDescriptor)} should be type of {typeof(ControllerActionDescriptor).AssemblyQualifiedName}");
			}

			return actionDescriptor as ControllerActionDescriptor;
		}

		public static MethodInfo GetMethodInfo(this ActionDescriptor actionDescriptor)
		{
			return actionDescriptor.AsControllerActionDescriptor().MethodInfo;
		}

		public static bool IsControllerAction(this ActionDescriptor actionDescriptor)
		{
			return actionDescriptor is ControllerActionDescriptor;
		}
	}
}
