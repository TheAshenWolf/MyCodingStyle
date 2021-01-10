#if !NOT_UNITY3D

using ModestTree;
using System;
using Object = UnityEngine.Object;

namespace Zenject
{
    [NoReflectionBaking]
    public class PrefabProviderCustom : IPrefabProvider
    {
        readonly Func<InjectContext, UnityEngine.Object> _getter;

        public PrefabProviderCustom(Func<InjectContext, UnityEngine.Object> getter)
        {
            _getter = getter;
        }

        public UnityEngine.Object GetPrefab(InjectContext context)
        {
            Object prefab = _getter(context);
            Assert.That(prefab != null, "Custom prefab provider returned null");
            return prefab;
        }
    }
}

#endif

