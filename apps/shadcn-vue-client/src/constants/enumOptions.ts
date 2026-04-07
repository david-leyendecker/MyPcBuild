import type {
  ProductCategory,
  CpuSocket,
  MemoryType,
  FormFactor,
  CoolerType,
  GpuPowerConnector,
  GpuChipsetManufacturer,
  SidePanelType,
  PsuEfficiency,
  PsuModularity,
  PsuFormFactor,
  StorageType,
  StorageInterface,
  StorageFormFactor
} from '@/types/product';

export const productCategoryOptions: { label: string; value: ProductCategory }[] = [
  { label: 'CPU', value: 'cpu' },
  { label: 'GPU', value: 'gpu' },
  { label: 'Motherboard', value: 'motherboard' },
  { label: 'RAM', value: 'ram' },
  { label: 'Storage', value: 'storage' },
  { label: 'Power Supply', value: 'powersupply' },
  { label: 'Cooler', value: 'cooler' },
  { label: 'Case', value: 'case' }
];

export const cpuSocketOptions: { label: string; value: CpuSocket }[] = [
  { label: 'LGA1700', value: 'LGA1700' },
  { label: 'LGA1200', value: 'LGA1200' },
  { label: 'LGA1151', value: 'LGA1151' },
  { label: 'LGA2066', value: 'LGA2066' },
  { label: 'AM5', value: 'AM5' },
  { label: 'AM4', value: 'AM4' },
  { label: 'sTRX4', value: 'sTRX4' },
  { label: 'TR4', value: 'TR4' }
];

export const formFactorOptions: { label: string; value: FormFactor }[] = [
  { label: 'ATX', value: 'ATX' },
  { label: 'Micro ATX', value: 'MicroATX' },
  { label: 'Mini ITX', value: 'MiniITX' },
  { label: 'E-ATX', value: 'EATX' }
];

export const ramMemoryTypeOptions: { label: string; value: MemoryType }[] = [
  { label: 'DDR3', value: 'DDR3' },
  { label: 'DDR4', value: 'DDR4' },
  { label: 'DDR5', value: 'DDR5' }
];

export const gpuMemoryTypeOptions: { label: string; value: MemoryType }[] = [
  { label: 'GDDR5', value: 'GDDR5' },
  { label: 'GDDR5X', value: 'GDDR5X' },
  { label: 'GDDR6', value: 'GDDR6' },
  { label: 'GDDR6X', value: 'GDDR6X' },
  { label: 'HBM2', value: 'HBM2' },
  { label: 'HBM2E', value: 'HBM2E' },
  { label: 'HBM3', value: 'HBM3' }
];

export const coolerTypeOptions: { label: string; value: CoolerType }[] = [
  { label: 'Air', value: 'Air' },
  { label: 'AIO (All-in-One)', value: 'AIO' },
  { label: 'Custom Loop', value: 'CustomLoop' }
];

export const gpuPowerConnectorOptions: { label: string; value: GpuPowerConnector }[] = [
  { label: 'Dual 8-Pin', value: 'Dual8Pin' },
  { label: 'Triple 8-Pin', value: 'Triple8Pin' },
  { label: '1x 16-Pin', value: 'One16Pin' }
];

export const gpuChipsetManufacturerOptions: { label: string; value: GpuChipsetManufacturer }[] = [
  { label: 'NVIDIA', value: 'NVIDIA' },
  { label: 'AMD', value: 'AMD' },
  { label: 'Intel', value: 'Intel' }
];

export const sidePanelTypeOptions: { label: string; value: SidePanelType }[] = [
  { label: 'None', value: 'None' },
  { label: 'Acrylic', value: 'Acrylic' },
  { label: 'Tempered Glass', value: 'TemperedGlass' }
];

export const psuEfficiencyOptions: { label: string; value: PsuEfficiency }[] = [
  { label: '80+ Bronze', value: 'Bronze' },
  { label: '80+ Silver', value: 'Silver' },
  { label: '80+ Gold', value: 'Gold' },
  { label: '80+ Platinum', value: 'Platinum' },
  { label: '80+ Titanium', value: 'Titanium' }
];

export const psuModularityOptions: { label: string; value: PsuModularity }[] = [
  { label: 'Non-Modular', value: 'NonModular' },
  { label: 'Semi-Modular', value: 'SemiModular' },
  { label: 'Fully Modular', value: 'FullyModular' }
];

export const psuFormFactorOptions: { label: string; value: PsuFormFactor }[] = [
  { label: 'ATX', value: 'ATX' },
  { label: 'SFX', value: 'SFX' },
  { label: 'SFX-L', value: 'SFXL' }
];

export const storageTypeOptions: { label: string; value: StorageType }[] = [
  { label: 'SSD', value: 'SSD' },
  { label: 'HDD', value: 'HDD' }
];

export const storageInterfaceOptions: { label: string; value: StorageInterface }[] = [
  { label: 'NVMe', value: 'NVMe' },
  { label: 'SATA', value: 'SATA' }
];

export const storageFormFactorOptions: { label: string; value: StorageFormFactor }[] = [
  { label: 'M.2 2280', value: 'M2_2280' },
  { label: '2.5 inch', value: 'TwoPointFiveInch' },
  { label: '3.5 inch', value: 'ThreePointFiveInch' }
];
