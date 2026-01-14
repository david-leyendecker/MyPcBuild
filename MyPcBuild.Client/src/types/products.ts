/**
 * TypeScript contracts for Product Management
 * These interfaces match the API DTOs from the backend
 */

// ========== Enums ==========

export enum ProductCategory {
  CPU = 'cpu',
  GPU = 'gpu',
  Motherboard = 'motherboard',
  RAM = 'ram',
  Storage = 'storage',
  PowerSupply = 'powersupply',
  Cooler = 'cooler',
  Case = 'case'
}

export enum CpuSocket {
  LGA1700 = 'LGA1700',
  LGA1200 = 'LGA1200',
  LGA1151 = 'LGA1151',
  LGA2066 = 'LGA2066',
  AM5 = 'AM5',
  AM4 = 'AM4',
  sTRX4 = 'sTRX4',
  TR4 = 'TR4'
}

export enum MemoryType {
  DDR3 = 'DDR3',
  DDR4 = 'DDR4',
  DDR5 = 'DDR5',
  GDDR5 = 'GDDR5',
  GDDR5X = 'GDDR5X',
  GDDR6 = 'GDDR6',
  GDDR6X = 'GDDR6X',
  HBM2 = 'HBM2',
  HBM2E = 'HBM2E',
  HBM3 = 'HBM3'
}

export enum FormFactor {
  ATX = 'ATX',
  MicroATX = 'MicroATX',
  MiniITX = 'MiniITX',
  EATX = 'EATX'
}

export enum CoolerType {
  Air = 'Air',
  AIO = 'AIO',
  CustomLoop = 'CustomLoop'
}

export enum GpuPowerConnector {
  Dual8Pin = 'Dual8Pin',
  Triple8Pin = 'Triple8Pin',
  One16Pin = 'One16Pin'
}

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

export interface Slot {
  name: string;
  allowedCategory: string;
  location?: Vector3;
}

export interface Chamber {
  name: string;
  dimensions: Dimensions;
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
  category: ProductCategory.CPU;
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
  category: ProductCategory.CPU;
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
  category: ProductCategory.GPU;
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
  category: ProductCategory.GPU;
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
  category: ProductCategory.Motherboard;
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
  category: ProductCategory.Motherboard;
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
  category: ProductCategory.RAM;
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
  category: ProductCategory.RAM;
  type: MemoryType;
  capacity: StorageCapacity;
  configuration: string;
  speed: Frequency;
  casLatency: string;
  voltage: Voltage;
}

// ========== Storage Product ==========

export interface StorageProductRequest {
  category: ProductCategory.Storage;
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
  category: ProductCategory.Storage;
  type: string;
  interface: string;
  storageFormFactor: string;
  capacity: StorageCapacity;
  readSpeed: DataSpeed;
  writeSpeed: DataSpeed;
}

// ========== PSU Product ==========

export interface PsuProductRequest {
  category: ProductCategory.PowerSupply;
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
  category: ProductCategory.PowerSupply;
  wattage: Power;
  efficiency: string;
  modular: string;
  formFactor: string;
  length: Length;
  pcie8Pin: number;
}

// ========== Cooler Product ==========

export interface CoolerProductRequest {
  category: ProductCategory.Cooler;
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
  category: ProductCategory.Cooler;
  coolerType: CoolerType;
  height: Length;
  tdp: Power;
  sockets: CpuSocket[];
  dimensions: Dimensions;
}

// ========== PC Case Product ==========

export interface PcCaseProductRequest {
  category: ProductCategory.Case;
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
  category: ProductCategory.Case;
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
