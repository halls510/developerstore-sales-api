describe('Adicionar Produto ao Carrinho - Home', () => {
  beforeEach(() => {
    cy.login();
  });

  it('deve adicionar o primeiro produto ao carrinho a partir da home', () => {
    cy.addProductToCart();
  });
});
