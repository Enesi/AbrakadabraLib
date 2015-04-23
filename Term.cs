using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbrakadabraLib
{
    public class Term:Ngramm
    {
        string content;

        public string Content { get { return content; } }

        List<Unigramm> lexicalForms;

        public List<Unigramm> LexicalForms { get { return lexicalForms; } set { lexicalForms = value; } }

        public Term() 
        {
            content = "";
            lexicalForms = new List<Unigramm>();
        }

        public Term(string cont)
        {
            content = cont;
            lexicalForms = new List<Unigramm>();
        }

        public override string ToString()
        {
            return content;
        }
    
    }
}
