using Godot.Collections;

namespace bit_shuter.Autoload.Structs
{
    public struct Credentials {
        public string ClientName {get; set;}

        public Dictionary<string, object> ToGodotDict() {
            return new Dictionary<string, object>(){
                {"ClientName", ClientName}
            };
        }
    }
}