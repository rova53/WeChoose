import { test, expect } from '@playwright/test';
import { uniqueName } from './test-utils';

test.describe.serial('Users CRUD', () => {
  let userLastName: string;
  let userEmail: string;

  test('should display the users page', async ({ page }) => {
    await page.goto('/admin/users');
    await expect(page.getByText('Gestion des Utilisateurs')).toBeVisible();
  });

  test('should create a new user', async ({ page }) => {
    userLastName = uniqueName('Testeur');
    userEmail = `e2e_${Date.now()}@test.com`;

    await page.goto('/admin/users');

    await page.getByRole('button', { name: 'Ajouter un utilisateur' }).click();
    await expect(page.getByText('Créer un utilisateur')).toBeVisible();

    await page.getByLabel('Nom').fill(userLastName);
    await page.getByLabel('Prénom').fill('E2E');
    await page.getByLabel('Email').fill(userEmail);
    await page.getByLabel('Mot de passe').fill('MotDePasse123!');
    await page.getByLabel('Entreprise').fill('E2E Corp');

    await page.getByRole('button', { name: 'Créer' }).click();

    // Verify refresh: the new user appears in the table
    await expect(page.getByText(userLastName)).toBeVisible({ timeout: 10000 });
    await expect(page.getByText(userEmail)).toBeVisible();
  });

  test('should filter users by search', async ({ page }) => {
    await page.goto('/admin/users');
    await expect(page.getByText(userLastName)).toBeVisible({ timeout: 10000 });

    const searchInput = page.getByPlaceholder('Filtrer par nom, email...');
    await searchInput.fill(userLastName);
    await expect(page.getByText(userLastName)).toBeVisible();
    await expect(page.getByText(userEmail)).toBeVisible();
  });

  test('should edit the user', async ({ page }) => {
    await page.goto('/admin/users');
    await expect(page.getByText(userLastName)).toBeVisible({ timeout: 10000 });

    const row = page.getByText(userLastName).locator('closest=tr');
    await row.getByRole('button', { name: 'Modifier' }).click();

    await expect(page.getByText(`Modifier : E2E ${userLastName}`)).toBeVisible();

    const updatedEmail = `edited_${Date.now()}@test.com`;
    await page.getByLabel('Email').fill(updatedEmail);
    await page.getByRole('button', { name: 'Enregistrer' }).click();

    // Verify refresh: the updated email appears in the table
    await expect(page.getByText(updatedEmail)).toBeVisible({ timeout: 10000 });
    userEmail = updatedEmail;
  });

  test('should delete the user', async ({ page }) => {
    await page.goto('/admin/users');
    await expect(page.getByText(userLastName)).toBeVisible({ timeout: 10000 });

    page.on('dialog', (dialog) => dialog.accept());

    const row = page.getByText(userLastName).locator('closest=tr');
    await row.getByRole('button', { name: 'Supprimer' }).click();

    // Verify refresh: the user disappears from the table
    await expect(page.getByText(userLastName)).not.toBeVisible({ timeout: 10000 });
  });
});
