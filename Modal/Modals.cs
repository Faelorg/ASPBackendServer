using InterfaceServer.Repos;

namespace InterfaceServer.Modal
{
    public class UserModal
    {
        public string? Login { get; set; }

        public string? Password { get; set; }

        public string? Role { get; set; }
    }

    public class PlaylistModal
    {
        public string IdPlaylist { get; set; } = null!;

        public string? Name { get; set; }
    }
}
