using Godot.Collections;

namespace bit_shuter.Scenes.Interfaces
{
    public interface ISynchronized
    {
        void ProcessState(Dictionary state);
        void Build(Dictionary buildData);
    }
}