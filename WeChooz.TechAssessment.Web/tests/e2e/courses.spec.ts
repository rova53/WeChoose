import { test, expect } from '@playwright/test';
import { uniqueName } from './test-utils';

test.describe.serial('Courses CRUD', () => {
  let courseName: string;

  test('should display the courses page', async ({ page }) => {
    await page.goto('/admin/courses');
    await expect(page.getByText('Catalogue des Cours')).toBeVisible();
  });

  test('should create a new course', async ({ page }) => {
    courseName = uniqueName('TestCours');
    await page.goto('/admin/courses');

    await page.getByRole('button', { name: 'Nouveau cours' }).click();
    await expect(page.getByText('Créer un nouveau cours')).toBeVisible();

    await page.getByLabel('Titre du cours').fill(courseName);
    await page.getByLabel('Description courte').fill('Description E2E');
    await page.getByLabel('Population cible').click();
    await page.getByRole('option', { name: 'Élu CSE' }).click();
    await page.getByLabel('Durée (jours)').fill('3');
    await page.getByLabel('Capacité max.').fill('15');
    await page.getByLabel('Prénom du formateur').fill('Jean');
    await page.getByLabel('Nom du formateur').fill('Dupont');

    await page.getByRole('button', { name: 'Créer le cours' }).click();

    // Verify refresh: the new course appears in the table
    await expect(page.getByText(courseName)).toBeVisible({ timeout: 10000 });
  });

  test('should filter courses by search', async ({ page }) => {
    await page.goto('/admin/courses');
    await expect(page.getByText(courseName)).toBeVisible({ timeout: 10000 });

    const searchInput = page.getByPlaceholder('Rechercher un cours...');
    await searchInput.fill(courseName);
    await expect(page.getByText(courseName)).toBeVisible();
  });

  test('should edit the course', async ({ page }) => {
    await page.goto('/admin/courses');
    await expect(page.getByText(courseName)).toBeVisible({ timeout: 10000 });

    // Click the edit button on the row containing our course
    const row = page.getByText(courseName).locator('closest=tr');
    await row.getByRole('button', { name: 'Modifier' }).click();

    await expect(page.getByText('Modifier le cours')).toBeVisible();

    const updatedName = courseName + '_edited';
    await page.getByLabel('Titre du cours').fill(updatedName);
    await page.getByRole('button', { name: 'Enregistrer' }).click();

    // Verify refresh: the updated name appears in the table
    await expect(page.getByText(updatedName)).toBeVisible({ timeout: 10000 });
    courseName = updatedName;
  });

  test('should delete the course', async ({ page }) => {
    await page.goto('/admin/courses');
    await expect(page.getByText(courseName)).toBeVisible({ timeout: 10000 });

    // Accept the upcoming confirm dialog
    page.on('dialog', (dialog) => dialog.accept());

    const row = page.getByText(courseName).locator('closest=tr');
    await row.getByRole('button', { name: 'Supprimer' }).click();

    // Verify refresh: the course disappears from the table
    await expect(page.getByText(courseName)).not.toBeVisible({ timeout: 10000 });
  });
});
