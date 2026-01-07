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

        // Extract products by type
        CpuProduct? cpu = products.OfType<CpuProduct>().FirstOrDefault();
        MotherboardProduct? motherboard = products.OfType<MotherboardProduct>().FirstOrDefault();
        GpuProduct? gpu = products.OfType<GpuProduct>().FirstOrDefault();
        List<RamProduct> rams = products.OfType<RamProduct>().ToList();
        PcCaseProduct? pcCase = products.OfType<PcCaseProduct>().FirstOrDefault();
        PsuProduct? psu = products.OfType<PsuProduct>().FirstOrDefault();
        CoolerProduct? cooler = products.OfType<CoolerProduct>().FirstOrDefault();

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

    private void ValidateCpuMotherboardCompatibility(CpuProduct? cpu, MotherboardProduct? motherboard, List<CompatibilityIssue> issues)
    {
        if (cpu == null || motherboard == null) return;

        if (cpu.Socket != motherboard.Socket)
        {
            issues.Add(new CompatibilityIssue(
                $"CPU socket {cpu.Socket} is incompatible with motherboard socket {motherboard.Socket}",
                IssueSeverity.Error,
                "CPU/Motherboard"
            ));
        }
    }

    private void ValidateRamCompatibility(List<RamProduct> rams, MotherboardProduct? motherboard, List<CompatibilityIssue> issues)
    {
        if (!rams.Any() || motherboard == null) return;

        foreach (RamProduct ram in rams)
        {
            // Check DDR type compatibility
            if (ram.Type != motherboard.MemoryType)
            {
                issues.Add(new CompatibilityIssue(
                    $"{ram.Name} ({ram.Type}) is incompatible with motherboard memory type ({motherboard.MemoryType})",
                    IssueSeverity.Error,
                    "RAM/Motherboard"
                ));
            }
        }

        // Check total RAM capacity
        int totalRamCapacity = rams.Sum(r => r.Capacity.ValueInGB);
        if (totalRamCapacity > motherboard.MaxMemory.ValueInGB)
        {
            issues.Add(new CompatibilityIssue(
                $"Total RAM capacity ({totalRamCapacity}GB) exceeds motherboard maximum ({motherboard.MaxMemory.ValueInGB}GB)",
                IssueSeverity.Error,
                "RAM/Motherboard"
            ));
        }

        // Check number of RAM sticks vs slots
        int totalRamSticks = rams.Sum(r => ParseRamConfiguration(r));
        int availableMemorySlots = motherboard.Slots.Count(s => s.AllowedCategoryName == "RAM");
        if (totalRamSticks > availableMemorySlots)
        {
            issues.Add(new CompatibilityIssue(
                $"Total RAM sticks ({totalRamSticks}) exceeds available memory slots ({availableMemorySlots})",
                IssueSeverity.Error,
                "RAM/Motherboard"
            ));
        }
    }

    private void ValidateGpuCompatibility(GpuProduct? gpu, PcCaseProduct? pcCase, PsuProduct? psu, List<CompatibilityIssue> issues)
    {
        if (gpu == null) return;

        // Check GPU length vs case
        if (pcCase != null)
        {
            if (gpu.Length.ValueInMm > pcCase.MaxGPULength.ValueInMm)
            {
                issues.Add(new CompatibilityIssue(
                    $"GPU length ({gpu.Length.ValueInMm}mm) exceeds case maximum ({pcCase.MaxGPULength.ValueInMm}mm)",
                    IssueSeverity.Error,
                    "GPU/Case"
                ));
            }
            else if (gpu.Length.ValueInMm > pcCase.MaxGPULength.ValueInMm * 0.9) // Within 10% warning
            {
                issues.Add(new CompatibilityIssue(
                    $"GPU length ({gpu.Length.ValueInMm}mm) is close to case limit ({pcCase.MaxGPULength.ValueInMm}mm) - tight fit",
                    IssueSeverity.Warning,
                    "GPU/Case"
                ));
            }
        }

        // Check GPU power requirements
        if (psu != null)
        {
            // Check if GPU needs 16-pin connector
            if (gpu.PowerConnectors.Contains("16-pin") && psu.PCIe8Pin < 2)
            {
                issues.Add(new CompatibilityIssue(
                    $"GPU requires 16-pin power connector (or adapter for 2x 8-pin), PSU has {psu.PCIe8Pin}x 8-pin connectors",
                    psu.PCIe8Pin == 0 ? IssueSeverity.Error : IssueSeverity.Warning,
                    "GPU/PSU"
                ));
            }
            // Check if GPU needs multiple 8-pin
            else if (gpu.PowerConnectors.Contains("2x 8-pin"))
            {
                if (psu.PCIe8Pin < 2)
                {
                    issues.Add(new CompatibilityIssue(
                        $"GPU requires 2x 8-pin power connectors, PSU has only {psu.PCIe8Pin}",
                        IssueSeverity.Error,
                        "GPU/PSU"
                    ));
                }
            }
        }
    }

    private void ValidateCaseCompatibility(PcCaseProduct? pcCase, MotherboardProduct? motherboard, GpuProduct? gpu, CoolerProduct? cooler, PsuProduct? psu, List<CompatibilityIssue> issues)
    {
        if (pcCase == null) return;

        // Check motherboard form factor
        if (motherboard != null)
        {
            bool isCompatible = IsFormFactorCompatible(pcCase.FormFactor, motherboard.FormFactor);
            if (!isCompatible)
            {
                issues.Add(new CompatibilityIssue(
                    $"Case form factor ({pcCase.FormFactor}) is incompatible with motherboard form factor ({motherboard.FormFactor})",
                    IssueSeverity.Error,
                    "Case/Motherboard"
                ));
            }
        }

        // Check cooler clearance
        if (cooler != null)
        {
            if (cooler.Height.ValueInMm > pcCase.MaxCPUCoolerHeight.ValueInMm)
            {
                issues.Add(new CompatibilityIssue(
                    $"CPU cooler height ({cooler.Height.ValueInMm}mm) exceeds case maximum ({pcCase.MaxCPUCoolerHeight.ValueInMm}mm)",
                    IssueSeverity.Error,
                    "Cooler/Case"
                ));
            }
            else if (cooler.Height.ValueInMm > pcCase.MaxCPUCoolerHeight.ValueInMm * 0.95) // Within 5% warning
            {
                issues.Add(new CompatibilityIssue(
                    $"CPU cooler height ({cooler.Height.ValueInMm}mm) is very close to case limit ({pcCase.MaxCPUCoolerHeight.ValueInMm}mm)",
                    IssueSeverity.Warning,
                    "Cooler/Case"
                ));
            }
        }

        // Check PSU clearance
        if (psu != null)
        {
            if (psu.Length.ValueInMm > pcCase.MaxPSULength.ValueInMm)
            {
                issues.Add(new CompatibilityIssue(
                    $"PSU length ({psu.Length.ValueInMm}mm) exceeds case maximum ({pcCase.MaxPSULength.ValueInMm}mm)",
                    IssueSeverity.Error,
                    "PSU/Case"
                ));
            }
        }
    }

    private void ValidatePowerSupply(PsuProduct? psu, CpuProduct? cpu, GpuProduct? gpu, List<CompatibilityIssue> issues)
    {
        if (psu == null) return;

        // Calculate total TDP
        int cpuTdp = cpu?.TDP.ValueInWatts ?? 0;
        int gpuTdp = gpu?.TDP.ValueInWatts ?? 0;
        
        // Add overhead for other components (motherboard, RAM, storage, fans, etc.)
        int systemOverhead = 150; // Approximate 150W for other components
        int totalEstimatedPower = cpuTdp + gpuTdp + systemOverhead;

        // PSU should be at least 20% more than estimated power for efficiency
        int recommendedWattage = (int)(totalEstimatedPower * 1.2);

        if (psu.Wattage.ValueInWatts < totalEstimatedPower)
        {
            issues.Add(new CompatibilityIssue(
                $"PSU wattage ({psu.Wattage.ValueInWatts}W) is insufficient for estimated system power draw ({totalEstimatedPower}W). Recommended: {recommendedWattage}W+",
                IssueSeverity.Error,
                "PSU"
            ));
        }
        else if (psu.Wattage.ValueInWatts < recommendedWattage)
        {
            issues.Add(new CompatibilityIssue(
                $"PSU wattage ({psu.Wattage.ValueInWatts}W) is below recommended ({recommendedWattage}W) for optimal efficiency",
                IssueSeverity.Warning,
                "PSU"
            ));
        }
    }

    private void ValidateCoolerCompatibility(CoolerProduct? cooler, CpuProduct? cpu, PcCaseProduct? pcCase, List<CompatibilityIssue> issues)
    {
        if (cooler == null || cpu == null) return;

        // Check socket compatibility
        if (!cooler.Sockets.Contains(cpu.Socket))
        {
            issues.Add(new CompatibilityIssue(
                $"Cooler does not support CPU socket {cpu.Socket}. Supported sockets: {string.Join(", ", cooler.Sockets)}",
                IssueSeverity.Error,
                "Cooler/CPU"
            ));
        }

        // Check TDP coverage
        if (cooler.TDP.ValueInWatts < cpu.TDP.ValueInWatts)
        {
            issues.Add(new CompatibilityIssue(
                $"Cooler TDP rating ({cooler.TDP.ValueInWatts}W) is below CPU TDP ({cpu.TDP.ValueInWatts}W)",
                IssueSeverity.Error,
                "Cooler/CPU"
            ));
        }
        else if (cooler.TDP.ValueInWatts < cpu.TDP.ValueInWatts * 1.1) // Less than 10% headroom
        {
            issues.Add(new CompatibilityIssue(
                $"Cooler TDP rating ({cooler.TDP.ValueInWatts}W) has minimal headroom for CPU TDP ({cpu.TDP.ValueInWatts}W)",
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

    private int ParseRamConfiguration(RamProduct ram)
    {
        // Parse "2x16GB" format - return the first number
        string[] parts = ram.Configuration.Split('x');
        if (parts.Length > 0 && int.TryParse(parts[0], out int count))
        {
            return count;
        }

        return 1;
    }
}
