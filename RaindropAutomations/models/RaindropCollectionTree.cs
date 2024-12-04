using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaindropAutomations.Models
{
    public class RaindropCollectionTree
    {
        public List<RaindropCollectionTreeNode> TopNodes { get; set; } = new();

        public List<long> AllIdsWithinTree { get; set; } = new();
    }

    public class RaindropCollectionTreeNode
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public List<RaindropCollectionTreeNode> Children { get; set; } = new();
    }
}
