using RMU.Wall.DeadWall;

namespace RMU.Wall
{
    public abstract class WallObject
    {
        protected Wall _wall;
        protected IDeadWall _deadWall;

        public virtual Wall GetWall()
        {
            return _wall;
        }

        public virtual IDeadWall GetDeadWall()
        {
            return _deadWall;
        }
        
        public abstract void GenerateDeadWall();
    }
}
