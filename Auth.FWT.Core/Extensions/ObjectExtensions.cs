namespace Auth.FWT.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static bool IsNull<T>(this T that) where T : class
        {
            return that == null;
        }

        public static bool IsNotNull<T>(this T that) where T : class
        {
            return that != null;
        }
    }
}
