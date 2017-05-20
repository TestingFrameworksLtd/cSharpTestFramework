using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace UITests.Helpers
{
    public static class SetComparisonExtensionMethods
    {
        public static void CompareToSet<T>(this Table table, IEnumerable<T> set, Dictionary<string, Type> jsonColumns)
        {
            List<T> setList = set.ToList();

            if (jsonColumns != null)
            {
                CompareJsonColumns(table, setList, jsonColumns);
            }

            TechTalk.SpecFlow.Assist.SetComparisonExtensionMethods.CompareToSet(table, setList);
        }

        public static void CompareToSet<T>(this Table table, IEnumerable<T> set, List<string> jsonColumns = null)
        {
            List<T> setList = set.ToList();

            if (jsonColumns != null)
            {
                CompareJsonColumns(table, setList, jsonColumns);
            }

            TechTalk.SpecFlow.Assist.SetComparisonExtensionMethods.CompareToSet(table, setList);
        }


        private static void CompareJsonColumns<T>(Table table, List<T> setList, List<string> JsonColumns)
        {
            Dictionary<string, Type> dict = JsonColumns.ToDictionary(x => x, x => typeof (JObject));
            CompareJsonColumns(table, setList, dict);
        }

        private static void CompareJsonColumns<T>(Table table, List<T> setList, Dictionary<string, Type> jsonColumns)
        {
            //var setComparer = new SetComparer<T>(table); 

            Dictionary<string, string> sourcePropertyName = table.Header.SelectMany(columnHeader =>
                typeof (T).GetProperties()
                    .Where(property => IsPropertyMatchingToColumnName(property, columnHeader))
                    .Select(property => new {property.Name, columnHeader})
                )
                .ToDictionary(x => x.columnHeader, x => x.Name);

            foreach (TableRow row in table.Rows)
            {
                foreach (var col in jsonColumns)
                {
                    string firstCol = table.Header.First();
                    bool colExists = table.ContainsColumn(col.Key);
                    bool propExists = sourcePropertyName.ContainsKey(col.Key);
                    if (colExists && propExists)
                    {
                        PropertyInfo idProperty = typeof (T).GetProperty(sourcePropertyName[firstCol]);
                        PropertyInfo colProperty = typeof (T).GetProperty(sourcePropertyName[col.Key]);
                        T setObj = default(T);
                        try
                        {
                            setObj = setList.FirstOrDefault(x => idProperty.GetValue(x).ToString() == row[firstCol]);
                        }
                        catch (Exception)
                        {
                        }

                        if (setObj == null)
                        {
                            throw new NullReferenceException(
                                string.Format("[FAILED] setObj is null. Trying to compare value of '{0}' with '{1}'"
                                    , firstCol, col.Key));
                        }

                        row[col.Key] = CleanJson(row[col.Key]);

                        colProperty.SetValue(setObj, CleanJson((string) colProperty.GetValue(setObj)));

                        AssertJsonIsEqual(row[col.Key], (string) colProperty.GetValue(setObj), col.Value);

                        // json matches so set the table value = obj value
                        row[col.Key] = (string) colProperty.GetValue(setObj);
                    }
                }
            }
        }

        public static void AssertJsonIsEqual(string jsonExpected, string jsonActual, Type objType = null)
        {
            List<string> differences;
            bool result = CompareJson(jsonExpected, jsonActual, out differences, objType);

            Assert.True(result);
        }

        private static string CleanJson(string json)
        {
            object obj = JsonConvert.DeserializeObject(json);
            string str = JsonConvert.SerializeObject(obj);
            return str;
        }


        public static bool CompareJson(string expected, string actual, out List<string> differences, Type objType = null)
        {
            differences = new List<string>();

            var actualObj = JsonConvert.DeserializeObject<JObject>(actual);
            var expectedObj = JsonConvert.DeserializeObject<JObject>(expected);

            if (!JToken.DeepEquals(expectedObj, actualObj))
            {
                if (objType == null)
                {
                    var actualDic = JsonConvert.DeserializeObject<Dictionary<string, object>>(actual);
                    Dictionary<string, string> actualDicStr = actualDic.ToDictionary(x => x.Key,
                        x => JsonConvert.SerializeObject(x.Value));

                    var expectedDic = JsonConvert.DeserializeObject<Dictionary<string, object>>(expected);
                    Dictionary<string, string> expectedDicStr = expectedDic.ToDictionary(x => x.Key,
                        x => JsonConvert.SerializeObject(x.Value));

                    AssertEx.PropertyValuesAreEquals(expectedDicStr, actualDicStr);
                }
                else
                {
                    object actualTyped = JsonConvert.DeserializeObject(actual, objType);
                    object expectedTyped = JsonConvert.DeserializeObject(expected, objType);

                    AssertEx.PropertyValuesAreEquals(expectedTyped, actualTyped);
                }
            }

            return true;
        }

        #region TEHelpers

        internal static bool IsPropertyMatchingToColumnName(PropertyInfo property, string columnName)
        {
            return MatchesThisColumnName(property.Name, columnName);
        }

        internal static bool MatchesThisColumnName(this string propertyName, string columnName)
        {
            return propertyName.Equals(columnName.Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }

    [TestFixture]
    public class SetComparisonExtensionMethodsTests
    {
        [TestCase("{\"age_max\":65,\"age_min\":18,\"geo_locations\":{\"countries\":[\"GB\"]}}",
            "{\"age_max\":65,\"age_min\":18,\"geo_locations\":{\"countries\":[\"GB\"]}}", null, true)]
        [TestCase("{\"age_min\":18,\"age_max\":65,\"geo_locations\":{\"countries\":[\"GB\"]}}",
            "{\"age_max\":65,\"age_min\":18,\"geo_locations\":{\"countries\":[\"GB\"]}}", null, true)]
        //[TestCase("{\"age_min\":18,\"age_max\":65,\"geo_locations\":{\"countries\":[\"GB\"]}}", "{\"age_max\":65,\"age_min\":18,\"geo_locations\":{\"countries\":[\"GB\"]}}", typeof(ads_targeting_graph), true)]
        //[TestCase("{\"age_max\":65,\"age_min\":18,\"geo_locations\":{\"countries\":[\"GB\",\"USA\"]}}", "{\"age_max\":65,\"age_min\":18,\"geo_locations\":{\"countries\":[\"GB\",\"USA\"]}}", null, true)]
        //[TestCase("{\"age_max\":65,\"age_min\":18,\"geo_locations\":{\"countries\":[\"USA\",\"GB\"]}}", "{\"age_max\":65,\"age_min\":18,\"geo_locations\":{\"countries\":[\"GB\",\"USA\"]}}", typeof(ads_targeting_graph), true)]

        //[TestCase("{\"age_min\":18,\"age_max\":64,\"geo_locations\":{\"countries\":[\"GB\"]}}", "{\"age_max\":65,\"age_min\":18,\"geo_locations\":{\"countries\":[\"GB\"]}}", null, false)]
        //[TestCase("{\"age_max\":65,\"age_min\":18,\"geo_locations\":{\"countries\":[\"USA\",\"GB\"]}}", "{\"age_max\":65,\"age_min\":18,\"geo_locations\":{\"countries\":[\"GB\",\"USA\", \"UK\"]}}", null, false)]
        public void AssertJsonIsEqual_Test(string json1, string json2, Type objType, bool expectedToPass)
        {
            List<string> differences;
            bool result = SetComparisonExtensionMethods.CompareJson(json1, json2, out differences, objType);

            Assert.AreEqual(expectedToPass, result);
            Assert.AreEqual(!expectedToPass, differences.Any());
        }
    }
}