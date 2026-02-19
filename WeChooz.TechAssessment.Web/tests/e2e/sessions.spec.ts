import { test, expect } from '@playwright/test';

test.describe.serial('Sessions CRUD', () => {
  let sessionCourseName: string;

  test('should display the sessions page', async ({ page }) => {
    await page.goto('/admin/sessions');
    await expect(page.getByText('Gestion des Sessions')).toBeVisible();
  });

  test('should create a new session', async ({ page }) => {
    await page.goto('/admin/sessions');

    await page.getByRole('button', { name: 'Nouvelle session' }).click();
    await expect(page.getByText('Planifier une session')).toBeVisible();

    // Select a course from the searchable dropdown
    const courseSelect = page.getByLabel('Sélectionner le cours');
    await courseSelect.click();
    // Pick the first available option
    const firstOption = page.getByRole('option').first();
    await firstOption.waitFor({ state: 'visible', timeout: 10000 });
    sessionCourseName = (await firstOption.textContent()) ?? '';
    await firstOption.click();

    // Fill the date
    await page.getByLabel('Date de début').fill('2026-12-01');

    // Select delivery mode
    await page.getByLabel('Mode de diffusion').click();
    await page.getByRole('option', { name: 'Présentiel' }).click();

    await page.getByRole('button', { name: 'Planifier' }).click();

    // Verify refresh: wait for the modal to close and the session to appear
    await expect(page.getByText('Planifier une session')).not.toBeVisible({ timeout: 10000 });
    // Verify the sessions page shows the course name of our new session
    await expect(page.getByText('1 décembre 2026').first()).toBeVisible({ timeout: 10000 });
  });

  test('should filter sessions by search', async ({ page }) => {
    await page.goto('/admin/sessions');
    await page.waitForTimeout(1000);

    const searchInput = page.getByPlaceholder('Filtrer par cours...');
    await searchInput.fill(sessionCourseName);
    await expect(page.getByText(sessionCourseName).first()).toBeVisible();
  });

  test('should edit the session', async ({ page }) => {
    await page.goto('/admin/sessions');

    // Find the row with "1 décembre 2026" and click edit
    const dateCell = page.getByText('1 décembre 2026').first();
    await dateCell.waitFor({ state: 'visible', timeout: 10000 });

    const row = dateCell.locator('closest=tr');
    await row.getByRole('button', { name: 'Modifier' }).click();

    await expect(page.getByText('Modifier la session')).toBeVisible();

    // Change delivery mode
    await page.getByLabel('Mode de diffusion').click();
    await page.getByRole('option', { name: 'Distanciel' }).click();

    await page.getByRole('button', { name: 'Enregistrer' }).click();

    // Verify refresh: modal closes and Distanciel badge shows
    await expect(page.getByText('Modifier la session')).not.toBeVisible({ timeout: 10000 });
    await expect(page.getByText('Distanciel').first()).toBeVisible({ timeout: 10000 });
  });

  test('should delete the session', async ({ page }) => {
    await page.goto('/admin/sessions');

    const dateCell = page.getByText('1 décembre 2026').first();
    await dateCell.waitFor({ state: 'visible', timeout: 10000 });

    page.on('dialog', (dialog) => dialog.accept());

    const row = dateCell.locator('closest=tr');
    await row.getByRole('button', { name: 'Supprimer' }).click();

    // Verify refresh: our session row disappears
    await expect(dateCell).not.toBeVisible({ timeout: 10000 });
  });
});
