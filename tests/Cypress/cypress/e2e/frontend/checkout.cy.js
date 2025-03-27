describe('Finalizar Compra', () => {
  beforeEach(() => {
    cy.login();
    cy.addProductToCart();   
    cy.wait(5000); 
    cy.visit('/cart');    
  });

  it('deve finalizar a compra com sucesso, exibir alerta e sumir o carrinho', () => {
    cy.window().then((win) => {
      cy.stub(win, 'alert').as('alertCheckout');
    });
  
    cy.get('button.checkout-button').click();
  
    cy.get('@alertCheckout').should('have.been.calledWithMatch', /Checkout realizado com sucesso/);
  });
  
});
