describe('API - Listar Produtos (com login)', () => {
  it('deve autenticar e retornar array de produtos no campo data', () => {
    cy.loginAPI().then((token) => {
      cy.request({
        method: 'GET',
        url: 'https://localhost:8081/api/products',
        headers: {
          Authorization: `Bearer ${token}`
        }
      }).then((res) => {
        expect(res.status).to.eq(200);
        expect(res.body).to.have.property('data');
        expect(res.body.data).to.be.an('array');
      });
    });
  });
});
