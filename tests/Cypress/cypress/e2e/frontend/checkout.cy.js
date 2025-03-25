describe('Finalizar Compra', () => {
  beforeEach(() => {
    cy.login();
    cy.addProductToCart();
    cy.visit('/cart');
  });

  it('deve finalizar a compra com sucesso, exibir alerta e sumir o carrinho', () => {
    // Captura o alert de sucesso
    cy.on('window:alert', (mensagem) => {
      expect(mensagem).to.contain('Checkout realizado com sucesso');
    });

    // Clica no botão "Finalizar Compra"
    cy.get('button.checkout-button').click();

    // Verifica que o carrinho foi limpado
    cy.get('p.empty-message').should('contain.text', 'Nenhum carrinho ativo encontrado.');
  });
});
