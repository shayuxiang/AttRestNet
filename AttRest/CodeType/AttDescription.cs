using System;
using System.Collections.Generic;
using System.Text;

namespace AttRest.CodeType
{
    [AttributeUsage(AttributeTargets.All)]
    public class AttDescription : Attribute
    {
        private string _text;

        public AttDescription(string txt)
        {
            this._text = txt;
        }

        public override string ToString()
        {
            return this._text;
        }
    }
}
