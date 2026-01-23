/**
 * Icon mapping utilities for Naive UI with @vicons/ionicons5
 */
import type { Component } from 'vue';
import {
  Add,
  ArrowBack,
  ArrowForward,
  TrashOutline,
  CheckmarkOutline,
  Close,
  Search,
  HammerOutline,
  CubeOutline,
  RemoveCircleOutline,
  CheckmarkCircleOutline,
  CreateOutline,
  ChevronBack,
  ChevronForward,
  SaveOutline,
  ExpandOutline,
  ContractOutline,
  RemoveOutline,
  GridOutline,
  CloseCircleOutline,
  InformationCircleOutline,
  WarningOutline,
  AlertCircleOutline,
  ArrowUpOutline,
  ArrowDownOutline,
  ArrowBackOutline,
  ArrowForwardOutline,
  OpenOutline,
  ResizeOutline,
  RocketOutline,
  BulbOutline,
  PencilOutline,
  SearchOutline,
  SunnyOutline,
  MoonOutline
} from '@vicons/ionicons5';

export const Icons = {
  Add,
  ArrowBack,
  ArrowForward,
  Trash: TrashOutline,
  Check: CheckmarkOutline,
  Close,
  Search,
  Hammer: HammerOutline,
  Cube: CubeOutline,
  Remove: RemoveCircleOutline,
  CheckCircle: CheckmarkCircleOutline,
  Edit: CreateOutline,
  ChevronBack,
  ChevronForward,
  Save: SaveOutline,
  Expand: ExpandOutline,
  Contract: ContractOutline,
  Minus: RemoveOutline,
  Grid: GridOutline,
  CloseCircle: CloseCircleOutline,
  Info: InformationCircleOutline,
  Warning: WarningOutline,
  Alert: AlertCircleOutline,
  ArrowUp: ArrowUpOutline,
  ArrowDown: ArrowDownOutline,
  ArrowBackAlt: ArrowBackOutline,
  ArrowForwardAlt: ArrowForwardOutline,
  Open: OpenOutline,
  Resize: ResizeOutline,
  Rocket: RocketOutline,
  Bulb: BulbOutline,
  Pencil: PencilOutline,
  SearchIcon: SearchOutline,
  Sun: SunnyOutline,
  Moon: MoonOutline
};

export type IconName = keyof typeof Icons;

export function getIcon(name: IconName): Component {
  return Icons[name];
}
