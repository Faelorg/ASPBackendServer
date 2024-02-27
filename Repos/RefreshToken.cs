using System;
using System.Collections.Generic;

namespace InterfaceServer.Repos;

public partial class RefreshToken
{
    public string UserId { get; set; } = null!;

    public string TokenId { get; set; } = null!;

    public string RefreshToken1 { get; set; } = null!;
}
