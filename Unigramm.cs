using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbrakadabraLib
{
    public class Unigramm:Ngramm
    {
        private Term lexicalBase;

        public Term LexicalBase { get { return lexicalBase; } }

        public Unigramm (Term lexBase): base()
        {
            lexicalBase = lexBase;
            lexBase.LexicalForms.Add(this);//обратная связь
        }
    }
}
