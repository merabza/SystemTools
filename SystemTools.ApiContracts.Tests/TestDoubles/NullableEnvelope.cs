namespace SystemTools.ApiContracts.Tests.TestDoubles;

//ApiNullableResult<T>-ის ანალოგი (ის CrawlerServiceShared-შია და SystemTools-იდან არ უნდა მოვიხმოთ)
public sealed class NullableEnvelope<T>
{
    public T? Value { get; set; }
}
