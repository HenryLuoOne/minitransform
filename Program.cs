using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniTransform
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                OperationExecutor operationExecutor = new OperationExecutor();

                TargetGeneration targetGeneration1 = CreateTargetGenerationForFileFormat1();
                operationExecutor.Validate(targetGeneration1);
                List<StandardRow> result = operationExecutor.GenerateStandardRows(GetAccountFileData1(), true, targetGeneration1);
                Show(result);

                TargetGeneration targetGeneration2 = CreateTargetGenerationForFileFormat2();
                operationExecutor.Validate(targetGeneration2);
                List<StandardRow> result2 = new OperationExecutor().GenerateStandardRows(GetAccountFileData2(), false, targetGeneration2);
                Show(result2);
            }
            catch (TransformException e)
            {
                Console.WriteLine(Environment.NewLine + e);
            }
        }

        //this is supposed to be read from csv file - with header row
        private static List<String[]> GetAccountFileData1()
        {

            return new List<String[]>()
            {
                new String[] {"Identifier", "Name", "Type", "Opened", "Currency" },
                new String[] {"123|AbcCode", "My Account", "2", "01-01-2018", "CD" }
            };
        }

        //this is supposed to be read from csv file - without header row
        private static List<String[]> GetAccountFileData2()
        {
            return new List<String[]>()
            {
                new String[] {"My Account", "RRSP", "C", "CustodianCodeNotSpecified"}
            };
        }

        //for each source file, we define the transform relationship between target(standard) and source(various) data.
        private static TargetGeneration CreateTargetGenerationForFileFormat1()
        {
            Dictionary<String, String> AccountTypeDictionary = new Dictionary<String, String>()
            {
                { "1", "Trading" },
                { "2", "RRSP" },
                { "3", "RESP" },
                { "4", "Fund"}
            };

            Dictionary<String, String> CurencyDictionary = new Dictionary<String, String>()
            {
                { "CD", "CAD" },
                { "US", "USD" }
            };

            return new TargetGeneration()
                .Append("AccountCode", new SplitOperation(0, '|', 1, OperationType.Split))
                .Append("Name", new PassThroughOperation(1, OperationType.PassThrough))
                .Append("Type", new MapOperation(2, AccountTypeDictionary, OperationType.DirectMap))
                .Append("Open Date", new ParseDateOperation(3, "dd-MM-yyyy", OperationType.ParseDate))
                .Append("Currency", new MapOperation(4, CurencyDictionary, OperationType.DirectMap));
        }

        //for each source file, we define the transform relationship between target(standard) and source(various) data.
        private static TargetGeneration CreateTargetGenerationForFileFormat2()
        {
            Dictionary<String, String> CurencyDictionary = new Dictionary<String, String>()
            {
                { "C", "CAD" },
                { "U", "USD" }
            };

            return new TargetGeneration()
                .Append("AccountCode", new PassThroughOperation(3, OperationType.PassThrough))
                .Append("Name", new PassThroughOperation(0, OperationType.PassThrough))
                .Append("Type", new PassThroughOperation(1, OperationType.PassThrough))
                .Append("Open Date", new FillConstantOperation(null, OperationType.FillConstant))
                .Append("Currency", new MapOperation(2, CurencyDictionary, OperationType.DirectMap));
        }

        private static void Show(List<StandardRow> standardData)
        {
            Console.WriteLine(Environment.NewLine + String.Join(",", StandardRow.GetAllFieldNames()));
            standardData.ForEach((standardRow) => Console.WriteLine(String.Join(",", standardRow.GetAllFieldValues())));
        }
    }
}
