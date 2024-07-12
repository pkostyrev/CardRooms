
using System;

namespace CardRooms.DTS
{
    [Serializable]
    public struct RoomState
    {
        public bool started;
        public long progress;
        public Room room;

        public bool Completed => progress >= MaxProgress;
        public bool InProgress => started == true && Completed == false;
        public long MaxProgress => room.enemies.Length;

        public void addProgress(long progress)
        {
            this.progress += progress;
        }

        public void setProgress(long progress)
        {
            this.progress = progress;
        }

        public void Complete()
        {
            this.progress = MaxProgress;
        }
    }
}
