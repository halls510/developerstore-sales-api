
Cypress.Commands.add('login', () => {
  cy.visit('/login');
  // espera 1 segundo para garantir tempo de renderização
  cy.wait(1000);
  cy.get('[data-testid="input-email"]').type('admin@example.com');
  cy.get('[data-testid="input-password"]').type('A#g7jfdsd#$%#');
  cy.get('[data-testid="btn-login"]').click();
  // espera 1 segundo para garantir tempo de renderização
  cy.wait(1000);
  cy.url().should('include', '/home');
});

Cypress.Commands.add('addProductToCart', () => {
  cy.window().then((win) => {
    cy.stub(win, 'alert').as('alertAddToCart');
  });

  cy.get('.product-card', { timeout: 10000 }).first().as('produto');

  cy.get('@produto').find('h3').invoke('text').then((tituloProduto) => {
    cy.get('@produto').within(() => {
      cy.contains('button', 'Adicionar ao Carrinho').click();
    });

    cy.get('@alertAddToCart').should('have.been.calledWithMatch', new RegExp(tituloProduto.trim()));
  });
});


Cypress.Commands.add('loginAPI', () => {
  return cy.request('POST', 'https://localhost:8081/api/auth', {
    email: 'admin@example.com',
    password: 'A#g7jfdsd#$%#'
  }).then((res) => res.body.data.token);
});
