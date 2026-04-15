import { describe, it, expect } from 'vitest';

export const validateTransactionRule = (age: number, transactionType: 'Receita' | 'Despesa') => {
    if (age < 18 && transactionType === 'Receita') {
        return { valid: false, error: 'Menores de 18 anos não podem registrar receitas' };
    }
    return { valid: true };
};

export const validateCategoryPurpose = (categoryPurpose: string, transactionType: 'Receita' | 'Despesa') => {
    if (categoryPurpose === 'Ambas') return true;
    return categoryPurpose === transactionType;
};

describe('Testes de Unidade - Validações Financeiras', () => {

    it('Deve validar tentativa de receita por menor de idade', () => {
        const result = validateTransactionRule(17, 'Receita');
        expect(result.valid).toBe(false);
        expect(result.error).toBe('Menores de 18 anos não podem registrar receitas');
    });

    it('Deve autorizar receita para maior de idade', () => {
        const result = validateTransactionRule(18, 'Receita');
        expect(result.valid).toBe(true);
    });

    it('Deve autorizar despesa para menor de idade', () => {
        const result = validateTransactionRule(15, 'Despesa');
        expect(result.valid).toBe(true);
    });

    it('Deve bloquear divergência de finalidade na categoria', () => {
        const isValid = validateCategoryPurpose('Despesa', 'Receita');
        expect(isValid).toBe(false);
    });
});
