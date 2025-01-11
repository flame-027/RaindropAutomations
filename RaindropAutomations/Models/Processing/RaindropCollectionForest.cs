namespace RaindropAutomations.Models.Processing
{
    public class RaindropCollectionForest : ICollectionScope
    {
        public List<long> AllIds { get; set; } = [];

        public List<RaindropCollectionTreeNode> TopLevelNodes { get; set; } = [];
    }
}
