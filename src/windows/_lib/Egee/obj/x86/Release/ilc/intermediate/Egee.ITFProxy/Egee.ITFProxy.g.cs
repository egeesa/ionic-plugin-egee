using System.Reflection;



namespace __InterfaceProxies
{
	public class ServiceChannelProxy_ISappelService : System.ServiceModel.Channels.ServiceChannelProxy_P, Egee.Proxy.Sappel.ISappelService_P
	{
		System.Threading.Tasks.Task_P<Egee.Proxy.Sappel.InitResponse_P> Egee.Proxy.Sappel.ISappelService_P.InitAsync(Egee.Proxy.Sappel.InitRequest_P request)
		{
			global::System.RuntimeMethodHandle interfaceMethod = global::System.Reflection.DispatchProxyHelpers.GetCorrespondingInterfaceMethodFromMethodImpl();
			global::System.RuntimeTypeHandle interfaceType = typeof(Egee.Proxy.Sappel.ISappelService_P).TypeHandle;
			global::System.Reflection.MethodBase targetMethodInfo = global::System.Reflection.MethodBase.GetMethodFromHandle(
								interfaceMethod, 
								interfaceType
							);
			object[] callsiteArgs = new object[] {
					request
			};
			System.Threading.Tasks.Task_P<Egee.Proxy.Sappel.InitResponse_P> retval = ((System.Threading.Tasks.Task_P<Egee.Proxy.Sappel.InitResponse_P>)base.Invoke(
								((global::System.Reflection.MethodInfo)targetMethodInfo), 
								callsiteArgs
							));
			return retval;
		}

		System.Threading.Tasks.Task_P<Egee.Proxy.Sappel.GetVersionResponse_P> Egee.Proxy.Sappel.ISappelService_P.GetVersionAsync(Egee.Proxy.Sappel.GetVersionRequest_P request)
		{
			global::System.RuntimeMethodHandle interfaceMethod = global::System.Reflection.DispatchProxyHelpers.GetCorrespondingInterfaceMethodFromMethodImpl();
			global::System.RuntimeTypeHandle interfaceType = typeof(Egee.Proxy.Sappel.ISappelService_P).TypeHandle;
			global::System.Reflection.MethodBase targetMethodInfo = global::System.Reflection.MethodBase.GetMethodFromHandle(
								interfaceMethod, 
								interfaceType
							);
			object[] callsiteArgs = new object[] {
					request
			};
			System.Threading.Tasks.Task_P<Egee.Proxy.Sappel.GetVersionResponse_P> retval = ((System.Threading.Tasks.Task_P<Egee.Proxy.Sappel.GetVersionResponse_P>)base.Invoke(
								((global::System.Reflection.MethodInfo)targetMethodInfo), 
								callsiteArgs
							));
			return retval;
		}
	}

	[global::System.Runtime.CompilerServices.ModuleConstructor]
	[global::System.Runtime.CompilerServices.DependencyReductionRoot]
	public static class __DispatchProxyImplementationData
	{
		static global::System.Reflection.DispatchProxyEntry[] s_entryTable = new global::System.Reflection.DispatchProxyEntry[] {
				new global::System.Reflection.DispatchProxyEntry() {
					ProxyClassType = typeof(System.ServiceModel.Channels.ServiceChannelProxy_P).TypeHandle,
					InterfaceType = typeof(Egee.Proxy.Sappel.ISappelService_P).TypeHandle,
					ImplementationClassType = typeof(ServiceChannelProxy_ISappelService).TypeHandle,
				}
		};
		static __DispatchProxyImplementationData()
		{
			global::System.Reflection.DispatchProxyHelpers.RegisterImplementations(s_entryTable);
		}
	}
}

namespace Egee.Proxy.Sappel
{
	[global::System.Runtime.InteropServices.McgRedirectedType("Egee.Proxy.Sappel.ISappelService, Egee.Proxy, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null, ContentType" +
		"=WindowsRuntime")]
	public interface ISappelService_P
	{
		System.Threading.Tasks.Task_P<InitResponse_P> InitAsync(InitRequest_P request);

		System.Threading.Tasks.Task_P<GetVersionResponse_P> GetVersionAsync(GetVersionRequest_P request);
	}
}

namespace System.ServiceModel.Channels
{
	[global::System.Runtime.InteropServices.McgRedirectedType("System.ServiceModel.Channels.ServiceChannelProxy, System.Private.ServiceModel, Version=4.1.0.0, Culture=neutral," +
		" PublicKeyToken=b03f5f7f11d50a3a")]
	public class ServiceChannelProxy_P : global::System.Reflection.DispatchProxy
	{
		protected override object Invoke(
					global::System.Reflection.MethodInfo targetMethodInfo, 
					object[] args)
		{
			return null;
		}
	}
}

namespace System.Reflection
{
	[global::System.Runtime.InteropServices.McgRedirectedType("System.Reflection.DispatchProxy, System.Private.Interop, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f" +
		"7f11d50a3a")]
	public class DispatchProxy_P
	{
	}
}

namespace Egee.Proxy.Sappel
{
	[global::System.Runtime.InteropServices.McgRedirectedType("Egee.Proxy.Sappel.InitResponse, Egee.Proxy, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null, ContentType=W" +
		"indowsRuntime")]
	public class InitResponse_P
	{
	}
}

namespace System.Threading.Tasks
{
	[global::System.Runtime.InteropServices.McgRedirectedType("System.Threading.Tasks.Task`1, System.Private.Threading, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f" +
		"7f11d50a3a")]
	public class Task_P<TResult>
	{
	}
}

namespace Egee.Proxy.Sappel
{
	[global::System.Runtime.InteropServices.McgRedirectedType("Egee.Proxy.Sappel.InitRequest, Egee.Proxy, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null, ContentType=Wi" +
		"ndowsRuntime")]
	public class InitRequest_P
	{
	}
}

namespace Egee.Proxy.Sappel
{
	[global::System.Runtime.InteropServices.McgRedirectedType("Egee.Proxy.Sappel.GetVersionResponse, Egee.Proxy, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null, Content" +
		"Type=WindowsRuntime")]
	public class GetVersionResponse_P
	{
	}
}

namespace Egee.Proxy.Sappel
{
	[global::System.Runtime.InteropServices.McgRedirectedType("Egee.Proxy.Sappel.GetVersionRequest, Egee.Proxy, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null, ContentT" +
		"ype=WindowsRuntime")]
	public class GetVersionRequest_P
	{
	}
}
