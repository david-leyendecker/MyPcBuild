/**
 * TypeScript contracts for Product Management
 * These interfaces match the API DTOs from the backend
 */

import type { Dimensions, Vector3, Rotation, Slot, Chamber } from './spatial';

// ========== Enums (as string literals for API compatibility) ==========

export type ProductCategory = 'cpu' | 'gpu' | 'motherboard' | 'ram' | 'storage' | 'powersupply' | 'cooler' | 'case';

export type CpuSocket = 'LGA1700' | 'LGA1200' | 'LGA1151' | 'LGA2066' | 'AM5' | 'AM4' | 'sTRX4' | 'TR4';

export type MemoryType = 'DDR3' | 'DDR4' | 'DDR5' | 'GDDR5' | 'GDDR5X' | 'GDDR6' | 'GDDR6X' | 'HBM2' | 'HBM2E' | 'HBM3';

export type FormFactor = 'ATX' | 'MicroATX' | 'MiniITX' | 'EATX';

export type CoolerType = 'Air' | 'AIO' | 'CustomLoop';

export type GpuPowerConnector = 'Dual8Pin' | 'Triple8Pin' | 'One16Pin';

// ========== Value Objects ==========

export interface Frequency {
  valueInGHz: number;
}

export interface StorageCapacity {
  valueInGB: number;
}

export interface Power {
  valueInWatts: number;
}

export interface Voltage {
  valueInVolts: number;
}

export interface Length {
  valueInMm: number;
}

export interface DataSpeed {
  valueInMBps: number;
}

// Re-export spatial types for convenience
export type { Dimensions, Vector3, Rotation, Slot, Chamber };

// ========== Product Base ==========

export interface ProductBase {
  id: string;
  category: ProductCategory;
  name: string;
  price: number;
  manufacturer: string;
  isDraft: boolean;
  publishedAt: string | null;
}

// ========== CPU Product ==========

export interface CpuProductRequest {
  category: 'cpu';
  name: string;
  price: number;
  manufacturer: string;
  socket: CpuSocket;
  cores: number;
  threads: number;
  baseClock: Frequency;
  boostClock: Frequency;
  tdp: Power;
  integratedGraphics: boolean;
}

export interface CpuProductResponse extends ProductBase {
  category: 'cpu';
  socket: CpuSocket;
  cores: number;
  threads: number;
  baseClock: Frequency;
  boostClock: Frequency;
  tdp: Power;
  integratedGraphics: boolean;
}

// ========== GPU Product ==========

export interface GpuProductRequest {
  category: 'gpu';
  name: string;
  price: number;
  manufacturer: string;
  chipsetManufacturer: string;
  series: string;
  vram: StorageCapacity;
  memoryType: MemoryType;
  coreClock: Frequency;
  boostClock: Frequency;
  tdp: Power;
  length: Length;
  powerConnectors: GpuPowerConnector;
  rayTracing: boolean;
  dimensions: Dimensions;
  slots?: Slot[];
}

export interface GpuProductResponse extends ProductBase {
  category: 'gpu';
  chipsetManufacturer: string;
  series: string;
  vram: StorageCapacity;
  memoryType: MemoryType;
  coreClock: Frequency;
  boostClock: Frequency;
  tdp: Power;
  length: Length;
  powerConnectors: GpuPowerConnector;
  rayTracing: boolean;
  dimensions: Dimensions;
  slots?: Slot[];
}

// ========== Motherboard Product ==========

export interface MotherboardProductRequest {
  category: 'motherboard';
  name: string;
  price: number;
  manufacturer: string;
  socket: CpuSocket;
  chipset: string;
  formFactor: FormFactor;
  memoryType: MemoryType;
  maxMemory: StorageCapacity;
  dimensions: Dimensions;
  slots?: Slot[];
}

export interface MotherboardProductResponse extends ProductBase {
  category: 'motherboard';
  socket: CpuSocket;
  chipset: string;
  formFactor: FormFactor;
  memoryType: MemoryType;
  maxMemory: StorageCapacity;
  dimensions: Dimensions;
  slots?: Slot[];
}

// ========== RAM Product ==========

export interface RamProductRequest {
  category: 'ram';
  name: string;
  price: number;
  manufacturer: string;
  type: MemoryType;
  capacity: StorageCapacity;
  configuration: string;
  speed: Frequency;
  casLatency: string;
  voltage: Voltage;
}

export interface RamProductResponse extends ProductBase {
  category: 'ram';
  type: MemoryType;
  capacity: StorageCapacity;
  configuration: string;
  speed: Frequency;
  casLatency: string;
  voltage: Voltage;
}

// ========== Storage Product ==========

export interface StorageProductRequest {
  category: 'storage';
  name: string;
  price: number;
  manufacturer: string;
  type: string;
  interface: string;
  storageFormFactor: string;
  capacity: StorageCapacity;
  readSpeed: DataSpeed;
  writeSpeed: DataSpeed;
}

export interface StorageProductResponse extends ProductBase {
  category: 'storage';
  type: string;
  interface: string;
  storageFormFactor: string;
  capacity: StorageCapacity;
  readSpeed: DataSpeed;
  writeSpeed: DataSpeed;
}

// ========== PSU Product ==========

export interface PsuProductRequest {
  category: 'powersupply';
  name: string;
  price: number;
  manufacturer: string;
  wattage: Power;
  efficiency: string;
  modular: string;
  formFactor: string;
  length: Length;
  pcie8Pin: number;
}

export interface PsuProductResponse extends ProductBase {
  category: 'powersupply';
  wattage: Power;
  efficiency: string;
  modular: string;
  formFactor: string;
  length: Length;
  pcie8Pin: number;
}

// ========== Cooler Product ==========

export interface CoolerProductRequest {
  category: 'cooler';
  name: string;
  price: number;
  manufacturer: string;
  coolerType: CoolerType;
  height: Length;
  tdp: Power;
  sockets: CpuSocket[];
  dimensions: Dimensions;
}

export interface CoolerProductResponse extends ProductBase {
  category: 'cooler';
  coolerType: CoolerType;
  height: Length;
  tdp: Power;
  sockets: CpuSocket[];
  dimensions: Dimensions;
}

// ========== PC Case Product ==========

export interface PcCaseProductRequest {
  category: 'case';
  name: string;
  price: number;
  manufacturer: string;
  formFactor: string;
  color: string;
  sidePanelWindow: string;
  dimensions: Dimensions;
  chambers?: Chamber[];
}

export interface PcCaseProductResponse extends ProductBase {
  category: 'case';
  formFactor: string;
  color: string;
  sidePanelWindow: string;
  dimensions: Dimensions;
  chambers?: Chamber[];
}

// ========== Union Types ==========

export type ProductRequest =
  | CpuProductRequest
  | GpuProductRequest
  | MotherboardProductRequest
  | RamProductRequest
  | StorageProductRequest
  | PsuProductRequest
  | CoolerProductRequest
  | PcCaseProductRequest;

export type ProductResponse =
  | CpuProductResponse
  | GpuProductResponse
  | MotherboardProductResponse
  | RamProductResponse
  | StorageProductResponse
  | PsuProductResponse
  | CoolerProductResponse
  | PcCaseProductResponse;
