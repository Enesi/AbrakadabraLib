using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;

namespace AbrakadabraLib
{

    public class NgrammsGraph : BidirectionalGraph<Ngramm, Relation>
    {
        public NgrammsGraph() { }

        public NgrammsGraph(bool allowParallelEdges)
            : base(allowParallelEdges) { }

        public NgrammsGraph(bool allowParallelEdges, int vertexCapacity)
            : base(allowParallelEdges, vertexCapacity) { }
    }
}
