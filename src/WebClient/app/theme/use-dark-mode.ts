import { useContext } from 'react';

import { DarkModeContext } from '@crudcounter/theme/dark-mode-provider';

export default function useDarkMode() {
  return useContext(DarkModeContext);
}
