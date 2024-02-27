using System;
using System.Collections.Generic;

namespace InterfaceServer.Repos;

public partial class User
{
    public string IdUser { get; set; } = null!;

    public string? Login { get; set; }

    public string? Password { get; set; }

    public int RoleIdRole { get; set; }

    public virtual ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();

    public virtual Role RoleIdRoleNavigation { get; set; } = null!;
}
