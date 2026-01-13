using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Moq;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Domain.Models.Spatial;
using MyPcBuild.ApiService.Features.Catalog;

namespace MyPcBuild.Tests.Services;

public class AiProductGeneratorTests
{
    [Fact]
    public async Task GenerateProductAsync_WithValidCpuDescription_ReturnsValidCpuProduct()
    {
        // Arrange
        string jsonResponse = """
{
  "Name": "Ryzen 9 7950X",
  "Manufacturer": "AMD",
  "Price": 699.99,
  "Socket": "AM5",
  "Cores": 16,
  "Threads": 32,
  "BaseClock": 4.5,
  "BoostClock": 5.7,
  "TDP": 170,
  "IntegratedGraphics": true
}
""";

        Mock<IChatClient> mockChatClient = new Mock<IChatClient>();
        Mock<ILogger<OpenAiProductGenerator>> mockLogger = new Mock<ILogger<OpenAiProductGenerator>>();

        ChatResponse chatResponse = new ChatResponse(
            [
                new ChatMessage(ChatRole.Assistant, jsonResponse)
            ]);

        mockChatClient
            .Setup(x => x.GetResponseAsync(
                It.IsAny<IEnumerable<ChatMessage>>(),
                It.IsAny<ChatOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(chatResponse);

        OpenAiProductGenerator generator = new OpenAiProductGenerator(mockLogger.Object, mockChatClient.Object, new ProductCategoryPromptFields());

        // Act
        Product product = await generator.GenerateProductAsync(ProductCategory.CPU, "High-end gaming processor", CancellationToken.None);

        // Assert
        Assert.NotNull(product);
        Assert.IsType<CpuProduct>(product);
        Assert.True(product.IsDraft);
        Assert.Null(product.PublishedAt);

        CpuProduct cpuProduct = (CpuProduct)product;
        Assert.Equal("Ryzen 9 7950X", cpuProduct.Name);
        Assert.Equal("AMD", cpuProduct.Manufacturer);
        Assert.Equal(699.99m, cpuProduct.Price);
        Assert.Equal(CpuSocket.AM5, cpuProduct.Socket);
        Assert.Equal(16, cpuProduct.Cores);
        Assert.Equal(32, cpuProduct.Threads);
        Assert.True(cpuProduct.IntegratedGraphics);
    }

    [Fact]
    public async Task GenerateProductAsync_WithValidGpuDescription_ReturnsValidGpuProduct()
    {
        // Arrange
        string jsonResponse = """
{
  "Name": "NVIDIA GeForce RTX 4090",
  "Manufacturer": "NVIDIA",
  "Price": 1599.99,
  "ChipsetManufacturer": "NVIDIA",
  "Series": "RTX 4000",
  "VRAM": 24,
  "MemoryType": "GDDR6X",
  "CoreClock": 2235,
  "BoostClock": 2520,
  "TDP": 450,
  "Length": 304,
  "PowerConnectors": "1x16-pin",
  "RayTracing": true,
  "Dimensions": "304,137,61",
  "Slots": "[]"
}
""";

        Mock<IChatClient> mockChatClient = new Mock<IChatClient>();
        Mock<ILogger<OpenAiProductGenerator>> mockLogger = new Mock<ILogger<OpenAiProductGenerator>>();

        ChatResponse chatResponse = new ChatResponse(
            [
                new ChatMessage(ChatRole.Assistant, jsonResponse)
            ]);

        mockChatClient
            .Setup(x => x.GetResponseAsync(
                It.IsAny<IEnumerable<ChatMessage>>(),
                It.IsAny<ChatOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(chatResponse);

        OpenAiProductGenerator generator = new OpenAiProductGenerator(mockLogger.Object, mockChatClient.Object, new ProductCategoryPromptFields());

        // Act
        Product product = await generator.GenerateProductAsync(ProductCategory.GPU, "Top-tier gaming graphics card", CancellationToken.None);

        // Assert
        Assert.NotNull(product);
        Assert.IsType<GpuProduct>(product);
        Assert.True(product.IsDraft);
        Assert.Null(product.PublishedAt);

        GpuProduct gpuProduct = (GpuProduct)product;
        Assert.Equal("GeForce RTX 4090", gpuProduct.Name);
        Assert.Equal("NVIDIA", gpuProduct.Manufacturer);
        Assert.Equal(1599.99m, gpuProduct.Price);
        Assert.Equal("NVIDIA", gpuProduct.ChipsetManufacturer);
        Assert.Equal(24, gpuProduct.VRAM.ValueInGB);
        Assert.True(gpuProduct.RayTracing);
    }

    [Fact]
    public async Task GenerateProductAsync_WithMarkdownCodeBlocks_StillParses()
    {
        // Arrange - Response wrapped in markdown code blocks
        string jsonResponse = """
```json
{
  "Name": "Corsair Vengeance DDR5",
  "Manufacturer": "Corsair",
  "Price": 129.99,
  "Type": "DDR5",
  "Capacity": 32,
  "Configuration": "2x16GB",
  "Speed": 6000,
  "CASLatency": "CL30",
  "Voltage": 1.35
}
```
""";

        Mock<IChatClient> mockChatClient = new Mock<IChatClient>();
        Mock<ILogger<OpenAiProductGenerator>> mockLogger = new Mock<ILogger<OpenAiProductGenerator>>();

        ChatResponse chatResponse = new ChatResponse(
            [
                new ChatMessage(ChatRole.Assistant, jsonResponse)
            ]);

        mockChatClient
            .Setup(x => x.GetResponseAsync(
                It.IsAny<IEnumerable<ChatMessage>>(),
                It.IsAny<ChatOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(chatResponse);

        OpenAiProductGenerator generator = new OpenAiProductGenerator(mockLogger.Object, mockChatClient.Object, new ProductCategoryPromptFields());

        // Act
        Product product = await generator.GenerateProductAsync(ProductCategory.RAM, "High-speed memory kit", CancellationToken.None);

        // Assert
        Assert.NotNull(product);
        Assert.IsType<RamProduct>(product);
        Assert.True(product.IsDraft);

        RamProduct ramProduct = (RamProduct)product;
        Assert.Equal("Corsair", ramProduct.Manufacturer);
        Assert.Equal("Vengeance DDR5", ramProduct.Name);
        Assert.Equal(MemoryType.DDR5, ramProduct.Type);
        Assert.Equal(32, ramProduct.Capacity.ValueInGB);
    }

    [Fact]
    public async Task GenerateProductAsync_WithInvalidJson_ThrowsException()
    {
        // Arrange
        string invalidJson = "This is not valid JSON";

        Mock<IChatClient> mockChatClient = new Mock<IChatClient>();
        Mock<ILogger<OpenAiProductGenerator>> mockLogger = new Mock<ILogger<OpenAiProductGenerator>>();

        ChatResponse chatResponse = new ChatResponse(
            [
                new ChatMessage(ChatRole.Assistant, invalidJson)
            ]);

        mockChatClient
            .Setup(x => x.GetResponseAsync(
                It.IsAny<IEnumerable<ChatMessage>>(),
                It.IsAny<ChatOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(chatResponse);

        OpenAiProductGenerator generator = new OpenAiProductGenerator(mockLogger.Object, mockChatClient.Object, new ProductCategoryPromptFields());

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await generator.GenerateProductAsync(ProductCategory.CPU, "Test description", CancellationToken.None)
        );
    }

    [Fact]
    public async Task GenerateProductAsync_RealResponse()
    {
        string jsonResponse = """
        ```json
        {
            "Name": "NZXT S320 Elite",
            "Manufacturer": "NZXT",
            "Price": 69.99,
            "FormFactor": "Mid Tower",
            "Color": "Black",
            "SidePanelWindow": "Tempered Glass",
            "Dimensions": {
                "length": 490,
                "width": 210,
                "height": 450
            },
            "Chambers": [
                {
                "Name": "Main Chamber",
                "Dimensions": {
                    "length": 450,
                    "width": 200,
                    "height": 450
                }
                },
                {
                "Name": "Power Supply Chamber",
                "Dimensions": {
                    "length": 450,
                    "width": 210,
                    "height": 200
                }
                }
            ]
        }
        ```
        """;

        Mock<IChatClient> mockChatClient = new Mock<IChatClient>();
        Mock<ILogger<OpenAiProductGenerator>> mockLogger = new Mock<ILogger<OpenAiProductGenerator>>();

        ChatResponse chatResponse = new ChatResponse(
            [
                new ChatMessage(ChatRole.Assistant, jsonResponse)
            ]);

        mockChatClient
            .Setup(x => x.GetResponseAsync(
                It.IsAny<IEnumerable<ChatMessage>>(),
                It.IsAny<ChatOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(chatResponse);

        OpenAiProductGenerator generator = new OpenAiProductGenerator(mockLogger.Object, mockChatClient.Object, new ProductCategoryPromptFields());

        // Act
        Product product = await generator.GenerateProductAsync(ProductCategory.Case, "NZXT S320 Elite", CancellationToken.None);

        // Assert
        Assert.NotNull(product);
        Assert.IsType<PcCaseProduct>(product);
        Assert.True(product.IsDraft);

        PcCaseProduct pcCaseProduct = (PcCaseProduct)product;
        Assert.Equal("S320 Elite", pcCaseProduct.Name);
        Assert.Equal("NZXT", pcCaseProduct.Manufacturer);
        Assert.Equal(69.99m, pcCaseProduct.Price);
        Assert.Equal(new Dimensions(Length: 490, Width: 210, Height: 450), pcCaseProduct.Dimensions);
    }
}
