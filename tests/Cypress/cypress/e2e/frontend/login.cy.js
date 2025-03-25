describe('Login - Frontend', () => {
  it('deve realizar login e exibir a Home com "Produtos Disponíveis"', () => {
    cy.login();
    cy.get('h2.title').should('contain.text', 'Produtos Disponíveis');
  });
});
