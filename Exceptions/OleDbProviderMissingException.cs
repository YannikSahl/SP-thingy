using System;

namespace Exceptions
{
    public class OleDbProviderMissingException : Exception
    {

        public string Text { get; set; }

        public OleDbProviderMissingException(string message)
        {
            Text = message;
        }

        public override string Message => Text;

    }
}
