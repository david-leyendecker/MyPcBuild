/**
 * Centralized Vuetify default configuration for input controls
 * Update these values to apply changes globally across all components
 */

export const inputDefaults = {
  variant: undefined,
  density: 'default',
  hideDetails: 'auto',
} as const;

/**
 * Gap spacing configuration for product forms
 * Change this value to adjust spacing globally across all product forms
 */
export const productFormGapClass = 'ga-3';

export const vuetifyDefaults = {
  VTextField: inputDefaults,
  VTextarea: inputDefaults,
  VSelect: inputDefaults,
  VAutocomplete: inputDefaults,
  VCombobox: inputDefaults,
  VFileInput: inputDefaults,
  VCheckbox: {
    density: inputDefaults.density,
    hideDetails: inputDefaults.hideDetails,
  },
  VRadio: {
    density: inputDefaults.density,
    hideDetails: inputDefaults.hideDetails,
  },
  VSwitch: {
    density: inputDefaults.density,
    hideDetails: inputDefaults.hideDetails,
  },
  // Product form container defaults
  'product-form-container': {
    class: `d-flex flex-column ${productFormGapClass}`,
  },
};
