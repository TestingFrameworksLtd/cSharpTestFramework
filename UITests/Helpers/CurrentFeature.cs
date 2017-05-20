using TechTalk.SpecFlow;

namespace UITests.Helpers
{
    public static class CurrentFeature<T> where T : class
    {
        internal static T Value
        {
            get
            {
                return FeatureContext.Current.ContainsKey(typeof (T).FullName)
                    ? FeatureContext.Current[typeof (T).FullName] as T
                    : null;
            }
            set { FeatureContext.Current[typeof (T).FullName] = value; }
        }
    }
}