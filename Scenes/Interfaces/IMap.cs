using Godot.Collections;

namespace bit_shuter.Scenes.Interfaces
{
    public interface IMap
    {
        void Setup(Dictionary<string, object> mapState);

        void ProcessStates(float delta);
        void Destroy();
    }
}