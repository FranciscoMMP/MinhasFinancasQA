import { test, expect } from '@playwright/test';

test.describe('Regras de Negócio UI', () => {

    test('Deve permitir a criação de uma nova pessoa', async ({ page }) => {
        await page.goto('/pessoas');
        await page.getByRole('button', { name: 'Adicionar Pessoa' }).click();

        await page.getByLabel('Nome').fill('João Silva');
        await page.getByLabel('Data de Nascimento').fill('1990-05-15');
        await page.getByRole('button', { name: 'Salvar' }).click();

        await expect(page.getByText('João Silva')).toBeVisible();
    });

    test('Deve impedir que menor de idade registre receita', async ({ page }) => {
        await page.goto('/pessoas');
        await page.getByRole('button', { name: 'Adicionar Pessoa' }).click();
        await page.getByLabel('Nome').fill('Criança E2E');
        await page.getByLabel('Data de Nascimento').fill('2020-01-01');
        await page.getByRole('button', { name: 'Salvar' }).click();

        await page.goto('/categorias');
        await page.getByRole('button', { name: 'Adicionar Categoria' }).click();
        await page.getByLabel('Descrição').fill('Mesada');

        await page.getByRole('combobox', { name: /Finalidade|Tipo/i }).click();
        await page.getByRole('option', { name: 'Receita' }).click();
        await page.getByRole('button', { name: 'Salvar' }).click();

        await page.goto('/transacoes');
        await page.getByRole('button', { name: 'Adicionar Transação' }).click();
        await page.getByLabel('Descrição').fill('Presente');
        await page.getByLabel('Valor').fill('100');

        await page.getByRole('combobox', { name: 'Pessoa' }).click();
        await page.getByRole('option', { name: 'Criança E2E' }).click();

        await page.getByRole('combobox', { name: 'Categoria' }).click();
        await page.getByRole('option', { name: 'Mesada' }).click();

        await page.getByRole('combobox', { name: 'Tipo' }).click();
        await page.getByRole('option', { name: 'Receita' }).click();

        await page.getByLabel('Data').fill('2026-04-14');
        await page.getByRole('button', { name: 'Salvar' }).click();

        await expect(page.locator('text=Menores de 18 anos não podem registrar receitas')).toBeVisible();
    });
});
