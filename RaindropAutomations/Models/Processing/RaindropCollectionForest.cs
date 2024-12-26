using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaindropAutomations.Models.Processing
{
    public class RaindropCollectionForest
    {
        public List<long> AllIdsWithinForest { get; set; } = [];

        public List<RaindropCollectionTreeNode> TopLevelNodes { get; set; } = [];
    }
}
