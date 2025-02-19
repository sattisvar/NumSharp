﻿namespace NumSharp
{
    public class IncorrectTypeException : System.Exception
    {
        public IncorrectTypeException() : base("This method does not work with this dtype or was not already implemented.") { }
        public IncorrectTypeException(string message) : base(message) { }
    }
}
