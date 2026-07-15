namespace Warehouse.Domain.Tests;

using Warehouse.Domain.Exceptions;
using WebApi.Middleware;
using Xunit;

public class ExceptionStatusMapperTests
{
    [Fact]
    public void Maps_NotFoundException_To_404()
    {
        var (statusCode, errorCode) = ExceptionStatusMapper.Map(new NotFoundException("missing"));

        Assert.Equal(404, statusCode);
        Assert.Equal("NOT_FOUND", errorCode);
    }

    [Fact]
    public void Maps_DomainException_To_400()
    {
        var (statusCode, errorCode) = ExceptionStatusMapper.Map(new DomainException("bad rule"));

        Assert.Equal(400, statusCode);
        Assert.Equal("BUSINESS_RULE_VIOLATION", errorCode);
    }

    [Fact]
    public void Maps_Unknown_Exception_To_500()
    {
        var (statusCode, errorCode) = ExceptionStatusMapper.Map(new InvalidOperationException("oops"));

        Assert.Equal(500, statusCode);
        Assert.Equal("UNEXPECTED_ERROR", errorCode);
    }
}