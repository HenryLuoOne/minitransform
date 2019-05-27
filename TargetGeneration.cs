using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace MiniTransform
{
    //This is a collection of FieldOperations
    //This defines FieldOperation for all the fields of the target format
    public class TargetGeneration
    {
        private List<KeyValuePair<String, FieldOperation>> targetGenerationDefinitions =
            new List<KeyValuePair<String, FieldOperation>>();

        //add FieldOperation for the target field
        public TargetGeneration Append(String targetFieldName, FieldOperation fieldOperation)
        {
            targetGenerationDefinitions.Add(new KeyValuePair<String, FieldOperation>(targetFieldName, fieldOperation));
            return this;
        }

        public void Reset()
        {
            targetGenerationDefinitions.Clear();
        }

        //get FieldOperation based on target filed name
        public FieldOperation GetOperationForTarget(String targetFieldName)
        {
            KeyValuePair<String, FieldOperation> targetGeneration = targetGenerationDefinitions.Find(x => x.Key.Equals(targetFieldName));
            return targetGeneration.Value;
        }

        //get all defined target fields' names
        public List<String> GetAllTargetNames()
        {
            return targetGenerationDefinitions.Select(keyValuePair => keyValuePair.Key).ToList();
        }

    }
}
