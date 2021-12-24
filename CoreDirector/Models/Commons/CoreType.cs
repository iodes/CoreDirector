using System.ComponentModel;

namespace CoreDirector.Models
{
    internal enum CoreType
    {
        [Description("기본값")]
        Default = 0,

        [Description("성능 코어")]
        Performance = 1,

        [Description("효율 코어")]
        Efficient = 2
    }
}
