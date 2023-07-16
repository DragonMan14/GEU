namespace Pathfinding
{
    public interface IPathfindingRestrictor
    {
        public bool IsValidPathAvailable(Edge edge);
    }
}