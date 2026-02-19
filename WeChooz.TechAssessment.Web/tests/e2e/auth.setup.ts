import { test as setup } from '@playwright/test';
import { loginAs } from './test-utils';

const authFile = (user: string) => `tests/e2e/.auth/${user}.json`;

setup('authenticate as formation', async ({ page }) => {
  await loginAs(page, 'formation');
  await page.context().storageState({ path: authFile('formation') });
});

setup('authenticate as sales', async ({ page }) => {
  await loginAs(page, 'sales');
  await page.context().storageState({ path: authFile('sales') });
});
