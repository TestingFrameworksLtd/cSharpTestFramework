using TechTalk.SpecFlow;

namespace UITests.Helpers
{
    public static class CurrentScenario<T> where T : class
    {
        internal static T Value
        {
            get
            {
                return ScenarioContext.Current.ContainsKey(typeof (T).FullName)
                    ? ScenarioContext.Current[typeof (T).FullName] as T
                    : null;
            }
            set { ScenarioContext.Current[typeof (T).FullName] = value; }
        }
    }
}