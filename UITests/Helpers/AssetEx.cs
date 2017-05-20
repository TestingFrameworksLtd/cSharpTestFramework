using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;

namespace UITests.Helpers
{
    public static class AssertEx
    {
        public static void PropertyValuesAreEquals(object expected, object actual)
        {
            string errorMessages = "";

            PropertyValuesAreEquals(expected, actual, ref errorMessages);
        }

        public static void PropertyValuesAreEquals(object expected, object actual, ref string errorMessages,
            string propertyName = "")
        {
            if (expected != null)
            {
                PropertyInfo[] properties = expected.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public);

                foreach (PropertyInfo property in properties)
                {
                    object expectedValue;
                    object actualValue;
                    propertyName += "." + property.Name;

                    //                    if (expected is Newtonsoft.Json.Linq.JObject && propertyName == "Type")
                    //                    {
                    //                        continue;
                    //                    }

                    try
                    {
                        expectedValue = property.GetValue(expected, null);
                        actualValue = property.GetValue(actual, null);
                    }
                    catch (Exception ex)
                    {
                        //Probably an actual value and not a method
                        expectedValue = expected;
                        actualValue = actual;
                    }

                    if (actualValue is Array)
                    {
                        errorMessages += AssertArraysAreEquals(property, (object[]) expectedValue,
                            (object[]) actualValue);
                    }
                    else if (actualValue is IList)
                    {
                        errorMessages += AssertListsAreEquals(property, (IList) expectedValue, (IList) actualValue,
                            propertyName);
                    }

                    else if (actualValue is IDictionary)
                    {
                        AssertDictionaryAreEquals(property, (IDictionary) expectedValue, (IDictionary) actualValue,
                            ref errorMessages);
                    }
                    else if (IsKnownType(actualValue))
                    {
                        if (!Equals(expectedValue, actualValue))
                        {
                            errorMessages +=
                                string.Format(
                                    "\r\nProperty {0}.{1} does not match ({4}). [Expected] '{2}' [Recieved] '{3}'",
                                    property.DeclaringType.Name, property.Name, expectedValue, actualValue, propertyName);
                        }
                    }
                    else
                    {
                        PropertyValuesAreEquals(expectedValue, actualValue, ref errorMessages);
                    }
                }
            }
            else
            {
                if (!Equals(expected, actual))
                {
                    Assert.Fail("[FAILED] Property does not match ({2}). [Expected] '{0}' [Recieved] '{1}'", expected,
                        actual, propertyName);
                }
            }

            if (errorMessages != "")
            {
                Assert.Fail("[FAILED] " + errorMessages);
            }
        }

        private static string AssertArraysAreEquals(PropertyInfo property, object[] expectedArray, object[] actualArray)
        {
            string errorMessages = "";

            if (expectedArray.Length != actualArray.Length)
            {
                Assert.Fail(
                    "[FAILED] Property {0}.{1} does not match number of elements. [Expected] '{2}' [Recieved] '{3}'",
                    property.PropertyType.Name, property.Name, expectedArray.Length, actualArray.Length);
            }

            for (int i = 0; i < actualArray.Length; i++)
            {
                if (IsKnownType(actualArray[i]))
                {
                    if (!Equals(expectedArray[i], actualArray[i]))
                    {
                        errorMessages +=
                            string.Format("\r\nProperty {0}.{1} does not match. [Expected] '{2}' [Recieved] '{3}'",
                                property.DeclaringType.Name, property.Name, expectedArray[i], actualArray[i]);
                    }
                }
                else
                {
                    PropertyValuesAreEquals(expectedArray[i], actualArray[i]);
                }
            }

            return errorMessages;
        }

        private static string AssertListsAreEquals(PropertyInfo property, IList expectedList, IList actualList,
            string propertyName)
        {
            string errorMessages = "";

            if (actualList.Count != expectedList.Count)
            {
                Assert.Fail(
                    "[FAILED] Property {0}.{1} does not match number of elements. [Expected] '{2}' [Contained] '{3}'",
                    property.PropertyType.Name, property.Name, expectedList.Count, actualList.Count);
            }

            for (int i = 0; i < actualList.Count; i++)
            {
                if (IsKnownType(actualList[i]))
                {
                    if (!Equals(expectedList[i], actualList[i]))
                    {
                        errorMessages +=
                            string.Format("\r\nProperty does not match({2}<{3}>). [Expected] '{0}' [Recieved] '{1}'",
                                expectedList[i], actualList[i], propertyName, property.PropertyType.Name);
                    }
                }
                else
                {
                    PropertyValuesAreEquals(expectedList[i], actualList[i]);
                }
            }

            return errorMessages;
        }

        private static void AssertDictionaryAreEquals(PropertyInfo property, IDictionary expectedList,
            IDictionary actualList, ref string errorMessages)
        {
            if (actualList.Count != expectedList.Count)
            {
                Assert.Fail(
                    "[FAILED] Property {0}.{1} does not match number of elements. [Expected] '{2}' [Contained] '{3}'",
                    property.PropertyType.Name, property.Name, expectedList.Count, actualList.Count);
            }

            foreach (object key in actualList.Keys)
            {
                PropertyValuesAreEquals(expectedList[key], actualList[key], ref errorMessages);
            }
        }

        public static void AssertDictionaryAreEquals(IDictionary expectedList, IDictionary actualList)
        {
            if (actualList.Count != expectedList.Count)
                Assert.Fail("[FAILED] Dictionarys are not the same length");

            foreach (object key in actualList.Keys)
            {
                PropertyValuesAreEquals(expectedList[key], actualList[key]);
            }
        }

        public static void AssertListsAreEquals(IList expectedList, IList actualList)
        {
            if (actualList.Count != expectedList.Count)
                Assert.Fail("[FAILED] Lists are not the same length");

            for (int i = 0; i < actualList.Count; i++)
                PropertyValuesAreEquals(expectedList[i], actualList[i]);
        }

        private static bool IsKnownType(object actual)
        {
            if (actual == null)
            {
                return true;
            }
            if (actual.GetType().IsValueType)
            {
                return true;
            }

            return actual is string || actual is DateTime || actual is Enum;
        }
    }
}