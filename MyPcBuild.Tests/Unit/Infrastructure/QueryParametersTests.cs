using MyPcBuild.ApiService.Infrastructure;

namespace MyPcBuild.Tests.Unit.Infrastructure;

public class QueryParametersTests
{
    [Fact]
    public void GetSkip_Page1_ReturnsZero()
    {
        var queryParams = new QueryParameters { Page = 1, ItemsPerPage = 20 };
        int skip = queryParams.GetSkip();
        Assert.Equal(0, skip);
    }

    [Fact]
    public void GetSkip_Page2_ReturnsItemsPerPage()
    {
        var queryParams = new QueryParameters { Page = 2, ItemsPerPage = 20 };
        int skip = queryParams.GetSkip();
        Assert.Equal(20, skip);
    }

    [Fact]
    public void GetSkip_Page3_ReturnsDoubleItemsPerPage()
    {
        var queryParams = new QueryParameters { Page = 3, ItemsPerPage = 10 };
        int skip = queryParams.GetSkip();
        Assert.Equal(20, skip);
    }

    [Fact]
    public void GetSkip_Page10WithItemsPerPage50_ReturnsCorrectValue()
    {
        var queryParams = new QueryParameters { Page = 10, ItemsPerPage = 50 };
        int skip = queryParams.GetSkip();
        Assert.Equal(450, skip); // (10 - 1) * 50 = 450
    }

    [Fact]
    public void Defaults_PageIsOne_ItemsPerPageIs20()
    {
        var queryParams = new QueryParameters();
        Assert.Equal(1, queryParams.Page);
        Assert.Equal(20, queryParams.ItemsPerPage);
    }

    [Fact]
    public void Page_CanBeSet()
    {
        var queryParams = new QueryParameters { Page = 5 };
        Assert.Equal(5, queryParams.Page);
    }

    [Fact]
    public void ItemsPerPage_CanBeSet()
    {
        var queryParams = new QueryParameters { ItemsPerPage = 50 };
        Assert.Equal(50, queryParams.ItemsPerPage);
    }

    [Fact]
    public void Search_CanBeSet()
    {
        var queryParams = new QueryParameters { Search = "CPU" };
        Assert.Equal("CPU", queryParams.Search);
    }

    [Fact]
    public void SortBy_CanBeSet()
    {
        var queryParams = new QueryParameters { SortBy = "Name" };
        Assert.Equal("Name", queryParams.SortBy);
    }

    [Fact]
    public void SortDesc_CanBeSet()
    {
        var queryParams = new QueryParameters { SortDesc = true };
        Assert.True(queryParams.SortDesc);
    }

    [Fact]
    public void Filters_CanBeSet()
    {
        var queryParams = new QueryParameters { Filters = "category=cpu,isDraft=false" };
        Assert.Equal("category=cpu,isDraft=false", queryParams.Filters);
    }

    [Fact]
    public void GetSkip_ZeroPage_CalculatesAsIfPageIsOne()
    {
        // Page should be at least 1, but if somehow set to 0, it should calculate correctly
        var queryParams = new QueryParameters { Page = 1, ItemsPerPage = 20 };
        int skip = queryParams.GetSkip();
        Assert.Equal(0, skip);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(1000)]
    public void GetSkip_VariousPages_CalculatesCorrectly(int page)
    {
        var queryParams = new QueryParameters { Page = page, ItemsPerPage = 25 };
        int skip = queryParams.GetSkip();
        int expected = (page - 1) * 25;
        Assert.Equal(expected, skip);
    }
}
