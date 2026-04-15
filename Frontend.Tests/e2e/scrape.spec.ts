import { test } from '@playwright/test';
import * as fs from 'fs';

test('Scrape DOM', async ({ page }) => {
    await page.goto('/');
    await page.waitForTimeout(1000); // Wait for React to render
    fs.writeFileSync('home.html', await page.content());

    await page.goto('/pessoas');
    await page.waitForTimeout(1000);
    fs.writeFileSync('pessoas.html', await page.content());

    await page.goto('/categorias');
    await page.waitForTimeout(1000);
    fs.writeFileSync('categorias.html', await page.content());

    await page.goto('/transacoes');
    await page.waitForTimeout(1000);
    fs.writeFileSync('transacoes.html', await page.content());
});
