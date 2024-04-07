using Google.Protobuf;

namespace ProjectionSccen.Core.Summer.Messages;

public class ClientMessageData
{
    /// <summary>
    /// 
    /// </summary>
    public  Connection connetion { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public IMessage message { get; set; }
}