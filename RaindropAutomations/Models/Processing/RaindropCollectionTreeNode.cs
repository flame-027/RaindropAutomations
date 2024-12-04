namespace RaindropAutomations.Models.Processing
{
    public class RaindropCollectionTreeNode
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public List<RaindropCollectionTreeNode> Children { get; set; } = new();
    }
}
