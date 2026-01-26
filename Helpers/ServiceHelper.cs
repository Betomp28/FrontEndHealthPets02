namespace FrontEndHealthPets.Helpers
{
    public static class ServiceHelper
    {
        public static IServiceProvider? Services { get; private set; }

        public static void Initialize(IServiceProvider services) => Services = services;

        public static T? GetService<T>() where T : class
        {
            return Services?.GetService(typeof(T)) as T;
        }
    }
}
