using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaindropAutomations.models
{
    public class RaindropTree
    {
        public List<RaindropTreeNode> TopNodes { get; set; } = new();

        public List<long> AllIdsWithinTree { get; set; } = new();
    }

    public class RaindropTreeNode
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public List<RaindropTreeNode> Children { get; set; } = new();
    }
}
