/**
 * Helper utilities for converting between old field-based format and new typed format
 */

import type {
  ProductRequest,
  ProductResponse,
  CpuSocket,
  MemoryType,
  FormFactor,
  CoolerType,
  GpuPowerConnector,
  Frequency,
  Power,
  StorageCapacity,
  Length,
  Voltage,
  DataSpeed,
  Dimensions
} from '@/types/products';

/**
 * Parses a numeric value from a string or returns default
 */
function parseNumeric(value: string | undefined, defaultValue: number): number {
  if (!value) return defaultValue;
  const parsed = parseFloat(value);
  return isNaN(parsed) ? defaultValue : parsed;
}

/**
 * Parses an integer value from a string or returns default
 */
function parseIntValue(value: string | undefined, defaultValue: number): number {
  if (!value) return defaultValue;
  const parsed = parseInt(value, 10);
  return isNaN(parsed) ? defaultValue : parsed;
}

/**
 * Parses a boolean value from string
 */
function parseBoolean(value: string | undefined): boolean {
  if (!value) return false;
  return value.toLowerCase() === 'true';
}

/**
 * Parses dimensions from comma-separated string
 */
function parseDimensions(value: string | undefined): Dimensions {
  if (!value) return { length: 0, width: 0, height: 0 };
  
  const parts = value.split(',').map(p => parseFloat(p.trim()));
  if (parts.length !== 3) return { length: 0, width: 0, height: 0 };
  
  return {
    length: parts[0] || 0,
    width: parts[1] || 0,
    height: parts[2] || 0
  };
}

/**
 * Converts fields dict to CPU product data
 */
export function fieldsToCpuProduct(fields: Record<string, string>): Partial<ProductRequest> {
  return {
    category: 'cpu',
    socket: fields.Socket as CpuSocket,
    cores: parseIntValue(fields.Cores, 8),
    threads: parseIntValue(fields.Threads, 16),
    baseClock: { valueInGHz: parseNumeric(fields.BaseClock, 3.5) },
    boostClock: { valueInGHz: parseNumeric(fields.BoostClock, 5.0) },
    tdp: { valueInWatts: parseIntValue(fields.TDP, 105) },
    integratedGraphics: parseBoolean(fields.IntegratedGraphics)
  };
}

/**
 * Converts fields dict to GPU product data
 */
export function fieldsToGpuProduct(fields: Record<string, string>): Partial<ProductRequest> {
  return {
    category: 'gpu',
    chipsetManufacturer: fields.ChipsetManufacturer || '',
    series: fields.Series || '',
    vram: { valueInGB: parseIntValue(fields.VRAM, 8) },
    memoryType: fields.MemoryType as MemoryType,
    coreClock: { valueInGHz: parseNumeric(fields.CoreClock, 2.0) },
    boostClock: { valueInGHz: parseNumeric(fields.BoostClock, 2.5) },
    tdp: { valueInWatts: parseIntValue(fields.TDP, 300) },
    length: { valueInMm: parseIntValue(fields.Length, 300) },
    powerConnectors: fields.PowerConnectors as GpuPowerConnector,
    rayTracing: parseBoolean(fields.RayTracing),
    dimensions: parseDimensions(fields.Dimensions)
  };
}

/**
 * Converts fields dict to Motherboard product data
 */
export function fieldsToMotherboardProduct(fields: Record<string, string>): Partial<ProductRequest> {
  return {
    category: 'motherboard',
    socket: fields.Socket as CpuSocket,
    chipset: fields.Chipset || '',
    formFactor: fields.FormFactor as FormFactor,
    memoryType: fields.MemoryType as MemoryType,
    maxMemory: { valueInGB: parseIntValue(fields.MaxMemory, 128) },
    dimensions: parseDimensions(fields.Dimensions)
  };
}

/**
 * Converts fields dict to RAM product data
 */
export function fieldsToRamProduct(fields: Record<string, string>): Partial<ProductRequest> {
  return {
    category: 'ram',
    type: fields.Type as MemoryType,
    capacity: { valueInGB: parseIntValue(fields.Capacity, 32) },
    configuration: fields.Configuration || '2x16GB',
    speed: { valueInGHz: parseNumeric(fields.Speed, 3.6) },
    casLatency: fields.CASLatency || 'CL16',
    voltage: { valueInVolts: parseNumeric(fields.Voltage, 1.35) }
  };
}

/**
 * Converts fields dict to Storage product data
 */
export function fieldsToStorageProduct(fields: Record<string, string>): Partial<ProductRequest> {
  return {
    category: 'storage',
    type: fields.Type || 'SSD',
    interface: fields.Interface || 'NVMe',
    storageFormFactor: fields.StorageFormFactor || 'M.2 2280',
    capacity: { valueInGB: parseIntValue(fields.Capacity, 1000) },
    readSpeed: { valueInMBps: parseIntValue(fields.ReadSpeed, 7000) },
    writeSpeed: { valueInMBps: parseIntValue(fields.WriteSpeed, 5000) }
  };
}

/**
 * Converts fields dict to PSU product data
 */
export function fieldsToPsuProduct(fields: Record<string, string>): Partial<ProductRequest> {
  return {
    category: 'powersupply',
    wattage: { valueInWatts: parseIntValue(fields.Wattage, 750) },
    efficiency: fields.Efficiency || '80+ Gold',
    modular: fields.Modular || 'Fully Modular',
    formFactor: fields.FormFactor || 'ATX',
    length: { valueInMm: parseIntValue(fields.Length, 160) },
    pcie8Pin: parseIntValue(fields.PCIe8Pin, 4)
  };
}

/**
 * Converts fields dict to Cooler product data
 */
export function fieldsToCoolerProduct(fields: Record<string, string>): Partial<ProductRequest> {
  const socketsStr = fields.Sockets || '';
  const sockets = socketsStr ? socketsStr.split(',').map(s => s.trim() as CpuSocket) : [];
  
  return {
    category: 'cooler',
    coolerType: fields.CoolerType as CoolerType,
    height: { valueInMm: parseIntValue(fields.Height, 155) },
    tdp: { valueInWatts: parseIntValue(fields.TDP, 220) },
    sockets: sockets,
    dimensions: parseDimensions(fields.Dimensions)
  };
}

/**
 * Converts fields dict to PC Case product data
 */
export function fieldsToPcCaseProduct(fields: Record<string, string>): Partial<ProductRequest> {
  return {
    category: 'case',
    formFactor: fields.FormFactor || 'Mid Tower',
    color: fields.Color || 'Black',
    sidePanelWindow: fields.SidePanelWindow || 'Tempered Glass',
    dimensions: parseDimensions(fields.Dimensions)
  };
}

/**
 * Main conversion function from fields to typed product
 */
export function fieldsToTypedProduct(fields: Record<string, string>, category: string): Partial<ProductRequest> {
  const categoryLower = category.toLowerCase();
  
  switch (categoryLower) {
    case 'cpu':
      return fieldsToCpuProduct(fields);
    case 'gpu':
      return fieldsToGpuProduct(fields);
    case 'motherboard':
      return fieldsToMotherboardProduct(fields);
    case 'ram':
      return fieldsToRamProduct(fields);
    case 'storage':
      return fieldsToStorageProduct(fields);
    case 'powersupply':
      return fieldsToPsuProduct(fields);
    case 'cooler':
      return fieldsToCoolerProduct(fields);
    case 'case':
      return fieldsToPcCaseProduct(fields);
    default:
      return {};
  }
}

/**
 * Converts typed product to fields dict (for API submission)
 */
export function typedProductToFields(product: Partial<ProductRequest>): Record<string, any> {
  const fields: Record<string, any> = {};
  
  // Remove category field as it's handled separately
  const { category, ...rest } = product;
  
  Object.entries(rest).forEach(([key, value]) => {
    if (value === undefined || value === null) return;
    
    // Handle value objects
    if (typeof value === 'object' && !Array.isArray(value)) {
      if ('valueInGHz' in value) {
        fields[key] = (value as Frequency).valueInGHz;
      } else if ('valueInWatts' in value) {
        fields[key] = (value as Power).valueInWatts;
      } else if ('valueInGB' in value) {
        fields[key] = (value as StorageCapacity).valueInGB;
      } else if ('valueInMm' in value) {
        fields[key] = (value as Length).valueInMm;
      } else if ('valueInVolts' in value) {
        fields[key] = (value as Voltage).valueInVolts;
      } else if ('valueInMBps' in value) {
        fields[key] = (value as DataSpeed).valueInMBps;
      } else if ('length' in value && 'width' in value && 'height' in value) {
        const dims = value as Dimensions;
        fields[key] = dims;
      } else {
        fields[key] = value;
      }
    } else if (Array.isArray(value)) {
      fields[key] = value;
    } else {
      fields[key] = value;
    }
  });
  
  return fields;
}
