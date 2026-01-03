using MyPcBuild.ApiService.Domain.Models;

namespace MyPcBuild.ApiService.Services;

public interface ICompatibilityValidator
{
    Task<CompatibilityResult> ValidateBuild(List<Product> products);
}

public class CompatibilityValidator : ICompatibilityValidator
{
    public async Task<CompatibilityResult> ValidateBuild(List<Product> products)
    {
        List<CompatibilityIssue> issues = [];

        // Extract products by category
        Product? cpu = products.FirstOrDefault(p => p.Category == ProductCategory.CPU);
        Product? motherboard = products.FirstOrDefault(p => p.Category == ProductCategory.Motherboard);
        Product? gpu = products.FirstOrDefault(p => p.Category == ProductCategory.GPU);
        List<Product> rams = products.Where(p => p.Category == ProductCategory.RAM).ToList();
        Product? pcCase = products.FirstOrDefault(p => p.Category == ProductCategory.PCCase);
        Product? psu = products.FirstOrDefault(p => p.Category == ProductCategory.PSU);
        Product? cooler = products.FirstOrDefault(p => p.Category == ProductCategory.Cooler);

        // Run all validation checks
        ValidateCpuMotherboardCompatibility(cpu, motherboard, issues);
        ValidateRamCompatibility(rams, motherboard, issues);
        ValidateGpuCompatibility(gpu, pcCase, psu, issues);
        ValidateCaseCompatibility(pcCase, motherboard, gpu, cooler, psu, issues);
        ValidatePowerSupply(psu, cpu, gpu, issues);
        ValidateCoolerCompatibility(cooler, cpu, pcCase, issues);

        return await Task.FromResult(new CompatibilityResult(
            !issues.Any(i => i.Severity == IssueSeverity.Error),
            issues
        ));
    }

    private void ValidateCpuMotherboardCompatibility(Product? cpu, Product? motherboard, List<CompatibilityIssue> issues)
    {
        if (cpu == null || motherboard == null) return;

        string cpuSocket = GetStringSpec(cpu, "Socket");
        string mbSocket = GetStringSpec(motherboard, "Socket");

        if (!string.IsNullOrEmpty(cpuSocket) && !string.IsNullOrEmpty(mbSocket) && cpuSocket != mbSocket)
        {
            issues.Add(new CompatibilityIssue(
                $"CPU socket {cpuSocket} is incompatible with motherboard socket {mbSocket}",
                IssueSeverity.Error,
                "CPU/Motherboard"
            ));
        }
    }

    private void ValidateRamCompatibility(List<Product> rams, Product? motherboard, List<CompatibilityIssue> issues)
    {
        if (!rams.Any() || motherboard == null) return;

        string mbMemoryType = GetStringSpec(motherboard, "MemoryType");
        int mbMemorySlots = GetIntSpec(motherboard, "MemorySlots");
        int mbMaxMemory = GetIntSpec(motherboard, "MaxMemory");

        foreach (Product ram in rams)
        {
            string ramType = GetStringSpec(ram, "Type");

            // Check DDR type compatibility
            if (!string.IsNullOrEmpty(mbMemoryType) && !string.IsNullOrEmpty(ramType) && ramType != mbMemoryType)
            {
                issues.Add(new CompatibilityIssue(
                    $"{ram.Name} ({ramType}) is incompatible with motherboard memory type ({mbMemoryType})",
                    IssueSeverity.Error,
                    "RAM/Motherboard"
                ));
            }
        }

        // Check total RAM capacity
        int totalRamCapacity = rams.Sum(r => GetIntSpec(r, "Capacity"));
        if (mbMaxMemory > 0 && totalRamCapacity > mbMaxMemory)
        {
            issues.Add(new CompatibilityIssue(
                $"Total RAM capacity ({totalRamCapacity}GB) exceeds motherboard maximum ({mbMaxMemory}GB)",
                IssueSeverity.Error,
                "RAM/Motherboard"
            ));
        }

        // Check number of RAM sticks vs slots
        int totalRamSticks = rams.Sum(r => ParseRamConfiguration(r));
        if (mbMemorySlots > 0 && totalRamSticks > mbMemorySlots)
        {
            issues.Add(new CompatibilityIssue(
                $"Total RAM sticks ({totalRamSticks}) exceeds available memory slots ({mbMemorySlots})",
                IssueSeverity.Error,
                "RAM/Motherboard"
            ));
        }
    }

    private void ValidateGpuCompatibility(Product? gpu, Product? pcCase, Product? psu, List<CompatibilityIssue> issues)
    {
        if (gpu == null) return;

        int gpuLength = GetIntSpec(gpu, "Length");
        int gpuTdp = GetIntSpec(gpu, "TDP");
        int gpuSlots = GetIntSpec(gpu, "Slots");

        // Check GPU length vs case
        if (pcCase != null && gpuLength > 0)
        {
            int maxGpuLength = GetIntSpec(pcCase, "MaxGPULength");
            if (maxGpuLength > 0)
            {
                if (gpuLength > maxGpuLength)
                {
                    issues.Add(new CompatibilityIssue(
                        $"GPU length ({gpuLength}mm) exceeds case maximum ({maxGpuLength}mm)",
                        IssueSeverity.Error,
                        "GPU/Case"
                    ));
                }
                else if (gpuLength > maxGpuLength * 0.9) // Within 10% warning
                {
                    issues.Add(new CompatibilityIssue(
                        $"GPU length ({gpuLength}mm) is close to case limit ({maxGpuLength}mm) - tight fit",
                        IssueSeverity.Warning,
                        "GPU/Case"
                    ));
                }
            }
        }

        // Check GPU power requirements
        if (psu != null && gpuTdp > 0)
        {
            string powerConnectors = GetStringSpec(gpu, "PowerConnectors");
            if (!string.IsNullOrEmpty(powerConnectors))
            {
                int psuPcie8Pin = GetIntSpec(psu, "PCIe8Pin");
                
                // Check if GPU needs 16-pin connector
                if (powerConnectors.Contains("16-pin") && psuPcie8Pin < 2)
                {
                    issues.Add(new CompatibilityIssue(
                        $"GPU requires 16-pin power connector (or adapter for 2x 8-pin), PSU has {psuPcie8Pin}x 8-pin connectors",
                        psuPcie8Pin == 0 ? IssueSeverity.Error : IssueSeverity.Warning,
                        "GPU/PSU"
                    ));
                }
                // Check if GPU needs multiple 8-pin
                else if (powerConnectors.Contains("2x 8-pin"))
                {
                    if (psuPcie8Pin < 2)
                    {
                        issues.Add(new CompatibilityIssue(
                            $"GPU requires 2x 8-pin power connectors, PSU has only {psuPcie8Pin}",
                            IssueSeverity.Error,
                            "GPU/PSU"
                        ));
                    }
                }
            }
        }
    }

    private void ValidateCaseCompatibility(Product? pcCase, Product? motherboard, Product? gpu, Product? cooler, Product? psu, List<CompatibilityIssue> issues)
    {
        if (pcCase == null) return;

        string caseFormFactor = GetStringSpec(pcCase, "FormFactor");

        // Check motherboard form factor
        if (motherboard != null)
        {
            string mbFormFactor = GetStringSpec(motherboard, "FormFactor");
            if (!string.IsNullOrEmpty(caseFormFactor) && !string.IsNullOrEmpty(mbFormFactor))
            {
                bool isCompatible = IsFormFactorCompatible(caseFormFactor, mbFormFactor);
                if (!isCompatible)
                {
                    issues.Add(new CompatibilityIssue(
                        $"Case form factor ({caseFormFactor}) is incompatible with motherboard form factor ({mbFormFactor})",
                        IssueSeverity.Error,
                        "Case/Motherboard"
                    ));
                }
            }
        }

        // Check cooler clearance
        if (cooler != null)
        {
            int coolerHeight = GetIntSpec(cooler, "Height");
            int maxCoolerHeight = GetIntSpec(pcCase, "MaxCPUCoolerHeight");
            
            if (coolerHeight > 0 && maxCoolerHeight > 0)
            {
                if (coolerHeight > maxCoolerHeight)
                {
                    issues.Add(new CompatibilityIssue(
                        $"CPU cooler height ({coolerHeight}mm) exceeds case maximum ({maxCoolerHeight}mm)",
                        IssueSeverity.Error,
                        "Cooler/Case"
                    ));
                }
                else if (coolerHeight > maxCoolerHeight * 0.95) // Within 5% warning
                {
                    issues.Add(new CompatibilityIssue(
                        $"CPU cooler height ({coolerHeight}mm) is very close to case limit ({maxCoolerHeight}mm)",
                        IssueSeverity.Warning,
                        "Cooler/Case"
                    ));
                }
            }
        }

        // Check PSU clearance
        if (psu != null)
        {
            int psuLength = GetIntSpec(psu, "Length");
            int maxPsuLength = GetIntSpec(pcCase, "MaxPSULength");
            
            if (psuLength > 0 && maxPsuLength > 0 && psuLength > maxPsuLength)
            {
                issues.Add(new CompatibilityIssue(
                    $"PSU length ({psuLength}mm) exceeds case maximum ({maxPsuLength}mm)",
                    IssueSeverity.Error,
                    "PSU/Case"
                ));
            }
        }
    }

    private void ValidatePowerSupply(Product? psu, Product? cpu, Product? gpu, List<CompatibilityIssue> issues)
    {
        if (psu == null) return;

        int psuWattage = GetIntSpec(psu, "Wattage");
        if (psuWattage == 0) return;

        // Calculate total TDP
        int cpuTdp = cpu != null ? GetIntSpec(cpu, "TDP") : 0;
        int gpuTdp = gpu != null ? GetIntSpec(gpu, "TDP") : 0;
        
        // Add overhead for other components (motherboard, RAM, storage, fans, etc.)
        int systemOverhead = 150; // Approximate 150W for other components
        int totalEstimatedPower = cpuTdp + gpuTdp + systemOverhead;

        // PSU should be at least 20% more than estimated power for efficiency
        int recommendedWattage = (int)(totalEstimatedPower * 1.2);

        if (psuWattage < totalEstimatedPower)
        {
            issues.Add(new CompatibilityIssue(
                $"PSU wattage ({psuWattage}W) is insufficient for estimated system power draw ({totalEstimatedPower}W). Recommended: {recommendedWattage}W+",
                IssueSeverity.Error,
                "PSU"
            ));
        }
        else if (psuWattage < recommendedWattage)
        {
            issues.Add(new CompatibilityIssue(
                $"PSU wattage ({psuWattage}W) is below recommended ({recommendedWattage}W) for optimal efficiency",
                IssueSeverity.Warning,
                "PSU"
            ));
        }
    }

    private void ValidateCoolerCompatibility(Product? cooler, Product? cpu, Product? pcCase, List<CompatibilityIssue> issues)
    {
        if (cooler == null || cpu == null) return;

        string cpuSocket = GetStringSpec(cpu, "Socket");
        List<string> coolerSockets = GetArraySpec(cooler, "Sockets");

        // Check socket compatibility
        if (!string.IsNullOrEmpty(cpuSocket) && coolerSockets.Any())
        {
            if (!coolerSockets.Contains(cpuSocket))
            {
                issues.Add(new CompatibilityIssue(
                    $"Cooler does not support CPU socket {cpuSocket}. Supported sockets: {string.Join(", ", coolerSockets)}",
                    IssueSeverity.Error,
                    "Cooler/CPU"
                ));
            }
        }

        // Check TDP coverage
        int cpuTdp = GetIntSpec(cpu, "TDP");
        int coolerTdp = GetIntSpec(cooler, "TDP");

        if (cpuTdp > 0 && coolerTdp > 0)
        {
            if (coolerTdp < cpuTdp)
            {
                issues.Add(new CompatibilityIssue(
                    $"Cooler TDP rating ({coolerTdp}W) is below CPU TDP ({cpuTdp}W)",
                    IssueSeverity.Error,
                    "Cooler/CPU"
                ));
            }
            else if (coolerTdp < cpuTdp * 1.1) // Less than 10% headroom
            {
                issues.Add(new CompatibilityIssue(
                    $"Cooler TDP rating ({coolerTdp}W) has minimal headroom for CPU TDP ({cpuTdp}W)",
                    IssueSeverity.Warning,
                    "Cooler/CPU"
                ));
            }
        }

        // Check if it's an AIO and validate radiator size
        string coolerType = GetStringSpec(cooler, "Type");
        if (coolerType == "AIO" && pcCase != null)
        {
            int radiatorSize = GetIntSpec(cooler, "RadiatorSize");
            if (radiatorSize > 0)
            {
                // This is a simplified check - in reality you'd check case specs for radiator support
                issues.Add(new CompatibilityIssue(
                    $"Verify that your case supports a {radiatorSize}mm radiator",
                    IssueSeverity.Warning,
                    "Cooler/Case"
                ));
            }
        }
    }

    private bool IsFormFactorCompatible(string caseFormFactor, string mbFormFactor)
    {
        // ATX cases support ATX, MicroATX, Mini-ITX
        if (caseFormFactor == "ATX")
        {
            return mbFormFactor is "ATX" or "MicroATX" or "Mini-ITX";
        }
        
        // MicroATX cases support MicroATX and Mini-ITX
        if (caseFormFactor == "MicroATX")
        {
            return mbFormFactor is "MicroATX" or "Mini-ITX";
        }
        
        // Mini-ITX cases only support Mini-ITX
        if (caseFormFactor == "Mini-ITX")
        {
            return mbFormFactor == "Mini-ITX";
        }

        return false;
    }

    private int ParseRamConfiguration(Product ram)
    {
        string config = GetStringSpec(ram, "Configuration");
        if (string.IsNullOrEmpty(config)) return 1;

        // Parse "2x16GB" format - return the first number
        string[] parts = config.Split('x');
        if (parts.Length > 0 && int.TryParse(parts[0], out int count))
        {
            return count;
        }

        return 1;
    }

    private string GetStringSpec(Product product, string key)
    {
        if (product.Specifications.TryGetValue(key, out var value))
        {
            return value?.ToString() ?? string.Empty;
        }
        return string.Empty;
    }

    private int GetIntSpec(Product product, string key)
    {
        if (product.Specifications.TryGetValue(key, out var value))
        {
            if (value is int intValue) return intValue;
            if (int.TryParse(value?.ToString(), out var parsed)) return parsed;
        }
        return 0;
    }

    private List<string> GetArraySpec(Product product, string key)
    {
        if (product.Specifications.TryGetValue(key, out var value))
        {
            if (value is string[] stringArray) return stringArray.ToList();
            if (value is List<string> stringList) return stringList;
        }
        return new List<string>();
    }
}
