import { useContext } from 'react';

import { DarkModeContext } from '@counter/components/theme/DarkMode';

const useDarkMode = () => useContext(DarkModeContext);

export default useDarkMode;
