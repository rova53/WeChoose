import { test as base, expect, type Page } from '@playwright/test';

export const test = base.extend({});
export { expect };

export async function loginAs(page: Page, username: 'formation' | 'sales') {
  await page.goto('/login');
  await page.locator('input[name="Username"]').fill(username);
  await page.locator('input[name="Password"]').fill('');
  await page.locator('button[type="submit"]').click();
  await page.waitForURL(/\/admin/);
}

export function uniqueName(prefix: string): string {
  return `${prefix}_${Date.now()}`;
}
