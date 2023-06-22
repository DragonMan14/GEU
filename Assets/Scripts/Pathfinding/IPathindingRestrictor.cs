namespace Pathfinding
{
    public interface IPathfindingRestrictor
    {
        public bool IsValidPathAvailable(Node current, Node next);
    }
}