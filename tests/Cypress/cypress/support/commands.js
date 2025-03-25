
Cypress.Commands.add('login', () => {
  cy.visit('/login');
  cy.get('[data-testid="input-email"]').type('admin@example.com');
  cy.get('[data-testid="input-password"]').type('A#g7jfdsd#$%#');
  cy.get('[data-testid="btn-login"]').click();
  cy.url().should('include', '/home');
});

Cypress.Commands.add('addProductToCart', () => {
  // Espera o primeiro produto renderizar
  cy.get('.product-card', { timeout: 10000 }).first().as('produto');

  // Captura o título antes do clique
  cy.get('@produto').find('h3').invoke('text').then((tituloProduto) => {
    // Escuta o alert e verifica se contém o nome do produto
    cy.on('window:alert', (mensagem) => {
      expect(mensagem).to.contain(tituloProduto.trim());
    });

    // Clica no botão "Adicionar ao Carrinho"
    cy.get('@produto').within(() => {
      cy.contains('button', 'Adicionar ao Carrinho').click();
    });
  });
});

Cypress.Commands.add('loginAPI', () => {
  return cy.request('POST', 'https://localhost:8081/api/auth', {
    email: 'admin@example.com',
    password: 'A#g7jfdsd#$%#'
  }).then((res) => res.body.data.token);
});
