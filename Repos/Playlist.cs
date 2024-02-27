using System;
using System.Collections.Generic;

namespace InterfaceServer.Repos;

public partial class Playlist
{
    public string IdPlaylist { get; set; } = null!;

    public string? Name { get; set; }

    public string UserIdUser { get; set; } = null!;

    public virtual User UserIdUserNavigation { get; set; } = null!;
}
