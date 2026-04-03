/**
 * TypeScript contracts for Product Management
 * These interfaces match the API DTOs from the backend
 */

// ========== Enums (as string literals for API compatibility) ==========

import type { ProductCategory } from '@/api/catalog';
export type { ProductCategory };

export type CpuSocket = 'LGA1700' | 'LGA1200' | 'LGA1151' | 'LGA2066' | 'AM5' | 'AM4' | 'sTRX4' | 'TR4';

export type MemoryType = 'DDR3' | 'DDR4' | 'DDR5' | 'GDDR5' | 'GDDR5X' | 'GDDR6' | 'GDDR6X' | 'HBM2' | 'HBM2E' | 'HBM3';

export type FormFactor = 'ATX' | 'MicroATX' | 'MiniITX' | 'EATX';

export type CoolerType = 'Air' | 'AIO' | 'CustomLoop';

export type GpuPowerConnector = 'Dual8Pin' | 'Triple8Pin' | 'One16Pin';

export type GpuChipsetManufacturer = 'NVIDIA' | 'AMD' | 'Intel';

export type SidePanelType = 'None' | 'Acrylic' | 'Tempered Glass';

export type PsuEfficiency = '80+ Bronze' | '80+ Silver' | '80+ Gold' | '80+ Platinum' | '80+ Titanium';

export type PsuModularity = 'Non-Modular' | 'Semi-Modular' | 'Fully Modular';

export type PsuFormFactor = 'ATX' | 'SFX' | 'SFX-L';

export type StorageType = 'SSD' | 'HDD';

export type StorageInterface = 'NVMe' | 'SATA';

export type StorageFormFactor = 'M.2 2280' | '2.5 inch' | '3.5 inch';

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

export interface Dimensions {
  length: number;
  width: number;
  height: number;
}

export interface Vector3 {
  x: number;
  y: number;
  z: number;
}

export interface Rotation {
  x: number;
  y: number;
  z: number;
}

export interface Slot {
  id?: string;
  name: string;
  allowedCategory: ProductCategory;
  relativePosition: Vector3;
  maxDimensions: Dimensions;
  rotation?: Rotation | null;
  subSlots?: Slot[];
}

export interface Chamber {
  id?: string;
  name: string;
  relativePosition: Vector3;
  dimensions: Dimensions;
  slots: Slot[];
}

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
  chipsetManufacturer: GpuChipsetManufacturer;
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
  chipsetManufacturer: GpuChipsetManufacturer;
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
  type: StorageType;
  interface: StorageInterface;
  storageFormFactor: StorageFormFactor;
  capacity: StorageCapacity;
  readSpeed: DataSpeed;
  writeSpeed: DataSpeed;
}

export interface StorageProductResponse extends ProductBase {
  category: 'storage';
  type: StorageType;
  interface: StorageInterface;
  storageFormFactor: StorageFormFactor;
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
  efficiency: PsuEfficiency;
  modular: PsuModularity;
  formFactor: PsuFormFactor;
  length: Length;
  pcie8Pin: number;
}

export interface PsuProductResponse extends ProductBase {
  category: 'powersupply';
  wattage: Power;
  efficiency: PsuEfficiency;
  modular: PsuModularity;
  formFactor: PsuFormFactor;
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
  formFactor: FormFactor;
  color: string;
  sidePanelWindow: SidePanelType;
  dimensions: Dimensions;
  chambers?: Chamber[];
}

export interface PcCaseProductResponse extends ProductBase {
  category: 'case';
  formFactor: FormFactor;
  color: string;
  sidePanelWindow: SidePanelType;
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
