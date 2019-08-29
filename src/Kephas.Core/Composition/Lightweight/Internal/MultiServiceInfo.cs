﻿namespace Kephas.Composition.Lightweight.Internal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Kephas.Composition;
    using Kephas.Services;
    using Kephas.Services.Reflection;

    internal class MultiServiceInfo : IServiceInfo, IEnumerable<IServiceInfo>
    {
        private IList<ServiceInfo> serviceInfos = new List<ServiceInfo>();

        public MultiServiceInfo(ServiceInfo serviceInfo)
        {
            this.ContractType = serviceInfo.ContractType;
            this.ServiceType = serviceInfo.ServiceType;
            this.serviceInfos.Add(serviceInfo);
        }

        AppServiceLifetime IAppServiceInfo.Lifetime => AppServiceLifetime.Transient;

        public bool AllowMultiple { get; } = true;

        bool IAppServiceInfo.AsOpenGeneric => false;

        Type[] IAppServiceInfo.MetadataAttributes => null;

        public Type ContractType { get; }

        object IAppServiceInfo.Instance => null;

        Type IAppServiceInfo.InstanceType => null;

        Func<ICompositionContext, object> IAppServiceInfo.InstanceFactory => null;

        public void Add(ServiceInfo serviceInfo)
        {
            this.serviceInfos.Add(serviceInfo);
        }

        public object GetService(IAmbientServices ambientServices)
        {
            throw new NotSupportedException("Only single service infos may provide services.");
        }

        public Type ServiceType { get; }

        public IDictionary<string, object> Metadata { get; }

        public IEnumerator<IServiceInfo> GetEnumerator() => this.serviceInfos.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}