using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaindropAutomations.Models.Processing
{
    public class RaindropCollectionTree
    {
        public List<RaindropCollectionTreeNode> TopNodes { get; set; } = new();

        public List<long> AllIdsWithinTree { get; set; } = new();
    }
}
