import { ReactNode } from 'react';

import type { Metadata } from 'next';

import '@crudcounter/globals.css';
import { DarkModeProvider, ThemeProvider } from '@crudcounter/theme';

export const metadata: Metadata = {
  title: 'CRUD Counter | Plus Ultra',
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
            <ThemeProvider>{children}</ThemeProvider>
          </DarkModeProvider>
        </div>
      </body>
    </html>
  );
};

export default RootLayout;
