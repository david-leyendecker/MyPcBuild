using MyPcBuild.ApiService.Domain.Models;

namespace MyPcBuild.ApiService.Features.Compatibility;

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
        if (cpu is not CpuProduct cpuProduct || motherboard is not MotherboardProduct mbProduct) return;

        if (cpuProduct.Socket != mbProduct.Socket)
        {
            issues.Add(new CompatibilityIssue(
                $"CPU socket {cpuProduct.Socket} is incompatible with motherboard socket {mbProduct.Socket}",
                IssueSeverity.Error,
                "CPU/Motherboard"
            ));
        }
    }

    private void ValidateRamCompatibility(List<Product> rams, Product? motherboard, List<CompatibilityIssue> issues)
    {
        if (!rams.Any() || motherboard is not MotherboardProduct mbProduct) return;

        foreach (Product ram in rams)
        {
            if (ram is not RamProduct ramProduct) continue;

            // Check DDR type compatibility
            if (ramProduct.Type != mbProduct.MemoryType)
            {
                issues.Add(new CompatibilityIssue(
                    $"{ram.Name} ({ramProduct.Type}) is incompatible with motherboard memory type ({mbProduct.MemoryType})",
                    IssueSeverity.Error,
                    "RAM/Motherboard"
                ));
            }
        }

        // Check total RAM capacity
        int totalRamCapacity = rams.OfType<RamProduct>().Sum(r => r.Capacity);
        if (totalRamCapacity > mbProduct.MaxMemory)
        {
            issues.Add(new CompatibilityIssue(
                $"Total RAM capacity ({totalRamCapacity}GB) exceeds motherboard maximum ({mbProduct.MaxMemory}GB)",
                IssueSeverity.Error,
                "RAM/Motherboard"
            ));
        }

        // Check number of RAM sticks vs slots
        int totalRamSticks = rams.Sum(r => ParseRamConfiguration(r));
        if (totalRamSticks > mbProduct.MemorySlots)
        {
            issues.Add(new CompatibilityIssue(
                $"Total RAM sticks ({totalRamSticks}) exceeds available memory slots ({mbProduct.MemorySlots})",
                IssueSeverity.Error,
                "RAM/Motherboard"
            ));
        }
    }

    private void ValidateGpuCompatibility(Product? gpu, Product? pcCase, Product? psu, List<CompatibilityIssue> issues)
    {
        if (gpu is not GpuProduct gpuProduct) return;

        // Check GPU length vs case
        if (pcCase is PcCaseProduct caseProduct)
        {
            if (gpuProduct.Length > caseProduct.MaxGPULength)
            {
                issues.Add(new CompatibilityIssue(
                    $"GPU length ({gpuProduct.Length}mm) exceeds case maximum ({caseProduct.MaxGPULength}mm)",
                    IssueSeverity.Error,
                    "GPU/Case"
                ));
            }
            else if (gpuProduct.Length > caseProduct.MaxGPULength * 0.9) // Within 10% warning
            {
                issues.Add(new CompatibilityIssue(
                    $"GPU length ({gpuProduct.Length}mm) is close to case limit ({caseProduct.MaxGPULength}mm) - tight fit",
                    IssueSeverity.Warning,
                    "GPU/Case"
                ));
            }
        }

        // Check GPU power requirements
        if (psu is PsuProduct psuProduct)
        {
            // Check if GPU needs 16-pin connector
            if (gpuProduct.PowerConnectors.Contains("16-pin") && psuProduct.PCIe8Pin < 2)
            {
                issues.Add(new CompatibilityIssue(
                    $"GPU requires 16-pin power connector (or adapter for 2x 8-pin), PSU has {psuProduct.PCIe8Pin}x 8-pin connectors",
                    psuProduct.PCIe8Pin == 0 ? IssueSeverity.Error : IssueSeverity.Warning,
                    "GPU/PSU"
                ));
            }
            // Check if GPU needs multiple 8-pin
            else if (gpuProduct.PowerConnectors.Contains("2x 8-pin"))
            {
                if (psuProduct.PCIe8Pin < 2)
                {
                    issues.Add(new CompatibilityIssue(
                        $"GPU requires 2x 8-pin power connectors, PSU has only {psuProduct.PCIe8Pin}",
                        IssueSeverity.Error,
                        "GPU/PSU"
                    ));
                }
            }
        }
    }

    private void ValidateCaseCompatibility(Product? pcCase, Product? motherboard, Product? gpu, Product? cooler, Product? psu, List<CompatibilityIssue> issues)
    {
        if (pcCase is not PcCaseProduct caseProduct) return;

        // Check motherboard form factor
        if (motherboard is MotherboardProduct mbProduct)
        {
            bool isCompatible = IsFormFactorCompatible(caseProduct.FormFactor, mbProduct.FormFactor);
            if (!isCompatible)
            {
                issues.Add(new CompatibilityIssue(
                    $"Case form factor ({caseProduct.FormFactor}) is incompatible with motherboard form factor ({mbProduct.FormFactor})",
                    IssueSeverity.Error,
                    "Case/Motherboard"
                ));
            }
        }

        // Check cooler clearance
        if (cooler is CoolerProduct coolerProduct)
        {
            if (coolerProduct.Height > caseProduct.MaxCPUCoolerHeight)
            {
                issues.Add(new CompatibilityIssue(
                    $"CPU cooler height ({coolerProduct.Height}mm) exceeds case maximum ({caseProduct.MaxCPUCoolerHeight}mm)",
                    IssueSeverity.Error,
                    "Cooler/Case"
                ));
            }
            else if (coolerProduct.Height > caseProduct.MaxCPUCoolerHeight * 0.95) // Within 5% warning
            {
                issues.Add(new CompatibilityIssue(
                    $"CPU cooler height ({coolerProduct.Height}mm) is very close to case limit ({caseProduct.MaxCPUCoolerHeight}mm)",
                    IssueSeverity.Warning,
                    "Cooler/Case"
                ));
            }
        }

        // Check PSU clearance
        if (psu is PsuProduct psuProduct)
        {
            if (psuProduct.Length > caseProduct.MaxPSULength)
            {
                issues.Add(new CompatibilityIssue(
                    $"PSU length ({psuProduct.Length}mm) exceeds case maximum ({caseProduct.MaxPSULength}mm)",
                    IssueSeverity.Error,
                    "PSU/Case"
                ));
            }
        }
    }

    private void ValidatePowerSupply(Product? psu, Product? cpu, Product? gpu, List<CompatibilityIssue> issues)
    {
        if (psu is not PsuProduct psuProduct) return;

        // Calculate total TDP
        int cpuTdp = cpu is CpuProduct cpuProduct ? cpuProduct.TDP : 0;
        int gpuTdp = gpu is GpuProduct gpuProduct ? gpuProduct.TDP : 0;
        
        // Add overhead for other components (motherboard, RAM, storage, fans, etc.)
        int systemOverhead = 150; // Approximate 150W for other components
        int totalEstimatedPower = cpuTdp + gpuTdp + systemOverhead;

        // PSU should be at least 20% more than estimated power for efficiency
        int recommendedWattage = (int)(totalEstimatedPower * 1.2);

        if (psuProduct.Wattage < totalEstimatedPower)
        {
            issues.Add(new CompatibilityIssue(
                $"PSU wattage ({psuProduct.Wattage}W) is insufficient for estimated system power draw ({totalEstimatedPower}W). Recommended: {recommendedWattage}W+",
                IssueSeverity.Error,
                "PSU"
            ));
        }
        else if (psuProduct.Wattage < recommendedWattage)
        {
            issues.Add(new CompatibilityIssue(
                $"PSU wattage ({psuProduct.Wattage}W) is below recommended ({recommendedWattage}W) for optimal efficiency",
                IssueSeverity.Warning,
                "PSU"
            ));
        }
    }

    private void ValidateCoolerCompatibility(Product? cooler, Product? cpu, Product? pcCase, List<CompatibilityIssue> issues)
    {
        if (cooler is not CoolerProduct coolerProduct || cpu is not CpuProduct cpuProduct) return;

        // Check socket compatibility
        if (!coolerProduct.Sockets.Contains(cpuProduct.Socket))
        {
            issues.Add(new CompatibilityIssue(
                $"Cooler does not support CPU socket {cpuProduct.Socket}. Supported sockets: {string.Join(", ", coolerProduct.Sockets)}",
                IssueSeverity.Error,
                "Cooler/CPU"
            ));
        }

        // Check TDP coverage
        if (coolerProduct.TDP < cpuProduct.TDP)
        {
            issues.Add(new CompatibilityIssue(
                $"Cooler TDP rating ({coolerProduct.TDP}W) is below CPU TDP ({cpuProduct.TDP}W)",
                IssueSeverity.Error,
                "Cooler/CPU"
            ));
        }
        else if (coolerProduct.TDP < cpuProduct.TDP * 1.1) // Less than 10% headroom
        {
            issues.Add(new CompatibilityIssue(
                $"Cooler TDP rating ({coolerProduct.TDP}W) has minimal headroom for CPU TDP ({cpuProduct.TDP}W)",
                IssueSeverity.Warning,
                "Cooler/CPU"
            ));
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
        if (ram is not RamProduct ramProduct) return 1;

        // Parse "2x16GB" format - return the first number
        string[] parts = ramProduct.Configuration.Split('x');
        if (parts.Length > 0 && int.TryParse(parts[0], out int count))
        {
            return count;
        }

        return 1;
    }
}
