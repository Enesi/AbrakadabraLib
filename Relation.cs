using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;

namespace AbrakadabraLib
{
    public class Relation : Edge<Ngramm>
    {
        public Relation(Ngramm source, Ngramm target)
            : base(source, target) {}

    }
}
