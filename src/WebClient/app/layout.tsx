import { ReactNode } from 'react';

import './globals.css';
import type { Metadata } from 'next';

import { Profile } from '@counter/components/layout';
import { DarkModeProvider, ThemeProvider } from '@counter/components/theme';

export const metadata: Metadata = {
  title: 'Counter | Plus Ultra',
  description: 'A simple counter application',
};

export const RootLayout = ({
  children,
}: Readonly<{
  children: ReactNode;
}>) => {
  return (
    <html lang="en">
      <body>
        <div id="root">
          <DarkModeProvider>
            <ThemeProvider>
              <Profile>{children}</Profile>
            </ThemeProvider>
          </DarkModeProvider>
        </div>
      </body>
    </html>
  );
};

export default RootLayout;
