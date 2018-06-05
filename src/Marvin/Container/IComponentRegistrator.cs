using System;

namespace Marvin.Container
{
    /// <summary>
    /// Responensible for component registration based on information given in <see cref="RegistrationAttribute"/> 
    /// </summary>
    public interface IComponentRegistrator
    {
        /// <summary>
        /// Check if a type shall be registered
        /// </summary>
        /// <param name="foundType"></param>
        /// <returns></returns>
        bool ShallInstall(Type foundType);

        /// <summary>
        /// Register a type in the container. Automatically registers interfaces as factory
        /// </summary>
        /// <param name="type">Type to register</param>
        void Register(Type type);

        /// <summary>
        /// Register a type for a couple of services
        /// </summary>
        /// <param name="type"></param>
        /// <param name="services"></param>
        void Register(Type type, Type[] services);

        /// <summary>
        /// Register a type for services under a given name
        /// </summary>
        void Register(Type type, Type[] services, string name);

        /// <summary>
        /// Full registration method
        /// </summary>
        void Register(Type type, Type[] services, string name, LifeCycle lifeCycle);

        /// <summary>
        /// Register a factory
        /// </summary>
        /// <param name="factoryInterface"></param>
        void RegisterFactory(Type factoryInterface);

        /// <summary>
        /// Register a factory under a special name
        /// </summary>
        void RegisterFactory(Type factoryInterface, string name);

        /// <summary>
        /// Register a factory under a special name
        /// </summary>
        void RegisterFactory(Type factoryInterface, string name, Type selector);
    }
}