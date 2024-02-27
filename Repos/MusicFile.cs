using System;
using System.Collections.Generic;

namespace InterfaceServer.Repos;

public partial class MusicFile
{
    public string IdMusicFile { get; set; } = null!;

    public string? Title { get; set; }

    public string? Artist { get; set; }

    public string? Albim { get; set; }

    public int? Year { get; set; }
}
