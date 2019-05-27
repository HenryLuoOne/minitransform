using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniTransform
{
    public class TransformException : Exception
    {
        public const String InvalidFieldMessage = "Field[{0}] is invalid(not defined in the standard)!";
        public const String MissingFieldInStandardMessage = "Missing field [{0}] for standard!";
        public const String WrongFieldOrderMessage = "Field order is not aligned to standard!";
        public const String FieldNotExistMessage = "Field [{0}] does not exist!";
        public TransformException(String message)
            : base(message)
        {
        }
    }
}
