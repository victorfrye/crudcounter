const config = {
  plugins: ['@trivago/prettier-plugin-sort-imports'],
  trailingComma: 'es5',
  tabWidth: 2,
  semi: true,
  singleQuote: true,
  importOrder: ['^react$|^react-dom', '<THIRD_PARTY_MODULES>', '^@crudcounter/'],
  importOrderSeparation: true,
  importOrderSortSpecifiers: true,
};

export default config;
