namespace Shopniu_api.Aplication.Media.UseCases.LinkMedia;

public sealed record LinkMediaRequest(
    int ProductId,
    List<int> MediaIds);
