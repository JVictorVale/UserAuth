using System.ComponentModel;

namespace UserAuth.Core.Enums;

public enum EPathAccess
{
    [Description("assets/public")]
    Public,
    [Description("assets/private")]
    Private
}