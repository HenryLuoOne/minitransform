using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniTransform
{
    //Execute FieldOperation to get each standard field by operating on columns from source data
    //The transformation between target file and source file is defined by TargetGeneration
    public class OperationExecutor
    {
        //iterate on all source row
        public List<StandardRow> GenerateStandardRows(List<String[]> sourceDataRows, Boolean dataContainHeader, TargetGeneration targetGeneration)
        {
            List<StandardRow> allStandardRows = new List<StandardRow>();

            for (int i = dataContainHeader ? 1 : 0; i < sourceDataRows.Count; i++)
            {
                StandardRow standardRow = ExecuteOperationsForEachRow(sourceDataRows[i], targetGeneration);
                allStandardRows.Add(standardRow);
            }

            return allStandardRows;
        }

        //validate the transformation definition
        public void Validate(TargetGeneration targetGeneration)
        {
            foreach(String targetFieldName in targetGeneration.GetAllTargetNames())
            {
                if (!StandardRow.IsFieldAllowed(targetFieldName))
                {
                    throw new TransformException(String.Format(TransformException.InvalidFieldMessage, targetFieldName));
                }
            }

            foreach (String standardFieldName in StandardRow.GetAllFieldNames())
            {
                if (!targetGeneration.GetAllTargetNames().Contains(standardFieldName))
                {
                    throw new TransformException(String.Format(TransformException.MissingFieldInStandardMessage, standardFieldName));
                }
            }

            if(!targetGeneration.GetAllTargetNames().SequenceEqual(StandardRow.GetAllFieldNames()))
            {
                throw new TransformException(TransformException.WrongFieldOrderMessage);
            }
        }

        //Execute FieldOperation field by field
        private StandardRow ExecuteOperationsForEachRow(String[] sourceDataRow, TargetGeneration targetGeneration)
        {
            StandardRow stadardRow = new StandardRow();

            List<String> targetFields = targetGeneration.GetAllTargetNames();

            foreach (String targetField in targetFields)
            {
                FieldOperation fieldOperation  = targetGeneration.GetOperationForTarget(targetField);
                stadardRow.With(targetField, fieldOperation.GetResult(sourceDataRow));
            }

            return stadardRow;
        }
    }
}
