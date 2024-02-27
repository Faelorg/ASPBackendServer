using System;
using System.Collections.Generic;

namespace InterfaceServer.Repos;

public partial class PlaylistHasMusicFile
{
    public string PlaylistIdPlaylist { get; set; } = null!;

    public string MusicFileIdMusicFile { get; set; } = null!;

    public virtual MusicFile MusicFileIdMusicFileNavigation { get; set; } = null!;

    public virtual Playlist PlaylistIdPlaylistNavigation { get; set; } = null!;
}
