using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniTransform
{
    //This defines the standard (target) output format
    public class StandardRow
    {
        private List<KeyValuePair<String, String>> targetContent =
            new List<KeyValuePair<String, String>>();

        private static List<String> allowedStandardFieldNames = new List<String>()
        {
            "AccountCode", "Name", "Type", "Open Date","Currency"
        };

        //no duplicate fields allowed - first one wins
        public StandardRow With(String targetFieldName, String targetFieldValue)
        {
            if (!Contains(targetFieldName))
            {
                targetContent.Add(new KeyValuePair<String, String>(targetFieldName, targetFieldValue));
            }

            return this;
        }

        //get field value based on field name
        public String GetFieldValue(String targetFieldName)
        {
            KeyValuePair<String, String> targetField = targetContent.Find(x => x.Key.Equals(targetFieldName));
            
            if (targetField.Key == null && targetField.Value == null)
            {
                throw new TransformException(String.Format(TransformException.FieldNotExistMessage, targetFieldName));
            }

            return targetField.Value;
        }

        //get all fields' values
        public List<String> GetAllFieldValues()
        {
            return targetContent.Select(keyValuePair => keyValuePair.Value).ToList();
        }

        //get all fields' names
        public static List<String> GetAllFieldNames()
        {
            return allowedStandardFieldNames;
        }

        //is a field defined by this standard format
        public static Boolean IsFieldAllowed(String fieldName)
        {
            return allowedStandardFieldNames.Contains(fieldName);
        }

        private Boolean Contains(String targetFieldName)
        {
            return targetContent.Select(keyValuePair => keyValuePair.Key).ToList().Contains(targetFieldName);
        }
    }
}
