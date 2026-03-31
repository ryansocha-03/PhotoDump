using System.Text.Json;
using App.Api.Models;
using Microsoft.AspNetCore.DataProtection;

namespace App.Api.Services;

public class MediaCursorService(IDataProtectionProvider protector)
{
    private readonly IDataProtector  _protector = protector.CreateProtector("media-download-protector");
    
    public string EncodeCursor(MediaCursorPayload cursorPayload)
    {
        var jsonString = JsonSerializer.Serialize(cursorPayload);
        return _protector.Protect(jsonString);
    }

    public int? DecodeCursor(string? payload, Guid expectedEventPublicId)
    {
        if (string.IsNullOrWhiteSpace(payload))
            return null;

        try
        {
            var json = _protector.Unprotect(payload);
            var payloadData = JsonSerializer.Deserialize<MediaCursorPayload>(json);

            if (payloadData is null || payloadData.EventPublicId != expectedEventPublicId)
            {
                return null;
            }

            return payloadData.MediaId;
        }
        catch
        {
            return null;
        }
    }
}