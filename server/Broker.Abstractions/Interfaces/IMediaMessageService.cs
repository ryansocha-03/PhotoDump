namespace Broker.Abstractions.Interfaces;

/// <summary>
/// Service interface defining functions relating to media messages.
/// </summary>
public interface IMediaMessageService
{
    /// <summary>
    /// Publishes a message with the necessary information to indicate a particular media has been uploaded. 
    /// </summary>
    /// <param name="mediaPublicName">The public file name of the media uploaded.</param>
    /// <param name="mediaInternalId">The internal identifier of the media uploaded.</param>
    /// <returns></returns>
    Task PublishMediaUploadMessageAsync(string mediaPublicName, long mediaInternalId);
}