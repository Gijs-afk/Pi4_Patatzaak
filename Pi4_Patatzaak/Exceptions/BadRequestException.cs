using Microsoft.VisualBasic;

namespace Pi4_Patatzaak.Exceptions;

    public class BadRequestException : Exception
    {
        public BadRequestException(string msg) : base(msg)
        { }

    }

