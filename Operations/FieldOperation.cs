using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MiniTransform
{
    //This defines how one field of the target (standard format) is derived
    //from one column (could be extended to multiple columns) from the source

    //This is the summary of the whole program to do the data transform
    public enum OperationType
    {
        PassThrough, //no-op: target data is the same as source data without change
        DirectMap, //map: target data is derived from map, where source data is the key
        Split, //sub field: target data is part of the source data
        ParseDate, //date format: target data is a date, parsed from source data using date/time pattern
        FillConstant //constant: target data is a constant (has nothing to do with source data)
    };

    //This defines how source data is transformed (operated) to target data
    public abstract class FieldOperation
    {
        public const String DefaultValueForInvalidIndex = "<Null>";
        public const String DefaultValueForInvalidSubIndex = "<N/A>";
        public const String DefaultValueForUndefined = "<Undefined>";
        public const String DefaultValueForInvalidDate = null;
        public const String StandardDatePattern = "yyyy-MM-dd";

        protected OperationType operationType;
        public FieldOperation(OperationType operationType)
        {
            this.operationType = operationType;
        }

        protected virtual Boolean Validate(int sourceColumnIndex, String[] sourceData)
        {
            return (sourceColumnIndex >= 0 && sourceColumnIndex < sourceData.Length);
        }

        public abstract String GetResult(String[] sourceData);
    }

    //implementing PassThrough FieldOperation
    public class PassThroughOperation : FieldOperation
    {
        private int sourceColumnIndex;

        public PassThroughOperation(int sourceColumnIndex, OperationType operationType)
            : base(operationType)
        {
            this.sourceColumnIndex = sourceColumnIndex;
        }

        public override String GetResult(String[] sourceData)
        {
            return Validate(sourceColumnIndex, sourceData) ?
                sourceData[sourceColumnIndex] : DefaultValueForInvalidIndex;
        }
    }

    //implementing DirectMap FieldOperation
    public class MapOperation : FieldOperation
    {
        private int sourceColumnIndex;
        private Dictionary<String, String> sourceToTarget;

        public MapOperation(int sourceColumnIndex, Dictionary<String, String> sourceToTarget, OperationType operationType)
            : base(operationType)
        {
            this.sourceColumnIndex = sourceColumnIndex;
            this.sourceToTarget = sourceToTarget;
        }

        public override String GetResult(String[] sourceData)
        {
            Boolean valid = Validate(sourceColumnIndex, sourceData);

            if (valid)
            {
                String rawData = sourceData[sourceColumnIndex];
                if ((rawData != null) && sourceToTarget.ContainsKey(rawData))
                {
                    return sourceToTarget[rawData];
                }
                else
                {
                    return DefaultValueForUndefined;
                }
            }
            else
            {
                return DefaultValueForInvalidIndex;
            }
        }

    }

    //implementing Split FieldOperation
    public class SplitOperation : FieldOperation
    {
        private int sourceColumnIndex;
        private Char splitter;
        private int subFieldIndex;

        public SplitOperation(int sourceColumnIndex, Char splitter, int subFieldIndex, OperationType operationType)
            : base(operationType)
        {
            this.sourceColumnIndex = sourceColumnIndex;
            this.splitter = splitter;
            this.subFieldIndex = subFieldIndex;
        }

        public override String GetResult(String[] sourceData)
        {
            Boolean valid = Validate(sourceColumnIndex, sourceData);

            if (valid)
            {
                String rawData = sourceData[sourceColumnIndex];
                if (rawData != null)
                {
                    String[] subFields = rawData.Split(new Char[] { splitter });
                    return Validate(subFieldIndex, subFields) ?
                        subFields[subFieldIndex] : DefaultValueForInvalidSubIndex;
                }
                else
                {
                    return DefaultValueForUndefined;
                }
            }
            else
            {
                return DefaultValueForInvalidIndex;
            }
        }
    }

    //implementing ParseDate FieldOperation
    public class ParseDateOperation : FieldOperation
    {
        private int sourceColumnIndex;
        private String datePattern;

        public ParseDateOperation(int sourceColumnIndex, String datePattern, OperationType operationType)
            : base(operationType)
        {
            this.sourceColumnIndex = sourceColumnIndex;
            this.datePattern = datePattern;
        }

        public override string GetResult(string[] sourceData)
        {
            Boolean valid = Validate(sourceColumnIndex, sourceData);

            if (valid)
            {
                String rawData = sourceData[sourceColumnIndex];
                DateTimeFormatInfo dateTimeFormatInfo = DateTimeFormatInfo.InvariantInfo;
                DateTimeFormatInfo customizedDateTimeFormatInfo = (DateTimeFormatInfo)dateTimeFormatInfo.Clone();
                customizedDateTimeFormatInfo.ShortDatePattern = datePattern;
                DateTime dateTime;
                Boolean isParseOk =  DateTime.TryParse(rawData, customizedDateTimeFormatInfo, DateTimeStyles.None, out dateTime);
                if (isParseOk)
                {
                    return dateTime.ToString(StandardDatePattern);
                }
                else
                {
                    return DefaultValueForInvalidDate;
                }
            }
            else
            {
                return DefaultValueForInvalidIndex;
            }
        }
    }

    //implementing FillConstant FieldOperation
    public class FillConstantOperation : FieldOperation
    {
        private String constantContent;
        public FillConstantOperation(String constantContent, OperationType operationType)
            : base(operationType)
        {
            this.constantContent = constantContent;
        }

        public override String GetResult(String[] sourceData)
        {
            return constantContent;
        }
    }
}
