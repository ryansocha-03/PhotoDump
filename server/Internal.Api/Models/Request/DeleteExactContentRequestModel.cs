namespace Internal.Api.Models.Request;

public record DeleteExactContentRequestModel(IEnumerable<string> contentNames);