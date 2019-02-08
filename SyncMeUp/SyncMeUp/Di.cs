using Unity;
using Unity.Lifetime;

namespace SyncMeUp
{
    public static class Di
    {
        private static readonly UnityContainer Container = new UnityContainer();

        public static void RegisterInstance<T>(T instance) => Container.RegisterInstance(instance);

        public static void RegisterType<T>(bool singleton)
        {
            if (singleton)
            {
                Container.RegisterType<T>(new ContainerControlledLifetimeManager());
            }
            else
            {
                Container.RegisterType<T>();
            }
        }

        public static void RegisterType<TInterface, TImplementation>(bool singleton) where TImplementation : TInterface
        {
            if (singleton)
            {
                Container.RegisterType<TInterface, TImplementation>(new ContainerControlledLifetimeManager());
            }
            else
            {
                Container.RegisterType<TInterface, TImplementation>();
            }
        }

        public static T GetInstance<T>() => Container.Resolve<T>();
    }
}